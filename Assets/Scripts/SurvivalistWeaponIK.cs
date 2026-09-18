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

        RecalibrateGrip();
    }

    // 오른손-총기 그립 오프셋을 현재 gunPivot/rightHandMount 기준으로 다시 계산한다
    // 장착한 무기가 바뀌어 rightHandMount가 다른 총으로 교체될 때마다 다시 호출해야 한다
    public void RecalibrateGrip() {
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

    }
}
