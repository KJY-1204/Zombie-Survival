// 아이템의 변하지 않는 정적 정의를 담는 ScriptableObject (런타임 상태는 ItemStack이 가진다)
using UnityEngine;

public abstract class ItemData : ScriptableObject {
    public string itemId; // 저장과 조회에 사용할 고유 식별자
    public string displayName; // 화면에 표시할 이름
    [TextArea] public string description; // 화면에 표시할 설명
    public Sprite icon; // 목록에 표시할 아이콘
    public float weight = 0.1f; // 개당 무게(kg)
    public int maxStack = 1; // 한 묶음에 쌓을 수 있는 최대 개수
}
