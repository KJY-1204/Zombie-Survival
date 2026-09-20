// 장착하면 받는 피해를 고정 수치만큼 줄여주는 방어구 아이템의 정의
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Item/Armor", fileName = "Armor Item")]
public class ArmorItemData : ItemData {
    public EquipmentSlot slot = EquipmentSlot.Head; // 장착 가능한 슬롯
    public int armor = 5; // 피해에서 차감할 방어력
}
