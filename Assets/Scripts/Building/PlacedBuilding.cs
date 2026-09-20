// 설치된 건설물 하나의 런타임 상태 (MonoBehaviour가 아니라 저장 DTO로 그대로 옮길 수 있는 순수 데이터)
using System;
using UnityEngine;

[Serializable]
public class PlacedBuilding {
    public string buildableId; // 어떤 BuildableData로 지었는지
    public Vector3 position; // 설치 위치
    public float rotationY; // 설치 회전(Y축만)
    public bool isOpen; // 문 같은 여닫이 건설물의 열림 상태

    [NonSerialized] public GameObject instance; // 씬에 실제로 놓인 오브젝트 (저장 대상이 아니다)

    public PlacedBuilding(string buildableId, Vector3 position, float rotationY) {
        this.buildableId = buildableId;
        this.position = position;
        this.rotationY = rotationY;
        this.isOpen = false;
    }
}
