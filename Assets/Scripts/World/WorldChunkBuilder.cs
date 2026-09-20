// 청크 데이터 한 칸을 실제 게임 오브젝트로 짓는다
// 에셋 프리팹의 피벗이 제각각이라(대부분 모서리 기준) 좌표를 하드코딩하지 않고
// 렌더러 bounds에서 배치를 계산한다
using UnityEngine;

public static class WorldChunkBuilder {
    // chunk 한 칸을 parent 아래에 짓고 루트 오브젝트를 돌려준다
    public static GameObject Build(
        WorldChunkData chunk, ChunkLibrary library, Transform parent,
        Vector3 center, float chunkSize) {
        var root = new GameObject($"Chunk {chunk.x},{chunk.z} {chunk.type}");
        root.transform.SetParent(parent, false);
        root.transform.position = center;

        // 칸 시드로 만든 난수. 같은 칸은 항상 같은 배치가 나온다
        var random = new System.Random(chunk.chunkSeed);

        PlaceGround(chunk, library, root.transform, chunkSize, random);

        // 바닥 타일은 평평하지 않다. 그 위에 올릴 것들이 실제 지면 높이를 읽을 수 있도록
        // 물리 상태를 먼저 갱신한다 (Instantiate 직후에는 콜라이더가 아직 반영되지 않는다)
        Physics.SyncTransforms();

        switch (chunk.type)
        {
            case ChunkType.Road:
                PlaceRoad(chunk, library, root.transform, chunkSize);
                break;
            case ChunkType.City:
                PlaceBuilding(library, root.transform, random);
                break;
            case ChunkType.Mountain:
                Scatter(library.rockPrefabs, library.rockMin, library.rockMax,
                    root.transform, chunkSize, random);
                break;
            case ChunkType.POI:
                PlacePoi(library, root.transform, chunkSize);
                break;
            default:
                Scatter(library.plainPropPrefabs, library.plainPropMin, library.plainPropMax,
                    root.transform, chunkSize, random);
                break;
        }

        return root;
    }

    // 바닥 타일은 칸을 가득 채우고 윗면이 y=0에 오게 놓는다
    private static void PlaceGround(
        WorldChunkData chunk, ChunkLibrary library, Transform parent,
        float chunkSize, System.Random random) {
        GameObject prefab = Pick(library.groundPrefabs, random);

        if (prefab == null)
        {
            return;
        }

        Place(prefab, parent, Vector3.zero, 0f, FitScale(prefab, chunkSize), VerticalAlign.TopAtZero);
    }

    // 도로는 이어지는 방향에 맞는 조각을 고르고 회전시킨다
    private static void PlaceRoad(
        WorldChunkData chunk, ChunkLibrary library, Transform parent, float chunkSize) {
        int connections =
            (chunk.roadNorth ? 1 : 0) + (chunk.roadEast ? 1 : 0)
            + (chunk.roadSouth ? 1 : 0) + (chunk.roadWest ? 1 : 0);

        GameObject prefab;
        float yaw;

        if (connections >= 3)
        {
            // 세 갈래 이상은 십자로 덮는다 (T자 조각이 에셋에 없다)
            prefab = library.roadCross;
            yaw = 0f;
        }
        else if (connections == 2 && chunk.roadNorth == chunk.roadSouth)
        {
            // 마주보는 두 방향 = 직선. 직선 조각의 긴 축은 Z다
            prefab = library.roadStraight;
            yaw = chunk.roadNorth ? 0f : 90f;
        }
        else if (connections == 2)
        {
            // 이웃한 두 방향 = 모퉁이
            prefab = library.roadCorner;
            yaw = CornerYaw(chunk);
        }
        else
        {
            // 막다른 길이나 외딴 도로도 직선으로 덮는다
            prefab = library.roadStraight;
            yaw = (chunk.roadNorth || chunk.roadSouth) ? 0f : 90f;
        }

        if (prefab == null)
        {
            return;
        }

        Place(prefab, parent, Vector3.zero, yaw, FitScale(prefab, chunkSize), VerticalAlign.TopOnGround);
    }

    // 모퉁이 조각의 기본 방향을 북+서로 보고 나머지를 90도씩 돌린다
    private static float CornerYaw(WorldChunkData chunk) {
        if (chunk.roadNorth && chunk.roadWest) return 0f;
        if (chunk.roadNorth && chunk.roadEast) return 90f;
        if (chunk.roadSouth && chunk.roadEast) return 180f;
        return 270f;
    }

    // 건물은 칸을 넘지 않도록 가로/세로와 높이 둘 다로 축소한다
    private static void PlaceBuilding(
        ChunkLibrary library, Transform parent, System.Random random) {
        GameObject prefab = Pick(library.buildingPrefabs, random);

        if (prefab == null)
        {
            return;
        }

        Bounds bounds = GetBounds(prefab);
        float footprint = Mathf.Max(bounds.size.x, bounds.size.z);

        // 가로/세로 제한과 높이 제한 중 더 빡빡한 쪽을 따른다
        float scale = Mathf.Min(
            library.buildingMaxFootprint / Mathf.Max(footprint, 0.01f),
            library.buildingMaxHeight / Mathf.Max(bounds.size.y, 0.01f));

        Place(prefab, parent, Vector3.zero, random.Next(0, 4) * 90f, scale, VerticalAlign.BottomOnGround);
    }

    private static void PlacePoi(ChunkLibrary library, Transform parent, float chunkSize) {
        if (library.poiPrefab == null)
        {
            return;
        }

        Place(library.poiPrefab, parent, Vector3.zero, 0f,
            FitScale(library.poiPrefab, library.poiFootprint), VerticalAlign.BottomOnGround);
    }

