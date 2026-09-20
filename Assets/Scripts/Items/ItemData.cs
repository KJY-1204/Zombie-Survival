// 아이템의 변하지 않는 정적 정의를 담는 ScriptableObject (런타임 상태는 ItemStack이 가진다)
// 이 클래스를 그대로 쓰면 효과가 없는 재료 아이템이 되고, 상속하면 소비/무기/방어구가 된다
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Item/Material", fileName = "Material Item")]
public class ItemData : ScriptableObject {
    public string itemId; // 저장과 조회에 사용할 고유 식별자
    public string displayName; // 화면에 표시할 이름
    [TextArea] public string description; // 화면에 표시할 설명
    public Sprite icon; // 목록에 표시할 아이콘
    public float weight = 0.1f; // 개당 무게(kg)
    public int maxStack = 1; // 한 묶음에 쌓을 수 있는 최대 개수
    public GameObject worldPrefab; // 인벤토리에서 버렸을 때 필드에 다시 떨어뜨릴 프리팹
}
