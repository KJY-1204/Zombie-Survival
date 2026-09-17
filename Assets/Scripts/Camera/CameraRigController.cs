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
    public Renderer bodyRenderer; // 1인칭 시점에서 카메라를 가리지 않도록 숨길 몸통 렌더러

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

        if (bodyRenderer != null)
        {
            // 1인칭 카메라가 몸통 메시 안쪽에서 시작해 화면을 가리는 것을 막기 위해 숨김
            bodyRenderer.enabled = mode != CameraMode.FirstPerson;
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
