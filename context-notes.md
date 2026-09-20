# 작업 컨텍스트

## 2026-09-18 - Prototype 작업 씬 준비

- 사용자 요청에 따라 이전 `checklist.md`와 `context-notes.md`의 내용을 새 작업 기준으로 초기화했다.
- 새 작업 씬은 `Assets/Scenes/Prototype.unity`로 만들었다.
- 씬에는 Unity 기본 3D 템플릿의 `Main Camera`와 `Directional Light`만 있다.
- 기존 `Assets/Scenes/Main.unity`와 `Assets/Scenes/FirstPersonTest.unity`는 수정하지 않았다.
- 기능 요구사항은 아직 정해지지 않았으므로 게임플레이 오브젝트나 스크립트는 추가하지 않았다.
- 작업 시작 시 사용자 소유 변경으로 `ProjectSettings/ProjectSettings.asset`, `ProjectSettings/ShaderGraphSettings.asset`, `.vsconfig`가 이미 변경 상태였으며 건드리지 않았다.

## 2026-09-18 - TPS 백뷰와 ADS 결정

- 사용자가 최종 카메라 방향을 TPS 백뷰/숄더뷰로 결정했다.
- 기본 카메라는 플레이어의 오른쪽 어깨 뒤에 배치하고, 우클릭 중에는 더 가까운 ADS 위치와 좁은 FOV로 전환한다.
- 기존 `Gun`은 `Camera.main`의 화면 중앙 레이를 사용하므로, 전환 중에도 활성 메인 카메라를 하나만 유지해 사격 조준 기준을 보존한다.
- 이번 수직 슬라이스는 카메라와 최소 플레이어 조작에 한정하며, 인벤토리·좀비·루팅은 포함하지 않는다.
- `ThirdPersonCameraController`는 `PlayerMovement`를 자동으로 찾아 기존 마우스 X 회전을 유지하고, 카메라의 마우스 Y 피치와 `Fire2` 입력 기반 ADS 위치·FOV 전환만 담당한다.
- `Prototype`에는 기존 `Player Character` 프리팹과 50x50 단위의 `Test Ground`를 배치했고, `Main Camera`에 TPS 카메라 컨트롤러를 연결했다.
- Unity Play Mode에서 기본 숄더뷰 렌더링과 신규 콘솔 오류 0건을 확인했다. Pipeline에서는 마우스 입력을 합성하지 못하므로 우클릭 ADS, 이동, 사격은 Editor에서 수동 확인이 남아 있다.
- TPS 카메라, Prototype 씬 구성, 빌드 등록과 작업 문서를 하나의 로컬 커밋으로 기록했다. 원격 push는 외부 공유 브랜치 변경에 대한 별도 사용자 승인이 필요해 수행하지 못했다.

## 2026-09-18 - TPS 스트레이프와 Survivalist 플레이어 선택

- `PlayerMovement`는 `Vertical` 입력만 전후 이동에 사용하고 `Horizontal` 입력은 회전에만 사용하므로 TPS 좌우 이동이 동작하지 않는다.
- 기획서의 `Survivalist character`는 3인칭 프로토타입 우선 플레이어 후보이며, 실제 `Assets/Survivalist/Prefab/PlayerArmature.prefab`은 Humanoid Avatar와 `StarterAssetsThirdPerson` Animator Controller를 보유한다.
- 기존 `Player Character` 루트는 체력·입력·이동·사격·총기 참조를 이미 가진다. 이를 유지하고 임시 `Woman` 메시를 Survivalist 비주얼로 교체해 전투 시스템의 재연결 범위를 최소화한다.
- `PlayerMovement`는 전후와 좌우 입력을 로컬 `forward`와 `right` 방향으로 합성하고, `Vector3.ClampMagnitude`로 대각선 가속을 막도록 변경했다. Play Mode에서 수평 입력 `1`로 `x` 위치가 `0.00`에서 `0.03`으로 이동하는 것을 확인했다.
- 기존 전투 루트를 새 `Assets/Prefabs/TPS Player.prefab`으로 저장하고, 해당 루트의 `Survivalist Visual` 자식에는 모델용 `Animator`만 남겼다. Survivalist 원본의 `CharacterController`와 Starter Assets 입력·이동 컴포넌트는 중복 제어를 막기 위해 포함하지 않았다.
- Unity Play Mode에서 Survivalist 후방 숄더뷰 렌더링과 콘솔 오류·경고 0건을 확인했다. 모델 전용 `StarterAssetsThirdPerson` Animator Controller와 기존 `PlayerMovement`의 Animator 파라미터·총기 IK는 아직 연결하지 않았으므로, 이동 애니메이션과 손 그립 보정은 별도 작업으로 남긴다.

## 2026-09-18 - Survivalist 애니메이션과 양손 IK

- 다음 범위는 새 모델이 정지 상태에 머물지 않게 기존 이동·전투 상태를 Survivalist Animator로 전달하고, 기존 총기 마운트를 해당 Humanoid Animator의 양손 IK에 적용하는 것으로 한정한다.
- 기존 루트 Animator와 전투 스크립트는 다른 시스템이 참조할 수 있으므로 삭제하거나 컨트롤러를 교체하지 않는다. Survivalist 자식 Animator를 명시적으로 참조하는 최소 변경을 사용한다.
- `StarterAssetsThirdPerson`의 파라미터는 `Speed`, `Jump`, `Grounded`, `FreeFall`, `MotionSpeed`이며 기본 레이어의 IK 패스는 꺼져 있었다. 원본 에셋을 변경하지 않고 `Assets/Animations/SurvivalistTPS.controller` 복사본에서만 IK 패스를 활성화했다.
- 기존 루트 Animator는 이전 여성 Avatar와 Shooter Animator를 사용하므로 비활성화했다. `PlayerMovement`는 원래 플레이어 프리팹 호환을 유지하면서 `Survivalist Visual`이 존재할 때만 새 Animator 상태를 갱신한다.
- `SurvivalistWeaponIK`는 Survivalist Animator가 IK를 갱신하는 시점에 기존 `PlayerShooter`의 총기 마운트를 읽어 양손 목표점으로 전달한다. Play Mode에서 수평 입력을 주입했을 때 `Speed=5`, `MotionSpeed=1`, `Grounded=true`를 확인했고, 양손 IK 목표와 각 총기 손잡이의 거리는 모두 0이었다.
- Survivalist 캐릭터 교체와 스트레이프 커밋 두 개는 원격 `main`에 push했다. 이번 애니메이션·IK 변경은 별도 커밋과 push로 이어서 기록한다.

## 2026-09-18 - Survivalist 총기 피벗 보정

- 현 구현은 총기 피벗을 오른쪽 아래팔의 위치로만 옮긴 뒤 양손을 IK로 총기 손잡이에 맞춘다. 이 방식은 피벗의 회전과 손잡이의 로컬 오프셋을 무시해 총구가 배 부근에 생길 수 있다.
- 총기 피벗과 오른손 손잡이의 상대 위치·회전을 보존하고, 오른손 본 위치에 오른손 손잡이가 정확히 겹치도록 피벗 변환을 역산한다. 오른손은 본래 애니메이션을 유지하고 왼손만 보조 IK로 맞춘다.
- 측면 렌더링에서 총구 높이는 상체 앞으로 보정됐지만 손이 모델에 반영되지 않았다. `Survivalist Visual` 루트와 `Geometry/SK_Military_Survivalist` 자식에 Animator가 각각 있고, 실제 렌더 메시는 자식 Animator가 구동한다. 따라서 이동 상태와 IK 컴포넌트를 자식 Animator로 연결한다.
- 총기 위치는 상체 앞 고정점 대신 오른손 본과 총기 오른손 그립의 초기 상대 변환으로 매 IK 프레임 역산한다. 오른손 애니메이션이 총을 직접 움직이고, 왼손만 IK로 전방 손잡이를 따르게 한다.
- Play Mode에서 오른손 본과 총기 오른손 손잡이 거리는 약 `0.00000006`으로 확인됐고, 총구는 총기 모델의 `Fire Position`을 그대로 따른다. 기본 자세는 오른손을 따라가는 힙 파이어이며, ADS에서 어깨 조준 자세로 올리는 보간은 별도 작업으로 남긴다.

## 2026-09-18 - IK Helper Tool 왼손 그립

- `Assets/Kevin Iglesias/IKHelperTool`에는 `KevinIglesias.IKHelperTool`과 소총 예제 프리팹·애니메이션이 있다. 문서의 의도는 물체를 제어하는 손은 애니메이션으로 유지하고 반대 손만 IK 효과기로 보정하는 것이다.
- 현재 오른손은 동적으로 총기 피벗을 제어한다. 따라서 기존 직접 `SetIK` 왼손 로직은 제거하고, 총기 왼손 손잡이를 따르는 효과기와 가중치 1의 IK Switch를 IK Helper Tool에 연결한다.
- `IK Left Hand Effector`는 총기의 `Left Handle` 자식으로 만들어 총기와 함께 움직이며, `IK Switch`는 실제 메시 Animator의 자식으로 로컬 Y값 `1`을 사용해 IK Helper Tool 가중치를 1로 만든다. Play Mode에서 왼손과 효과기 거리는 약 1.8cm, 오른손과 오른손 그립 거리는 약 0이었다.

## 2026-09-18 - Claude 인수인계

- `CLAUDE_HANDOFF.md`에 기준 커밋, 보존할 사용자 변경, 실제 Survivalist Animator 계층, IK Helper Tool 연결, 허리춤 힙 파이어 원인, 소총 자세 애니메이션을 적용하는 권장 순서와 검증 명령을 정리했다.

## 2026-09-18 - 소총 조준 자세 (힙 파이어 -> ADS)

- 인수인계 메모의 권장 순서대로 `IKHelperTool/Animations/IK@RifleIdle.anim`, `IK@RifleRun.anim`을 후보로 확인했다. 둘 다 `humanMotion=true`인 Humanoid 클립이라 Survivalist Avatar에 원본 리그와 무관하게 리타겟된다.
- 원본 IK Helper Tool 에셋은 건드리지 않고 기존 복사본인 `SurvivalistTPS.controller`에 `Aiming`(Bool) 파라미터와 `Rifle Aim Blend` 상태를 추가했다. 이 상태는 기존 `Idle Walk Run Blend`와 동일한 `Speed` 파라미터 기준 Simple1D 블렌드 트리(`Idle` 임계값 0, `Run` 임계값 6)를 사용해 IK@RifleIdle/IK@RifleRun을 블렌드한다.
- 전환은 `Idle Walk Run Blend` <-> `Rifle Aim Blend` (Aiming 조건, 0.1초)이며, 조준 중 점프/낙하도 끊기지 않도록 `Rifle Aim Blend` -> `InAir`/`JumpStart` 전환도 기존과 동일한 조건·시간으로 추가했다.
- `PlayerMovement`는 기존에 없던 카메라 참조가 필요해 `ThirdPersonCameraController`를 `FindFirstObjectByType`으로 찾고, `UpdateVisualAnimator`에서 `cameraController.isAiming`을 `Aiming` 파라미터로 전달한다. `SurvivalistWeaponIK`와 `PlayerShooter`는 오른손 애니메이션을 그대로 따라가므로 별도 수정 없이 새 조준 포즈에 자동으로 맞춰진다.
- Pipeline은 마우스 우클릭(Fire2)을 합성하지 못해, Play Mode에서 `PlayerMovement.enabled = false`로 실시간 덮어쓰기를 잠깐 멈추고 Animator의 `Aiming` 파라미터를 직접 토글해 검증했다(검증 후 다시 `enabled = true`로 복구). 측정 결과: 힙 파이어 상태 총구 Y=1.194 (상체 Y=1.387보다 낮음, 기존 문제 재확인) -> 조준 상태 총구 Y=1.380 (상체 Y=1.308보다 높음). 오른손-그립, 왼손-효과기 거리는 조준 상태에서 모두 0.00000이었다.
- `recompile_status`는 성공, `console_status`에는 신규 스크립트/콘솔 오류가 없었다(유일한 오류 1건은 이 세션 중 eval 호출 하나가 5초 타임아웃난 MCP 전송 오류였고 게임 코드와 무관).
- 남은 작업은 실제 마우스 우클릭으로 Unity Editor에서 ADS 전환·이동·사격을 육안 확인하는 수동 테스트뿐이다.

