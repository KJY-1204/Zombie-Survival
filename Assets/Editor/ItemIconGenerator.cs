// 아이템 아이콘을 프리팹 미리보기에서 뽑아 PNG로 저장하고 ItemData.icon에 연결하는 에디터 도구
// 런타임 어셈블리에 UnityEditor가 섞이면 빌드가 깨지므로 반드시 Assets/Editor 아래에 둔다
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ItemIconGenerator {
    private const string IconFolder = "Assets/Art/ItemIcons";
    private const int IconSize = 128;

    [MenuItem("Tools/아이템 아이콘 생성")]
    public static void GenerateFromMenu() {
        Debug.Log(Generate(false, int.MaxValue));
    }

    // 아이콘을 만들어 붙인다. 미리보기가 아직 안 구워졌으면 그 아이템은 다음 호출로 미룬다
    // force가 참이면 이미 아이콘이 있어도 다시 만든다
    public static string Generate(bool force, int maxCount) {
        Directory.CreateDirectory(IconFolder);

        var done = new List<string>();
        var pending = new List<string>();
        var placeholder = new List<string>();

        foreach (string guid in AssetDatabase.FindAssets("t:ItemData"))
        {
            var item = AssetDatabase.LoadAssetAtPath<ItemData>(AssetDatabase.GUIDToAssetPath(guid));

            if (item == null || (!force && item.icon != null))
            {
                continue;
            }

            if (done.Count + placeholder.Count >= maxCount)
            {
                pending.Add(item.itemId);
                continue;
            }

            GameObject source = FindSourcePrefab(item);
            string path = $"{IconFolder}/{item.itemId}.png";

            if (source == null)
            {
                File.WriteAllBytes(path, MakePlaceholder(item).EncodeToPNG());
                placeholder.Add(item.itemId);
            }
            else
            {
                Texture2D preview = AssetPreview.GetAssetPreview(source);

                // 첫 호출에서는 null이 나오고 내부적으로 굽기 시작한다. 다음 호출에서 받는다
                if (preview == null)
                {
                    pending.Add(item.itemId);
                    continue;
                }

                File.WriteAllBytes(path, Readable(preview).EncodeToPNG());
                done.Add(item.itemId);
            }

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            ApplySpriteSettings(path);
            item.icon = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            EditorUtility.SetDirty(item);
        }

        // 건설물도 같은 방식으로 프리팹을 찍어 아이콘을 만든다
        foreach (string guid in AssetDatabase.FindAssets("t:BuildableData"))
        {
            var buildable = AssetDatabase.LoadAssetAtPath<BuildableData>(
                AssetDatabase.GUIDToAssetPath(guid));

            if (buildable == null || buildable.prefab == null
                || (!force && buildable.icon != null))
            {
                continue;
            }

            if (done.Count + placeholder.Count >= maxCount)
            {
                pending.Add(buildable.buildableId);
                continue;
            }

            Texture2D preview = AssetPreview.GetAssetPreview(buildable.prefab);

            if (preview == null)
            {
                pending.Add(buildable.buildableId);
                continue;
            }

            string buildPath = $"{IconFolder}/build_{buildable.buildableId}.png";
            File.WriteAllBytes(buildPath, Readable(preview).EncodeToPNG());
            AssetDatabase.ImportAsset(buildPath, ImportAssetOptions.ForceUpdate);
            ApplySpriteSettings(buildPath);
            buildable.icon = AssetDatabase.LoadAssetAtPath<Sprite>(buildPath);
            EditorUtility.SetDirty(buildable);
            done.Add(buildable.buildableId);
        }

        AssetDatabase.SaveAssets();

        return $"미리보기 {done.Count}개 | 대체 타일 {placeholder.Count}개 | 대기 {pending.Count}개"
            + (pending.Count > 0 ? $" ({string.Join(", ", pending)})" : "");
    }

    // 아이콘으로 쓸 3D 모델을 찾는다 (필드에 떨어지는 프리팹 -> 무기 프리팹 순)
    private static GameObject FindSourcePrefab(ItemData item) {
        if (item.worldPrefab != null)
        {
            return item.worldPrefab;
        }

        if (item is WeaponItemData weapon && weapon.weaponPrefab != null)
        {
            return weapon.weaponPrefab;
        }

        return null;
    }

    // 미리보기 텍스처는 읽기 불가 상태로 올 수 있어서 한 번 베껴온다
    private static Texture2D Readable(Texture2D source) {
        var buffer = RenderTexture.GetTemporary(
            IconSize, IconSize, 0, RenderTextureFormat.ARGB32);
        RenderTexture previous = RenderTexture.active;

        GL.Clear(true, true, Color.clear);
        Graphics.Blit(source, buffer);
        RenderTexture.active = buffer;

        var copy = new Texture2D(IconSize, IconSize, TextureFormat.RGBA32, false);
        copy.ReadPixels(new Rect(0, 0, IconSize, IconSize), 0, 0);
        copy.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(buffer);

        CutOutBackground(copy);
        return copy;
    }

    // 미리보기는 회색 배경이 깔려 나온다. 테두리에서 번져나가며 배경만 투명하게 만든다.
    // 색을 전역으로 지우면 물체 안의 비슷한 회색까지 띑어져 구멍이 뚫린다
    private static void CutOutBackground(Texture2D texture) {
        Color32[] pixels = texture.GetPixels32();
        Color32 background = pixels[0];
        int size = texture.width;

        var queue = new Queue<int>();
        var visited = new bool[pixels.Length];

        for (int i = 0; i < size; i++)
        {
            Enqueue(queue, visited, pixels, background, i);                       // 아래줄
            Enqueue(queue, visited, pixels, background, (size - 1) * size + i);    // 윗줄
            Enqueue(queue, visited, pixels, background, i * size);                 // 왼쪽
            Enqueue(queue, visited, pixels, background, i * size + size - 1);      // 오른쪽
        }

        while (queue.Count > 0)
        {
            int index = queue.Dequeue();
            pixels[index].a = 0;

            int x = index % size;
            int y = index / size;

            if (x > 0) Enqueue(queue, visited, pixels, background, index - 1);
            if (x < size - 1) Enqueue(queue, visited, pixels, background, index + 1);
            if (y > 0) Enqueue(queue, visited, pixels, background, index - size);
            if (y < size - 1) Enqueue(queue, visited, pixels, background, index + size);
        }

        Brighten(pixels);
        texture.SetPixels32(pixels);
        texture.Apply();
    }

    // 프리팹 미리보기는 조명이 약해 어둡게 나온다.
    // 어두운 패널 위에서도 보이도록 감마 보정으로 띄운다
    private static void Brighten(Color32[] pixels) {
        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a == 0)
            {
                continue;
            }

            pixels[i].r = Lift(pixels[i].r);
            pixels[i].g = Lift(pixels[i].g);
            pixels[i].b = Lift(pixels[i].b);
        }
    }

    private static byte Lift(byte value) {
        return (byte)Mathf.RoundToInt(255f * Mathf.Pow(value / 255f, 0.62f));
    }

    private static void Enqueue(
        Queue<int> queue, bool[] visited, Color32[] pixels, Color32 background, int index) {
        if (visited[index] || !IsBackground(pixels[index], background))
        {
            return;
        }

        visited[index] = true;
        queue.Enqueue(index);
    }

    // 경계의 부드러운 픽셀까지 잖히도록 여유를 준다
    private static bool IsBackground(Color32 pixel, Color32 background) {
        return Mathf.Abs(pixel.r - background.r) <= 10
            && Mathf.Abs(pixel.g - background.g) <= 10
            && Mathf.Abs(pixel.b - background.b) <= 10;
    }

    // 3D 모델이 없는 아이템(방어구)은 종류별 색의 둥근 타일로 대신한다
    private static Texture2D MakePlaceholder(ItemData item) {
        Color color = PlaceholderColor(item);
        var texture = new Texture2D(IconSize, IconSize, TextureFormat.RGBA32, false);

        const float radius = 26f;
        const float margin = 14f;
        float max = IconSize - margin;

        for (int y = 0; y < IconSize; y++)
        {
            for (int x = 0; x < IconSize; x++)
            {
                texture.SetPixel(x, y, InsideRoundedBox(x, y, margin, max, radius)
                    ? color
                    : Color.clear);
            }
        }

        texture.Apply();
        return texture;
    }

    // 모서리를 둥글린 사각형 안쪽인지
    private static bool InsideRoundedBox(float x, float y, float min, float max, float radius) {
        if (x < min || x > max || y < min || y > max)
        {
            return false;
        }

        float cx = Mathf.Clamp(x, min + radius, max - radius);
        float cy = Mathf.Clamp(y, min + radius, max - radius);
        return (new Vector2(x - cx, y - cy)).sqrMagnitude <= radius * radius;
    }

    private static Color PlaceholderColor(ItemData item) {
        if (item is ArmorItemData armor)
        {
            switch (armor.slot)
            {
                case EquipmentSlot.Head: return new Color32(0x5C, 0x7A, 0x99, 0xFF);
                case EquipmentSlot.Chest: return new Color32(0x44, 0x63, 0x80, 0xFF);
                default: return new Color32(0x35, 0x4E, 0x66, 0xFF);
            }
        }

        if (item is WeaponItemData)
        {
            return new Color32(0x6B, 0x6F, 0x76, 0xFF);
        }

        return new Color32(0x7A, 0x5C, 0x3A, 0xFF);
    }

    private static void ApplySpriteSettings(string path) {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.SaveAndReimport();
    }
}
