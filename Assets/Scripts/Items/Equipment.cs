// 인벤토리에 있는 아이템 중 무엇을 장착 중인지 관리한다. UI를 알지 못하고 상태와 변경 이벤트만 공개한다
using System;
using UnityEngine;

public class Equipment : MonoBehaviour {
    public ItemData[] startingItems; // 시작할 때 인벤토리에 넣고 바로 장착할 아이템

    // 장착 중인 아이템. 인덱스는 EquipmentSlot 순서와 같다
    // 아이템 자체는 인벤토리에 그대로 남아 있고, 여기서는 "무엇을 장착 중인지"만 가리킨다
    // (그래야 장착품의 무게가 두 번 세어지지 않는다)
    private readonly ItemData[] slots =
        new ItemData[Enum.GetValues(typeof(EquipmentSlot)).Length];

    private Inventory inventory; // 장착 가능 여부를 확인할 인벤토리

    public event Action onChanged; // 장착 상태가 바뀔 때 발동

    private void Awake() {
        inventory = GetComponent<Inventory>();
    }

    private void Start() {
        // 인벤토리에서 사라진 아이템을 자동으로 해제하기 위해 변경을 구독
        inventory.onChanged += ValidateEquipped;

        if (startingItems == null)
        {
            return;
        }

        foreach (ItemData item in startingItems)
        {
            if (item == null)
            {
                continue;
            }

            inventory.Add(item, 1);
            Equip(item);
        }
    }

    private void OnDestroy() {
        if (inventory != null)
        {
            inventory.onChanged -= ValidateEquipped;
        }
    }

    // 해당 슬롯에 장착 중인 아이템 (없으면 null)
    public ItemData Get(EquipmentSlot slot) {
        return slots[(int)slot];
    }

    // 이 아이템이 들어갈 수 있는 슬롯을 알려준다. 장착할 수 없는 아이템이면 false
    public static bool TryGetSlot(ItemData item, out EquipmentSlot slot) {
        if (item is WeaponItemData weapon)
        {
            slot = weapon.slot;
            return true;
        }

        if (item is ArmorItemData armor)
        {
            slot = armor.slot;
            return true;
        }

        slot = EquipmentSlot.PrimaryWeapon;
        return false;
    }

    // 인벤토리에 있는 아이템을 알맞은 슬롯에 장착한다
    public bool Equip(ItemData item) {
        if (item == null || !TryGetSlot(item, out EquipmentSlot slot))
        {
            return false;
        }

        // 인벤토리에 없는 아이템은 장착할 수 없다
        if (inventory.CountOf(item) <= 0 || slots[(int)slot] == item)
        {
            return false;
        }

        slots[(int)slot] = item;
        NotifyChanged();
        return true;
    }

    // 해당 슬롯의 장착을 해제한다 (아이템은 인벤토리에 그대로 남는다)
    public bool Unequip(EquipmentSlot slot) {
        if (slots[(int)slot] == null)
        {
            return false;
        }

        slots[(int)slot] = null;
        NotifyChanged();
        return true;
    }

    // 장착 중인 방어구의 방어력 합계
    public int totalArmor {
        get {
            int total = 0;

            foreach (ItemData item in slots)
            {
                if (item is ArmorItemData armor)
                {
                    total += armor.armor;
                }
            }

            return total;
        }
    }

    // 버리거나 다 써서 인벤토리에서 사라진 아이템은 자동으로 해제한다
    private void ValidateEquipped() {
        bool changed = false;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && inventory.CountOf(slots[i]) <= 0)
            {
                slots[i] = null;
                changed = true;
            }
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    private void NotifyChanged() {
        if (onChanged != null)
        {
            onChanged();
        }
    }
}
