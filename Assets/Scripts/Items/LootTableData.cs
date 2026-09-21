// 상자에서 나올 아이템의 가중치와 수량을 정의하는 정적 루팅 테이블
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LootTableEntry {
    public ItemData data;
    [Min(1)] public int minCount = 1;
    [Min(1)] public int maxCount = 1;
    [Min(0f)] public float weight = 1f;
}

[CreateAssetMenu(menuName = "Scriptable/Item/Loot Table", fileName = "Loot Table")]
public class LootTableData : ScriptableObject {
    [Min(1)] public int rolls = 1;
    public LootTableEntry[] entries;

    // 전달받은 결정론적 난수로 여러 번 뽑고, 같은 아이템은 한 항목으로 합친다
    public LootEntry[] Roll(System.Random random) {
        if (random == null || entries == null || entries.Length == 0 || rolls <= 0)
        {
            return Array.Empty<LootEntry>();
        }

        var results = new List<LootEntry>();

        for (int roll = 0; roll < rolls; roll++)
        {
            LootTableEntry selected = PickEntry(random);

            if (selected == null)
            {
                break;
            }

            int minCount = Mathf.Max(1, selected.minCount);
            int maxCount = Mathf.Max(minCount, selected.maxCount);
            int count = random.Next(minCount, maxCount + 1);
            Add(results, selected.data, count);
        }

        return results.ToArray();
    }

    private LootTableEntry PickEntry(System.Random random) {
        float totalWeight = 0f;

        foreach (LootTableEntry entry in entries)
        {
            if (entry != null && entry.data != null && entry.weight > 0f)
            {
                totalWeight += entry.weight;
            }
        }

        if (totalWeight <= 0f)
        {
            return null;
        }

        float target = (float)random.NextDouble() * totalWeight;
        float accumulated = 0f;

        foreach (LootTableEntry entry in entries)
        {
            if (entry == null || entry.data == null || entry.weight <= 0f)
            {
                continue;
            }

            accumulated += entry.weight;

            if (target < accumulated)
            {
                return entry;
            }
        }

        return null;
    }

    private static void Add(List<LootEntry> results, ItemData data, int count) {
        foreach (LootEntry result in results)
        {
            if (result.data == data)
            {
                result.count += count;
                return;
            }
        }

        results.Add(new LootEntry { data = data, count = count });
    }
}
