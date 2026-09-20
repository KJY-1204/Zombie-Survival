// 인벤토리의 공개 상태와 onChanged 이벤트만 읽어서 아이콘 목록/무게/상세를 보여주는 탭
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : TabView {
    [Header("목록")]
    public Transform content; // 아이템 줄이 붙을 부모
    public GameObject rowPrefab; // 아이템 한 줄의 프리팹
    public Text emptyText; // 아이템이 없을 때 보여줄 안내

    [Header("무게")]
    public RectTransform weightFill; // 무게 막대의 채움 부분
    public Text weightText; // 현재 무게 / 최대 무게

    [Header("상세")]
    public GameObject detailRoot; // 아무것도 고르지 않았을 때 숨길 영역
    public Image detailIcon;
    public Text detailName;
    public Text detailDescription;
    public Text detailStats;
    public Button useButton;
    public Button equipButton;
    public Button dropButton;

    private Inventory inventory; // 표시 대상 인벤토리
    private Equipment equipment; // 장착 버튼이 쓸 장비
    private GameObject inventoryOwner; // 아이템 사용 효과가 적용될 대상

    // 고른 아이템은 순번이 아니라 정의로 기억한다.
    // 목록은 사용/버리기 때마다 다시 그려져서 순번이 밀리기 때문이다
    private ItemData selected;

    private void Start() {
        inventory = FindFirstObjectByType<Inventory>();

        if (inventory != null)
        {
            inventoryOwner = inventory.gameObject;
            equipment = inventory.GetComponent<Equipment>();
            inventory.onChanged += Refresh;
        }

        if (equipment != null)
        {
            equipment.onChanged += Refresh;
        }

        useButton.onClick.AddListener(UseSelected);
        equipButton.onClick.AddListener(EquipSelected);
        dropButton.onClick.AddListener(DropSelected);
    }

    private void OnDestroy() {
        if (inventory != null)
        {
            inventory.onChanged -= Refresh;
        }

        if (equipment != null)
        {
            equipment.onChanged -= Refresh;
        }
    }

    public override void Refresh() {
        // 보이지 않는 동안에는 다시 그릴 필요가 없다
        if (inventory == null || !isVisible)
        {
            return;
        }

        ClearRows(content);

        for (int i = 0; i < inventory.items.Count; i++)
        {
            CreateRow(inventory.items[i]);
        }

        emptyText.gameObject.SetActive(inventory.items.Count == 0);

        RefreshWeight();
        RefreshDetail();
    }

    // 아이템 한 줄: 아이콘 + 이름 + 개수 + 무게, 누르면 상세에 표시된다
    private void CreateRow(ItemStack stack) {
        GameObject row = Instantiate(rowPrefab, content);
        bool isSelected = stack.data == selected;

        row.GetComponent<Image>().color = isSelected
            ? UiTheme.RowSelected
            : UiTheme.RowBackground;

        row.transform.Find("Icon Slot/Icon").GetComponent<Image>().sprite = stack.data.icon;
        row.transform.Find("Name").GetComponent<Text>().text = stack.data.displayName;
        row.transform.Find("Detail").GetComponent<Text>().text = $"{stack.totalWeight:F2} kg";

        Text count = row.transform.Find("Count").GetComponent<Text>();
        count.text = stack.count > 1 ? $"x{stack.count}" : "";

        ItemData data = stack.data;
        row.GetComponent<Button>().onClick.AddListener(() => {
            selected = data;
            Refresh();
        });
    }

    private void RefreshWeight() {
        float ratio = inventory.maxWeight > 0f
            ? Mathf.Clamp01(inventory.totalWeight / inventory.maxWeight)
            : 0f;

        // 막대는 오른쪽 앵커를 비율만큼만 늘려서 채운다 (스프라이트 없이도 동작한다)
        weightFill.anchorMax = new Vector2(ratio, 1f);
        weightFill.GetComponent<Image>().color = UiTheme.BarColorForFill(
            inventory.isOverweight ? 1f : ratio);

        weightText.text = $"{inventory.totalWeight:F2} / {inventory.maxWeight:F1} kg"
            + (inventory.isOverweight ? "   과적" : "");
        weightText.color = inventory.isOverweight ? UiTheme.TextDanger : UiTheme.TextPrimary;
    }

    private void RefreshDetail() {
        // 목록에서 사라진 아이템을 고른 상태로 남겨두지 않는다
        if (selected != null && inventory.CountOf(selected) <= 0)
        {
            selected = null;
        }

        detailRoot.SetActive(selected != null);

        if (selected == null)
        {
            return;
        }

        detailIcon.sprite = selected.icon;
        detailName.text = selected.displayName;
        detailDescription.text = selected.description;
        detailStats.text = DescribeStats(selected);

        useButton.interactable = selected is ConsumableItemData;
        equipButton.interactable = equipment != null
            && Equipment.TryGetSlot(selected, out EquipmentSlot slot)
            && equipment.Get(slot) != selected;
    }

    // 상세에 보여줄 수치 요약
    private string DescribeStats(ItemData item) {
        string stats = $"무게 {item.weight:F2} kg   보유 {inventory.CountOf(item)}개";

        if (item is ArmorItemData armor)
        {
            return $"방어 +{armor.armor}\n{stats}";
        }

        if (item is ConsumableItemData consumable)
        {
            return $"{consumable.DescribeEffect()}\n{stats}";
        }

        if (item is WeaponItemData weapon)
        {
            return $"{SlotName(weapon.slot)} 무기\n{stats}";
        }

        return stats;
    }

    private static string SlotName(EquipmentSlot slot) {
        switch (slot)
        {
            case EquipmentSlot.PrimaryWeapon: return "주무기";
            case EquipmentSlot.SecondaryWeapon: return "보조무기";
            case EquipmentSlot.Melee: return "근접";
            case EquipmentSlot.Head: return "머리";
            case EquipmentSlot.Chest: return "상체";
            default: return "하체";
        }
    }

    // 고른 아이템의 인벤토리 순번 (없으면 -1)
    private int IndexOfSelected() {
        for (int i = 0; i < inventory.items.Count; i++)
        {
            if (inventory.items[i].data == selected)
            {
                return i;
            }
        }

        return -1;
    }

    private void UseSelected() {
        int index = IndexOfSelected();

        if (index >= 0)
        {
            inventory.Use(index, inventoryOwner);
        }
    }

    private void EquipSelected() {
        if (selected != null && equipment != null)
        {
            equipment.Equip(selected);
        }
    }

    private void DropSelected() {
        int index = IndexOfSelected();

        if (index >= 0)
        {
            inventory.DropAt(index, 1);
        }
    }
}
