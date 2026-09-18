// Survivalist 모델의 양손을 기존 총기 마운트에 맞추는 IK 컴포넌트
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SurvivalistWeaponIK : MonoBehaviour {
    private Animator visualAnimator;
    private PlayerShooter playerShooter;

    private void Awake() {
        visualAnimator = GetComponent<Animator>();
        playerShooter = GetComponentInParent<PlayerShooter>();
    }

    private void OnAnimatorIK(int layerIndex) {
        if (playerShooter == null || playerShooter.gunPivot == null ||
            playerShooter.leftHandMount == null || playerShooter.rightHandMount == null)
        {
            return;
        }

        if (playerShooter.useFirstPersonMount &&
            playerShooter.firstPersonWeaponMount != null)
        {
            playerShooter.gunPivot.SetPositionAndRotation(
                playerShooter.firstPersonWeaponMount.position,
                playerShooter.firstPersonWeaponMount.rotation);
        }
        else
        {
            Transform rightForearm = visualAnimator.GetBoneTransform(
                HumanBodyBones.RightLowerArm);
            if (rightForearm != null)
            {
                playerShooter.gunPivot.position = rightForearm.position;
            }
        }

        SetHandIK(AvatarIKGoal.LeftHand, playerShooter.leftHandMount);
        SetHandIK(AvatarIKGoal.RightHand, playerShooter.rightHandMount);
    }

    // 손 목표점의 위치와 회전을 총기 손잡이에 맞춘다
    private void SetHandIK(AvatarIKGoal hand, Transform handMount) {
        visualAnimator.SetIKPositionWeight(hand, 1f);
        visualAnimator.SetIKRotationWeight(hand, 1f);
        visualAnimator.SetIKPosition(hand, handMount.position);
        visualAnimator.SetIKRotation(hand, handMount.rotation);
    }
}
