// 장비의 공개 상태와 onChanged 이벤트만 읽어서 6개 슬롯과 장착 후보를 보여주는 탭
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : TabView {
    public Transform slotContent; // 장착 중인 슬롯 줄이 붙을 부모
    public Transform availableContent; // 장착 가능한 아이템 줄이 붙을 부모
    public GameObject rowPrefab; // 아이템 줄 프리팹 (인벤토리와 같은 것을 쓴다)
    public Text armorText; // 총 방어력
    public Text availableEmptyText; // 장착할 것이 없을 때의 안내

    private Equipment equipment; // 표시 대상 장비
    private Inventory inventory; // 장착 후보를 가져올 인벤토리

    // 슬롯 순서대로의 한글 이름
    private static readonly string[] slotNames = {
        "주무기", "보조무기", "근접", "머리", "상체", "하체"
    };

    private void Start() {
        equipment = FindFirstObjectByType<Equipment>();

        if (equipment != null)
        {
            inventory = equipment.GetComponent<Inventory>();
            equipment.onChanged += Refresh;
            inventory.onChanged += Refresh;
        }
    }

    private void OnDestroy() {
        if (equipment != null)
        {
            equipment.onChanged -= Refresh;
        }

        if (inventory != null)
        {
            inventory.onChanged -= Refresh;
        }
    }

    public override void Refresh() {
        if (equipment == null || !isVisible)
        {
            return;
        }

        armorText.text = $"총 방어력 {equipment.totalArmor}";

        ClearRows(slotContent);
        ClearRows(availableContent);

        for (int i = 0; i < slotNames.Length; i++)
        {
            CreateSlotRow((EquipmentSlot)i);
        }

        availableEmptyText.gameObject.SetActive(CreateAvailableRows() == 0);
    }

    // 슬롯 한 줄: 무엇을 장착 중인지. 누르면 해제된다
    private void CreateSlotRow(EquipmentSlot slot) {
        ItemData equipped = equipment.Get(slot);
        GameObject row = Instantiate(rowPrefab, slotContent);

        row.GetComponent<Image>().color = equipped != null
            ? UiTheme.RowSelected
            : UiTheme.RowBackground;

        Image icon = row.transform.Find("Icon Slot/Icon").GetComponent<Image>();
        icon.sprite = equipped != null ? equipped.icon : null;
        icon.enabled = equipped != null;

        row.transform.Find("Name").GetComponent<Text>().text =
            equipped != null ? equipped.displayName : "비어 있음";
        row.transform.Find("Detail").GetComponent<Text>().text = slotNames[(int)slot];

        Text action = row.transform.Find("Count").GetComponent<Text>();
        action.text = equipped != null ? "해제" : "";

        row.GetComponent<Button>().interactable = equipped != null;
        row.GetComponent<Button>().onClick.AddListener(() => equipment.Unequip(slot));
    }

    // 인벤토리에 있는 아이템 중 장착 가능하고 아직 장착하지 않은 것만 후보로 보여준다
    private int CreateAvailableRows() {
        int count = 0;

        foreach (ItemStack stack in inventory.items)
        {
            if (!Equipment.TryGetSlot(stack.data, out EquipmentSlot slot)
                || equipment.Get(slot) == stack.data)
            {
                continue;
            }

            GameObject row = Instantiate(rowPrefab, availableContent);
            row.GetComponent<Image>().color = UiTheme.RowBackground;
            row.transform.Find("Icon Slot/Icon").GetComponent<Image>().sprite = stack.data.icon;
            row.transform.Find("Name").GetComponent<Text>().text = stack.data.displayName;
            row.transform.Find("Detail").GetComponent<Text>().text = DescribeItem(stack.data, slot);
            row.transform.Find("Count").GetComponent<Text>().text = "장착";

            ItemData item = stack.data;
            row.GetComponent<Button>().onClick.AddListener(() => equipment.Equip(item));
            count++;
        }

        return count;
    }

    // 후보 목록에 함께 보여줄 한 줄 설명
    private static string DescribeItem(ItemData item, EquipmentSlot slot) {
        if (item is ArmorItemData armor)
        {
            return $"{slotNames[(int)slot]}   방어 +{armor.armor}   {armor.weight:F2} kg";
        }

        return $"{slotNames[(int)slot]}   {item.weight:F2} kg";
    }
}
