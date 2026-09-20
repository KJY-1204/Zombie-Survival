// 저장 파일의 id 문자열을 실제 에셋(ItemData/BuildableData)으로 되찾는 표
// 저장에는 에셋 참조 대신 id만 넣으므로 불러올 때 이 표가 없으면 복원할 수 없다
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Save Registry", fileName = "Save Registry")]
public class SaveRegistry : ScriptableObject {
    public ItemData[] items;
    public BuildableData[] buildables;

    private Dictionary<string, ItemData> itemsById;
    private Dictionary<string, BuildableData> buildablesById;

    public ItemData FindItem(string itemId) {
        if (string.IsNullOrEmpty(itemId))
        {
            return null;
        }

        if (itemsById == null)
        {
            itemsById = new Dictionary<string, ItemData>();

            foreach (ItemData item in items)
            {
                if (item != null && !string.IsNullOrEmpty(item.itemId))
                {
                    itemsById[item.itemId] = item;
                }
            }
        }

        return itemsById.TryGetValue(itemId, out ItemData found) ? found : null;
    }

    public BuildableData FindBuildable(string buildableId) {
        if (string.IsNullOrEmpty(buildableId))
        {
            return null;
        }

        if (buildablesById == null)
        {
            buildablesById = new Dictionary<string, BuildableData>();

            foreach (BuildableData buildable in buildables)
            {
                if (buildable != null && !string.IsNullOrEmpty(buildable.buildableId))
                {
                    buildablesById[buildable.buildableId] = buildable;
                }
            }
        }

        return buildablesById.TryGetValue(buildableId, out BuildableData found) ? found : null;
    }

    // 빠진 id나 중복 id가 있는지 본다 (검증용)
    public string Describe() {
        var itemIds = new HashSet<string>();
        var buildableIds = new HashSet<string>();
        int itemProblems = 0, buildableProblems = 0;

        foreach (ItemData item in items)
        {
            if (item == null || string.IsNullOrEmpty(item.itemId) || !itemIds.Add(item.itemId))
            {
                itemProblems++;
            }
        }

        foreach (BuildableData buildable in buildables)
        {
            if (buildable == null || string.IsNullOrEmpty(buildable.buildableId)
                || !buildableIds.Add(buildable.buildableId))
            {
                buildableProblems++;
            }
        }

        return $"아이템 {items.Length}개(문제 {itemProblems}) | 건설물 {buildables.Length}개(문제 {buildableProblems})";
    }
}