## 2026-09-18 - 총기 파지 프리팹 재구성 (사용자 요청으로 처음부터 재작성)

- 사용자가 Play Mode 스크린샷으로 확인한 결과 기본(비조준) 상태에서 왼손이 총 몸통에 바짝 붙어 팔꿈치가 몸에 끼는 부자연스러운 한손 파지처럼 보였다. 수치상 왼손-이펙터 거리는 1.8cm로 작았지만, 실제 원인은 총을 잡지 않는 기본 로코모션 애니메이션(구 Survivalist idle/walk/run)이 애초에 무기를 들지 않는 클립이라 오른손이 힙 높이에서 자연스럽게 스윙하고 있었기 때문이었다.
- 사용자가 "플레이어 프리팹을 처음부터 새로 만드는 게 낫겠다"고 판단해 전체 재구성을 요청했다. 기존 `TPS Player.prefab`을 `TPS Player (Backup 0918).prefab`으로 백업한 뒤, `Survivalist Visual` 하위 트리를 `Assets/Survivalist/Prefab/PlayerArmature.prefab` 원본에서 완전히 새로 인스턴스화했다(여러 세션에 걸쳐 누적된 비활성 Animator·중복 컴포넌트 잔재 제거). `CharacterController`, `ThirdPersonController`, `BasicRigidBodyPush`, `StarterAssetsInputs`, Input System `PlayerInput`, 그리고 원본 프리팹에 남아있던 우리 프로젝트 전용 `PlayerInput` 컴포넌트까지 모두 제거했다. `SurvivalistWeaponIK`와 `KevinIglesias.IKHelperTool`은 새 `SK_Military_Survivalist` 자식 Animator에 다시 붙이고, 기존 `Gun`의 `IK Left Hand Effector`를 그대로 재사용해 왼손 효과기로 연결했다.
- 이어서 사용자가 "Human Soldier Animations FREE 에셋으로 총을 올바르게 잡게 하라"고 요청했다. `Assets/Kevin Iglesias/Human Animations/Unity Demo Scenes/Human Soldier Animations/AnimatorControllers/HumanM@SoldierAnimations.controller`를 조사한 결과, 이 에셋은 다른 아바타(HumanM_ModelAvatar)를 쓰지만 완전한 Humanoid 리타겟 클립(`Weapon Hold Arms > Rifle`, 151개 커브 바인딩, humanMotion=true)과 팔 전용 `Human Arms Mask`(양팔+양손+손가락 전체, 다리/몸통/머리는 제외)를 제공한다는 것을 확인했다.
- `SurvivalistTPS.controller`에 새 레이어 `Weapon Hold Arms`를 추가했다(mask=Human Arms Mask, blendingMode=Override, weight=1, 상태 하나만 가지고 항상 켜져 있음, 파라미터 게이팅 없음). 이 레이어는 Base Layer가 어떤 상태(Idle Walk Run Blend든 Rifle Aim Blend든)에 있든 상관없이 팔/손/손가락을 항상 `HumanM@WeaponHold_Rifle01` 포즈로 덮어써서, 두 손으로 총을 쥔 자연스러운 자세를 걷기/달리기/대기 전체 구간에서 유지한다.
- 이 레이어를 추가한 뒤 부가 효과로 왼손-이펙터 거리가 1.8cm에서 정확히 0.00000으로 줄었다(오른손도 0.00000 유지). 새 팔 포즈가 왼팔을 총의 실제 도달 가능 범위 안으로 자연스럽게 가져다 놓았기 때문이며, 별도의 이펙터 위치 재보정 작업(문서의 Step 5-7 수동 캘리브레이션)은 필요하지 않았다.
- 기존 `Rifle Aim Blend`/`Aiming` 파라미터(지난 세션에 힙파이어→조준 자세 전환용으로 추가)는 이제 팔 포즈 차원에서는 사실상 무의미해졌다(Weapon Hold Arms가 항상 팔을 덮어쓰므로). 다만 이번 작업 범위를 벗어나므로 제거하지 않고 그대로 남겨뒀다 — 다리/카메라 줌 차원에서는 여전히 약간의 차이를 만들 수 있고, 나중에 `Shoots Upper Body > Rifle Aim`(같은 에셋의 진짜 조준 포즈) 레이어로 대체할 여지가 있다.
- Play Mode 스크린샷(측면/후면/정면)으로 새 파지가 자연스러운 양손 무기 휴대 자세임을 육안으로 확인했다. 검증 후 `PrefabUtility.SaveAsPrefabAssetAndConnect`로 씬의 인스턴스를 `TPS Player.prefab`에 다시 저장했다. `recompile_status` 성공, `console_status`에 신규 스크립트 오류 없음(세션 중 유일한 오류는 MCP eval 호출 타임아웃 1건으로 게임 코드와 무관).

## 2026-09-18 - 플레이어 캐릭터 프리팹 완전 재작성 (사용자가 기존 프리팹 전부 삭제 후 재요청)

- 사용자가 "이상하게 참고할까봐" `TPS Player.prefab`, `TPS Player (Backup 0918).prefab`, `Player Character.prefab`, `PlayerTestRig.prefab`, `Assets/Prefabs/Gun.prefab`을 직접 디스크에서 삭제했다. 씬(`Prototype.unity`)에서도 `Player Character` 게임오브젝트가 사라진 상태였다. 삭제된 프리팹은 git으로 되살리지 않고 완전히 새로 구성했다(사용자가 기존 것을 참고하지 않길 원했으므로).
- 총(`Gun.prefab`)은 초기 커밋부터 있던 원본 코스 에셋이었는데 사용자가 삭제했다. 다행히 같은 리깅 컨벤션(Left Handle/Right Handle 로컬 회전값이 완전히 동일: `(21.97, 58.91, 150.52)`/`(357.18, 320.43, 271.35)`)을 가진 `Assets/Prefabs/Weapons/Pistol Gun.prefab`이 남아있어 이것을 재사용했다. 짝이 되는 `Assets/ScriptableData/Pistol Data.asset`(GunData, 사운드 클립 포함 완비)도 그대로 있어서 재사용했다.
- 플레이어 루트는 초기 커밋의 `Player Character.prefab`(git 히스토리로 원본 수치만 참고, 파일 자체는 복원하지 않음) 설정을 따라 CapsuleCollider(radius 0.2, height 1.5, center (0,0.75,0)), Rigidbody(mass 1, angularDrag 20, constraints=FreezeRotationX|FreezeRotationZ)를 그대로 재현했다.
- `Survivalist Visual`은 지난 세션과 동일한 절차로 `PlayerArmature.prefab`에서 새로 인스턴스화하고 `CharacterController`/`ThirdPersonController`/`BasicRigidBodyPush`/`StarterAssetsInputs`/Input System `PlayerInput`을 제거했다. `ThirdPersonController`를 `CharacterController`보다 먼저 제거해야 `RequireComponent` 의존성 오류가 나지 않는다(지난 세션에서 겪은 순서 문제를 이번엔 피함).
- `PlayerHealth.Awake()`와 `PlayerMovement.Start()`가 루트 게임오브젝트에서 직접 `GetComponent<Animator>()`를 호출하기 때문에, 루트에 컨트롤러/아바타 없는 **비활성 더미 Animator**를 추가해야 `MissingComponentException`이 나지 않는다. 이걸 빠뜨려서 Play Mode 진입 직후 232개의 예외가 발생했었고, 원인을 확인한 뒤 더미 Animator를 추가해 해결했다.
- 체력 슬라이더 UI는 기존 `Assets/Prefabs/HUD Canvas.prefab`(전역 탄약/점수/웨이브 UI)과는 별개로, 플레이어 전용 Canvas(ScreenSpaceOverlay)+Slider를 코드로 새로 만들었다(Background+Fill Area/Fill 최소 구성, interactable=false). `PlayerHealth`의 `deathClip`/`hitClip`/`itemPickupClip`은 git 초기 커밋에서 GUID로 확인한 `Assets/Audios/Woman Die.ogg`, `Woman Damage.ogg`, `Pick Up.ogg`를 그대로 연결했다.
- Play Mode에서 오른손-그립/왼손-이펙터 거리 모두 0.00000, `ThirdPersonCameraController.target`이 새 `PlayerMovement`를 자동으로 찾은 것, 체력 슬라이더가 게임 뷰에 렌더링되는 것을 확인했다. `Assets/Prefabs/Player Character.prefab`로 저장했고 `recompile_status`/`console_status` 모두 깨끗하다.

## 2026-09-18 - 총이 대각선으로 들리는 문제 수정

- 사용자가 "총을 대각선으로 들고 있는거 빼면 괜찮다"고 피드백. 스크린샷으로 확인해보니 총구가 거의 위쪽(world up 성분 0.79)을 향하고 있었다.
- 원인: `SurvivalistWeaponIK`는 `gunPivot`의 회전을 `rightHand.rotation * Inverse(pivotToRightGripRotation)`으로 매 프레임 계산하고, `pivotToRightGripRotation`은 Awake 시점에 `Right Handle`의 로컬 회전값을 그대로 캡처한다. 이 `Right Handle` 로컬 회전(357.18, 320.43, 271.35)은 원래 이 프로젝트의 초기 커밋 도구(다른 캐릭터/다른 손 포즈 컨벤션)를 기준으로 만들어진 값이었고, 지금 오른손을 구동하는 `Weapon Hold Arms`(Human Soldier Animations, 라이플용) 포즈의 손목 방향과는 전혀 다른 기준이었다. 두 값이 안 맞아서 총이 엉뚱한 각도로 튀어나왔다.
- 수학적으로 확인: `pivotToRightGripRotation`은 결국 "오른손의 로컬(캐릭터 기준) 회전값"과 같아진다. 원하는 목표 회전(플레이어 정면·수평, 즉 `Quaternion.LookRotation(player.forward, Vector3.up)`)을 하나 정하고, 그 순간의 실제 `rightHand.rotation`으로 `newRightHandleLocalRotation = Inverse(target) * rightHand.rotation`을 계산하면, 이후 플레이어가 어떤 방향을 보고 있어도(회전에 관계없이) `weaponRotation`이 항상 플레이어의 현재 정면 방향과 정확히 일치한다는 것을 증명했다(플레이어 회전 P(t)가 손의 월드 회전에도 그대로 곱해지기 때문에 서로 상쇄됨).
- Play Mode에서 실제 오른손 회전을 측정해 `Right Handle.localRotation`을 `(0.19779, -0.81233, -0.49433, 0.23800)`(쿼터니언)으로 교체했다. 적용 후 `gunPivot.forward=(0,0,1)`, `up=(0,1,0)`으로 완벽하게 플레이어 정면·수평을 향했고, 오른손-그립/왼손-이펙터 거리는 여전히 0.00000이었다(왼손 IK는 위치+회전을 매 프레임 재계산하므로 총의 새 방향에 자동으로 다시 맞춰짐).
- 이 보정값은 `Pistol Gun.prefab` 원본이 아니라 `Player Character.prefab` 안의 `Gun` 인스턴스에만 적용했다(이 값은 "이 플레이어의 이 팔 포즈"에 종속적인 보정이며, 다른 곳에서 `Pistol Gun.prefab`을 다른 캐릭터/포즈로 재사용할 경우에는 맞지 않을 수 있기 때문).
- `recompile_status`/`console_status` 모두 깨끗함(기존에 있던 "Animator is not playing an AnimatorController" 경고 2건은 더미 Animator + Reload 트리거 호출 때문이며 이번 수정과 무관, 향후 필요시 별도로 정리).

## 2026-09-18 - 조준 시 총 처짐 / 총구 이펙트 위치 / 탄피 이펙트 제거

