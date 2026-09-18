// Survivalist 모델의 양손을 기존 총기 마운트에 맞추는 IK 컴포넌트
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SurvivalistWeaponIK : MonoBehaviour {
    private Animator visualAnimator;
    private PlayerShooter playerShooter;
    private Vector3 pivotToRightGripPosition;
    private Quaternion pivotToRightGripRotation;

    private void Awake() {
        visualAnimator = GetComponent<Animator>();
        playerShooter = GetComponentInParent<PlayerShooter>();

        if (playerShooter != null && playerShooter.gunPivot != null &&
            playerShooter.rightHandMount != null)
        {
            pivotToRightGripPosition = playerShooter.gunPivot.InverseTransformPoint(
                playerShooter.rightHandMount.position);
            pivotToRightGripRotation = Quaternion.Inverse(
                playerShooter.gunPivot.rotation) *
                playerShooter.rightHandMount.rotation;
        }
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
            Transform rightHand = visualAnimator.GetBoneTransform(
                HumanBodyBones.RightHand);
            if (rightHand != null)
            {
                Quaternion weaponRotation = rightHand.rotation *
                    Quaternion.Inverse(pivotToRightGripRotation);

                // 오른손 그립이 손 본에 겹치도록 총기 피벗을 매 프레임 계산한다
                playerShooter.gunPivot.SetPositionAndRotation(
                    rightHand.position - weaponRotation * pivotToRightGripPosition,
                    weaponRotation);
            }
        }

        // 오른손은 애니메이션이 직접 총을 들고, 왼손만 총기 전방 손잡이를 따른다
        SetHandIK(AvatarIKGoal.LeftHand, playerShooter.leftHandMount);
    }

    // 손 목표점의 위치와 회전을 총기 손잡이에 맞춘다
    private void SetHandIK(AvatarIKGoal hand, Transform handMount) {
        visualAnimator.SetIKPositionWeight(hand, 1f);
        visualAnimator.SetIKRotationWeight(hand, 1f);
        visualAnimator.SetIKPosition(hand, handMount.position);
        visualAnimator.SetIKRotation(hand, handMount.rotation);
    }
}
