// 인벤토리의 공개 상태와 onChanged 이벤트만 읽어서 보유 목록과 무게를 표시하는 화면
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : ScreenPanel {
    public Text weightText; // 현재 무게 / 최대 무게 표시
    public Transform content; // 아이템 줄이 붙을 부모
    public GameObject rowPrefab; // 아이템 한 줄의 프리팹

    private Inventory inventory; // 표시 대상 인벤토리
    private GameObject inventoryOwner; // 아이템 사용 효과가 적용될 대상

    private void Start() {
        inventory = FindObjectOfType<Inventory>();

        if (inventory != null)
        {
            inventoryOwner = inventory.gameObject;
            inventory.onChanged += Refresh;
        }
    }

    private void OnDestroy() {
        if (inventory != null)
        {
            inventory.onChanged -= Refresh;
        }
    }

    // 보유 목록과 무게 표시를 현재 인벤토리 상태로 다시 그린다
    protected override void Refresh() {
        // 닫혀 있는 동안에는 다시 그릴 필요가 없다
        if (inventory == null || !panel.activeSelf)
        {
            return;
        }

        weightText.text =
            $"무게 {inventory.totalWeight:F2} / {inventory.maxWeight:F1} kg"
            + (inventory.isOverweight ? "  (과적)" : "");

        ClearRows(content);

        for (int i = 0; i < inventory.items.Count; i++)
        {
            CreateRow(i, inventory.items[i]);
        }
    }

    // 아이템 한 줄을 만들어 이름/개수/무게와 사용·버리기 버튼을 연결
    private void CreateRow(int index, ItemStack stack) {
        GameObject row = Instantiate(rowPrefab, content);

        Text label = row.transform.Find("Label").GetComponent<Text>();
        label.text = $"{stack.data.displayName} x{stack.count}    {stack.totalWeight:F2} kg";

        Button useButton = row.transform.Find("Use Button").GetComponent<Button>();
        Button dropButton = row.transform.Find("Drop Button").GetComponent<Button>();

        // 소비 아이템이 아니면 사용 버튼을 누를 수 없게 한다
        useButton.interactable = stack.data is ConsumableItemData;

        useButton.onClick.AddListener(() => inventory.Use(index, inventoryOwner));
        dropButton.onClick.AddListener(() => inventory.DropAt(index, 1));
    }
}
