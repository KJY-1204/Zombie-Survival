// 1인칭/3인칭 카메라를 교체 가능한 상태로 전환하는 컨트롤러
using UnityEngine;

public class CameraRigController : MonoBehaviour {
    public enum CameraMode {
        ThirdPerson,
        FirstPerson
    }

    public GameObject thirdPersonCamera; // 3인칭 카메라 가상 카메라 오브젝트
    public GameObject firstPersonCamera; // 1인칭 카메라 가상 카메라 오브젝트

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
    }
}
