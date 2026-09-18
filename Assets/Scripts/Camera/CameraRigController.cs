// 1인칭/3인칭 카메라를 교체 가능한 상태로 전환하는 컨트롤러
using UnityEngine;

public class CameraRigController : MonoBehaviour {
    public enum CameraMode {
        ThirdPerson,
        FirstPerson
    }

    public GameObject thirdPersonCamera; // 3인칭 카메라 가상 카메라 오브젝트
    public GameObject firstPersonCamera; // 1인칭 카메라 가상 카메라 오브젝트
    public PlayerShooter playerShooter; // 카메라 모드에 맞춰 총 배치 기준을 바꿔줄 슈터
    public PlayerMovement playerMovement; // 1인칭에서 마우스 좌우 회전으로 전환할 이동 스크립트
    public FirstPersonLook firstPersonLook; // 1인칭 카메라 상하 시점(피치) 조작 컴포넌트
    public GameObject bodyRoot; // 1인칭 시점에서 카메라를 가리지 않도록 숨길 몸통(모든 파츠) 루트

    public GameObject weaponViewModelTarget; // 1인칭 전용 카메라로만 보이게 할 총 오브젝트 (Gun)
    public string firstPersonWeaponLayer = "FirstPersonWeapon"; // 1인칭 전용 무기 카메라가 그리는 레이어
    public string defaultWeaponLayer = "Default"; // 3인칭에서 총이 원래 속하는 레이어

    public KeyCode switchKey = KeyCode.C; // 카메라 모드 전환 키

    public CameraMode currentMode { get; private set; } // 현재 카메라 모드

    private void Awake() {
        SetMode(CameraMode.ThirdPerson);
    }

    private void Update() {
        if (Input.GetKeyDown(switchKey))
        {
            SetMode(currentMode == CameraMode.ThirdPerson
                ? CameraMode.FirstPerson
                : CameraMode.ThirdPerson);
        }
    }

    // 카메라 모드를 전환. 두 가상 카메라 중 하나만 활성화하여 CinemachineBrain이 그것을 따르게 한다
    public void SetMode(CameraMode mode) {
        currentMode = mode;

        if (thirdPersonCamera != null)
        {
            thirdPersonCamera.SetActive(mode == CameraMode.ThirdPerson);
        }

        if (firstPersonCamera != null)
        {
            firstPersonCamera.SetActive(mode == CameraMode.FirstPerson);
        }

        if (playerShooter != null)
        {
            playerShooter.useFirstPersonMount = mode == CameraMode.FirstPerson;
        }

        if (playerMovement != null)
        {
            playerMovement.useMouseLook = mode == CameraMode.FirstPerson;
        }

        if (firstPersonLook != null)
        {
            firstPersonLook.enabled = mode == CameraMode.FirstPerson;
        }

        // 1인칭에서는 마우스로 시점을 조작하므로 커서를 고정하고 숨긴다
        Cursor.lockState = mode == CameraMode.FirstPerson ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = mode != CameraMode.FirstPerson;

        if (bodyRoot != null)
        {
            // 1인칭 카메라가 몸통 메시 안쪽에서 시작해 화면을 가리는 것을 막기 위해 숨김
            bodyRoot.SetActive(mode != CameraMode.FirstPerson);
        }

        if (weaponViewModelTarget != null)
        {
            // 1인칭에서는 총을 전용 무기 카메라만 그리는 레이어로 옮겨서
            // 메인 카메라에서는 안 보이고(포스트 프로세싱/근접 왜곡 없이) 무기 카메라에서만 보이게 한다
            string layerName = mode == CameraMode.FirstPerson ? firstPersonWeaponLayer : defaultWeaponLayer;
            SetLayerRecursively(weaponViewModelTarget, LayerMask.NameToLayer(layerName));
        }
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
