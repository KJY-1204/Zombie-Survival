// 인벤토리 안에 실제로 들어있는 아이템 묶음의 런타임 상태 (정적 정의는 ItemData가 가진다)
using System;

[Serializable]
public class ItemStack {
    public ItemData data; // 어떤 아이템인지
    public int count; // 이 묶음에 들어있는 개수

    public ItemStack(ItemData data, int count) {
        this.data = data;
        this.count = count;
    }

    // 이 묶음 전체의 무게
    public float totalWeight => data.weight * count;

    // 이 묶음에 더 쌓을 수 있는 개수
    public int freeSpace => data.maxStack - count;
}
