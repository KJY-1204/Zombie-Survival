// 청크 한 칸의 생성 결과 (순수 데이터, 씬 오브젝트를 모른다)
using System;

[Serializable]
public class WorldChunkData {
    public int x; // 격자 좌표
    public int z;
    public ChunkType type; // 청크 종류
    public int rotationSteps; // 90도 단위 회전 (0~3)
    public int chunkSeed; // 이 칸 안의 세부 배치에 쓸 시드

    // 도로 청크가 어느 방향으로 이어지는지 (북/동/남/서)
    public bool roadNorth;
    public bool roadEast;
    public bool roadSouth;
    public bool roadWest;

    public WorldChunkData(int x, int z) {
        this.x = x;
        this.z = z;
    }

    // 이 청크를 통해 지나갈 수 있는지 (연결성 검사에 쓴다)
    public bool isPassable => type != ChunkType.Mountain;

    // 도로망의 일부인지
    public bool hasRoad => roadNorth || roadEast || roadSouth || roadWest;
}