- 사용자가 "조준하면 총이 대각선 아래로 내려가고 총알의 불꽃이 총구에서 안나온다 그리고 탄피 떨어지는 효과는 제거해줘"라고 요청.
- **조준 시 총 처짐 원인**: `Aiming=true`일 때 Base Layer가 `Idle Walk Run Blend`에서 `Rifle Aim Blend`(IK@RifleIdle/IK@RifleRun)로 전환되는데, `Weapon Hold Arms` 오버라이드 레이어(Human Arms Mask)는 팔/손/손가락만 덮어쓰고 척추·어깨는 덮어쓰지 않는다. `Rifle Aim Blend`의 척추 커브가 `Idle Walk Run Blend`와 다르게 캐릭터를 앞으로 기울이면서 오른손의 **월드** 회전이 바뀌었고, 지난번에 계산한 `Right Handle` 보정값은 "척추가 기본 자세일 때"만 유효했기 때문에 조준 중에는 다시 어긋났다. 실측: 기본 상태 `gunPivot.forward=(0,0,1)` → 조준 상태 `(0.57, -0.26, 0.78)`.
- 지난 세션 노트에서 이미 "`Rifle Aim Blend`는 팔 포즈 차원에서 무의미해졌다"고 적어뒀던 대로, 이번엔 실제 버그까지 유발한다는 게 확인되어 완전히 제거했다: `SurvivalistTPS.controller`에서 `Aiming` 파라미터, `Rifle Aim Blend` 상태(및 딸린 블렌드 트리), 관련 전환 3개를 모두 삭제했다. Base Layer는 항상 `Idle Walk Run Blend`만 사용하므로 척추 포즈가 고정되고, 조준 여부는 이제 순수하게 카메라 줌(`ThirdPersonCameraController`)만으로 표현된다.
- `PlayerMovement.cs`에서 `cameraController` 필드, `FindFirstObjectByType<ThirdPersonCameraController>()` 호출, `visualAnimator.SetBool("Aiming", ...)` 줄을 제거했다(파라미터가 없어져 호출해도 경고만 뜨고 아무 효과가 없으므로 죽은 코드였다).
- **총구 이펙트 위치**: `MuzzleFlashEffect`의 로컬 위치 `(0, 0.08, 0.20)`가 실제 총구인 `Fire Position`(`(0, -0.05, 0.85)`)과 전혀 다른 곳에 있었다. 두 무기(옛 Uzi, 현재 Pistol Gun)가 같은 좌표값을 공유하는 걸로 봐서 코스 공용 템플릿의 기본값이 그대로 남아있던 것으로 보인다. `MuzzleFlashEffect`의 로컬 위치/회전을 `Fire Position`과 동일하게 맞춰 총구에서 정확히 발사되도록 했다.
- **탄피 이펙트 제거**: `Gun.cs`의 `Shot()` 코루틴이 `shellEjectEffect.Play()`를 무조건 호출하고 있어서, 단순히 참조만 비우면 `NullReferenceException`이 난다. `if (shellEjectEffect != null)` 가드를 추가한 뒤, 플레이어 총 인스턴스에서 `ShellEjectEffect` 자식 오브젝트를 완전히 삭제하고 `Gun.shellEjectEffect`를 null로 비웠다(다른 무기가 이 이펙트를 쓰고 싶다면 그대로 참조를 넣으면 되므로 스크립트 자체는 여전히 재사용 가능).
- Play Mode에서 검증: 조준 강제 시에도 `gunPivot.forward=(0,0,1)` 유지, `gun.Fire()` 호출 시 `MuzzleFlashEffect` 월드 위치가 `Fire Position`과 완전히 일치(dist=0.00000), 오류 없이 발사됨. `recompile_status`/`console_status` 모두 깨끗함.

## 2026-09-18 - 조준점 / 탄퍼짐 / 탄퍼짐에 따른 조준점 벌어짐

- 사용자가 "조준점을 만들어 주고 조준점으로 총알이 발사되게 해줘 그리고 탄퍼짐도 구현해주고 조준점이 탄퍼짐에 따라 벌어지는 효과도 넣어줘"라고 요청. `Gun.Shot()`은 이미 화면 중앙(뷰포트 0.5,0.5) 기준으로 발사하고 있었으므로, 조준점은 그 지점을 시각적으로 보여주기만 하면 된다.
- `GunData`에 `minSpread`(정지 시 최소 탄퍼짐), `maxSpread`(최대 탄퍼짐), `spreadIncrement`(발사 1회당 증가량), `spreadRecoverSpeed`(초당 회복량) 4개 필드를 추가했다.
- `Gun.cs`에 런타임 `currentSpread` 상태를 추가했다. `OnEnable`에서 `minSpread`로 초기화, 새로 추가한 `Update()`에서 시간이 지나면 `minSpread`까지 서서히 회복시킨다. `Shot()`에서는 `ApplySpread()`로 조준 레이 방향을 카메라의 실제 상하(`transform.up`)·좌우(`transform.right`) 축 기준 무작위 각도(`±currentSpread`)만큼 흔들고, 발사 후 `currentSpread`를 `spreadIncrement`만큼 늘린다(월드축이 아닌 카메라 축 기준으로 회전시켜야 위/아래를 보고 쏠 때도 탄퍼짐이 일관되게 동작한다).
- 탄퍼짐 비율은 `Gun.spreadRatio`(0~1, `Mathf.InverseLerp(minSpread, maxSpread, currentSpread)`)로 외부에 노출했다.
- 조준점 UI는 `Assets/Scripts/SpreadCrosshair.cs`(새 스크립트)로 구현했다. 이름을 `Crosshair`가 아니라 `SpreadCrosshair`로 지은 이유: 이미 소유 중인 `Too Many Crosshairs` 에셋 패키지의 `HELLSLAYER Crosshairs/_Demo/Scripts/Crosshair.cs`가 네임스페이스 없이 전역에 `Crosshair` 클래스를 정의하고 있어 이름이 충돌했다(그 에셋의 데모 스크립트는 손대지 않고 우리 스크립트 쪽을 리네임해서 해결). 상/하/좌/우 4개의 흰색 UI 바(Image)의 `anchoredPosition`을 탄퍼짐 비율에 따라 중심에서 `baseGap`~`baseGap+maxGap` 픽셀만큼 벌어지게 한다.
- `UIManager`에 `crosshair` 필드와 `UpdateCrosshairSpread(float)` 메서드를 추가했고, `PlayerShooter.UpdateUI()`가 매 프레임 `UIManager.instance.UpdateCrosshairSpread(gun.spreadRatio)`를 호출하도록 연결했다(기존 `UpdateAmmoText` 호출과 동일한 패턴).
- 조준점 UI 자체는 기존 `Assets/Prefabs/HUD Canvas.prefab`(전역 UI, `UIManager` 보유) 안에 자식으로 만들어 넣었다. 이 프리팹이 지금까지 `Prototype` 씬에 배치된 적이 없었다는 걸 발견해서(즉 `UIManager.instance`가 그동안 계속 null이었음), 조준점이 실제로 보이도록 씬에 인스턴스를 배치했다.
- Play Mode에서 `gun.Fire()` 한 번 호출 후 `spreadRatio`가 정확히 `(1.2)/(6-1)=0.24`로 계산되는 것과, 조준점의 `top`/`left` `anchoredPosition`이 그 비율에 맞춰 실제로 움직이는 것을 확인했다.
- 작업 중 `HUD Canvas.prefab`의 `Gameover UI` 자식 오브젝트가 기본값으로 활성화되어 있어(원래부터 있던, 이번 작업과 무관한 프리팹 설정) 스크린샷에 "YOU DIE" 화면이 항상 덮여 보이는 것을 발견했다. 사용자가 "요청하지 않은 것은 추가/수정하지 말라"고 명확히 해서 이 부분은 조사만 하고 손대지 않았다 — 있는 그대로 남겨둔다.
- (참고, 사소한 이슈) `EditorSceneManager.SaveOpenScenes()`가 이번 세션에서 씬 변경사항을 디스크에 실제로 쓰지 않는 것처럼 보인 적이 있었다. `MarkSceneDirty()` 후 `EditorSceneManager.SaveScene(scene)`을 명시적으로 호출하니 정상적으로 저장됐다.

## 2026-09-18 - 쓸모없는 UI 삭제 (게임오버/웨이브/스코어/탄약)

- 사용자가 "쓸모없는 UI는 삭제하고 프리팹을 갱신해"라고 요청. 범위가 모호해 `AskUserQuestion`으로 확인한 결과 게임오버 UI, 웨이브 텍스트, 스코어 텍스트, 탄약 수 텍스트 전부를 대상으로 하고, 관련 코드(`SetActiveGameoverUI`, `GameManager` 호출부 등)까지 함께 정리하기로 확정했다(직전 세션에서 "요청하지 않은 건 건드리지 말라"고 명확히 한 사용자였으므로, 이번엔 범위를 먼저 명시적으로 확인받았다).
- `UIManager.cs`를 조준점(`SpreadCrosshair`) 갱신 하나만 담당하도록 완전히 재작성했다. `ammoText`/`scoreText`/`waveText`/`gameoverUI` 필드와 `UpdateAmmoText`/`UpdateScoreText`/`UpdateWaveText`/`SetActiveGameoverUI`/`GameRestart` 메서드, 그리고 이제 안 쓰는 `UnityEngine.SceneManagement`/`UnityEngine.UI` using을 모두 제거했다.
- `GameManager.cs`는 `AddScore()`/`EndGame()`에서 UI 호출(`UpdateScoreText`, `SetActiveGameoverUI`)만 제거했다. `score`/`isGameover` 필드 자체는 그대로 남겼다 — 특히 `isGameover`는 `PlayerInput.cs`가 입력 게이팅에 쓰는 게임플레이 상태라 UI와 무관하게 반드시 유지해야 한다.
- `ZombieSpawner.cs`는 `UpdateWaveText`만 호출하던 `UpdateUI()` 메서드 전체와 `Update()` 안의 호출부를 제거했다(다른 로직에 영향 없음).
- `PlayerShooter.cs`는 `UpdateUI()`에서 `UpdateAmmoText` 호출 줄만 제거하고, 지난 세션에 구현한 `UpdateCrosshairSpread` 호출은 그대로 남겼다.
- `HUD Canvas.prefab`은 `PrefabUtility.LoadPrefabContents`로 열어 `Ammo Display`/`Score Text`/`Enemy Wave Text`/`Gameover UI` 4개 자식 오브젝트를 `DestroyImmediate`로 삭제하고 `SaveAsPrefabAsset`으로 저장했다. 콘솔 로그로 4개 모두 존재가 확인되어 삭제됐고, 삭제 후 남은 자식은 `Crosshair` 하나뿐임을 확인했다.
- `recompile_status`(컴파일 성공, 오류 0건)와 `console_status`(consoleErrors=0)로 검증했다. 콘솔에 있던 "Animator is not playing an AnimatorController" 경고 2건은 더미 Animator + Reload 트리거 호출 때문이며(지난 세션에서 이미 원인 파악, 이번 작업과 무관) 이번에도 그대로 남아있다 — 요청 범위 밖이라 손대지 않았다.
- `ProjectSettings/ProjectSettings.asset`, `ProjectSettings/ShaderGraphSettings.asset`, `.vsconfig`는 이번 세션 시작 전부터 이미 변경/미추적 상태였던 사용자 소유 변경이라 커밋에 포함하지 않았다(계속 유지 중인 원칙).
- 변경 사항을 하나의 커밋(`57d16eb`)으로 기록하고 원격 `main`에 push했다.

## 2026-09-18 - 다른 무기(저격총/돌격소총)로 무기 교체 시스템 구현

