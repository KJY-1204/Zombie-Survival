// 시드로 월드를 생성하고 플레이어 주변 청크만 실제로 씬에 올린다
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldStreamer : MonoBehaviour {
    public WorldGenerationSettings settings = new WorldGenerationSettings();
    public ChunkLibrary library; // 청크를 지을 때 쓸 프리팹 모음

    [Header("스트리밍")]
    public Transform viewer; // 기준이 될 대상 (보통 플레이어). 비우면 자동으로 찾는다
    public int loadRadius = 2; // 이 격자 반경 안의 청크를 올린다
    public float updateInterval = 0.25f; // 몇 초마다 주변을 다시 계산할지

    public WorldMap map { get; private set; } // 생성된 청크 맵
    public int loadedCount => loaded.Count; // 지금 씬에 올라와 있는 청크 수

    // 올라와 있는 청크 목록이 실제로 바뀌었을 때 발동
    // NavMesh를 굽는 쪽은 이 시점을 기다려야 한다. 먼저 구우면 아직 없는 지형을 굽게 된다
    public event Action onChunksChanged;

    private readonly Dictionary<Vector2Int, GameObject> loaded =
        new Dictionary<Vector2Int, GameObject>();

    private Vector2Int lastCenter = new Vector2Int(int.MinValue, int.MinValue);
    private float nextUpdateTime;

    private void Awake() {
        map = WorldGenerator.Generate(settings);
    }

    private void Start() {
        if (viewer == null)
        {
            PlayerHealth player = FindObjectOfType<PlayerHealth>();

            if (player != null)
            {
                viewer = player.transform;
            }
        }

        // 시작 지점을 도로 위로 옮겨 플레이어가 허공이나 산에 놓이지 않게 한다
        MoveViewerToStart();
        UpdateChunks(force: true);
    }

    private void Update() {
        if (viewer == null || Time.time < nextUpdateTime)
        {
            return;
        }

        nextUpdateTime = Time.time + updateInterval;
        UpdateChunks(force: false);
    }

    // 플레이어를 시작 청크의 중앙으로 옮긴다
    public void MoveViewerToStart() {
        if (viewer == null || map == null)
        {
            return;
        }

        Vector3 start = map.ChunkToWorld(map.startChunk.x, map.startChunk.y);
        viewer.position = start + Vector3.up * 1f;

        var body = viewer.GetComponent<Rigidbody>();

        if (body != null && !body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }
    }

    // 주변 청크를 올리고 멀어진 청크를 내린다
    private void UpdateChunks(bool force) {
        Vector2Int center = map.WorldToChunk(viewer.position);

        if (!force && center == lastCenter)
        {
            return;
        }

        lastCenter = center;

        // 반경 밖으로 나간 청크를 내린다
        var toUnload = new List<Vector2Int>();

        foreach (KeyValuePair<Vector2Int, GameObject> pair in loaded)
        {
            if (Mathf.Abs(pair.Key.x - center.x) > loadRadius
                || Mathf.Abs(pair.Key.y - center.y) > loadRadius)
            {
                toUnload.Add(pair.Key);
            }
        }

        bool changed = toUnload.Count > 0;

        foreach (Vector2Int key in toUnload)
        {
            Destroy(loaded[key]);
            loaded.Remove(key);
        }

        // 반경 안의 청크를 올린다
        for (int z = center.y - loadRadius; z <= center.y + loadRadius; z++)
        {
            for (int x = center.x - loadRadius; x <= center.x + loadRadius; x++)
            {
                var key = new Vector2Int(x, z);

                if (loaded.ContainsKey(key))
                {
                    continue;
                }

                WorldChunkData chunk = map.Get(x, z);

                if (chunk == null)
                {
                    continue;
                }

                loaded[key] = WorldChunkBuilder.Build(
                    chunk, library, transform,
                    map.ChunkToWorld(x, z), map.chunkSize);
                changed = true;
            }
        }

        // 청크가 다 올라온 뒤에 알린다 (NavMesh를 굽는 쪽이 이걸 기다린다)
        if (changed && onChunksChanged != null)
        {
            onChunksChanged();
        }
    }

    // 현재 상태 요약 (검증용)
    public string Describe() {
        WorldConnectivityReport report = WorldGenerator.CheckConnectivity(map);
        Vector2Int center = viewer != null
            ? map.WorldToChunk(viewer.position)
            : map.startChunk;

        return $"시드 {map.worldSeed}/v{map.generatorVersion} | {map.chunksX}x{map.chunksZ}"
            + $" | 시작 {map.startChunk} | 현재 {center}"
            + $" | 로드됨 {loaded.Count} | {report}";
    }
}
