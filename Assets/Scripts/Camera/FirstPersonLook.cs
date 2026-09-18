// 마우스 Y 입력으로 1인칭 카메라의 상하 시야각(피치)을 조작하는 컴포넌트
using UnityEngine;

public class FirstPersonLook : MonoBehaviour {
    public float mouseSensitivity = 3f; // 마우스 상하 감도
    public float minPitch = -80f; // 아래를 볼 수 있는 최대 각도
    public float maxPitch = 80f; // 위를 볼 수 있는 최대 각도

    private float pitch;

    private void OnEnable() {
        // 1인칭으로 전환할 때마다 정면을 보도록 초기화
        pitch = 0f;
        transform.localRotation = Quaternion.identity;
    }

    private void Update() {
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch - mouseY, minPitch, maxPitch);
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
