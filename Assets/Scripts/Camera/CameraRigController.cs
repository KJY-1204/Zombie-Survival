// 1인칭 시점을 구성하는 컨트롤러 (탑다운/3인칭 시점 제거, 1인칭 전용)
using UnityEngine;

public class CameraRigController : MonoBehaviour {
    public GameObject firstPersonCamera; // 1인칭 카메라 가상 카메라 오브젝트
    public PlayerShooter playerShooter; // 총 배치 기준을 1인칭 마운트로 맞춰줄 슈터
    public PlayerMovement playerMovement; // 마우스 좌우 회전으로 전환할 이동 스크립트
    public FirstPersonLook firstPersonLook; // 카메라 상하 시점(피치) 조작 컴포넌트
    public GameObject bodyRoot; // 카메라를 가리지 않도록 숨길 몸통(모든 파츠) 루트

    public GameObject weaponViewModelTarget; // 1인칭 전용 카메라로만 보이게 할 총 오브젝트 (Gun)
    public string firstPersonWeaponLayer = "FirstPersonWeapon"; // 1인칭 전용 무기 카메라가 그리는 레이어

    private void Awake() {
        if (firstPersonCamera != null)
        {
            firstPersonCamera.SetActive(true);
        }

        if (playerShooter != null)
        {
            playerShooter.useFirstPersonMount = true;
        }

        if (playerMovement != null)
        {
            playerMovement.useMouseLook = true;
        }

        if (firstPersonLook != null)
        {
            firstPersonLook.enabled = true;
        }

        if (bodyRoot != null)
        {
            // 1인칭 카메라가 몸통 메시 안쪽에서 시작해 화면을 가리는 것을 막기 위해 숨김
            bodyRoot.SetActive(false);
        }

        if (weaponViewModelTarget != null)
        {
            // 총을 전용 무기 카메라만 그리는 레이어로 옮겨서
            // 메인 카메라에서는 안 보이고(포스트 프로세싱/근접 왜곡 없이) 무기 카메라에서만 보이게 한다
            SetLayerRecursively(weaponViewModelTarget, LayerMask.NameToLayer(firstPersonWeaponLayer));
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // target과 모든 자식 오브젝트의 레이어를 재귀적으로 변경
    private static void SetLayerRecursively(GameObject target, int layer) {
        if (layer < 0)
        {
            return;
        }

        target.layer = layer;
        foreach (Transform child in target.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
