// 필드에 떨어져 있다가 플레이어가 밟으면 인벤토리로 들어가는 아이템 픽업
using UnityEngine;

public class WorldItem : MonoBehaviour {
    public ItemData data; // 어떤 아이템인지
    public int count = 1; // 한 번에 주워지는 개수

    // inventory에 내용물을 넣고 자신을 파괴한다
    // 실제로 주워진 경우에만 true를 반환한다
    public bool PickUp(Inventory inventory) {
        if (inventory == null || data == null || count <= 0)
        {
            return false;
        }

        inventory.Add(data, count);
        Destroy(gameObject);
        return true;
    }
}
