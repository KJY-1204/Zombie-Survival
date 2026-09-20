// 생성된 청크 격자와 그 위의 조회/좌표 변환 (순수 데이터, 씬 오브젝트를 모른다)
using System.Collections.Generic;
using UnityEngine;

public class WorldMap {
    public readonly int chunksX;
    public readonly int chunksZ;
    public readonly float chunkSize;
    public readonly int worldSeed;
    public readonly int generatorVersion;

    public Vector2Int startChunk { get; set; } // 플레이어가 시작하는 칸

    private readonly WorldChunkData[] chunks;

    public WorldMap(WorldGenerationSettings settings) {
        chunksX = settings.chunksX;
        chunksZ = settings.chunksZ;
        chunkSize = settings.chunkSize;
        worldSeed = settings.worldSeed;
        generatorVersion = settings.generatorVersion;
        chunks = new WorldChunkData[chunksX * chunksZ];
    }

    public bool Contains(int x, int z) {
        return x >= 0 && x < chunksX && z >= 0 && z < chunksZ;
    }

    public WorldChunkData Get(int x, int z) {
        return Contains(x, z) ? chunks[z * chunksX + x] : null;
    }

    public void Set(WorldChunkData chunk) {
        chunks[chunk.z * chunksX + chunk.x] = chunk;
    }

    public IEnumerable<WorldChunkData> All() {
        foreach (WorldChunkData chunk in chunks)
        {
            if (chunk != null)
            {
                yield return chunk;
            }
        }
    }

    // 격자 좌표 -> 청크 중심의 월드 좌표
    public Vector3 ChunkToWorld(int x, int z) {
        return new Vector3(
            (x + 0.5f) * chunkSize,
            0f,
            (z + 0.5f) * chunkSize);
    }

    // 월드 좌표 -> 격자 좌표
    public Vector2Int WorldToChunk(Vector3 position) {
        return new Vector2Int(
            Mathf.FloorToInt(position.x / chunkSize),
            Mathf.FloorToInt(position.z / chunkSize));
    }

    // 같은 월드인지 비교할 때 쓰는 요약 (시드 + 버전 + 모든 칸의 타입)
    public string Fingerprint() {
        var sb = new System.Text.StringBuilder();
        sb.Append(worldSeed).Append('/').Append(generatorVersion).Append(':');

        for (int z = 0; z < chunksZ; z++)
        {
            for (int x = 0; x < chunksX; x++)
            {
                WorldChunkData chunk = Get(x, z);
                sb.Append(chunk != null ? (int)chunk.type : -1);
            }
        }

        return sb.ToString();
    }
}
