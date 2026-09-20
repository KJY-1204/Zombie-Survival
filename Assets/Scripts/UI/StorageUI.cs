// 보관 상자를 열면 플레이어 인벤토리와 상자 내용물을 나란히 보여주고 서로 옮기게 해주는 화면
using UnityEngine;
using UnityEngine.UI;

public class StorageUI : ScreenPanel {
    public Text titleText; // 상자 이름
    public Transform playerContent; // 플레이어 아이템 줄이 붙을 부모
    public Transform storageContent; // 상자 아이템 줄이 붙을 부모
    public GameObject rowPrefab; // 아이템 줄 프리팹 (인벤토리와 같은 것을 쓴다)

    [Header("무게")]
    public RectTransform playerWeightFill;
    public Text playerWeightText;
    public Text storageWeightText;

    private StorageContainer current; // 지금 열어둔 상자
    private Inventory playerInventory; // 플레이어 인벤토리

    private void Start() {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();

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

        ClearRows(playerContent);
        ClearRows(storageContent);

        // 왼쪽: 내 아이템 -> 상자로 넣기
        foreach (ItemStack stack in playerInventory.items)
        {
            CreateRow(playerContent, stack, "넣기", playerInventory, current.storage);
        }

        // 오른쪽: 상자 아이템 -> 내 인벤토리로 꺼내기
        foreach (ItemStack stack in current.storage.items)
        {
            CreateRow(storageContent, stack, "꺼내기", current.storage, playerInventory);
        }

        RefreshWeights();
    }

    // 아이템 한 줄. 누르면 한 묶음을 통째로 옮긴다
    private void CreateRow(Transform parent, ItemStack stack, string caption,
        Inventory from, Inventory to) {
        GameObject row = Instantiate(rowPrefab, parent);

        row.GetComponent<Image>().color = UiTheme.RowBackground;
        row.transform.Find("Icon Slot/Icon").GetComponent<Image>().sprite = stack.data.icon;
        row.transform.Find("Name").GetComponent<Text>().text = stack.data.displayName;
        row.transform.Find("Detail").GetComponent<Text>().text =
            $"{stack.count}개   {stack.totalWeight:F2} kg";
        row.transform.Find("Count").GetComponent<Text>().text = caption;

        ItemData data = stack.data;
        int count = stack.count;

        row.GetComponent<Button>().onClick.AddListener(() => {
            if (from.Remove(data, count))
            {
                to.Add(data, count);
            }
        });
    }

    private void RefreshWeights() {
        float ratio = playerInventory.maxWeight > 0f
            ? Mathf.Clamp01(playerInventory.totalWeight / playerInventory.maxWeight)
            : 0f;

        playerWeightFill.anchorMax = new Vector2(ratio, 1f);
        playerWeightFill.GetComponent<Image>().color = UiTheme.BarColorForFill(
            playerInventory.isOverweight ? 1f : ratio);

        playerWeightText.text =
            $"{playerInventory.totalWeight:F2} / {playerInventory.maxWeight:F1} kg"
            + (playerInventory.isOverweight ? "   과적" : "");
        playerWeightText.color = playerInventory.isOverweight
            ? UiTheme.TextDanger
            : UiTheme.TextPrimary;

        storageWeightText.text = $"{current.storage.items.Count}종   "
            + $"{current.storage.totalWeight:F2} kg";
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
