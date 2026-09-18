// 플레이어 뒤쪽 숄더뷰와 정조준 시점을 전환하는 TPS 카메라 컨트롤러
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour {
    [Header("Target")]
    public Transform target;

    [Header("View")]
    public float defaultShoulderOffset = 0.65f;
    public float defaultDistance = 3.5f;
    public float defaultHeight = 1.55f;
    public float defaultFov = 60f;
    public float adsShoulderOffset = 0.45f;
    public float adsDistance = 1.45f;
    public float adsHeight = 1.5f;
    public float adsFov = 42f;
    public float pitchSensitivity = 3f;
    public float minPitch = -35f;
    public float maxPitch = 55f;
    public float transitionSpeed = 14f;

    public bool isAiming { get; private set; }

    private Camera targetCamera;
    private float pitch = 12f;

    private void Awake() {
        targetCamera = GetComponent<Camera>();

        if (target == null)
        {
            PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
            if (playerMovement != null)
            {
                target = playerMovement.transform;
                playerMovement.useMouseLook = true;
            }
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate() {
        if (target == null)
        {
            return;
        }

        isAiming = Input.GetButton("Fire2");
        pitch = Mathf.Clamp(
            pitch - Input.GetAxis("Mouse Y") * pitchSensitivity,
            minPitch,
            maxPitch);

        float blend = isAiming ? 1f : 0f;
        float shoulderOffset = Mathf.Lerp(defaultShoulderOffset, adsShoulderOffset, blend);
        float distance = Mathf.Lerp(defaultDistance, adsDistance, blend);
        float height = Mathf.Lerp(defaultHeight, adsHeight, blend);
        float fov = Mathf.Lerp(defaultFov, adsFov, blend);

        Quaternion viewRotation = Quaternion.Euler(pitch, target.eulerAngles.y, 0f);
        Vector3 pivot = target.position + Vector3.up * height;
        Vector3 desiredPosition = pivot + viewRotation * new Vector3(shoulderOffset, 0f, -distance);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            transitionSpeed * Time.deltaTime);
        transform.rotation = viewRotation;
        targetCamera.fieldOfView = Mathf.Lerp(
            targetCamera.fieldOfView,
            fov,
            transitionSpeed * Time.deltaTime);
    }
}