- 사용자가 "다른 무기들을 착용해보자"라고 요청. 범위가 넓어 `AskUserQuestion`으로 확인한 결과 총기류(Weapons Pack - Realistic LowPoly)를 근접무기보다 먼저 시도하고, 단순 IK 확인용이 아니라 실제 무기 교체(장착) 시스템을 구현하는 것으로 확정했다. 근접무기(Crusader/medieval)는 발사/탄약 개념이 없는 완전히 다른 전투 방식(스윙, 히트박스, 별도 애니메이션)이 필요해 이번 슬라이스에서는 제외하고 다음 작업으로 남겼다.
- 기존에 `Assets/Prefabs/Weapons/Sniper Gun.prefab`과 `Assets/ScriptableData/Sniper Data.asset`/`Assault Rifle Data.asset`이 이미 준비되어 있었지만(아마 Codex가 미리 만들어둠) 실제 게임 로직 어디에서도 참조되지 않는 죽은 에셋이었다(`Assault Rifle Gun.prefab`은 아예 없었음). `PlayerInput.cs`도 여전히 레거시 Input Manager(`Input.GetAxis`/`GetButton`) 기반이라 Input System `PlayerInput` 컴포넌트와는 무관하다.
- **사전 정리(공용 에셋 버그 수정)**: `Pistol Gun.prefab`과 `Sniper Gun.prefab`의 `Right Handle` 로컬 회전이 여전히 옛 템플릿 값(삼각비 회전, `Human Soldier Animations` 이전 컨벤션)이었고, `MuzzleFlashEffect`도 각자의 `Fire Position`과 위치가 어긋나 있었다(이전 세션에서 `Player Character.prefab` 안의 개별 Gun 인스턴스에만 임시로 고쳤던 것과 동일한 근본 버그가 공용 프리팹 자체에는 남아있었음). 두 프리팹 모두 공용 에셋 레벨에서 `Right Handle.localRotation`을 검증된 보정값 `(0.19779, -0.81233, -0.49433, 0.23800)`으로, `MuzzleFlashEffect`를 각자의 `Fire Position`과 일치하도록 고쳤다. 이 보정값이 모든 총에 그대로 재사용 가능했던 이유: `Left/Right Handle`의 로컬 **회전**은 총마다 동일한 템플릿 값을 쓰고(위치만 총 크기에 맞춰 다름), 보정값은 "이 플레이어의 팔 포즈에서 오른손이 실제로 향하는 방향"에만 의존하는 값이라 무기 모델에 무관하기 때문이다.
- **Assault Rifle Gun.prefab 신규 제작**: `Sniper Gun.prefab`을 템플릿으로 인스턴스화한 뒤 `Model`만 `WeaponsPack (LowPoly)/American AssaultRifle.prefab`로 교체했다. 실제 총기 메쉬의 총구/그립 위치를 눈대중으로 추측하지 않기 위해, 임시로 씬에 인스턴스화한 뒤 Cube/Sphere/Cylinder 마커를 여러 후보 좌표에 배치하고 Scene View 스크린샷으로 눈으로 확인하며 좌표를 확정했다(`Fire Position=(0,0,0.62)`, `Left Handle=(0,0,0.30)`, `Right Handle=(0,-0.05,0)`). 검증 후 임시 오브젝트는 모두 삭제했다.
  - 이 과정에서 실수로 `CreatePrimitive`로 만든 마커의 색을 `sharedMaterial.color`로 바꿨는데, 이 머티리얼이 URP 기본 `Lit.mat`(패키지 공용 에셋, `Test Ground`도 같은 걸 참조)이라 씬의 땅 색이 초록으로 바뀌는 부작용이 발생했다. 이후 `Color.white`로 되돌려 정상화했다(패키지 에셋이라 git 추적 대상은 아니지만, 앞으로는 임시 테스트용 프리미티브에 항상 `new Material(...)` 인스턴스를 새로 만들어 쓰는 것으로 주의할 것).
- **무기 교체 아키텍처**: 별도의 `WeaponSwitcher` 컴포넌트를 새로 만드는 대신 `PlayerShooter`가 직접 `weaponPrefabs`(GameObject 배열)와 `EquipWeapon(int)`을 갖도록 확장했다. 이유: 새 컴포넌트를 따로 만들면 `Awake()` 순서(어느 컴포넌트가 먼저 `gun`/`rightHandMount`를 채우는지)에 의존하게 되어 실행 순서 버그 위험이 생긴다. `PlayerShooter.Start()`에서 `EquipWeapon(0)`으로 최초 장착까지 처리하므로, 기존에 프리팹에 정적으로 박혀 있던 `Gun Pivot/Gun` 자식을 완전히 제거하고 항상 런타임에 인스턴스화하는 단일 경로로 통일했다.
- **IK 재보정이 반드시 필요했던 이유**: `SurvivalistWeaponIK`는 Awake 시점에 딱 한 번 "오른손 그립 오프셋"(`pivotToRightGripPosition/Rotation`)을 캐시해서 매 프레임 재사용한다. 이 오프셋은 사실상 무기의 `Right Handle.localPosition/localRotation`과 수학적으로 동일한 값(부모 `gunPivot`의 현재 상태와 무관하게 상쇄되어 사라짐 - `InverseTransformPoint(TransformPoint(x)) = x` 항등식)이라 계산 자체는 언제 호출해도 항상 정확하다. 다만 캐시된 값 자체가 "그 순간 장착된 무기"의 것이므로, 무기를 교체하면 이 캐시가 이전 무기 기준으로 남아 새 무기의 그립 위치와 어긋난다. 그래서 `Awake()`의 계산 로직을 공개 메서드 `RecalibrateGrip()`으로 분리하고, `PlayerShooter.EquipWeapon()`이 무기를 바꿀 때마다 다시 호출하도록 연결했다.
- **왼손 IK 이펙터도 무기마다 새로 생성**: `IK Left Hand Effector`는 각 무기의 `Left Handle` 아래 원점(0,0,0)에 만드는 자식 오브젝트로, 원본 무기 프리팹(`Pistol/Sniper/Assault Rifle Gun.prefab`)에는 포함돼 있지 않고 `Player Character.prefab`에만 있던 인스턴스 전용 오브젝트였다. `EquipWeapon()`이 무기를 바꿀 때마다 새 `Left Handle` 아래 이 오브젝트를 새로 만들고 `IKHelperTool.handEffector` 참조를 갱신한다.
- **검증 시 발견한 함정**: 무기를 바꾼 직후 같은 eval 스크립트 안에서 바로 손/총구 거리를 측정하면 값이 크게 어긋나 보인다(예: Sniper 장착 직후 즉시 측정 시 rightDist=0.45). 원인은 `gunPivot`의 실제 위치/회전이 `Animator.OnAnimatorIK` 콜백에서만 갱신되는데, 같은 프레임 안에서 `EquipWeapon()` 호출 직후에는 아직 그 콜백이 다시 돌지 않아 이전 무기 기준의 낡은 `gunPivot` 값을 읽게 되기 때문이다. 별도의 MCP 호출로 나눠 실제 시간이 흐른 뒤 다시 측정하면(오른손 기준) 정확히 0.00000으로 수렴한다. 이 프로젝트에서 `eval`로 즉각적인 IK 결과를 검증할 때는 항상 최소 한 프레임 이상 시간차를 두고 측정해야 한다.
- **왼손 그립의 잔여 오차**: 오른손은 항상 0.00000으로 정확히 맞지만, 왼손은 무기별로 약간의 잔여 오차가 남을 수 있다(Sniper 기준 약 2.4cm). 이는 버그가 아니라 "Weapon Hold Arms" 팔 포즈 애니메이션이 실제로 도달 가능한 범위와 내가 지정한 `Left Handle` 좌표가 무기마다 완벽히 일치하지는 않기 때문이며(오른손과 달리 왼손은 `IK Helper Tool`이 자유롭게 끌어당기는 보조 IK라 한계가 있음), 총의 조준 방향(오른손 기준)에는 영향이 없다. 무기별 정밀한 손 위치 캘리브레이션은 필요하면 나중에 폴리싱 단계에서 다룬다.
- Play Mode에서 Pistol(기본)/Sniper/Assault Rifle 세 가지 모두 `EquipWeapon()`으로 전환해 오른손 그립 거리 0.00000, 총구 이펙트-Fire Position 거리 0.00000, `gunPivot`이 캐릭터 정면을 향함(Scene View 스크린샷으로 어깨총 자세 확인)을 검증했다. `recompile_status`/`console_status` 모두 새 오류 없음.
- **문서-코드 불일치 발견(미해결로 기록만)**: `GAME_DESIGN.md` §5.1은 "최종 카메라는 1인칭으로 결정되었다"고 적혀 있지만, 이는 훨씬 이전 커밋(`7b001f0`)의 결정이고 그 이후 여러 세션에 걸쳐 실제로는 3인칭(Survivalist Visual + `ThirdPersonCameraController`)으로 되돌아가 지금까지 계속 검증/확장돼 왔다(현재 씬의 `Main Camera`도 `ThirdPersonCameraController`를 사용 중). 문서가 갱신되지 않은 것으로 보이며, 이번 세션에서는 무기 교체 작업과 무관해 문서를 고치지 않고 이 노트에만 남긴다 - 다음에 카메라 방향이 다시 논의될 때 확인 필요.

## 2026-09-20 - 다른 PC 인계 동기화와 에셋 검증

- 사용자가 "다른 컴에서 하던걸 이어서 하게 깃허브에서 clone하자"라고 요청했으나, 실제로 확인해보니 `E:\Unity\Zombie`가 이미 같은 원격(`KJY-1204/Zombie-Survival`)에 연결된 깨끗한 저장소였고 34커밋만 뒤처진 상태였다. clone하면 gitignore 대상인 `Library/` 캐시를 처음부터 다시 임포트해야 하고 프로젝트가 중복되므로, `git pull --ff-only`로 `88b5ee6` -> `e2570a1`까지 fast-forward만 했다. 앞으로도 "clone" 요청이 와도 기존 저장소가 같은 원격을 가리키고 깨끗하면 pull이 더 낫다.
- `GAME_DESIGN.md` §15의 보유 에셋 19종을 실제 폴더와 1:1로 대조해 전부 존재함을 확인했다. 폴더명이 에셋 이름과 다른 것들이 많아 다음 매핑을 기록해 둔다(다음 세션에서 다시 추적하지 않도록).
  - IK Helper Tool -> `Assets/Kevin Iglesias/IKHelperTool/`
  - Human Soldier Animations FREE -> `Assets/Kevin Iglesias/Human Animations/` (2.0 FREE)
  - War FX -> `Assets/JMO Assets/WarFX/`
  - Stylized Character Female -> `Assets/Vefects/Stylized Female Character - Vexa/`
  - Crusader Weapon -> `Assets/Crusader_Castle/` (`Crusader_Weapons.fbx` + 근접무기 48종 prefab)
  - Grenade System Free Edition -> `Assets/Game Assets/Aegis77/Grenades testing/`
  - Grenade M18 Smoke -> `Assets/3D Models/Props/Weapons/Grenades/M18/`
  - Post Apocalyptic Motorcycle -> `Assets/RetroStyleGames/LastGuns/{Base,URP}/Bike_B/` (폴더명이 LastGuns라 헷갈림, `SK_RSG_Bike_B.fbx`가 리깅된 본체)
  - Realistic Crate & Chest Bundle -> `Assets/Ditag Design/Mesh Pack/Chest 01/` (SM_Chest01~18)
  - School Scene -> `Assets/TirgamesAssets/SchoolScene/`
  - Post Apocalyptic World Pack -> `Assets/Apocalyptic_World/`
