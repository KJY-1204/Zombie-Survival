// 필드에 놓인 상자. 한 번 열면 내용물을 전부 인벤토리로 옮기고 빈 상자가 된다
using System;
using UnityEngine;

// 상자에 들어있는 아이템 한 종류
[Serializable]
public class LootEntry {
    public ItemData data; // 어떤 아이템인지
    public int count = 1; // 몇 개 들어있는지
}

public class LootContainer : MonoBehaviour {
    public string displayName = "상자"; // 상호작용 안내에 표시할 이름
    public LootEntry[] contents; // 들어있는 내용물

    public bool isEmpty { get; private set; } // 이미 열어서 비었는지

    // 내용물을 전부 inventory로 옮긴다. 실제로 열렸을 때만 true를 반환한다
    public bool Loot(Inventory inventory) {
        if (isEmpty || inventory == null || contents == null)
        {
            return false;
        }

        foreach (LootEntry entry in contents)
        {
            if (entry.data == null || entry.count <= 0)
            {
                continue;
            }

            inventory.Add(entry.data, entry.count);
        }

        // 내용물이 비어 있던 상자도 한 번 열면 다시 열리지 않는다
        isEmpty = true;
        return true;
    }
}