    // 칸 안에 여러 개를 흩뿌린다
    private static void Scatter(
        GameObject[] prefabs, int min, int max, Transform parent,
        float chunkSize, System.Random random) {
        if (prefabs == null || prefabs.Length == 0 || max <= 0)
        {
            return;
        }

        int count = random.Next(min, max + 1);
        float half = chunkSize * 0.5f - 4f;

        for (int i = 0; i < count; i++)
        {
            GameObject prefab = Pick(prefabs, random);

            if (prefab == null)
            {
                continue;
            }

            var offset = new Vector3(
                (float)(random.NextDouble() * 2.0 - 1.0) * half,
                0f,
                (float)(random.NextDouble() * 2.0 - 1.0) * half);

            Place(prefab, parent, offset, (float)random.NextDouble() * 360f, 1f, VerticalAlign.BottomOnGround);
        }
    }

    private enum VerticalAlign {
        TopAtZero, // 윗면을 y=0에 맞춘다 (바닥 타일)
        TopOnGround, // 윗면을 실제 지면보다 살짝 위에 맞춘다 (도로)
        BottomOnGround, // 아랫면을 실제 지면에 맞춘다 (건물, 바위, 나무)
    }

    // 바닥 타일이 평평하지 않아서 y=0을 그대로 쓰면 물체가 뜨거나 파묻힌다.
    // 해당 지점 위에서 아래로 쏴 실제 지면 높이를 찾는다
    // self는 지금 놓는 중인 오브젝트다. 이걸 빼지 않으면 자기 콜라이더를 지면으로 잡고
    // 자기 키만큼 공중에 뜬다
    private static float SampleGroundHeight(
        Vector3 worldPosition, Transform chunkRoot, Transform self) {
        var ray = new Ray(worldPosition + Vector3.up * 200f, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, 400f, ~0, QueryTriggerInteraction.Ignore);

        float best = 0f;
        bool found = false;

        foreach (RaycastHit hit in hits)
        {
            // 같은 청크의 바닥만 본다 (옆 청크나 플레이어를 딛지 않도록)
            if (!hit.transform.IsChildOf(chunkRoot))
            {
                continue;
            }

            // 자기 자신과, 앞서 놓인 나무/바위 위에는 올라가지 않는다
            if (self != null && hit.transform.IsChildOf(self))
            {
                continue;
            }

            if (!hit.collider.name.Contains("Grounds"))
            {
                continue;
            }

            if (!found || hit.point.y > best)
            {
                best = hit.point.y;
                found = true;
            }
        }

        return found ? best : 0f;
    }

    // 프리팹을 청크 안에 놓는다. 피벗이 어디든 bounds 기준으로 중심과 높이를 맞춘다
    private static void Place(
        GameObject prefab, Transform parent, Vector3 localOffset,
        float yaw, float scale, VerticalAlign align) {
        GameObject instance = Object.Instantiate(prefab, parent);
        instance.transform.localScale = Vector3.one * scale;
        instance.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        instance.transform.localPosition = Vector3.zero;

        // 회전과 스케일이 적용된 뒤의 실제 bounds로 보정값을 구한다
        Bounds bounds = GetWorldBounds(instance);

        if (bounds.size == Vector3.zero)
        {
            instance.transform.localPosition = localOffset;
            return;
        }

        Vector3 pivotWorld = instance.transform.position;
        Vector3 delta = pivotWorld - bounds.center; // 피벗이 중심에서 얼마나 떨어져 있는지

        // 먼저 수평 위치를 잡아야 그 지점의 지면 높이를 잴 수 있다
        instance.transform.localPosition = localOffset + new Vector3(delta.x, 0f, delta.z);

        float ground = align == VerticalAlign.TopAtZero
            ? 0f
            : SampleGroundHeight(
                parent.TransformPoint(localOffset), parent, instance.transform);

        float targetY = align switch {
            VerticalAlign.TopAtZero => -bounds.extents.y,
            VerticalAlign.TopOnGround => ground - parent.position.y - bounds.extents.y + 0.18f,
            _ => ground - parent.position.y + bounds.extents.y,
        };

        instance.transform.localPosition = localOffset
            + new Vector3(delta.x, delta.y + targetY, delta.z);
    }

    // 긴 축이 target에 딱 맞도록 하는 균등 스케일 (비균등 스케일은 텍스처가 늘어난다)
    private static float FitScale(GameObject prefab, float target) {
        Bounds bounds = GetBounds(prefab);
        float longest = Mathf.Max(bounds.size.x, bounds.size.z);
        return longest > 0.01f ? target / longest : 1f;
    }

    private static GameObject Pick(GameObject[] prefabs, System.Random random) {
        if (prefabs == null || prefabs.Length == 0)
        {
            return null;
        }

        return prefabs[random.Next(0, prefabs.Length)];
    }

    private static Bounds GetBounds(GameObject prefab) {
        var bounds = new Bounds();
        bool first = true;

        foreach (Renderer renderer in prefab.GetComponentsInChildren<Renderer>(true))
        {
            if (first) { bounds = renderer.bounds; first = false; }
            else { bounds.Encapsulate(renderer.bounds); }
        }

        return bounds;
    }

    private static Bounds GetWorldBounds(GameObject instance) {
        var bounds = new Bounds();
        bool first = true;

        foreach (Renderer renderer in instance.GetComponentsInChildren<Renderer>(true))
        {
            if (first) { bounds = renderer.bounds; first = false; }
            else { bounds.Encapsulate(renderer.bounds); }
        }

        return bounds;
    }
}
