// 건설물의 변하지 않는 정적 정의를 담는 ScriptableObject (설치된 상태는 PlacedBuilding이 가진다)
using System;
using UnityEngine;

// 건설에 필요한 재료 한 종류
[Serializable]
public class BuildCost {
    public ItemData data; // 어떤 재료인지
    public int count = 1; // 몇 개 필요한지
}

[CreateAssetMenu(menuName = "Scriptable/Buildable", fileName = "Buildable")]
public class BuildableData : ScriptableObject {
    public string buildableId; // 저장과 조회에 사용할 고유 식별자
    public string displayName; // 건설 메뉴에 표시할 이름
    [TextArea] public string description; // 건설 메뉴에 표시할 설명

    public GameObject prefab; // 실제로 설치할 프리팹
    public BuildCost[] costs; // 설치에 필요한 재료

    public Vector3 checkCenter = new Vector3(0f, 1f, 0f); // 겹침 판정 상자의 중심(로컬)
    public Vector3 checkSize = new Vector3(1f, 2f, 1f); // 겹침 판정 상자의 크기

    // 필요한 재료를 전부 갖고 있는지
    public bool CanAfford(Inventory inventory) {
        if (inventory == null || costs == null)
        {
            return false;
        }

        foreach (BuildCost cost in costs)
        {
            if (cost.data == null)
            {
                continue;
            }

            if (inventory.CountOf(cost.data) < cost.count)
            {
                return false;
            }
        }

        return true;
    }

    // 필요한 재료를 실제로 차감한다. 하나라도 모자라면 아무것도 차감하지 않는다
    public bool Pay(Inventory inventory) {
        if (!CanAfford(inventory))
        {
            return false;
        }

        foreach (BuildCost cost in costs)
        {
            if (cost.data == null)
            {
                continue;
            }

            inventory.Remove(cost.data, cost.count);
        }

        return true;
    }

    // 재료를 그대로 되돌려준다 (철거용)
    public void Refund(Inventory inventory) {
        if (inventory == null || costs == null)
        {
            return;
        }

        foreach (BuildCost cost in costs)
        {
            if (cost.data == null)
            {
                continue;
            }

            inventory.Add(cost.data, cost.count);
        }
    }

    // 건설 메뉴에 표시할 재료 요약 (예: "목재 10, 돌 5")
    public string DescribeCosts() {
        if (costs == null || costs.Length == 0)
        {
            return "재료 없음";
        }

        var parts = new System.Collections.Generic.List<string>();

        foreach (BuildCost cost in costs)
        {
            if (cost.data != null)
            {
                parts.Add($"{cost.data.displayName} {cost.count}");
            }
        }

        return string.Join(", ", parts);
    }
}
