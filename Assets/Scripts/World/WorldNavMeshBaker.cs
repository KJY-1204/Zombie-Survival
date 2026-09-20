// 스트리밍된 청크 영역의 NavMesh를 플레이어를 따라다니며 비동기로 다시 굽는다
// NavMeshSurface 대신 저수준 NavMeshBuilder를 쓴다.
// NavMeshSurface.UpdateNavMesh는 볼륨이 움직여도 지오메트리를 다시 수집하지 않아서
// 플레이어가 이동하면 NavMesh가 거의 빈 채로 갱신된다(정점 1160 -> 122를 확인했다)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldNavMeshBaker : MonoBehaviour {
    public WorldStreamer streamer; // 청크 맵과 기준 대상을 알려준다
    public float heightRange = 60f; // 베이크할 높이 범위
    public int chunkMargin = 1; // 로드 반경보다 이만큼 좁게 굽는다(청크 단위)
    public LayerMask includeLayers = ~0; // 수집할 레이어

    private NavMeshData navMeshData;
    private NavMeshDataInstance dataInstance;
    private readonly List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
    private AsyncOperation pendingBake;

    private Vector2Int bakedCenter = new Vector2Int(int.MinValue, int.MinValue);
    private bool needsRebuild;
    private bool hasBakedOnce; // 첫 베이크만 동기로 한다

    public bool isBaking => pendingBake != null && !pendingBake.isDone;
    public Vector2Int lastBakedCenter => bakedCenter;

    private void Awake() {
        if (streamer == null)
        {
            streamer = GetComponentInParent<WorldStreamer>();
        }

        navMeshData = new NavMeshData();
        dataInstance = NavMesh.AddNavMeshData(navMeshData);
    }

    private void OnEnable() {
        if (streamer != null)
        {
            streamer.onChunksChanged += MarkDirty;
        }
    }

    private void OnDisable() {
        if (streamer != null)
        {
            streamer.onChunksChanged -= MarkDirty;
        }
    }

    private void OnDestroy() {
        if (dataInstance.valid)
        {
            dataInstance.Remove();
        }
    }

    private void MarkDirty() {
        needsRebuild = true;
    }

    // 굽는 시점은 "청크가 다 올라온 뒤"다. 플레이어 위치만 보고 구우면
    // 아직 지어지지 않은 지형을 굽게 되어 NavMesh가 텅 빈다
    private void Update() {
        if (streamer == null || streamer.viewer == null || isBaking || !needsRebuild)
        {
            return;
        }

        needsRebuild = false;
        Rebuild();
    }

    private void Rebuild() {
        Vector2Int center = streamer.map.WorldToChunk(streamer.viewer.position);
        bakedCenter = center;

        // 가장자리 청크는 아직 지어지지 않았을 수 있으므로 로드 반경보다 조금 좁게 굽는다
        float span = streamer.map.chunkSize
            * Mathf.Max(1, (streamer.loadRadius - chunkMargin) * 2 + 1);

        Vector3 origin = streamer.map.ChunkToWorld(center.x, center.y);
        transform.position = origin;

        // NavMeshData는 원점에 identity로 붙여두었으므로 로컬 좌표가 곧 월드 좌표다.
        // 수집원도 월드 좌표로 나오므로 빌드 범위에 월드 bounds를 그대로 넘긴다.
        // 여기에 로컬 bounds(원점 기준)를 넘기면 지형과 어긋나 NavMesh가 비어 버린다
        var worldBounds = new Bounds(origin, new Vector3(span, heightRange, span));

        sources.Clear();
        NavMeshBuilder.CollectSources(
            worldBounds, includeLayers, NavMeshCollectGeometry.PhysicsColliders,
            0, new List<NavMeshBuildMarkup>(), sources);

        NavMeshBuildSettings settings = NavMesh.GetSettingsByID(0);

        // 첫 번째는 동기로 굽는다. 시작하자마자 좀비가 설 바닥이 있어야 한다
        if (!hasBakedOnce)
        {
            hasBakedOnce = true;
            NavMeshBuilder.UpdateNavMeshData(
                navMeshData, settings, sources, worldBounds);
            return;
        }

        pendingBake = NavMeshBuilder.UpdateNavMeshDataAsync(
            navMeshData, settings, sources, worldBounds);
    }

    // 검증용 요약
    public string Describe() {
        NavMeshTriangulation tri = NavMesh.CalculateTriangulation();
        return $"구운 중심={bakedCenter} 수집원={sources.Count}"
            + $" | 굽는 중={isBaking} | 정점={tri.vertices.Length} 삼각형={tri.indices.Length / 3}";
    }
}
