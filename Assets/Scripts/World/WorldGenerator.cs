// 시드와 생성기 버전으로 청크 맵을 결정론적으로 만든다 (씬 오브젝트를 만들지 않는 순수 계산)
using System.Collections.Generic;
using UnityEngine;

public static class WorldGenerator {
    // 같은 settings면 항상 같은 WorldMap이 나온다
    public static WorldMap Generate(WorldGenerationSettings settings) {
        var map = new WorldMap(settings);

        // UnityEngine.Random은 전역 상태라 호출 순서에 영향을 받는다. 자체 난수만 쓴다
        var random = new System.Random(Hash(settings.worldSeed, settings.generatorVersion, 0, 0));

        CreateChunks(map, settings);
        LayRoads(map, settings, random);
        PlaceCityAndMountain(map, settings, random);
        PlacePoi(map, settings, random);
        ResolveRoadConnections(map);
        ChooseStartChunk(map);

        return map;
    }

    // 모든 칸을 초원으로 채우고 칸마다 고유한 시드를 심는다
    private static void CreateChunks(WorldMap map, WorldGenerationSettings settings) {
        for (int z = 0; z < map.chunksZ; z++)
        {
            for (int x = 0; x < map.chunksX; x++)
            {
                var chunk = new WorldChunkData(x, z);
                chunk.type = ChunkType.Plain;

                // 칸 시드는 좌표에서 직접 뽑는다. 생성 순서가 바뀌어도 같은 값이 나온다
                chunk.chunkSeed = Hash(
                    settings.worldSeed, settings.generatorVersion, x, z);

                map.Set(chunk);
            }
        }
    }

    // 일정 간격의 가로/세로 도로를 깐다. 격자로 깔기 때문에 도로망은 항상 연결된다
    private static void LayRoads(
        WorldMap map, WorldGenerationSettings settings, System.Random random) {
        int minSpacing = Mathf.Max(2, settings.minRoadSpacing);
        int maxSpacing = Mathf.Max(minSpacing, settings.maxRoadSpacing);

        for (int x = random.Next(0, minSpacing); x < map.chunksX;
            x += random.Next(minSpacing, maxSpacing + 1))
        {
            for (int z = 0; z < map.chunksZ; z++)
            {
                map.Get(x, z).type = ChunkType.Road;
            }
        }

        for (int z = random.Next(0, minSpacing); z < map.chunksZ;
            z += random.Next(minSpacing, maxSpacing + 1))
        {
            for (int x = 0; x < map.chunksX; x++)
            {
                map.Get(x, z).type = ChunkType.Road;
            }
        }
    }

    // 도로에 붙은 칸은 도시, 도로에서 먼 칸은 산이 될 수 있다
    private static void PlaceCityAndMountain(
        WorldMap map, WorldGenerationSettings settings, System.Random random) {
        foreach (WorldChunkData chunk in map.All())
        {
            if (chunk.type != ChunkType.Plain)
            {
                continue;
            }

            bool nextToRoad = HasNeighborOfType(map, chunk.x, chunk.z, ChunkType.Road);
            double roll = random.NextDouble();

            if (nextToRoad)
            {
                if (roll < settings.cityChance)
                {
                    chunk.type = ChunkType.City;
                }
            }
            else if (roll < settings.mountainChance)
            {
                chunk.type = ChunkType.Mountain;
            }
        }
    }

    // 도로에 붙어 있고 서로 충분히 떨어진 칸에 대형 POI를 놓는다
    private static void PlacePoi(
        WorldMap map, WorldGenerationSettings settings, System.Random random) {
        var candidates = new List<WorldChunkData>();

        foreach (WorldChunkData chunk in map.All())
        {
            // 도로 자체를 덮으면 길이 끊기므로 도로 옆 칸만 후보로 본다
            if (chunk.type != ChunkType.Road
                && HasNeighborOfType(map, chunk.x, chunk.z, ChunkType.Road))
            {
                candidates.Add(chunk);
            }
        }

        // 후보 순서를 시드 기반으로 섞는다 (Fisher-Yates)
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        var placed = new List<WorldChunkData>();

        foreach (WorldChunkData candidate in candidates)
        {
            if (placed.Count >= settings.poiCount)
            {
                break;
            }

            bool tooClose = false;

            foreach (WorldChunkData other in placed)
            {
                int distance = Mathf.Abs(candidate.x - other.x)
                    + Mathf.Abs(candidate.z - other.z);

                if (distance < settings.poiMinDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                candidate.type = ChunkType.POI;
                placed.Add(candidate);
                ClearAroundPoi(map, candidate);
            }
        }
    }

    // 대형 POI는 한 칸보다 크다. 주변 칸을 비워 건물이나 바위와 겹치지 않게 한다
    // 도로는 비우지 않는다. 비우면 길이 끊겨 연결성 검사가 깨진다
    private static void ClearAroundPoi(WorldMap map, WorldChunkData poi) {
        for (int dz = -1; dz <= 1; dz++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                WorldChunkData neighbor = map.Get(poi.x + dx, poi.z + dz);

                if (neighbor == null || neighbor == poi
                    || neighbor.type == ChunkType.Road
                    || neighbor.type == ChunkType.POI)
                {
                    continue;
                }

                neighbor.type = ChunkType.Plain;
            }
        }
    }