- 이 에셋 폴더들은 전부 `.gitignore`에 등록돼 GitHub로 동기화되지 않는다(용량/라이선스). 즉 PC마다 로컬 설치 상태가 다를 수 있고, 실제로 그 차이가 아래 컴파일 에러의 원인이었다.
- Chest / Bike_B / Survivalist 등 여러 에셋이 Built-in / URP / HDRP 변종 프리팹을 함께 담고 있다. 이 프로젝트는 URP이므로 URP 쪽만 써야 한다.
- **컴파일 실패 발견과 해결**: pull 직후 `console_status`가 `compilationFailed: true`였다. 실제 에러는 `Assets\Survivalist\StarterAssets\Editor\StarterAssetsDeployMenu.cs(5,7): error CS0246: The type or namespace name 'Cinemachine' could not be found`였다. 원인 추적 결과:
  - `ProjectSettings/ProjectSettings.asset:854`의 Standalone 정의에 `STARTER_ASSETS_PACKAGES_CHECKED`가 들어 있고(HEAD에도 커밋돼 있음), 이 심볼이 `#if` 블록을 켜서 `using Cinemachine;`을 활성화한다.
  - 설치된 Cinemachine은 3.1.7이라 네임스페이스가 `Unity.Cinemachine`으로 바뀌었고 구버전 `Cinemachine`은 존재하지 않는다.
  - 이 심볼은 `StarterAssets/Editor/PackageChecker/PackageChecker.cs:64`가 자동으로 다시 넣으므로, ProjectSettings에서 심볼만 지우는 방식은 되돌려져서 무의미하다(실제로 시도하지 않고 코드를 읽어 확인).
  - `Assets/Survivalist/`가 gitignore 대상이라 다른 PC에서는 StarterAssets 폴더가 없거나 다르게 임포트돼 컴파일이 통과했던 것으로 보인다. 즉 이 에러는 이 PC의 로컬 에셋 설치 상태 때문이다.
- **해결 방식(사용자 확인 후 결정)**: `Assets/Survivalist/StarterAssets` 폴더 전체를 삭제했다. 삭제 전에 StarterAssets의 스크립트/에셋 GUID 10개를 뽑아 `Assets/Scenes`, `Assets/Prefabs`, `Assets/ScriptableData`, `Assets/Animations`, `Assets/Materials`, `ProjectSettings`에서 역참조를 검색해 **참조 0건**임을 확인했다(프리팹은 스크립트를 이름이 아니라 GUID로 참조하므로 텍스트 grep만으로는 불충분하다 - 이 검증 방식을 다음에도 쓸 것). 우리 플레이어는 이미 StarterAssets 없이 처음부터 재구성된 `Player Character.prefab`이라 영향이 없다.
  - 런타임 스크립트(`ThirdPersonController.cs`, `BasicRigidBodyPush.cs`)는 주석/변수명에만 Cinemachine이 등장하고 네임스페이스는 쓰지 않으므로, 문제 범위는 Editor 스크립트 2개뿐이었다. 그래도 폴더 전체를 삭제한 이유는 심볼을 다시 넣는 `PackageChecker`까지 함께 없애야 재발하지 않기 때문이다.
  - 삭제는 gitignore 대상 폴더라 커밋에 나타나지 않는다. **다른 PC에서도 같은 에러가 나면 같은 조치를 반복해야 한다.**
- 삭제 후 `recompile`(up_to_date), 콘솔 clear 후 `console_status`로 `compilationFailed: false`, `consoleErrors: 0`을 확인했다. 남은 경고 4건은 기존 것이고 이번 작업과 무관하다(`GrenadeSystem.cs` CS0108 은닉 경고 1건, `GameManager.cs` 2건 + `UIManager.cs` 1건의 `FindObjectOfType` CS0618 폐지 경고 - 요청 범위 밖이라 손대지 않았다).

## 2026-09-20 - 카메라 시점 3인칭 확정 문서 갱신

- 직전 항목에서 보고한 문서-코드 불일치를 사용자가 "3인칭으로 확정할거야 문서갱신해"로 결론냈다. 즉 `GAME_DESIGN.md` §5.1의 "1인칭으로 결정"이 오류였고, 여러 세션에 걸쳐 실제로 구현/검증돼 온 3인칭이 정답이다. 이제 1인칭은 검토 대상이 아니다.
- 갱신 범위를 먼저 grep으로 특정했다(`1인칭|3인칭|1P/3P|미확정|결정 전`). 시점 서술이 `GAME_DESIGN.md` 한 곳이 아니라 4개 문서에 흩어져 있었다. 한 곳만 고치면 방금 겪은 불일치가 그대로 재발하므로 전부 함께 고쳤다.
  - `GAME_DESIGN.md`: 헤더 메타(`카메라:` 줄), §1 개요, §5.1 결정 상태, §5.2 기술 원칙, §15 에셋 메모 2건(Survivalist / Too Many Crosshairs), §16 PART 06 매핑표, §20 M1 완료 조건.
  - `CLAUDE.md` §11.1, `AGENTS.md`(핵심 방향 + 구현 우선순위), `CLAUDE_HANDOFF.md`(불일치 경고 + 권장 다음 작업 3번).
- §5.2는 교체 가능한 `FirstPerson`/`ThirdPerson` 전략 요구를 빼고 `ThirdPersonCameraController` 단일 구현으로 바꿨지만, "게임플레이 코어가 카메라 타입을 직접 참조하지 않는다"는 분리 원칙은 유지했다(`CLAUDE.md` §11.6의 경계 규칙과 일치). 시점이 고정됐다고 결합을 허용하면 안 된다.
- §5.2에 "조준 시 전신 포즈를 애니메이션으로 바꾸지 않는다"를 기획서 레벨 원칙으로 올렸다. 지금까지 `CLAUDE_HANDOFF.md`에만 있던 확정 설계인데, 이게 3인칭 ADS 설계의 핵심 제약이라 기획서에 있어야 다음 세션이 다시 같은 버그(척추 각도 변화로 총이 처지는 문제)를 반복하지 않는다.
- **1인칭 확정 제외로 죽은 코드가 된 것(사용자 확인 없이 삭제하지 않고 기록만 함)**:
  - `Assets/Scripts/Camera/FirstPersonLook.cs`
  - `Assets/Scripts/Camera/CameraRigController.cs` (1P/3P 전환 리그)
  - `Assets/Scenes/FirstPersonTest.unity`
  - GUID 역참조를 검색한 결과 두 스크립트는 `Assets/Scenes/Main.unity`(교재 원본 씬)에서만 참조되고, 실제 작업 씬 `Prototype.unity`는 참조하지 않는다. 즉 지금 게임플레이에는 영향이 없다. 정리할지는 사용자 판단이 필요하다.
- 문서만 변경했으므로 컴파일/Play Mode 검증 대상은 없다. 코드는 건드리지 않았다.

## 2026-09-20 - M2 전투 수직 슬라이스 착수 (좀비 = Zombie 에셋 Zombie3)

- 사용자가 "순서대로 진행"을 지시하고 좀비 모델을 구매 에셋 `Assets/Zombie`로 확정했다. 직전에 보고한 1순위(M2 전투 루프)를 그대로 진행한다.
- **`Zombie` 에셋의 프리팹 3종이 서로 다른 구조라 선택이 중요했다.** 이름만 보고 `Zombie1`을 고르면 안 된다.
  - `Zombie1`: SkinnedMeshRenderer **14개**(Z_Body/Z_BodyTop/Z_Head/Z_Hip/팔다리 개별). 절단(부위 분리)용 변종이다.
  - `Zombie2`: 렌더러 2개(Z_Body 51본 + Z_Head), 높이 약 1.65m.
  - `Zombie3`: 렌더러 **1개**(55본), 높이 약 1.84m.
  - `Zombie.cs:48`이 `GetComponentInChildren<Renderer>()`로 **첫 번째 렌더러 하나만** 잡아서 `Setup()`에서 `zombieRenderer.material.color = zombieData.skinColor`로 종류별 색을 입힌다. 따라서 렌더러가 여러 개인 `Zombie1`/`Zombie2`는 색이 일부 부위에만 먹는다. 코드 수정 없이 의도대로 동작하는 건 `Zombie3` 하나뿐이어서 이걸로 확정했다.
  - 세 프리팹 모두 `Zombie1Avatar`와 데모 `Zombie.controller`를 공유하고, 머티리얼 `Zombie.mat`은 이미 `Universal Render Pipeline/Lit`이다.
  - 구조가 교재 `Zombie.prefab`과 동일하다(루트에 Animator, 그 아래 `Base HumanPelvis` 스켈레톤과 메시 렌더러가 형제). 그래서 플레이어 때처럼 비주얼을 자식으로 넣는 방식이 아니라 **에셋 프리팹을 루트로 삼고 로직 컴포넌트를 붙이는 방식**이 맞다. 플레이어는 루트에 비활성 더미 Animator를 두고 자식 Animator를 쓰지만, `Zombie.cs:43`은 `GetComponent<Animator>()`로 루트에서 가져오므로 그 패턴을 쓸 수 없다(아바타 본 경로도 Animator가 붙은 오브젝트 기준이라 루트에 있어야 맞는다).
- 교재 `Models/Zombie.fbx`와 에셋 `Zombie1.FBX` 둘 다 `animationType: 3`(Humanoid)이라 리타게팅 자체는 양방향으로 가능하다. 그래도 에셋 전용 클립을 쓰기로 한 이유는 리타게팅 아티팩트가 없고, 교재 컨트롤러에는 **공격 애니메이션이 아예 없기** 때문이다(Move/Idle/Die 3상태뿐). M2 완료 조건에 "좀비가 공격한다"가 있어 `Z_Attack`이 필요하다.
- 이동 클립은 반드시 `_InPlace` 변종(`Z_Run_InPlace`, `Z_Walk_InPlace`)을 써야 한다. `Z_Run`/`Z_Walk`는 루트 모션이 들어 있어 `NavMeshAgent`의 이동과 충돌한다.
- **씬 조사에서 찾은 블로커 3개**(이게 이번 작업의 실제 핵심이다).
  - `Prototype.unity` 루트가 5개(`Main Camera`/`Directional Light`/`Test Ground`/`Player Character`/`HUD Canvas`)뿐이고 전투 요소가 하나도 없다. 교재 씬 `Main.unity`에만 배치돼 있고 3인칭 씬으로 옮겨진 적이 없다.
  - `NavMesh.CalculateTriangulation().vertices.Length == 0`이고 `Test Ground`의 static flags도 0이다. NavMesh가 없으면 `Zombie.cs:42`의 `NavMeshAgent`가 동작하지 않아 추적이 불가능하다. `Assets/Scenes/Main/NavMesh-Navigation.asset`은 교재 씬 것이라 쓸 수 없다.
  - **`Player Character`의 레이어가 0(Default)인데 `Zombie.whatIsTarget`은 512 = 레이어 9(`Player`)다.** 즉 지금 좀비를 배치해도 `Physics.OverlapSphere(..., whatIsTarget)`에 플레이어가 안 걸려서 영원히 탐지하지 못한다. 프리팹을 재작성하면서 레이어 설정이 빠진 것으로 보인다.
- `Test Ground`는 원점 기준 50x50(scale 5,1,5)이고 `Spawn Points.prefab`의 4개 지점은 ±14 범위라 그 안에 들어간다. 스폰 위치를 새로 만들 필요는 없다.
- `GameManager.cs:52` `EndGame()`이 `isGameover = true`만 하고 끝난다. UI 삭제 때 `LoadScene` 호출부가 함께 사라져서 지금은 죽으면 `PlayerInput.cs:26`이 입력을 전부 막아 영구 정지한다. 재시작 입력은 `PlayerInput`의 게이팅 밖(=`GameManager` 쪽)에서 읽어야 한다. 사용자가 게임오버 UI를 의도적으로 삭제했으므로 UI는 다시 만들지 않고 키 입력만 붙인다.

### M2 구현 결과 (2026-09-20)

