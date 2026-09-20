// 저장 파일에 실제로 담기는 순수 데이터 (각 시스템의 내부 객체를 직접 직렬화하지 않는다)
using System;
using System.Collections.Generic;
using UnityEngine;

// 아이템 묶음 하나 (ItemData 참조 대신 id로 담는다)
[Serializable]
public class ItemStackSaveData {
    public string itemId;
    public int count;
}

// 장비 슬롯 하나
[Serializable]
public class EquipmentSaveData {
    public EquipmentSlot slot;
    public string itemId;
}

// 보관 상자 하나의 내용물 (어느 건설물의 상자인지는 건설물 순번으로 가리킨다)
[Serializable]
public class StorageSaveData {
    public int buildingIndex; // SaveData.buildings 안에서의 순번
    public List<ItemStackSaveData> items = new List<ItemStackSaveData>();
}

// 플레이어의 상태
[Serializable]
public class PlayerSaveData {
    public Vector3 position;
    public float rotationY;
    public float health;
    public int score;
}

// 저장 슬롯 하나의 전체 내용
[Serializable]
public class SaveData {
    public int saveVersion; // 저장 형식의 버전 (SaveSystem.SaveVersion)
    public string savedAtUtc; // 저장 시각 (ISO 8601 UTC)
    public bool isAuto; // 자동저장인지
    public float playTime; // 누적 플레이 시간(초)

    public int worldSeed;
    public int generatorVersion;

    public PlayerSaveData player = new PlayerSaveData();
    public List<ItemStackSaveData> inventory = new List<ItemStackSaveData>();
    public List<EquipmentSaveData> equipment = new List<EquipmentSaveData>();
    public List<PlacedBuilding> buildings = new List<PlacedBuilding>();
    public List<StorageSaveData> storages = new List<StorageSaveData>();
    public MotorcycleSaveData motorcycle = new MotorcycleSaveData();
    public WorldRuntimeSaveData worldRuntime = new WorldRuntimeSaveData();

    // 슬롯 목록에 보여줄 한 줄 요약
    public string Summarize() {
        var time = TimeSpan.FromSeconds(playTime);
        return $"시드 {worldSeed} | {time.Hours}시간 {time.Minutes}분"
            + $" | 아이템 {inventory.Count}종 | 건설물 {buildings.Count}개";
    }
}
