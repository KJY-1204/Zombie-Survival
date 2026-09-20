// 탑승 중 플레이어의 조작·물리·애니메이션·카메라를 차량 상태로 전환하고 하차 시 되돌린다
using UnityEngine;

public class RiderControl : MonoBehaviour {
    public string mountedParameter = "Mounted"; // 앉은 포즈로 전환할 애니메이터 파라미터
    public string armsLayerName = "Weapon Hold Arms"; // 탑승 중 가중치를 0으로 내릴 레이어

    public Motorcycle vehicle { get; private set; } // 타고 있는 차량 (없으면 null)
    public bool isRiding => vehicle != null;

    private PlayerMovement movement;
    private PlayerShooter shooter;
    private BuildPlacer placer;
    private Rigidbody body;
    private Collider bodyCollider;
    private Animator visualAnimator; // Survivalist 비주얼의 애니메이터
    private ThirdPersonCameraController cameraController;

    private int armsLayerIndex = -1;
    private float armsLayerWeight = 1f; // 하차 시 되돌릴 원래 가중치

    private void Start() {
        movement = GetComponent<PlayerMovement>();
        shooter = GetComponent<PlayerShooter>();
        placer = GetComponent<BuildPlacer>();
        body = GetComponent<Rigidbody>();
        bodyCollider = GetComponent<Collider>();
        cameraController = FindObjectOfType<ThirdPersonCameraController>();

        // 아바타가 있는 비주얼 애니메이터를 찾는다 (루트의 것은 컨트롤러 없는 더미다)
        foreach (Animator animator in GetComponentsInChildren<Animator>())
        {
            if (animator.avatar != null && animator.avatar.isValid)
            {
                visualAnimator = animator;
            }
        }

        if (visualAnimator != null)
        {
            armsLayerIndex = visualAnimator.GetLayerIndex(armsLayerName);

            if (armsLayerIndex >= 0)
            {
                armsLayerWeight = visualAnimator.GetLayerWeight(armsLayerIndex);
            }
        }
    }

    // 좌석에 붙고 플레이어 조작을 전부 멈춘다
    public void EnterVehicle(Motorcycle target) {
        if (isRiding || target == null)
        {
            return;
        }

        vehicle = target;

        // 건설 모드는 켜둔 채로 타면 프리뷰가 남으므로 먼저 끈다
        if (placer != null)
        {
            placer.Cancel();
            placer.enabled = false;
        }

        if (movement != null)
        {
            movement.enabled = false;
        }

        if (shooter != null)
        {
            shooter.enabled = false;
        }

        // 물리를 멈추고 좌석에 고정한다 (콜라이더가 켜져 있으면 오토바이와 서로 민다)
        if (body != null)
        {
            body.isKinematic = true;
        }

        if (bodyCollider != null)
        {
            bodyCollider.enabled = false;
        }

        transform.SetParent(vehicle.seat, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        SetMountedPose(true);

        // 카메라가 오토바이를 따라가게 한다 (카메라 코어는 그대로 두고 타깃만 바꾼다)
        if (cameraController != null)
        {
            cameraController.target = vehicle.transform;
        }
    }

    // 좌석에서 떨어져 지면에 내려서고 플레이어 조작을 되돌린다
    public void ExitVehicle(Vector3 exitPosition) {
        if (!isRiding)
        {
            return;
        }

        transform.SetParent(null, true);
        transform.position = exitPosition;
        transform.rotation = Quaternion.Euler(0f, vehicle.transform.eulerAngles.y, 0f);

        SetMountedPose(false);

        if (bodyCollider != null)
        {
            bodyCollider.enabled = true;
        }

        if (body != null)
        {
            body.isKinematic = false;
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        if (movement != null)
        {
            movement.enabled = true;
        }

        if (shooter != null)
        {
            shooter.enabled = true;
        }

        if (placer != null)
        {
            placer.enabled = true;
        }

        if (cameraController != null)
        {
            cameraController.target = transform;
        }

        vehicle = null;
    }

    // 앉은 포즈로 바꾸고, 총 쥔 팔 오버라이드 레이어를 내린다
    private void SetMountedPose(bool mounted) {
        if (visualAnimator == null)
        {
            return;
        }

        visualAnimator.SetBool(mountedParameter, mounted);

        if (armsLayerIndex >= 0)
        {
            visualAnimator.SetLayerWeight(
                armsLayerIndex, mounted ? 0f : armsLayerWeight);
        }
    }
}
