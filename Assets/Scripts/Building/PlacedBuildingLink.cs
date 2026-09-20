// 씬에 놓인 건설물 오브젝트와 그 런타임 기록(PlacedBuilding)을 이어주는 연결고리
using UnityEngine;

public class PlacedBuildingLink : MonoBehaviour {
    // BaseBuildState.Place가 채워준다. 문 같은 건설물이 자기 상태를 기록에 반영할 때 쓴다
    public PlacedBuilding record { get; set; }
}