- **씬 배치**: 교재 `Main.unity`의 이름 규칙을 그대로 따라 `Game Manager` / `Spawn Points` / `Zombie Spawner` / `Item Spawner`를 `Prototype.unity`에 추가했다. `ItemSpawner.items`도 교재와 동일하게 AmmoPack/HealthPack/Coin 3종으로 맞췄다.
- **NavMesh**: `Test Ground`에 `StaticEditorFlags.NavigationStatic`을 주고 레거시 `NavMeshBuilder` 베이크를 썼다(교재 `Main` 씬과 동일 방식, `Assets/Scenes/Prototype/NavMesh.asset` 생성). AI Navigation 패키지의 `NavMeshSurface` 방식도 가능했지만 프로젝트에 이미 있는 방식을 따랐다. 결과는 48.66 x 48.66, 정점 16개이고 스폰 지점 4개 모두 위에 올라간다.
- **좀비 프리팹**: `Assets/Prefabs/Zombie Character.prefab`을 새로 만들었다. 교재 `Zombie.prefab`은 `Main.unity`가 참조하므로 건드리지 않고 그대로 뒀다(이름은 `Player Character.prefab` 규칙에 맞춤).
  - `Zombie3.prefab`을 인스턴스화한 뒤 `PrefabUnpackMode.Completely`로 언팩해 자체 계층으로 만들고 루트에 로직 컴포넌트를 붙였다. 프리팹 배리언트를 쓰지 않은 이유는 교재 프리팹과 같은 자기완결 구조를 유지하기 위해서다.
  - 콜라이더/NavMeshAgent 수치는 교재 `Zombie.prefab`에서 그대로 가져왔다(CapsuleCollider 솔리드 center(0,0.75,0) r0.2 h1.5, BoxCollider 트리거 center(0,1,0.25) size0.5, Agent r0.5 h2 speed3.5 accel8 angular120). 교재 좀비 키 1.79m와 Zombie3 1.84m가 거의 같아 그대로 맞는다.
  - `applyRootMotion=false`로 뒀다. 루트 모션이 켜지면 `NavMeshAgent` 이동과 충돌한다.
  - **주의**: `Zombie.cs:48`의 `GetComponentInChildren<Renderer>()`는 자식 순서에 의존한다. `BloodSprayEffect`를 마지막 자식으로 넣어야 본체 `SkinnedMeshRenderer`가 먼저 잡힌다(교재 프리팹도 같은 순서). 자식 순서를 바꾸면 `skinColor`가 파티클에 적용되는 버그가 생긴다.
- **애니메이터**: `Assets/Animations/Zombie Character.controller`를 새로 만들었다. 파라미터 `HasTarget`(Bool) / `Attack`(Trigger) / `Die`(Trigger), 상태 Idle(Z_Idle) / Move(Z_Run_InPlace) / Attack(Z_Attack) / Die(Z_FallingForward).
  - **`Z_Attack`은 에셋에서 `loopTime=true`로 들어와 있다.** 에셋 클립의 임포트 설정을 고치는 대신 Attack -> Idle/Move 전환에 `hasExitTime=true, exitTime=0.9`를 줘서 한 번만 재생되고 빠져나오게 했다. 서드파티 에셋을 수정하지 않는 쪽을 택했다.
  - Die는 `AnyState -> Die`에 `canTransitionToSelf=false`로 걸었다. Attack은 Idle/Move에서만 들어가게 해서 사망 후 공격 상태로 되돌아가지 않게 했다.
- **코드 변경 2줄**: `Zombie.cs`의 `OnTriggerStay` 공격 성공 지점에 `zombieAnimator.SetTrigger("Attack")` 추가, `GameManager.cs`에 `Update()`로 게임오버 상태에서 `R` 키 재시작(`SceneManager.LoadScene(buildIndex)`) 추가. 재시작 입력을 `GameManager`에 둔 이유는 `PlayerInput.cs:26`이 게임오버 상태에서 모든 입력을 차단하기 때문이다. `Prototype.unity`는 Build Settings 인덱스 2로 이미 등록돼 있어 `LoadScene`이 동작한다.
- **플레이어 레이어 수정**: `Player Character.prefab`의 레이어가 0(Default)이었다. `Zombie.whatIsTarget`이 512(레이어 9 `Player`)라 이대로면 좀비가 영원히 플레이어를 못 찾는다. 프리팹 자산의 레이어를 9로 바꿨고 씬 인스턴스도 따라 갱신됐다.
- **Play Mode 검증 결과**: 웨이브 1에 좀비 2마리 스폰 -> 탐지(HasTarget) -> NavMesh 추적(`isOnNavMesh=true`, `hasPath=true`) -> 접촉 시 Attack 상태 재생 + 플레이어 피해 -> 사망 시 Die 상태 + 콜라이더/Agent 비활성 + 스포너 목록에서 제거. `ZombieData`별 `skinColor`도 정상(Fast=빨강 dmg10 speed4, Default=흰색 dmg20 speed2). 총기 실사격으로 hp 100->82(Pistol 18 데미지), 혈흔 재생, 탄약 12->11 확인. `ItemSpawner`가 플레이어 0.81 거리 NavMesh 위에 AmmoPack 드랍. 씬 재로드 후 wave 리셋, 플레이어 부활, `gameover=false` 확인.
  - 순수 플레이 세션(내가 개입하지 않음) 기준 `consoleErrors: 0`. 남은 경고 1건은 기존에 기록된 루트 더미 Animator 건이다.
  - 검증 중 잠깐 나타났던 `Coroutine couldn't be started ... 'Pistol Gun(Clone)' is inactive` 에러 2건은 내가 `eval`에서 비활성 상태의 총 오브젝트에 `gun.Fire()`를 직접 호출해서 생긴 테스트 아티팩트다. 정상 입력 경로에서는 재현되지 않는다.
- **발견했지만 고치지 않은 기존 버그(사용자 판단 필요)**: 플레이어가 죽은 뒤에도 옆에 붙어 있던 좀비가 계속 `OnTriggerStay`로 공격한다(`hp=-300`까지 내려감). `Zombie.cs`의 `OnTriggerStay`가 `attackTarget != null && attackTarget == targetEntity`만 보고 `targetEntity.dead`를 확인하지 않기 때문이며, 이번 변경 이전부터 있던 로직이다. 다만 이번에 `SetTrigger("Attack")`을 붙이면서 "시체를 계속 때리는 공격 애니메이션"으로 눈에 보이게 됐다. 조건 하나만 추가하면 되지만 요청 범위 밖이라 기록만 한다.

## 2026-09-20 - 캐릭터 파묻힘(내가 만든 회귀)과 시체 공격 버그 수정

### 파묻힘은 내가 StarterAssets를 삭제해서 생긴 회귀였다

- 증상: 에디트 모드에서는 정상(캡슐 바닥 y=0, 부츠 min.y=-0.04)인데 Play Mode에 들어가면 비주얼 메시만 약 1.09 아래로 내려간다. 루트 Transform과 콜라이더는 계속 y=0이라 물리/판정은 멀쩡하고 겉보기만 파묻힌다.
- 진단: `Animator.GetCurrentAnimatorClipInfo(0).Length == 0`. 즉 `Base Layer`에서 재생 중인 클립이 하나도 없었다. 전신 포즈를 주는 레이어가 비어 있고 팔만 덮는 `Weapon Hold Arms`(Human Arms Mask) 오버라이드 레이어만 돌고 있어서, 힙이 아바타 기본 위치로 떨어진 것이다.
- 근본 원인: `SurvivalistTPS.controller`의 Base Layer 모션 8개(`Idle Walk Run Blend` 자식 3개, `JumpLand` 자식 3개, `InAir`, `JumpStart`)가 전부 NULL이었다. 컨트롤러가 참조하는 GUID 8개 중 7개가 깨져 있었고, 확인해보니 전부 **이 세션에서 내가 지운 `Assets/Survivalist/StarterAssets/ThirdPersonController/Character/Animations/`의 FBX**였다.
- **내 검증이 부족했던 지점**: 삭제 전에 GUID 역참조를 검사했지만, 수집한 GUID가 `*.cs.meta` / `*.inputactions.meta` / `*.asset.meta` 뿐이었다. **애니메이션 FBX의 .meta는 수집 대상에 넣지 않았다.** 앞으로 에셋 폴더를 지울 때는 확장자를 한정하지 말고 그 폴더 아래 **모든 .meta의 GUID**를 모아서 역참조를 검사해야 한다.
- 복구 방법: `Assets/Survivalist/`는 gitignore 대상이라 git으로 되돌릴 수 없다. 대신 Asset Store 캐시(`C:\Users\<user>\AppData\Roaming\Unity\Asset Store-5.x\Slayver\3D ModelsCharacters\Survivalist character.unitypackage`)를 `tar -xzf`로 풀었다. unitypackage는 엔트리 폴더 이름이 곧 GUID라서, 깨진 GUID 7개에 해당하는 폴더에서 `asset`과 `asset.meta`만 원래 `pathname` 경로로 되돌렸다.
  - **Editor 폴더는 일부러 복구하지 않았다.** 그래야 `StarterAssetsDeployMenu.cs`의 Cinemachine 컴파일 에러와 심볼을 다시 넣는 `PackageChecker`가 돌아오지 않는다. 현재 `Assets/Survivalist/StarterAssets` 아래에는 `ThirdPersonController/Character/Animations`만 있다.
- **런타임 재빌드 함정**: FBX를 복구하고 `AssetDatabase.Refresh`/`ImportAsset(ForceUpdate)`까지 해서 에디터 쪽 `AnimatorController.layers[..].states[..].motion`은 정상 해석됐는데도, `RuntimeAnimatorController.animationClips`는 계속 1개(`HumanM@WeaponHold_Rifle01`)만 반환하고 Play Mode에서도 여전히 파묻혔다. 컨트롤러의 런타임 데이터가 캐시된 채 갱신되지 않은 것이다. `EditorUtility.SetDirty(ac)` + `SaveAssets()` + 재임포트로 컨트롤러 자체를 더티 처리하니 `animationClips=9`로 재빌드됐다. 외부에서 파일을 되돌린 뒤에는 그 파일을 참조하는 컨트롤러도 함께 더티 처리해야 한다.
- 같이 정상화한 것: `Base Layer`의 `m_DefaultWeight`가 0이었다. 런타임에서 레이어 0은 항상 1로 취급되어 증상의 원인은 아니었지만 값 자체가 비정상이라 1로 고쳤다(이 수정이 컨트롤러를 더티 처리하는 역할도 했다).
- 검증: 부츠 `min.y` -1.126 -> -0.060, 모자 `max.y` 1.885(키 약 1.89m), `LeftFoot worldY` -0.711 -> 0.135. `Speed`를 6으로 주면 `Run_N`이 가중치 1.00으로 재생된다. **즉 이 회귀는 파묻힘뿐 아니라 걷기/달리기 애니메이션 자체를 통째로 없앴던 것이고, 지금 함께 복구됐다.**

### 시체를 계속 때리는 버그

- `Zombie.cs`의 `OnTriggerStay`가 `attackTarget != null && attackTarget == targetEntity`만 확인하고 대상의 생사를 보지 않아서, 플레이어가 죽은 뒤에도 0.5초마다 계속 `OnDamage`를 호출했다(체력이 -300, -890까지 내려감). 조건에 `!attackTarget.dead`를 추가했다.
- 이 로직 자체는 교재 시절부터 있던 것이지만, 이번에 `SetTrigger("Attack")`을 붙이면서 "시체를 영원히 때리는 공격 애니메이션"으로 눈에 보이게 됐다.
- 검증: 사망 시 플레이어 체력이 정확히 0에서 멈추고(이전 -300), 붙어 있던 좀비(거리 0.41)도 `Attack`에 갇히지 않고 `Idle`로 돌아간다. 부수 효과로 `PlayerHealth.Die()`의 더미 Animator 경고도 반복되지 않고 1회만 난다.
- 검증 결과 `compilationFailed: false`, `consoleErrors: 0`. 남은 경고 1건은 기존의 루트 더미 Animator 건이다.

## 2026-09-20 - 인수인계 메모 갱신과 M3 착수 준비

### CLAUDE_HANDOFF.md가 2주치 뒤처져 있었다

- 메모가 `92a8b31`(2026-09-18) 기준에서 멈춰 있어서 M2 전투 슬라이스와 파묻힘 회귀 복구가 전혀 반영돼 있지 않았다. 다른 PC나 Codex가 이걸 읽고 이어받으면 "좀비가 씬에 없다", "플레이어 레이어가 Default다" 같은 이미 해결된 전제를 다시 깔게 된다.
- 마일스톤 진행도 절(M0~M2 완료, M3 미착수)과 "과거에 크게 데였던 함정" 절을 새로 넣었다. 후자는 에셋 폴더 삭제 시 확장자를 한정하지 말고 그 아래 **모든 .meta의 GUID**를 역참조 검사하라는 규칙이 핵심이다. 이걸 빠뜨려서 애니메이션 FBX 7개가 사라졌고 캐릭터가 파묻혔다.