    // 도로 청크가 어느 방향 이웃과 이어지는지 기록한다 (프리팹 선택과 회전에 쓴다)
    private static void ResolveRoadConnections(WorldMap map) {
        foreach (WorldChunkData chunk in map.All())
        {
            if (chunk.type != ChunkType.Road)
            {
                continue;
            }

            chunk.roadNorth = IsRoad(map, chunk.x, chunk.z + 1);
            chunk.roadEast = IsRoad(map, chunk.x + 1, chunk.z);
            chunk.roadSouth = IsRoad(map, chunk.x, chunk.z - 1);
            chunk.roadWest = IsRoad(map, chunk.x - 1, chunk.z);
        }
    }

    // 시작 칸은 월드 중앙에서 가장 가까운 도로로 정한다
    private static void ChooseStartChunk(WorldMap map) {
        var center = new Vector2Int(map.chunksX / 2, map.chunksZ / 2);
        WorldChunkData best = null;
        int bestDistance = int.MaxValue;

        foreach (WorldChunkData chunk in map.All())
        {
            if (chunk.type != ChunkType.Road)
            {
                continue;
            }

            int distance = Mathf.Abs(chunk.x - center.x) + Mathf.Abs(chunk.z - center.y);

            if (distance < bestDistance)
            {
                best = chunk;
                bestDistance = distance;
            }
        }

        map.startChunk = best != null
            ? new Vector2Int(best.x, best.z)
            : center;
    }

    // 시작 칸에서 지나갈 수 있는 칸을 모두 훑어 연결성을 검사한다
    // (`CLAUDE.md` §11.7 - 시작 지점과 필수 경로가 막히지 않아야 한다)
    public static WorldConnectivityReport CheckConnectivity(WorldMap map) {
        var report = new WorldConnectivityReport();
        var visited = new HashSet<int>();
        var queue = new Queue<Vector2Int>();

        queue.Enqueue(map.startChunk);
        visited.Add(map.startChunk.y * map.chunksX + map.startChunk.x);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            WorldChunkData chunk = map.Get(current.x, current.y);

            if (chunk == null)
            {
                continue;
            }

            report.reachableCount++;

            if (chunk.type == ChunkType.Road)
            {
                report.reachableRoads++;
            }
            else if (chunk.type == ChunkType.POI)
            {
                report.reachablePoi++;
            }

            foreach (Vector2Int offset in Neighbors)
            {
                var next = new Vector2Int(current.x + offset.x, current.y + offset.y);
                WorldChunkData neighbor = map.Get(next.x, next.y);

                if (neighbor == null || !neighbor.isPassable)
                {
                    continue;
                }

                int key = next.y * map.chunksX + next.x;

                if (visited.Add(key))
                {
                    queue.Enqueue(next);
                }
            }
        }

        foreach (WorldChunkData chunk in map.All())
        {
            if (chunk.type == ChunkType.Road)
            {
                report.totalRoads++;
            }
            else if (chunk.type == ChunkType.POI)
            {
                report.totalPoi++;
            }
        }

        return report;
    }

    private static readonly Vector2Int[] Neighbors = {
        new Vector2Int(0, 1), new Vector2Int(1, 0),
        new Vector2Int(0, -1), new Vector2Int(-1, 0),
    };

    private static bool IsRoad(WorldMap map, int x, int z) {
        WorldChunkData chunk = map.Get(x, z);
        return chunk != null && chunk.type == ChunkType.Road;
    }

    private static bool HasNeighborOfType(
        WorldMap map, int x, int z, ChunkType type) {
        foreach (Vector2Int offset in Neighbors)
        {
            WorldChunkData neighbor = map.Get(x + offset.x, z + offset.y);

            if (neighbor != null && neighbor.type == type)
            {
                return true;
            }
        }

        return false;
    }

    // 좌표와 시드를 섞어 안정적인 정수를 만든다 (플랫폼/실행에 상관없이 같은 값)
    private static int Hash(int seed, int version, int x, int z) {
        unchecked
        {
            int h = 17;
            h = h * 486187739 + seed;
            h = h * 486187739 + version;
            h = h * 486187739 + x;
            h = h * 486187739 + z;
            h ^= h >> 13;
            h *= 1274126177;
            h ^= h >> 16;
            return h & 0x7FFFFFFF;
        }
    }
}
