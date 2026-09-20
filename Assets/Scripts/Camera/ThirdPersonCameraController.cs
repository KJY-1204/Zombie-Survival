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

    [Header("벽 회피")]
    public float collisionRadius = 0.25f; // 카메라가 벽을 감지할 반경
    public float minCollisionDistance = 0.4f; // 벽에 막혔을 때 피벗에서 최소한 떨어질 거리

    public bool isAiming { get; private set; }

    private Camera targetCamera;
    private float pitch = 12f;
    private bool hasSnapped; // 첫 프레임에는 보간 없이 바로 제자리로 간다

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
        // 전체 화면이 열려 있는 동안에는 마우스로 시점이 돌아가지 않게 한다
        if (target == null || UIManager.isScreenOpen)
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

        // 실내처럼 좁은 곳에서는 카메라가 벽을 뚫고 나간다.
        // 피벗에서 목표 지점까지 구를 굴려 막히면 그 앞까지만 물러난다
        desiredPosition = PullOutOfWalls(pivot, desiredPosition);

        // 시작 지점이 원점에서 멀면(시드 월드는 수백 m 떨어져 있다)
        // 보간으로 따라가는 동안 몇 초간 엉뚱한 곳을 비춘다. 처음 한 번은 바로 붙인다
        transform.position = hasSnapped
            ? Vector3.Lerp(transform.position, desiredPosition, transitionSpeed * Time.deltaTime)
            : desiredPosition;
        hasSnapped = true;
        transform.rotation = viewRotation;
        targetCamera.fieldOfView = Mathf.Lerp(
            targetCamera.fieldOfView,
            fov,
            transitionSpeed * Time.deltaTime);
    }

    // 피벗에서 목표 지점 사이에 막히는 것이 있으면 그 앞까지만 물러난다
    private Vector3 PullOutOfWalls(Vector3 pivot, Vector3 desiredPosition) {
        Vector3 offset = desiredPosition - pivot;
        float distance = offset.magnitude;

        if (distance < 0.01f)
        {
            return desiredPosition;
        }

        Vector3 direction = offset / distance;

        RaycastHit[] hits = Physics.SphereCastAll(
            pivot, collisionRadius, direction, distance,
            ~0, QueryTriggerInteraction.Ignore);

        float nearest = distance;

        foreach (RaycastHit hit in hits)
        {
            // 플레이어 자신과 그가 든 물건은 무시한다
            if (target != null && hit.transform.IsChildOf(target))
            {
                continue;
            }

            if (hit.distance < nearest)
            {
                nearest = hit.distance;
            }
        }

        // 벽에 딱 붙으면 면이 보이므로 약간 띄운다
        return pivot + direction * Mathf.Max(minCollisionDistance, nearest - 0.1f);
    }
}
