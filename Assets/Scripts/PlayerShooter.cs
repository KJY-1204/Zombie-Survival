using UnityEngine;
using KevinIglesias;

// 주어진 Gun 오브젝트를 쏘거나 재장전
// 알맞은 애니메이션을 재생하고 IK를 사용해 캐릭터 양손이 총에 위치하도록 조정
public class PlayerShooter : MonoBehaviour {
    public Gun gun; // 현재 장착한 총
    public Transform gunPivot; // 총 배치의 기준점
    public Transform leftHandMount; // 총의 왼쪽 손잡이, 왼손이 위치할 지점
    public Transform rightHandMount; // 총의 오른쪽 손잡이, 오른손이 위치할 지점

    public Transform firstPersonWeaponMount; // 1인칭 모드에서 총을 배치할 기준점
    public bool useFirstPersonMount; // true면 팔꿈치 IK 힌트 대신 firstPersonWeaponMount 위치를 사용

    // 숫자키로 고를 수 있는 손에 드는 무기 슬롯 (1번=주무기, 2번=보조무기)
    private static readonly EquipmentSlot[] selectableSlots = {
        EquipmentSlot.PrimaryWeapon,
        EquipmentSlot.SecondaryWeapon
    };

    public EquipmentSlot activeSlot { get; private set; } = EquipmentSlot.PrimaryWeapon; // 현재 손에 든 슬롯

    private GameObject currentWeaponInstance; // 현재 gunPivot 아래 생성되어 있는 총 인스턴스
    private ItemData currentWeaponItem; // 현재 생성되어 있는 총의 아이템 정의
    private SurvivalistWeaponIK weaponIK; // 무기 교체 시 그립 보정을 다시 계산시킬 IK 컴포넌트
    private IKHelperTool ikHelperTool; // 왼손 IK 이펙터를 갱신할 IK Helper Tool 컴포넌트

    private Equipment equipment; // 어떤 무기를 장착 중인지 알려주는 장비 컴포넌트
    private BuildPlacer buildPlacer; // 건설 모드인지 알려주는 컴포넌트
    private PlayerInput playerInput; // 플레이어의 입력
    private Animator playerAnimator; // 애니메이터 컴포넌트

    private void Start() {
        // 사용할 컴포넌트들을 가져오기
        playerInput = GetComponent<PlayerInput>();
        playerAnimator = GetComponent<Animator>();
        equipment = GetComponent<Equipment>();
        buildPlacer = GetComponent<BuildPlacer>();
        weaponIK = GetComponentInChildren<SurvivalistWeaponIK>();
        ikHelperTool = GetComponentInChildren<IKHelperTool>();

        // 장비 슬롯이 바뀌면 손에 든 총도 따라 바뀐다
        equipment.onChanged += RefreshWeapon;
        RefreshWeapon();
    }

    private void OnDestroy() {
        if (equipment != null)
        {
            equipment.onChanged -= RefreshWeapon;
        }
    }

    private void OnEnable() {
        // 슈터가 활성화될 때 총도 함께 활성화
        if (gun != null)
        {
            gun.gameObject.SetActive(true);
        }
    }

    private void OnDisable() {
        // 슈터가 비활성화될 때 총도 함께 비활성화
        if (gun != null)
        {
            gun.gameObject.SetActive(false);
        }
    }

    private void Update() {
        // 숫자키 입력을 손에 드는 무기 슬롯 선택으로 처리
        if (playerInput.selectWeaponIndex >= 0
            && playerInput.selectWeaponIndex < selectableSlots.Length)
        {
            SelectSlot(selectableSlots[playerInput.selectWeaponIndex]);
        }

        // 아무 무기도 장착하지 않았거나 건설 모드면 발사·재장전하지 않는다
        // (건설 모드의 좌클릭은 설치 입력이므로 같이 발사되면 안 된다)
        if (gun == null || (buildPlacer != null && buildPlacer.isBuilding))
        {
            UpdateUI();
            return;
        }

        // 입력을 감지하고 총 발사하거나 재장전
        if (playerInput.fire)
        {
            // 발사 입력 감지시 총 발사
            gun.Fire();
        }
        else if (playerInput.reload)
        {
            // 재장전 입력 감지시 재장전
            if (gun.Reload())
            {
                // 재장전 성공시에만 재장전 애니메이션 재생
                playerAnimator.SetTrigger("Reload");
            }
        }

        // 조준점 UI를 갱신
        UpdateUI();
    }

