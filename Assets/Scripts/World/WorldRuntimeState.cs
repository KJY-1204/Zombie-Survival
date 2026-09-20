// 청크 안 콘텐츠의 런타임 변경 상태를 청크 밖에서 들고 있는다
// 스트리밍으로 청크가 내려가면 그 안의 오브젝트는 사라지므로, 채집/루팅 여부는 여기에 남아야 한다
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorldRuntimeState : MonoBehaviour {
    public static WorldRuntimeState instance { get; private set; }

    // 채집된 자원 노드의 id와 다시 자라날 시각
    private readonly Dictionary<string, float> harvested = new Dictionary<string, float>();

    // 이미 열어본 루팅 상자의 id
    private readonly HashSet<string> looted = new HashSet<string>();

    private void Awake() {
        instance = this;
    }

    // 청크 좌표와 칸 안 순번으로 만드는 안정적인 id
    // 같은 시드면 청크가 다시 만들어져도 같은 순번에 같은 오브젝트가 온다
    public static string MakeId(int chunkX, int chunkZ, string kind, int index) {
        return $"{chunkX}_{chunkZ}_{kind}_{index}";
    }

    // 자원 노드가 지금 채집된 상태인지 (재생성 시각이 지났으면 다시 자란 것으로 본다)
    public bool IsHarvested(string id) {
        if (!harvested.TryGetValue(id, out float respawnAt))
        {
            return false;
        }

        if (respawnAt > 0f && Time.time >= respawnAt)
        {
            harvested.Remove(id);
            return false;
        }

        return true;
    }

    // 다시 자라기까지 남은 시간(초). 0이면 다시 자라지 않는다
    public float RemainingRespawn(string id) {
        if (!harvested.TryGetValue(id, out float respawnAt) || respawnAt <= 0f)
        {
            return 0f;
        }

        return Mathf.Max(0f, respawnAt - Time.time);
    }

    public void MarkHarvested(string id, float respawnDelay) {
        harvested[id] = respawnDelay > 0f ? Time.time + respawnDelay : 0f;
    }

    public bool IsLooted(string id) {
        return looted.Contains(id);
    }

    public void MarkLooted(string id) {
        looted.Add(id);
    }

    // 저장용 스냅샷. 남은 재생성 시간을 상대값으로 담아 불러오기 시점이 달라도 맞는다
    public WorldRuntimeSaveData ToSaveData() {
        var data = new WorldRuntimeSaveData();

        foreach (KeyValuePair<string, float> pair in harvested)
        {
            data.harvested.Add(new HarvestedEntry {
                id = pair.Key,
                remainingRespawn = pair.Value > 0f
                    ? Mathf.Max(0f, pair.Value - Time.time)
                    : 0f,
            });
        }

        data.looted.AddRange(looted);
        return data;
    }

    public void LoadFromSaveData(WorldRuntimeSaveData data) {
        harvested.Clear();
        looted.Clear();

        if (data == null)
        {
            return;
        }

        foreach (HarvestedEntry entry in data.harvested)
        {
            harvested[entry.id] = entry.remainingRespawn > 0f
                ? Time.time + entry.remainingRespawn
                : 0f;
        }

        foreach (string id in data.looted)
        {
            looted.Add(id);
        }
    }

    // 검증용 요약
    public string Describe() {
        return $"채집됨 {harvested.Count}개 | 루팅됨 {looted.Count}개";
    }
}

// 채집된 자원 하나의 저장 형태
[Serializable]
public class HarvestedEntry {
    public string id;
    public float remainingRespawn; // 남은 재생성 시간(초). 0이면 다시 자라지 않는다
}

// 청크 콘텐츠의 런타임 상태 저장 형태 (순수 데이터)
[Serializable]
public class WorldRuntimeSaveData {
    public List<HarvestedEntry> harvested = new List<HarvestedEntry>();
    public List<string> looted = new List<string>();
}
