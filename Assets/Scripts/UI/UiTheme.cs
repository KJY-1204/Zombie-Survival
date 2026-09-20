// 게임 내 UI가 공유하는 색과 치수 (팰월드풍 어두운 반투명 패널 + 호박색 강조)
using UnityEngine;

public static class UiTheme {
    // 배경
    public static readonly Color PanelBackground = new Color32(0x11, 0x14, 0x1A, 0xF0);
    public static readonly Color HeaderBackground = new Color32(0x1B, 0x20, 0x29, 0xFF);
    public static readonly Color RowBackground = new Color32(0xFF, 0xFF, 0xFF, 0x12);
    public static readonly Color RowSelected = new Color32(0xFF, 0xB2, 0x3F, 0x3A);
    public static readonly Color SlotBackground = new Color32(0x00, 0x00, 0x00, 0x66);

    // 글자
    public static readonly Color TextPrimary = new Color32(0xF2, 0xF4, 0xF8, 0xFF);
    public static readonly Color TextSecondary = new Color32(0x9A, 0xA3, 0xAD, 0xFF);
    public static readonly Color TextAccent = new Color32(0xFF, 0xB2, 0x3F, 0xFF);
    public static readonly Color TextDanger = new Color32(0xE8, 0x6A, 0x5E, 0xFF);

    // 버튼
    public static readonly Color ButtonNeutral = new Color32(0x2C, 0x34, 0x42, 0xFF);
    public static readonly Color ButtonAccent = new Color32(0xC8, 0x81, 0x1E, 0xFF);
    public static readonly Color ButtonDanger = new Color32(0x7A, 0x33, 0x2E, 0xFF);

    // 막대 (무게, 체력, 연료 등)
    public static readonly Color BarBackground = new Color32(0x00, 0x00, 0x00, 0x99);
    public static readonly Color BarNormal = new Color32(0x6F, 0xBF, 0x73, 0xFF);
    public static readonly Color BarWarning = new Color32(0xE0, 0xA5, 0x30, 0xFF);
    public static readonly Color BarDanger = new Color32(0xD3, 0x50, 0x45, 0xFF);

    // 치수
    public const int TitleFontSize = 24;
    public const int TabFontSize = 18;
    public const int BodyFontSize = 17;
    public const int SmallFontSize = 14;
    public const float RowHeight = 56f;
    public const float IconSize = 44f;
    public const float Padding = 20f;

    // 값이 낮을수록 위험한 막대(체력/연료)의 색
    public static Color BarColorForRemaining(float ratio) {
        if (ratio <= 0.25f) return BarDanger;
        if (ratio <= 0.5f) return BarWarning;
        return BarNormal;
    }

    // 값이 높을수록 위험한 막대(무게)의 색
    public static Color BarColorForFill(float ratio) {
        if (ratio >= 1f) return BarDanger;
        if (ratio >= 0.8f) return BarWarning;
        return BarNormal;
    }
}