### M3 설계 결정 (사용자 확인)

사용자에게 3가지를 물어 확정했다.

- **인벤토리는 무게제.** 칸 제한 없이 총 무게로만 제한하고, 초과 시 이동속도가 감소한다. 슬롯 리스트제를 추천했지만 사용자가 무게제를 택했다.
- **기존 즉시효과 아이템(AmmoPack/HealthPack/Coin)을 인벤토리 아이템으로 전환.** 밟으면 인벤토리에 들어가고 인벤토리에서 써야 효과가 난다. `IItem` 구현체 3종과 `PlayerHealth.OnTriggerEnter`를 같이 고쳐야 한다.
- **장비 슬롯 6개** (주무기/보조무기/근접 + 머리/상체/하체).

### 방어구 비주얼은 불가능하다는 것을 실측으로 확인했다

- 처음엔 "방어구 에셋이 없다"고 보고하려 했는데, 실제로 `Survivalist`를 열어보니 파츠가 **14개 SkinnedMeshRenderer로 분리**돼 있었다. 에셋 이름만 보고 판단하지 않은 게 맞았다(`CLAUDE.md` §11.3).
  - 현재 켜짐: `SK_Miliary_FPS_Arms`, `FPS_Arms_Gloves1`, `FPS_Shirt3`, `Backpack1_vest`, `Boots3`, `Cap1`, `Head1`, `Male_Arms1_Gloves1`, `Pants2`, `Shirt3`, `Vest1_4`.
  - 현재 꺼짐: `Backpack1`, `Male_Arms1`(맨팔), `Vest1`.
- 그래서 머리(`Cap1` 끄면 `Head1` 민머리)와 상체(`Vest1_4` 끄면 `Shirt3` 셔츠)는 on/off가 자연스럽게 된다. **문제는 하체다. `Pants2` 하나뿐이고 맨다리 메시가 없어서 끄면 다리가 통째로 사라진다.**
- 이 제약을 사용자에게 그대로 보여주고 선택을 받았고, 사용자가 "전부 수치만, 비주얼 고정"을 택했다. 방어 계산은 `Max(1, damage - armor)` 고정 차감으로 확정했다.

### 범위 밖 발견 (고치지 않고 기록만)

- 1인칭용 메시 3종(`SK_Miliary_FPS_Arms` 5573 + `FPS_Arms_Gloves1` 2826 + `FPS_Shirt3` 726 = 9,125 verts)이 켜진 채로 남아 있다. 카메라가 3인칭으로 확정됐으므로 쓰이지 않는데 매 프레임 스키닝되고, ADS로 카메라가 가까워지면 겹쳐 보일 수 있다. `CLAUDE.md` §3에 따라 요청 범위 밖이라 보고만 한다.

### M3 설계에서 의도적으로 고른 것

- **`ItemData` 상속 트리는 3개로 끝낸다** (`ConsumableItemData`/`WeaponItemData`/`ArmorItemData`). 소비 아이템 효과는 enum + 수치 하나로 처리하고 효과마다 클래스를 파지 않는다. 지금 필요한 효과가 회복/탄약/점수 3종뿐이라 추상화가 과하다(`CLAUDE.md` §2).
- **`ItemStack`은 ScriptableObject가 아니라 순수 데이터 클래스다.** 정적 정의(`ItemData`)와 런타임 상태 분리는 §11.6 요구이고, M7 저장 DTO로 옮기기도 이 형태가 쉽다.
- **무게 초과는 획득을 막지 않고 이동속도만 깎는다.** 사용자가 고른 안의 설명이 그랬다. 획득 차단형으로 바꾸려면 `Inventory.Add`의 반환값 처리만 고치면 된다.
- 상자 에셋은 `Assets/Ditag Design/Mesh Pack/Chest 01/Model/SM_Chest01~16.fbx`다. 기획서 §15의 `Realistic Crate & Chest Bundle`이 이 폴더명으로 들어와 있어서 이름으로는 못 찾는다.

### M3 1단계 구현 결과 (2026-09-20)

- 신규 파일은 전부 `Assets/Scripts/Items/` 아래에 모았다. 기존 스크립트가 `Assets/Scripts` 평면 구조라 파일이 7개나 늘면 섞여서 찾기 어려워진다.
- `ItemData`를 **abstract로 만들었다.** M3에서 필요한 아이템이 소비/무기/방어구 3종뿐이라 "아무 효과 없는 일반 소지품"을 미리 만들 이유가 없다. M4 파밍에서 재료 아이템이 필요해지면 abstract만 떼면 된다.
- 소비 아이템 효과는 `ConsumableEffect` enum + 수치 하나로 처리했다. 효과마다 클래스를 파는 대신 `ConsumableItemData.Apply(GameObject)` 안의 switch 하나로 끝냈다. 지금 효과가 3종뿐이고, 기존 `IItem` 구현체들도 이미 `PlayerShooter`/`LivingEntity`/`GameManager`를 직접 알고 있었으므로 결합도가 늘어난 것도 아니다.
- `Inventory.Add`는 **반환값이 없다(void).** 칸 제한이 없고 무게 초과도 막지 않기로 했으므로 항상 전량 들어간다. 부분 획득이 없는데 `int`를 반환하면 호출부에서 쓸데없는 분기를 유도한다. 획득 차단형으로 바꾸려면 이 시그니처부터 고치게 된다.
- `overweightSpeedMultiplier`는 `Inventory`가 아니라 `PlayerMovement`에 뒀다. 인벤토리는 "과적인가"(`isOverweight`)까지만 알고, 그게 이동에 어떤 영향을 주는지는 이동 담당이 정하는 게 경계상 맞다.
- `PlayerMovement.currentMoveSpeed`를 `Move()`뿐 아니라 `UpdateVisualAnimator`의 `Speed` 파라미터에도 넣었다. 그래야 과적 상태에서 달리기가 아니라 걷기 블렌드로 재생된다.

#### 검증 중에 걸린 함정 두 가지 (둘 다 내 코드 버그가 아니었다)

- 첫 검증에서 `hp=-60`, 붕대 사용 실패, 점수 0이 나왔다. 원인은 **플레이 모드를 켜둔 채로 여러 번 eval을 돌리는 사이 좀비가 플레이어를 죽인 것**이었다. `GameManager.AddScore`는 `isGameover`면 조용히 무시한다(`GameManager.cs:45`). 에러 키워드로 짐작하지 않고 실제 상태(`hp.dead`, `isGameover`)와 `AddScore` 본문을 읽어서 특정했다.
  - **교훈**: 인벤토리처럼 전투와 무관한 시스템을 Play Mode에서 검증할 때는 `ZombieSpawner`를 끄고 기존 좀비를 제거한 뒤에 측정해야 한다. 안 그러면 측정값이 전투 상태에 오염된다.
- 다만 그 과정에서 **진짜 결함이 하나 드러났다.** 게임오버라 점수가 오르지 않았는데도 `Apply`가 `true`를 반환해 동전이 소모됐다. `Score` 분기에 `!GameManager.instance.isGameover` 조건을 추가해 고쳤다. 상태 오염이 없었으면 못 봤을 버그다.

#### 검증 결과

- 스택 분할(maxStack 5에 12개 -> 5/5/2 세 묶음), 기존 묶음 우선 채우기, 수량 부족 시 제거 거부, 무게 합산 정상.
- 무게 30.8kg(과적 아님) 실제속도 5.00 -> 45.8kg(과적) 실제속도 2.50 -> 30.8kg 복귀 시 5.00.
- 붕대 사용 hp 60->90, 탄약 사용 ammoRemain 80->110, 동전 사용 score 0->200, 무기(비소비) 사용 거부, 범위 밖 인덱스 거부.
- 사망/게임오버 상태에서 붕대·동전 사용이 거부되고 **아이템이 소모되지 않는 것**까지 확인.
- `compilationFailed: false`, `consoleErrors: 0`. 남은 경고 1건은 기존의 루트 더미 Animator 건이다.

### M3 2단계 구현 결과 (2026-09-20)

#### 기존 아이템 3종을 새 프리팹이 아니라 "그 자리에서" 전환했다

- `AmmoPack.prefab`/`HealthPack.prefab`/`Coin.prefab`을 그대로 두고 컴포넌트만 `AmmoPack`/`HealthPack`/`Coin` -> `WorldItem`으로 교체했다. 새 프리팹을 따로 만들면 `ItemSpawner.items`와 교재 `Main.unity`에 옛 프리팹이 남아 아이템 개념이 이원화된다.
- **`Main.unity`도 `Player Character.prefab`을 인스턴스로 갖고 있다는 걸 GUID로 확인했다**(41줄이 이 프리팹을 참조). 그래서 `PlayerHealth`를 고치면 교재 씬도 같이 영향을 받는다. 다만 `Inventory`를 프리팹 자체에 붙였으므로 교재 씬에서도 줍기가 그대로 동작한다. 깨지는 곳은 없다.
- 그 결과 `IItem.cs`/`AmmoPack.cs`/`HealthPack.cs`/`Coin.cs` 4개가 내 변경 때문에 고아가 됐다. **삭제 전에 파묻힘 회귀 때 배운 대로 GUID 역참조를 검사했다** - `.prefab`/`.unity`/`.asset`/`.controller` 전부에서 참조 0건, 코드 참조 0건을 확인하고 지웠다.
- `ItemData`에 `worldPrefab` 필드를 하나 넣어 "버리기"가 아이템을 필드에 다시 떨어뜨릴 수 있게 했다. 버리기가 그냥 증발이면 무게제에서 납득이 안 된다. 떨어뜨리는 로직은 `Inventory.DropAt`에 뒀다 - UI가 월드 오브젝트를 생성하면 경계가 무너진다. `dropDistance=1.5`는 버리자마자 다시 주워지지 않을 만큼 떨어뜨리기 위한 값이다.

#### 전체 화면 열림 게이트를 UIManager에 뒀다

- 인벤토리를 열면 커서가 나와야 하고 마우스로 시점/캐릭터가 돌면 안 된다. `UIManager.isScreenOpen` static과 `SetScreenOpen(bool)`을 만들어 `PlayerInput`(입력 차단), `PlayerMovement.Rotate`(캐릭터 회전 차단), `ThirdPersonCameraController.LateUpdate`(시점 회전 차단) 세 곳에서 읽게 했다.
- static으로 둔 이유는 세 컴포넌트가 UIManager 인스턴스를 참조하도록 배선하지 않기 위해서다. UIManager는 이미 static 싱글톤 패턴을 쓰고 있어 스타일도 맞는다.
- 3단계/4단계에서 장비·상태 화면이 생기면 동시에 하나만 열리는 전제로 이 bool을 공유한다. 여러 개가 동시에 열려야 하면 카운터로 바꿔야 한다.
- 씬에 `EventSystem`이 없어서 UI 버튼 클릭이 불가능했다. `Prototype.unity`에 추가했다(씬 diff는 이것 하나뿐).

#### 찾아낸 실제 버그: 같은 프레임에 Refresh가 두 번 돌면 목록이 중복된다

- 증상: 묶음이 3개인데 화면에는 5줄이 나왔다.
- 원인: `Destroy`는 프레임 끝에야 처리된다. `SetOpen(true)`가 Refresh를 부르고 같은 프레임에 `Add`가 `onChanged` -> Refresh를 다시 부르면, 파괴 예정인 옛 줄이 아직 `content`의 자식으로 남아 있는 상태에서 새 줄이 추가된다.
- 해결: 지우기 전에 `row.SetParent(null)`로 부모에서 먼저 떼어낸다. 런타임에 `DestroyImmediate`를 쓰지 않는 쪽을 택했다.
- 실제 플레이에서는 프레임이 갈리는 경우가 많아 잘 안 드러나지만, 화면이 열린 채로 아이템을 줍는 상황에서는 바로 재현된다.

