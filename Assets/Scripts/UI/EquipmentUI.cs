// 장비의 공개 상태와 onChanged 이벤트만 읽어서 6개 슬롯과 장착 가능한 아이템을 보여주는 화면
using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : TabView {
    public Text armorText; // 총 방어력 표시
    public Transform slotContent; // 6개 슬롯 줄이 붙을 부모
    public Transform availableContent; // 장착 가능한 인벤토리 아이템 줄이 붙을 부모
    public GameObject rowPrefab; // 줄 하나의 프리팹 (Label + Action Button)

    private Equipment equipment; // 표시 대상 장비
    private Inventory inventory; // 장착 후보를 가져올 인벤토리

    // 슬롯 순서대로의 한글 이름
    private static readonly string[] slotNames = {
        "주무기", "보조무기", "근접", "머리", "상체", "하체"
    };

    private void Start() {
        equipment = FindObjectOfType<Equipment>();

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

    // 슬롯 목록과 장착 후보 목록을 현재 상태로 다시 그린다
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

        CreateAvailableRows();
    }

    // 슬롯 한 줄: 무엇을 장착 중인지와 해제 버튼
    private void CreateSlotRow(EquipmentSlot slot) {
        GameObject row = Instantiate(rowPrefab, slotContent);
        ItemData equipped = equipment.Get(slot);

        Text label = row.transform.Find("Label").GetComponent<Text>();
        label.text = $"[{slotNames[(int)slot]}]  " + (equipped != null ? equipped.displayName : "-");

        Button action = row.transform.Find("Action Button").GetComponent<Button>();
        action.transform.Find("Text").GetComponent<Text>().text = "해제";
        action.interactable = equipped != null;
        action.onClick.AddListener(() => equipment.Unequip(slot));
    }

    // 인벤토리에 있는 아이템 중 장착 가능하고 아직 장착하지 않은 것만 후보로 보여준다
    private void CreateAvailableRows() {
        foreach (ItemStack stack in inventory.items)
        {
            if (!Equipment.TryGetSlot(stack.data, out EquipmentSlot slot)
                || equipment.Get(slot) == stack.data)
            {
                continue;
            }

            GameObject row = Instantiate(rowPrefab, availableContent);

            Text label = row.transform.Find("Label").GetComponent<Text>();
            label.text = $"{stack.data.displayName}    {DescribeItem(stack.data)}";

            Button action = row.transform.Find("Action Button").GetComponent<Button>();
            action.transform.Find("Text").GetComponent<Text>().text = "장착";

            ItemData item = stack.data;
            action.onClick.AddListener(() => equipment.Equip(item));
        }
    }

    // 후보 목록에 함께 보여줄 한 줄 설명
    private static string DescribeItem(ItemData item) {
        if (item is ArmorItemData armor)
        {
            return $"방어 +{armor.armor}   {armor.weight:F2} kg";
        }

        return $"{item.weight:F2} kg";
    }
}
