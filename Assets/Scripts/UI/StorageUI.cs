// 보관 상자를 열면 플레이어 인벤토리와 상자 내용물을 나란히 보여주고 서로 옮기게 해주는 화면
using UnityEngine;
using UnityEngine.UI;

public class StorageUI : ScreenPanel {
    public Text titleText; // 상자 이름
    public Text playerWeightText; // 플레이어 무게 표시
    public Transform playerContent; // 플레이어 아이템 줄이 붙을 부모
    public Transform storageContent; // 상자 아이템 줄이 붙을 부모
    public GameObject rowPrefab; // 줄 하나의 프리팹 (Label + Action Button)

    private StorageContainer current; // 지금 열어둔 상자
    private Inventory playerInventory; // 플레이어 인벤토리

    private void Start() {
        PlayerHealth player = FindObjectOfType<PlayerHealth>();

        if (player != null)
        {
            playerInventory = player.GetComponent<Inventory>();
            playerInventory.onChanged += Refresh;
        }

        StorageContainer.onOpenRequested += Open;
    }

    private void OnDestroy() {
        StorageContainer.onOpenRequested -= Open;

        if (playerInventory != null)
        {
            playerInventory.onChanged -= Refresh;
        }

        UnsubscribeCurrent();
    }

    // 상자가 열어달라고 요청하면 그 상자를 대상으로 화면을 연다
    private void Open(StorageContainer container) {
        UnsubscribeCurrent();

        current = container;
        current.storage.onChanged += Refresh;

        SetOpen(true);
    }

    // 이 화면은 상자에서만 열리므로 토글 키로는 열지 않고 닫기만 한다
    protected override void Update() {
        if (panel.activeSelf && Input.GetKeyDown(toggleKey))
        {
            SetOpen(false);
        }
    }

    protected override void Refresh() {
        if (current == null || !panel.activeSelf)
        {
            return;
        }

        titleText.text = current.displayName;
        playerWeightText.text =
            $"내 무게 {playerInventory.totalWeight:F2} / {playerInventory.maxWeight:F1} kg"
            + (playerInventory.isOverweight ? "  (과적)" : "");

        ClearRows(playerContent);
        ClearRows(storageContent);

        // 왼쪽: 내 아이템 -> 상자로 넣기
        for (int i = 0; i < playerInventory.items.Count; i++)
        {
            CreateRow(playerContent, playerInventory.items[i], "넣기",
                playerInventory, current.storage);
        }

        // 오른쪽: 상자 아이템 -> 내 인벤토리로 꺼내기
        for (int i = 0; i < current.storage.items.Count; i++)
        {
            CreateRow(storageContent, current.storage.items[i], "꺼내기",
                current.storage, playerInventory);
        }
    }

    // 아이템 한 줄을 만들고 옮기기 버튼을 연결한다 (한 번에 한 묶음씩 통째로 옮긴다)
    private void CreateRow(Transform parent, ItemStack stack, string caption,
        Inventory from, Inventory to) {
        GameObject row = Instantiate(rowPrefab, parent);

        Text label = row.transform.Find("Label").GetComponent<Text>();
        label.text = $"{stack.data.displayName} x{stack.count}    {stack.totalWeight:F2} kg";

        Button action = row.transform.Find("Action Button").GetComponent<Button>();
        action.transform.Find("Text").GetComponent<Text>().text = caption;

        ItemData data = stack.data;
        int count = stack.count;

        action.onClick.AddListener(() => {
            if (from.Remove(data, count))
            {
                to.Add(data, count);
            }
        });
    }

    public void SetClosed() {
        SetOpen(false);
    }

    private void UnsubscribeCurrent() {
        if (current != null)
        {
            current.storage.onChanged -= Refresh;
            current = null;
        }
    }
}