    // 손에 들 무기 슬롯을 바꾼다
    public void SelectSlot(EquipmentSlot slot) {
        if (activeSlot == slot)
        {
            return;
        }

        activeSlot = slot;
        RefreshWeapon();
    }

    // 현재 슬롯에 장착된 무기를 실제로 손에 들려준다
    // 이미 같은 무기를 들고 있으면 아무것도 하지 않는다
    private void RefreshWeapon() {
        ItemData equipped = equipment.Get(activeSlot);

        if (equipped == currentWeaponItem)
        {
            return;
        }

        currentWeaponItem = equipped;
        SpawnWeapon(equipped as WeaponItemData);
    }

    // 총 인스턴스를 gunPivot 아래에 새로 생성하고 기존 총은 제거한다
    // weaponItem이 null이면 맨손 상태가 된다
    private void SpawnWeapon(WeaponItemData weaponItem) {
        if (currentWeaponInstance != null)
        {
            Destroy(currentWeaponInstance);
        }

        currentWeaponInstance = null;
        gun = null;
        leftHandMount = null;
        rightHandMount = null;

        if (weaponItem == null || weaponItem.weaponPrefab == null)
        {
            // 총과 함께 파괴된 왼손 이펙터를 IK Helper Tool이 계속 참조하면
            // 매 프레임 MissingReferenceException이 난다. 맨손 동안에는 꺼둔다
            if (ikHelperTool != null)
            {
                ikHelperTool.enabled = false;
            }

            return;
        }

        // 새 총을 gunPivot의 자식으로 생성하고 로컬 트랜스폼을 gunPivot과 일치시킨다
        currentWeaponInstance = Instantiate(weaponItem.weaponPrefab, gunPivot);
        currentWeaponInstance.transform.localPosition = Vector3.zero;
        currentWeaponInstance.transform.localRotation = Quaternion.identity;
        currentWeaponInstance.transform.localScale = Vector3.one;

        gun = currentWeaponInstance.GetComponent<Gun>();
        // 총이 자기 자신(플레이어)의 콜라이더를 조준 레이에서 제외할 수 있도록 소유자 등록
        gun.SetOwner(transform);

        leftHandMount = currentWeaponInstance.transform.Find("Left Handle");
        rightHandMount = currentWeaponInstance.transform.Find("Right Handle");

        // IK Helper Tool이 왼손을 맞출 이펙터를 새 총의 Left Handle 자식으로 새로 만든다
        if (ikHelperTool != null && leftHandMount != null)
        {
            var effector = new GameObject("IK Left Hand Effector").transform;
            effector.SetParent(leftHandMount, false);
            ikHelperTool.handEffector = effector;
            ikHelperTool.enabled = true;
        }

        // 오른손-총기 그립 오프셋이 새 총 기준으로 다시 계산되도록 갱신
        if (weaponIK != null)
        {
            weaponIK.RecalibrateGrip();
        }
    }

    // 조준점 UI 갱신
    private void UpdateUI() {
        if (gun != null && UIManager.instance != null)
        {
            // 조준점 UI에 현재 탄퍼짐 비율을 반영
            UIManager.instance.UpdateCrosshairSpread(gun.spreadRatio);
        }
    }

    // 애니메이터의 IK 갱신
    private void OnAnimatorIK(int layerIndex) {
        // 아무 무기도 장착하지 않은 맨손 상태에서는 손을 맞출 대상이 없다
        if (leftHandMount == null || rightHandMount == null)
        {
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
            playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            return;
        }

        if (useFirstPersonMount && firstPersonWeaponMount != null)
        {
            // 1인칭 모드 : 총의 기준점 gunPivot을 1인칭 전용 마운트 위치/회전으로 이동
            gunPivot.position = firstPersonWeaponMount.position;
            gunPivot.rotation = firstPersonWeaponMount.rotation;
        }
        else
        {
            // 3인칭 모드 : 총의 기준점 gunPivot을 3D 모델의 오른쪽 팔꿈치 위치로 이동
            gunPivot.position =
                playerAnimator.GetIKHintPosition(AvatarIKHint.RightElbow);
        }

        // IK를 사용하여 왼손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand,
            leftHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand,
            leftHandMount.rotation);

        // IK를 사용하여 오른손의 위치와 회전을 총의 오른쪽 손잡이에 맞춘다
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

        playerAnimator.SetIKPosition(AvatarIKGoal.RightHand,
            rightHandMount.position);
        playerAnimator.SetIKRotation(AvatarIKGoal.RightHand,
            rightHandMount.rotation);
    }
}