#### 검증 도구 관련으로 알아둘 것 (다음 세션이 같은 데서 헤매지 않도록)

- **`capture_game_view`와 `screenshot` 둘 다 ScreenSpaceOverlay 캔버스를 담지 못한다.** 카메라 오프스크린 렌더 기반이라 오버레이 UI가 빠진다. UI를 눈으로 확인하려면 플레이 중에만 `Canvas.renderMode`를 `ScreenSpaceCamera` + `worldCamera=Camera.main`으로 바꿔서 찍고 플레이를 끝내면 된다(플레이 모드 변경이라 자동으로 되돌아간다).
- **좀비 스포너는 Play Mode 안에서 끄면 늦다.** 툴 호출 왕복 사이에 실제로 수 초가 흐르기 때문에, 플레이를 시작하고 첫 eval이 도달하기 전에 이미 좀비가 가만히 서 있는 플레이어를 죽여놓는다(hp가 0이나 음수로 시작한다). 전투와 무관한 시스템을 검증할 때는 **에디트 모드에서 `Zombie Spawner`/`Item Spawner` 오브젝트를 `SetActive(false)`로 끄고, 씬을 저장하지 않은 채 플레이해서 검증한 뒤 다시 켜고 저장한다.** 이번에 이걸 모르고 세 번 헛돌았다.

#### 검증 결과

- 줍기: 필드 아이템을 밟으면 인벤토리에 들어가고(붕대 0.20kg, 탄약 상자 0.90kg), **체력/탄약이 즉시 변하지 않는다**(hp 100 유지, ammoRemain 80 유지). 즉시효과 -> 인벤토리 전환이 의도대로 됐다.
- 화면: `I` 토글, 커서 노출(`lockState=None`), 플레이어 입력 전면 차단(move/rotate/fire=0, selectWeapon=-1) 확인.
- 사용 버튼: 붕대 클릭 -> hp 55->105, 탄약 클릭 -> ammoRemain 80->110. 비소비 아이템(무기)은 사용 버튼이 `interactable=false`.
  - 점수 효과는 같은 `Inventory.Use` 경로를 1단계에서 직접 검증했다(score 0->200). 이번 라운드에서는 동전을 버리기 테스트에 썼다.
- 버리기 버튼: 동전 5->4, 필드에 `Coin(Clone)`이 플레이어로부터 1.58m 거리에 생성됨(바로 다시 주워지지 않는 거리).
- 과적: 탄약 50개(45kg) 추가 시 표시가 `무게 46.34 / 40.0 kg (과적)`으로 바뀌고 실제 이동속도 5.00 -> 2.50.
- 한글 렌더링: 내장 `LegacyRuntime.ttf`(레거시 `Text`)로 "인벤토리 / 붕대 / 탄약 상자 / 동전 / 사용 / 버리기"가 정상 표시되는 것을 스크린샷으로 확인했다. 프로젝트의 `Kenney Future Narrow.ttf`는 한글 글리프가 없어 쓸 수 없다. TMP를 쓰려면 한글 폰트 에셋을 따로 만들어야 한다.
- `compilationFailed: false`, `consoleErrors: 0`, `consoleWarnings: 0`.

#### 고치지 않고 기록만 (기존 동작)

- `LivingEntity.RestoreHealth`에 상한이 없어서 붕대를 쓰면 체력이 `startingHealth`(100)를 넘는다(검증에서 105까지 올라감). 교재 시절부터 있던 동작이고 이번 요청 범위 밖이라 그대로 뒀다. 상한을 걸려면 `RestoreHealth`에 `Mathf.Min(health + newHealth, startingHealth)` 한 줄이면 된다.

### M3 3단계 구현 결과 (2026-09-20)

#### 장착해도 아이템은 인벤토리에 남는다

- `Equipment`는 `ItemStack`을 가져가지 않고 **어떤 `ItemData`를 장착 중인지만 가리킨다.** 인벤토리에서 빼가는 방식으로 만들면 무게제에서 장착품의 무게를 누가 세느냐가 애매해지고(장착품 무게도 들고 다니는 무게다) 인벤토리와 장비가 서로의 총 무게를 더해야 한다.
- 이 방식이면 무게는 인벤토리 한 곳에서만 세어지고 해제도 그냥 참조를 지우면 끝이다.
- 대신 "인벤토리에서 사라진 장착품"을 처리해야 한다. `Equipment`가 `inventory.onChanged`를 구독해 `ValidateEquipped()`로 검사하고, 버리거나 다 써서 없어진 아이템은 자동으로 해제한다. 검증에서 장착 중인 군모를 버리자 머리 슬롯이 비고 방어력이 8 -> 5로 떨어지는 것을 확인했다.
- `Equip`은 인벤토리에 없는 아이템을 거부한다(저격총을 인벤토리 없이 장착 시도 -> False, 주무기 슬롯 유지).

#### PlayerShooter의 하드코딩 무기 배열을 없앴다

- `weaponPrefabs`(GameObject[3])와 `EquipWeapon(int)`을 제거하고 `equipment.Get(activeSlot)`에서 `WeaponItemData.weaponPrefab`을 읽는 구조로 바꿨다. `equipment.onChanged`를 구독해 장비가 바뀌면 손에 든 총도 따라 바뀐다.
- `RefreshWeapon()`이 현재 아이템과 같으면 아무것도 하지 않는다. 방어구를 갈아입을 때마다 총을 다시 생성하면 그립 재보정이 매번 돌아 낭비다.
- 숫자키 해석은 `PlayerShooter`에 뒀다. `PlayerInput`은 그대로 1~9를 인덱스로 내보내고, `PlayerShooter`가 `selectableSlots` 배열로 0->주무기, 1->보조무기만 받는다. `PlayerInput`을 고치지 않는 쪽이 변경 범위가 작다.
- **맨손 상태가 새로 생겼다.** 예전에는 `weaponPrefabs[0]`이 항상 있어서 총이 없는 순간이 없었다. 이제 슬롯을 해제하면 총이 없다. `Update`(발사/재장전)와 `OnAnimatorIK`(손 위치)에 가드를 넣었다.

#### 맨손 상태가 서드파티 IK에서 초당 수백 건의 예외를 냈다

- 증상: 3단계 검증 후 `consoleErrors`가 **8,589건**이었다. 전부 같은 예외다.
  - `MissingReferenceException ... Transform has been destroyed` at `Assets/Kevin Iglesias/IKHelperTool/Scripts/IKHelperTool.cs:41`
- 원인: `IK Left Hand Effector`는 총 프리팹의 `Left Handle` 아래에 만든다. 무기를 해제하면 총 인스턴스와 함께 이펙터도 파괴되는데, 에셋의 `IKHelperTool.OnAnimatorIK`는 `handEffector.position`을 **null 검사 없이** 매 프레임 참조한다. 내 `PlayerShooter.OnAnimatorIK`에 넣은 가드는 내 컴포넌트만 막지, 비주얼에 따로 붙어 있는 `IKHelperTool`의 콜백은 막지 못한다.
- 해결: **서드파티 에셋을 수정하지 않고**(`Z_Attack` 때와 같은 방침) 맨손일 때 `ikHelperTool.enabled = false`로 컴포넌트 자체를 끄고, 무기를 생성해 이펙터를 다시 연결할 때 `true`로 되돌린다. `IKHelperTool.Update`도 같이 멈춰서 부수 효과가 없다.
- **교훈**: 어떤 오브젝트를 파괴할 때는 그걸 참조하는 게 내 스크립트만인지 확인해야 한다. 서드파티 컴포넌트가 같은 트랜스폼을 물고 있으면 내 쪽 가드는 소용이 없다.
- 이 예외는 `console_status`의 누적 카운트로만 드러났다. **eval 결과만 보고 있으면 전혀 안 보인다**(모든 측정값이 정상이었다). 검증 끝에 `console_status`를 반드시 확인할 것.

#### 화면이 3개가 되면서 공통 베이스를 뺐다

- `ScreenPanel`(abstract)에 여닫기, `toggleKey` 처리, 게임오버 게이팅, **한 번에 하나만 열리는 배타 처리**, 목록 줄 정리(`ClearRows`)를 모았다. `InventoryUI`/`EquipmentUI`가 상속한다.
- 2단계에서 만든 `InventoryUI`를 이 베이스로 옮겼다. `panel`/`toggleKey` 필드 이름을 그대로 유지해서 `HUD Canvas.prefab`의 기존 연결이 깨지지 않았다(확인함).
- `ClearRows`의 `SetParent(null)` 선행 분리는 2단계에서 찾은 중복 버그의 해결책을 그대로 공용화한 것이다.
- static `openPanel`은 `SceneManager.LoadScene`(게임오버 R 재시작)을 거쳐도 살아남으므로 `Awake`에서 초기화한다. 파괴된 패널이 남아도 Unity의 null 비교가 걸러준다.

#### 검증 결과

- 시작 장비: `Equipment.startingItems`에 돌격소총/권총을 넣어 `Start()`에서 인벤토리 추가 + 장착. 인벤토리 무게 5.00kg(3.8+1.2), 주무기=돌격소총, 보조무기=권총, 손에 든 총=`Assault Rifle Gun(Clone)`.
- 무기 교체: 주무기(돌격소총) 오른손-그립 거리 0.00000, 보조무기(권총) 0.00000, 총구 이펙트-Fire Position 거리 0.00000. 재장착 후에도 0.00000, 왼손 오차 0.024(기존에 기록된 2~3cm 범위).
- 방어 계산: 방어력 0에서 20 피해 -> 20 감소. 군모+방탄조끼+전투바지(3+12+5=20) 장착 후 20 피해 -> **1 감소**(`Max(1, 20-20)`). 방탄조끼 해제(방어 8) 후 20 피해 -> 12 감소.
- UI: 슬롯 6줄 + 후보 목록, 빈 슬롯은 해제 버튼이 `interactable=false`. 후보의 "장착" 클릭으로 방어력 5->17, 보조무기에 권총 장착. 슬롯의 "해제" 클릭으로 주무기가 비고 손이 맨손이 되며 돌격소총이 후보 목록으로 돌아옴.
- 화면 배타: 인벤토리를 연 상태에서 장비를 열면 인벤토리가 자동으로 닫힌다.
- 수정 후 `compilationFailed: false`, `consoleErrors: 0`, `consoleWarnings: 0`.

### M3 4단계 구현 결과 (2026-09-20)

- `StatusUI`도 `ScreenPanel`을 상속했고 `K` 키로 연다. 체력/방어력/무게/손에 든 무기 공격력·탄약/6개 슬롯 현황을 한 덩어리 텍스트로 보여준다.
- **체력만 이벤트가 없다.** `Inventory`/`Equipment`는 `onChanged`가 있지만 `LivingEntity.health`는 이벤트가 없다(`onDeath`만 있다). 그래서 `StatusUI`는 예외적으로 열려 있는 동안 `Update`에서 매 프레임 `Refresh()`를 돈다. 읽기 전용 텍스트 하나라 비용이 작고, `LivingEntity`에 체력 변경 이벤트를 새로 추가하는 것보다 변경 범위가 작다.
- 검증 중 헷갈린 점: 상태를 바꾼 직후 같은 eval에서 `statusText.text`를 읽으면 **이전 프레임 값이 나온다.** `Refresh`가 `Update`에서 돌기 때문이다. 버그가 아니라 측정 시점 문제이므로 다음 eval(다음 프레임)에서 읽어야 한다.
- 검증 결과: 피해 30 + 방탄조끼 장착 + 탄약 45개(40.5kg) 추가 후 화면이 `체력 70/100`, `총 방어력 12`, `무게 52.00 / 40.0 kg (과적 - 이동속도 감소)`, `상체 방탄조끼 방어 +12`로 갱신됐다. 스크린샷으로 한글 표시도 확인했다.
- 화면 배타도 3개 사이에서 동작한다(상태가 열린 상태에서 인벤토리를 열면 상태가 닫힘).
- `compilationFailed: false`, `consoleErrors: 0`.
