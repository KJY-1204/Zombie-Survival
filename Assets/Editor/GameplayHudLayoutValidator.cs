// 게임플레이 HUD 프리팹이 새 레이아웃과 기존 데이터 표시기 참조를 모두 만드는지 검증한다.
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class GameplayHudLayoutValidator {
    private const string HudPrefabPath = "Assets/Prefabs/HUD Canvas.prefab";

    public static void Validate() {
        GameObject hud = PrefabUtility.LoadPrefabContents(HudPrefabPath);

        try {
            GameplayHudLayout layout = hud.GetComponent<GameplayHudLayout>();

            if (layout == null) {
                throw new InvalidOperationException("HUD Canvas에 GameplayHudLayout이 없습니다.");
            }

            MethodInfo awake = typeof(GameplayHudLayout).GetMethod("Awake",
                BindingFlags.Instance | BindingFlags.NonPublic);
            awake.Invoke(layout, null);

            Require(hud.transform.Find("Top Navigation"), "상단 방향 HUD");
            Require(hud.transform.Find("Survival Guide"), "우측 생존 안내 HUD");
            Require(hud.transform.Find("Player Vitals"), "좌하단 생존 상태 HUD");
            Require(hud.transform.Find("Weapon Hud"), "우하단 무기 HUD");
            Require(hud.transform.Find("Interaction Prompt"), "상호작용 HUD");
            Require(hud.transform.Find("Crosshair"), "중앙 조준점 HUD");

            WeaponHudUI weapon = hud.GetComponent<WeaponHudUI>();
            VehicleHudUI vehicle = hud.GetComponent<VehicleHudUI>();
            InteractionPromptUI prompt = hud.GetComponent<InteractionPromptUI>();
            BuildMenuUI build = hud.GetComponent<BuildMenuUI>();
            UIManager manager = hud.GetComponent<UIManager>();

            if (weapon.root == null || weapon.weaponIcon == null || weapon.weaponName == null
                || weapon.ammoText == null || vehicle.root == null || vehicle.fuelFill == null
                || vehicle.durabilityFill == null || prompt.promptText == null
                || prompt.background == null || build.hintText == null || manager.crosshair == null) {
                throw new InvalidOperationException("새 HUD의 기존 UI 스크립트 참조가 완전하지 않습니다.");
            }

            Debug.Log("Gameplay HUD layout validation passed.");
        }
        finally {
            PrefabUtility.UnloadPrefabContents(hud);
        }
    }

    private static void Require(Transform element, string label) {
        if (element == null) {
            throw new InvalidOperationException($"{label}을 만들지 못했습니다.");
        }
    }
}
