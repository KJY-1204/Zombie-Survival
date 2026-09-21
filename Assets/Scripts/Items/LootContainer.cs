// 필드에 놓인 상자. 한 번 열면 내용물을 전부 인벤토리로 옮기고 빈 상자가 된다
using System;
using UnityEngine;

// 상자에 들어있는 아이템 한 종류
[Serializable]
public class LootEntry {
    public ItemData data; // 어떤 아이템인지
    public int count = 1; // 몇 개 들어있는지
}

public class LootContainer : MonoBehaviour, IInteractable {
    public string displayName = "상자"; // 상호작용 안내에 표시할 이름
    public LootEntry[] contents; // 들어있는 내용물

    // 청크가 다시 올라와도 루팅 상태를 이어받기 위한 id (청크 빌더가 채운다)
    [System.NonSerialized] public string runtimeId;

    public bool isEmpty { get; private set; } // 이미 열어서 비었는지

    // 청크 생성기가 유형별 테이블을 굴린 결과를 넣는다
    public void SetContents(LootEntry[] newContents) {
        contents = newContents;
    }

    // 이미 열어본 상자라면 빈 상태로 시작한다
    private void Start() {
        if (!string.IsNullOrEmpty(runtimeId) && WorldRuntimeState.instance != null
            && WorldRuntimeState.instance.IsLooted(runtimeId))
        {
            isEmpty = true;
        }
    }

    public bool CanInteract(GameObject interactor) {
        return !isEmpty;
    }

    public string GetInteractionLabel() {
        return isEmpty ? $"{displayName} (비어 있음)" : $"{displayName} 열기";
    }

    public bool Interact(GameObject interactor) {
        return Loot(interactor.GetComponent<Inventory>());
    }

    // 내용물을 전부 inventory로 옮긴다. 실제로 열렸을 때만 true를 반환한다
    public bool Loot(Inventory inventory) {
        if (isEmpty || inventory == null || contents == null)
        {
            return false;
        }

        foreach (LootEntry entry in contents)
        {
            if (entry.data == null || entry.count <= 0)
            {
                continue;
            }

            inventory.Add(entry.data, entry.count);
        }

        // 내용물이 비어 있던 상자도 한 번 열면 다시 열리지 않는다
        isEmpty = true;

        if (!string.IsNullOrEmpty(runtimeId) && WorldRuntimeState.instance != null)
        {
            WorldRuntimeState.instance.MarkLooted(runtimeId);
        }

        return true;
    }
}
