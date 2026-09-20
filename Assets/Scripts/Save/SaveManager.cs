// 씬의 각 시스템에서 저장할 상태를 모으고, 불러온 상태를 되돌린다
// 각 시스템의 내부 객체를 직접 직렬화하지 않고 SaveData(DTO)를 거친다
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {
    public static SaveManager instance { get; private set; }

    public SaveRegistry registry; // id -> 에셋 표
    public WorldStreamer streamer; // 월드 시드와 청크를 담당

    [Header("자동저장")]
    public float autoSaveInterval = 300f; // 이 주기마다 자동저장한다 (0 이하면 꺼진다)

    public float playTime { get; private set; } // 이번 세션까지 누적된 플레이 시간

    // 마지막으로 저장하거나 불러온 슬롯. 자동저장은 이 슬롯의 자동 칸에만 쓴다
    public int currentSlot { get; private set; }

    public float timeUntilAutoSave => autoSaveInterval > 0f
        ? Mathf.Max(0f, lastAutoSaveTime + autoSaveInterval - Time.time)
        : -1f;

    // 마지막 자동저장 시점을 기준으로 재산한다.
    // 다음 시각을 미리 잡아두면 주기를 바꿔도 다음 번까지 반영되지 않는다
    private float lastAutoSaveTime;

    private PlayerHealth player;
    private Inventory inventory;
    private Equipment equipment;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        player = FindFirstObjectByType<PlayerHealth>();

        if (player != null)
        {
            inventory = player.GetComponent<Inventory>();
            equipment = player.GetComponent<Equipment>();
        }

        if (streamer == null)
        {
            streamer = FindFirstObjectByType<WorldStreamer>();
        }

        if (StartupLoadRequest.TryConsume(out int slot, out bool auto))
        {
            StartCoroutine(LoadRequestedSave(slot, auto));
        }
    }

    // 월드의 Start가 끝난 다음 타이틀에서 선택한 저장을 기존 복원 경로로 적용한다
    private System.Collections.IEnumerator LoadRequestedSave(int slot, bool auto) {
        yield return null;

        if (!Load(slot, auto))
        {
            Debug.LogError($"시작 저장을 불러오지 못했다. 슬롯 {slot}, 자동={auto}");
        }
    }

    private void Update() {
        playTime += Time.deltaTime;

        if (autoSaveInterval <= 0f || Time.time < lastAutoSaveTime + autoSaveInterval)
        {
            return;
        }

        lastAutoSaveTime = Time.time;

        // 자동저장은 자동 칸에만 쓴다. 수동 저장은 절대 덮지 않는다
        Save(currentSlot, true);
    }

    private void OnEnable() {
        lastAutoSaveTime = Time.time;
    }

    public bool Save(int slot, bool auto) {
        bool ok = SaveSystem.Write(slot, auto, Collect());

        if (ok && !auto)
        {
            currentSlot = slot;
        }

        return ok;
    }

    public bool Load(int slot, bool auto) {
        SaveData data = SaveSystem.Read(slot, auto);

        if (data == null)
        {
            return false;
        }

        Apply(data);
        currentSlot = slot;
        lastAutoSaveTime = Time.time;
        return true;
    }

    // 지금 씬 상태를 저장 DTO로 모은다
    public SaveData Collect() {
        var data = new SaveData { playTime = playTime };

        if (streamer != null && streamer.map != null)
        {
            data.worldSeed = streamer.map.worldSeed;
            data.generatorVersion = streamer.map.generatorVersion;
        }

        CollectPlayer(data);
        CollectBuildings(data);

        var motorcycle = FindFirstObjectByType<Motorcycle>();

        if (motorcycle != null)
        {
            data.motorcycle = motorcycle.ToSaveData();
        }

        if (WorldRuntimeState.instance != null)
        {
            data.worldRuntime = WorldRuntimeState.instance.ToSaveData();
        }

        return data;
    }

    // 불러온 DTO를 씬에 되돌린다
    public void Apply(SaveData data) {
        playTime = data.playTime;

        ApplyPlayer(data);

        // 플레이어를 먼저 옮긴 뒤에 월드를 만들어야 그 주변 청크가 올라온다
        if (streamer != null && streamer.map != null
            && (streamer.map.worldSeed != data.worldSeed
                || streamer.map.generatorVersion != data.generatorVersion))
        {
            streamer.Regenerate(data.worldSeed, data.generatorVersion);
        }

        // 채집/루팅 상태는 청크가 다시 올라오기 전에 되돌려야 반영된다
        if (WorldRuntimeState.instance != null)
        {
            WorldRuntimeState.instance.LoadFromSaveData(data.worldRuntime);
        }

        ApplyBuildings(data);

        var motorcycle = FindFirstObjectByType<Motorcycle>();

        if (motorcycle != null)
        {
            motorcycle.LoadFromSaveData(data.motorcycle);
        }
    }

    private void CollectPlayer(SaveData data) {
        if (player == null)
        {
            return;
        }

        data.player.position = player.transform.position;
        data.player.rotationY = player.transform.eulerAngles.y;
        data.player.health = player.health;
        data.player.score = GameManager.instance != null ? GameManager.instance.score : 0;

        foreach (ItemStack stack in inventory.items)
        {
            data.inventory.Add(new ItemStackSaveData {
                itemId = stack.data.itemId,
                count = stack.count,
            });
        }

        foreach (EquipmentSlot slot in System.Enum.GetValues(typeof(EquipmentSlot)))
        {
            ItemData item = equipment.Get(slot);

            if (item != null)
            {
                data.equipment.Add(new EquipmentSaveData { slot = slot, itemId = item.itemId });
            }
        }
    }

    private void ApplyPlayer(SaveData data) {
        if (player == null)
        {
            return;
        }

        // 라이더 상태로 저장했더라도 내려선 상태로 되돌린다
        var rider = player.GetComponent<RiderControl>();

        if (rider != null && rider.isRiding)
        {
            rider.ExitVehicle(data.player.position);
        }

        player.transform.SetPositionAndRotation(
            data.player.position, Quaternion.Euler(0f, data.player.rotationY, 0f));

        var body = player.GetComponent<Rigidbody>();

        if (body != null && !body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        player.SetHealth(data.player.health);

        if (GameManager.instance != null)
        {
            GameManager.instance.SetScore(data.player.score);
        }

        // 장비는 인벤토리 안의 아이템을 가리키므로 인벤토리를 먼저 되돌린다
        ClearInventory();

        foreach (ItemStackSaveData stack in data.inventory)
        {
            ItemData item = registry.FindItem(stack.itemId);

            if (item == null)
            {
                Debug.LogError($"저장에 있는 아이템 id를 찾을 수 없다: {stack.itemId}");
                continue;
            }

            inventory.Add(item, stack.count);
        }

        foreach (EquipmentSlot slot in System.Enum.GetValues(typeof(EquipmentSlot)))
        {
            equipment.Unequip(slot);
        }

        foreach (EquipmentSaveData entry in data.equipment)
        {
            ItemData item = registry.FindItem(entry.itemId);

            if (item != null)
            {
                equipment.Equip(item);
            }
        }
    }

    private void ClearInventory() {
        var snapshot = new List<ItemStack>(inventory.items);

        foreach (ItemStack stack in snapshot)
        {
            inventory.Remove(stack.data, stack.count);
        }
    }

    private void CollectBuildings(SaveData data) {
        BaseBuildState state = BaseBuildState.instance;

        if (state == null)
        {
            return;
        }

        for (int i = 0; i < state.buildings.Count; i++)
        {
            PlacedBuilding record = state.buildings[i];

            data.buildings.Add(new PlacedBuilding(
                record.buildableId, record.position, record.rotationY) {
                isOpen = record.isOpen,
            });

            // 보관 상자의 내용물은 건설물 순번으로 연결한다
            var storage = record.instance == null
                ? null
                : record.instance.GetComponent<StorageContainer>();

            if (storage == null || storage.storage == null)
            {
                continue;
            }

            var entry = new StorageSaveData { buildingIndex = i };

            foreach (ItemStack stack in storage.storage.items)
            {
                entry.items.Add(new ItemStackSaveData {
                    itemId = stack.data.itemId,
                    count = stack.count,
                });
            }

            data.storages.Add(entry);
        }
    }

    private void ApplyBuildings(SaveData data) {
        BaseBuildState state = BaseBuildState.instance;

        if (state == null)
        {
            return;
        }

        // 지금 서 있는 건설물을 모두 철거하고 저장된 것으로 다시 세운다
        var existing = new List<PlacedBuilding>(state.buildings);

        foreach (PlacedBuilding record in existing)
        {
            state.Remove(record);
        }

        var rebuilt = new List<PlacedBuilding>();

        foreach (PlacedBuilding saved in data.buildings)
        {
            BuildableData buildable = state.Find(saved.buildableId);

            if (buildable == null)
            {
                Debug.LogError($"저장에 있는 건설물 id를 찾을 수 없다: {saved.buildableId}");
                rebuilt.Add(null);
                continue;
            }

            PlacedBuilding record = state.Place(buildable, saved.position, saved.rotationY);

            // 문은 Start에서 이 값을 읽어 열림 상태를 맞춘다
            if (record != null)
            {
                record.isOpen = saved.isOpen;
            }

            rebuilt.Add(record);
        }

        foreach (StorageSaveData entry in data.storages)
        {
            if (entry.buildingIndex < 0 || entry.buildingIndex >= rebuilt.Count
                || rebuilt[entry.buildingIndex] == null)
            {
                continue;
            }

            var storage = rebuilt[entry.buildingIndex].instance.GetComponent<StorageContainer>();

            if (storage == null || storage.storage == null)
            {
                continue;
            }

            foreach (ItemStackSaveData stack in entry.items)
            {
                ItemData item = registry.FindItem(stack.itemId);

                if (item != null)
                {
                    storage.storage.Add(item, stack.count);
                }
            }
        }
    }

    // 검증용 요약
    public string Describe() {
        SaveData data = Collect();
        return $"플레이 시간 {playTime:F0}초 | {data.Summarize()}";
    }
}
