// 월드 생성에 필요한 설정값 모음 (같은 설정 + 같은 시드 = 같은 월드)
using System;
using UnityEngine;

[Serializable]
public class WorldGenerationSettings {
    [Header("시드")]
    public int worldSeed = 12345; // 월드를 결정하는 시드
    public int generatorVersion = 1; // 생성 규칙이 바뀌면 올린다. 시드가 같아도 결과가 달라진다

    [Header("크기")]
    public int chunksX = 20; // 가로 청크 수
    public int chunksZ = 20; // 세로 청크 수
    public float chunkSize = 50f; // 청크 한 변의 길이(m)

    [Header("도로")]
    // 간격이 좁으면 월드의 절반이 도로가 된다. 3~6으로 두면 도로가 36~48%를 차지했다
    [Range(3, 10)] public int minRoadSpacing = 4; // 도로 간격의 최소값
    [Range(3, 12)] public int maxRoadSpacing = 8; // 도로 간격의 최대값

    [Header("배치 가중치")]
    [Range(0f, 1f)] public float cityChance = 0.55f; // 도로에 붙은 칸이 도시가 될 확률
    [Range(0f, 1f)] public float mountainChance = 0.35f; // 도로에서 먼 칸이 산이 될 확률
    public int poiCount = 3; // 배치할 대형 POI 수
    public int poiMinDistance = 5; // POI끼리 떨어져야 하는 최소 격자 거리

    // 월드 전체의 월드 좌표 크기
    public float worldWidth => chunksX * chunkSize;
    public float worldDepth => chunksZ * chunkSize;
}
