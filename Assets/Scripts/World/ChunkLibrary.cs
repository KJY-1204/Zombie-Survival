// 청크를 지을 때 쓸 프리팹 모음 (어떤 타입에 무엇을 놓을지의 정적 정의)
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/World/Chunk Library", fileName = "Chunk Library")]
public class ChunkLibrary : ScriptableObject {
    [Header("바닥")]
    public GameObject[] groundPrefabs; // 모든 청크 아래 깔 바닥 타일

    [Header("도로")]
    public GameObject roadStraight; // 마주보는 두 방향을 잇는다
    public GameObject roadCorner; // 이웃한 두 방향을 잇는다 (L자)
    public GameObject roadCross; // 세 방향 이상을 잇는다

    [Header("도시")]
    public GameObject[] buildingPrefabs; // 도시 청크에 세울 건물
    public float buildingMaxFootprint = 34f; // 건물이 차지할 최대 가로/세로(m)
    public float buildingMaxHeight = 40f; // 건물의 최대 높이(m). 에셋에는 120m짜리도 있다

    [Header("산")]
    public GameObject[] rockPrefabs; // 산 청크에 흩뿌릴 바위
    public int rockMin = 3;
    public int rockMax = 6;

    [Header("초원")]
    public GameObject[] plainPropPrefabs; // 초원에 드문드문 놓을 것 (나무 등)
    public int plainPropMin = 0;
    public int plainPropMax = 3;

    [Header("POI")]
    public GameObject poiPrefab; // 대형 거점
    // 한 칸(50m)보다 크게 둔다. 칸에 맞춰 줄이면 도시 건물보다 작아져 "대형"이 아니게 된다
    // 생성기가 POI 주변 칸을 비우므로 넘쳐도 겹치지 않는다
    public float poiFootprint = 85f;
}
