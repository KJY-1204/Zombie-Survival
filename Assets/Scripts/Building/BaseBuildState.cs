// 설치된 건설물 목록의 단일 소유자 (월드 초기 배치와 분리된 런타임 변경 상태)
using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseBuildState : MonoBehaviour {
    public static BaseBuildState instance { get; private set; }

    public BuildableData[] catalog; // 건설 메뉴에 나올 건설물 목록

    private readonly List<PlacedBuilding> placed = new List<PlacedBuilding>();

    public IReadOnlyList<PlacedBuilding> buildings => placed; // 외부에서는 읽기만 가능
    public event Action onChanged; // 설치나 철거가 일어날 때 발동

    private void Awake() {
        instance = this;
    }

    // id로 건설물 정의를 찾는다 (저장에서 불러올 때도 이 경로를 쓴다)
    public BuildableData Find(string buildableId) {
        if (catalog == null)
        {
            return null;
        }

        foreach (BuildableData data in catalog)
        {
            if (data != null && data.buildableId == buildableId)
            {
                return data;
            }
        }

        return null;
    }

    // 건설물을 실제로 생성하고 목록에 등록한다
    public PlacedBuilding Place(BuildableData data, Vector3 position, float rotationY) {
        if (data == null || data.prefab == null)
        {
            return null;
        }

        var record = new PlacedBuilding(data.buildableId, position, rotationY);
        record.instance = Instantiate(
            data.prefab, position, Quaternion.Euler(0f, rotationY, 0f), transform);

        // 건설물 쪽에서 자기 기록을 알 수 있게 연결한다 (문의 열림 상태 반영 등)
        var building = record.instance.GetComponent<PlacedBuildingLink>();
        if (building != null)
        {
            building.record = record;
        }

        placed.Add(record);
        NotifyChanged();
        return record;
    }

    // 건설물을 철거하고 목록에서 제거한다
    public bool Remove(PlacedBuilding record) {
        if (record == null || !placed.Remove(record))
        {
            return false;
        }

        if (record.instance != null)
        {
            Destroy(record.instance);
        }

        NotifyChanged();
        return true;
    }

    // 씬 오브젝트로부터 해당 기록을 찾는다 (철거 대상 지정용)
    public PlacedBuilding FindByInstance(GameObject instance) {
        foreach (PlacedBuilding record in placed)
        {
            if (record.instance == instance)
            {
                return record;
            }
        }

        return null;
    }

    // 설치 상태 전체를 JSON으로 덤프한다
    // PlacedBuilding의 instance는 [NonSerialized]라 빠지므로, 나온 결과가 곧 M7 저장 DTO의 형태다
    public string ToJson() {
        return JsonUtility.ToJson(new SaveSnapshot { buildings = placed }, true);
    }

    [Serializable]
    private class SaveSnapshot {
        public List<PlacedBuilding> buildings;
    }

    private void NotifyChanged() {
        if (onChanged != null)
        {
            onChanged();
        }
    }
}
