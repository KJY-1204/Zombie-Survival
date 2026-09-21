// 근접무기의 피해량과 사거리, 전방 판정 각도를 정의하는 정적 데이터
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Combat/Melee Weapon", fileName = "Melee Weapon Data")]
public class MeleeWeaponData : ScriptableObject {
    public float damage = 45f;
    public float range = 2f;
    [Range(1f, 180f)] public float arc = 90f;
    public float attackInterval = 0.65f;
    public float noiseRadius = 8f;
}
