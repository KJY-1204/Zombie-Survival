// 장비 슬롯에 장착해 실제로 손에 들 수 있는 무기 아이템의 정의
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Item/Weapon", fileName = "Weapon Item")]
public class WeaponItemData : ItemData {
    public EquipmentSlot slot = EquipmentSlot.PrimaryWeapon; // 장착 가능한 슬롯
    public GameObject weaponPrefab; // gunPivot 아래에 생성할 무기 프리팹
}
