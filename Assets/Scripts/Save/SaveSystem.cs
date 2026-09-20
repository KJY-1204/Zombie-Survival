// 저장 슬롯 파일을 읽고 쓴다 (경로, 원자적 쓰기, 목록, 삭제)
// 쓰기는 임시 파일에 먼저 쓰고 바꿔치기한다. 도중에 실패해도 기존 저장이 깨지지 않아야 한다
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 슬롯 하나에 들어있는 수동/자동 저장의 요약 (슬롯 UI가 쓴다)
public class SaveSlotInfo {
    public int slot;
    public SaveData manual; // 없으면 null
    public SaveData auto; // 없으면 null

    public bool isEmpty => manual == null && auto == null;
}

public static class SaveSystem {
    public const int SaveVersion = 1; // 저장 형식이 바뀌면 올린다
    public const int SlotCount = 10; // 플레이어가 볼 수 있는 슬롯 수

    public static string saveDirectory => Path.Combine(Application.persistentDataPath, "Saves");

    public static string GetPath(int slot, bool auto) {
        return Path.Combine(saveDirectory, auto ? $"slot{slot}.auto.json" : $"slot{slot}.json");
    }

    public static bool Exists(int slot, bool auto) {
        return File.Exists(GetPath(slot, auto));
    }

    // 임시 파일에 쓴 뒤 교체한다. 쓰다가 실패하면 기존 파일은 그대로 남는다
    public static bool Write(int slot, bool auto, SaveData data) {
        data.saveVersion = SaveVersion;
        data.savedAtUtc = DateTime.UtcNow.ToString("o");
        data.isAuto = auto;

        string path = GetPath(slot, auto);
        string temp = path + ".tmp";

        try
        {
            Directory.CreateDirectory(saveDirectory);
            File.WriteAllText(temp, JsonUtility.ToJson(data, true));

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Move(temp, path);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"저장 실패 (슬롯 {slot}, 자동={auto}): {exception.Message}");

            // 실패한 임시 파일은 남기지 않는다
            try
            {
                if (File.Exists(temp))
                {
                    File.Delete(temp);
                }
            }
            catch (Exception) { }

            return false;
        }
    }

    public static SaveData Read(int slot, bool auto) {
        string path = GetPath(slot, auto);

        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(path));

            if (data == null || data.saveVersion > SaveVersion)
            {
                Debug.LogError($"읽을 수 없는 저장 (슬롯 {slot}, 자동={auto}): 버전 {data?.saveVersion}");
                return null;
            }

            return data;
        }
        catch (Exception exception)
        {
            Debug.LogError($"불러오기 실패 (슬롯 {slot}, 자동={auto}): {exception.Message}");
            return null;
        }
    }

    public static bool Delete(int slot, bool auto) {
        string path = GetPath(slot, auto);

        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            File.Delete(path);
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"저장 삭제 실패 (슬롯 {slot}, 자동={auto}): {exception.Message}");
            return false;
        }
    }

    // 슬롯 10개의 현재 상태를 슬롯 번호 순으로 돌려준다
    public static List<SaveSlotInfo> ListSlots() {
        var list = new List<SaveSlotInfo>();

        for (int slot = 0; slot < SlotCount; slot++)
        {
            list.Add(new SaveSlotInfo {
                slot = slot,
                manual = Read(slot, false),
                auto = Read(slot, true),
            });
        }

        return list;
    }
}
