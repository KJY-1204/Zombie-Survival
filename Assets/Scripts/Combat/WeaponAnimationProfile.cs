// 활과 근접무기 전용 대기 및 공격 애니메이션 클립을 정의하는 데이터.

using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Combat/Weapon Animation Profile", fileName = "Weapon Animation Profile")]
public class WeaponAnimationProfile : ScriptableObject {
    public AnimationClip idleClip;
    public AnimationClip attackClip;
}
