using UnityEngine;

// 플레이어 캐릭터를 사용자 입력에 따라 움직이는 스크립트
public class PlayerMovement : MonoBehaviour {
    public float moveSpeed = 5f; // 앞뒤 움직임의 속도
    public float rotateSpeed = 180f; // 좌우 회전 속도(키보드)

    public bool useMouseLook; // 1인칭 모드에서 마우스로 좌우 회전할지 여부 (CameraRigController가 전환)
    public float mouseYawSpeed = 3f; // 마우스 좌우 회전 감도

    public float jumpForce = 5f; // 점프 시 위로 가하는 순간 속도
    public float groundCheckDistance = 0.6f; // 캐릭터 중심에서 바닥까지 검사할 거리

    private Animator playerAnimator; // 플레이어 캐릭터의 애니메이터
    private Animator visualAnimator; // Survivalist 비주얼의 애니메이터
    private ThirdPersonCameraController cameraController; // ADS 상태를 알려주는 TPS 카메라
    private PlayerInput playerInput; // 플레이어 입력을 알려주는 컴포넌트
    private Rigidbody playerRigidbody; // 플레이어 캐릭터의 리지드바디
    private Collider playerCollider; // 바닥 검사에서 자기 자신을 제외하기 위한 콜라이더

    private bool isGrounded; // 현재 바닥에 닿아 있는지 여부
    private bool jumpRequested; // 다음 FixedUpdate에서 처리할 점프 요청

    private void Start() {
        // 사용할 컴포넌트들의 참조를 가져오기
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider>();
        cameraController = FindFirstObjectByType<ThirdPersonCameraController>();

        Transform visualRoot = transform.Find("Survivalist Visual");
        if (visualRoot != null)
        {
            Animator[] animators = visualRoot.GetComponentsInChildren<Animator>();
            foreach (Animator animator in animators)
            {
                if (animator.transform != visualRoot)
                {
                    visualAnimator = animator;
                    break;
                }
            }

            if (visualAnimator == null)
            {
                visualAnimator = visualRoot.GetComponent<Animator>();
            }
        }
    }

    private void Update() {
        // 점프 입력은 Update에서만 true가 되는 단발성 값이라
        // FixedUpdate가 놓치지 않도록 요청 플래그에 저장해둔다
        if (playerInput.jump)
        {
            jumpRequested = true;
        }
    }

    // FixedUpdate는 물리 갱신 주기에 맞춰 실행됨
    private void FixedUpdate() {
        // 바닥 접촉 여부 갱신
        CheckGrounded();

        // 회전 실행
        Rotate();
        // 움직임 실행
        Move();
        // 점프 요청이 있고 바닥에 있을 때만 점프 실행
        bool isJumping = jumpRequested && isGrounded;
        if (isJumping)
        {
            Jump();
        }
        jumpRequested = false;

        // 입력값에 따라 애니메이터의 Move 파라미터 값을 변경
        float moveAmount = new Vector2(
            playerInput.rotate, playerInput.move).magnitude;
        if (playerAnimator.enabled)
        {
            playerAnimator.SetFloat("Move", moveAmount);
            playerAnimator.SetBool("IsGrounded", isGrounded);
        }

        UpdateVisualAnimator(moveAmount, isJumping);
    }

    // 캐릭터 중심에서 아래로 검사해 바닥에 닿아 있는지 확인 (자기 자신의 콜라이더는 제외)
    private void CheckGrounded() {
        Vector3 origin = playerRigidbody.position + Vector3.up * 0.5f;
        RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down, groundCheckDistance);
        isGrounded = false;
        foreach (var hit in hits)
        {
            if (hit.collider != playerCollider)
            {
                isGrounded = true;
                break;
            }
        }
    }

    // 위로 순간 속도를 가해 점프시키고 점프 애니메이션을 재생
    private void Jump() {
        playerRigidbody.linearVelocity = new Vector3(
            playerRigidbody.linearVelocity.x, jumpForce, playerRigidbody.linearVelocity.z);
        if (playerAnimator.enabled)
        {
            playerAnimator.SetTrigger("Jump");
        }
    }

    // Survivalist Animator가 사용하는 이동·점프·지면 상태를 갱신
    private void UpdateVisualAnimator(float moveAmount, bool isJumping) {
        if (visualAnimator == null)
        {
            return;
        }

        visualAnimator.SetFloat("Speed", moveAmount * moveSpeed);
        visualAnimator.SetFloat("MotionSpeed", moveAmount > 0f ? 1f : 0f);
        visualAnimator.SetBool("Grounded", isGrounded);
        visualAnimator.SetBool("FreeFall", !isGrounded && !isJumping);
        visualAnimator.SetBool("Jump", isJumping);
        visualAnimator.SetBool("Aiming", cameraController != null && cameraController.isAiming);
    }

    // 입력값에 따라 캐릭터를 전후좌우로 움직임
    private void Move() {
        // 대각선 이동 속도가 빨라지지 않도록 입력 방향의 길이를 제한
        Vector3 moveDirection = Vector3.ClampMagnitude(
            transform.forward * playerInput.move
            + transform.right * playerInput.rotate,
            1f);
        Vector3 moveDistance = moveDirection * moveSpeed * Time.deltaTime;
        // 리지드바디를 통해 게임 오브젝트 위치 변경
        playerRigidbody.MovePosition(playerRigidbody.position + moveDistance);
    }

    // 입력값에 따라 캐릭터를 좌우로 회전
    private void Rotate() {
        // 1인칭 모드에서는 마우스 X 입력으로, 그 외에는 키보드 입력으로 회전량을 계산
        float turn = useMouseLook
            ? Input.GetAxis("Mouse X") * mouseYawSpeed
            : playerInput.rotate * rotateSpeed * Time.deltaTime;
        // 리지드바디를 통해 게임 오브젝트 회전 변경
        playerRigidbody.rotation = playerRigidbody.rotation * Quaternion.Euler(0, turn, 0f);
    }
}
