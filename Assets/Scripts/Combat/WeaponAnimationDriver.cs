// 총기 컨트롤러와 분리된 상체 무기 애니메이션을 재생하는 플레이어 컴포넌트
using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class WeaponAnimationDriver : MonoBehaviour {
    private Animator targetAnimator;
    private PlayableGraph graph;
    private AnimationLayerMixerPlayable layerMixer;
    private AnimationClipPlayable weaponPlayable;
    private AvatarMask upperBodyMask;
    private WeaponAnimationProfile activeProfile;
    private AnimationClip activeClip;
    private float attackEndTime;

    public event Action onHit;
    public bool isAttacking => activeProfile != null && activeClip == activeProfile.attackClip
        && Time.time < attackEndTime;

    private void Awake() {
        targetAnimator = GetComponent<Animator>();

        if (targetAnimator.runtimeAnimatorController == null)
        {
            return;
        }

        graph = PlayableGraph.Create("Weapon Animation Layer");
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(
            graph, "Weapon Animation Output", targetAnimator);
        AnimatorControllerPlayable controllerPlayable = AnimatorControllerPlayable.Create(
            graph, targetAnimator.runtimeAnimatorController);

        layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);
        graph.Connect(controllerPlayable, 0, layerMixer, 0);
        layerMixer.SetInputWeight(0, 1f);

        upperBodyMask = CreateUpperBodyMask();
        layerMixer.SetLayerMaskFromAvatarMask(1, upperBodyMask);
        layerMixer.SetInputWeight(1, 0f);
        output.SetSourcePlayable(layerMixer);
        graph.Play();
    }

    private void Update() {
        if (activeProfile != null && activeClip == activeProfile.attackClip
            && Time.time >= attackEndTime)
        {
            PlayClip(activeProfile.idleClip);
        }
    }

    private void OnDestroy() {
        if (graph.IsValid())
        {
            graph.Destroy();
        }

        if (upperBodyMask != null)
        {
            Destroy(upperBodyMask);
        }
    }

    public void SetProfile(WeaponAnimationProfile profile) {
        activeProfile = profile;

        if (profile == null || profile.idleClip == null || !graph.IsValid())
        {
            if (graph.IsValid())
            {
                layerMixer.SetInputWeight(1, 0f);
            }

            activeClip = null;
            return;
        }

        PlayClip(profile.idleClip);
    }

    public void PlayAttack() {
        if (activeProfile == null || activeProfile.attackClip == null || !graph.IsValid()
            || isAttacking)
        {
            return;
        }

        PlayClip(activeProfile.attackClip);
        attackEndTime = Time.time + activeProfile.attackClip.length;
    }

    // 외부 전투 클립의 명중 이벤트를 현재 장착 무기에 전달한다.
    public void Hit() {
        onHit?.Invoke();
    }

    public void FootL() {
    }

    public void FootR() {
    }

    private void PlayClip(AnimationClip clip) {
        if (clip == null || activeClip == clip)
        {
            return;
        }

        if (weaponPlayable.IsValid())
        {
            layerMixer.DisconnectInput(1);
            weaponPlayable.Destroy();
        }

        weaponPlayable = AnimationClipPlayable.Create(graph, clip);
        weaponPlayable.SetApplyFootIK(false);
        graph.Connect(weaponPlayable, 0, layerMixer, 1);
        layerMixer.SetInputWeight(1, 1f);
        activeClip = clip;
    }

    private static AvatarMask CreateUpperBodyMask() {
        AvatarMask mask = new AvatarMask();

        for (int index = 0; index < (int)AvatarMaskBodyPart.LastBodyPart; index++)
        {
            mask.SetHumanoidBodyPartActive((AvatarMaskBodyPart)index, false);
        }

        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Body, true);
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.Head, true);
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftArm, true);
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightArm, true);
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.LeftFingers, true);
        mask.SetHumanoidBodyPartActive(AvatarMaskBodyPart.RightFingers, true);
        return mask;
    }
}
