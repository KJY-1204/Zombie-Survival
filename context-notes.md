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

### M3 5단계 구현 결과 (2026-09-20)

#### 상자 에셋에서 걸린 두 가지

- 기획서 §15의 `Realistic Crate & Chest Bundle`은 `Assets/Ditag Design/Mesh Pack/Chest 01/`에 들어와 있다. **폴더 이름으로는 절대 못 찾는다.** URP/Built-in/HDRP 프리팹이 따로 있고 프로젝트가 URP이므로 `Prefab/URP/SM_Chest01.prefab`을 썼다.
- **에셋의 URP 프리팹이 FBX 내장 머티리얼(`Chest01`, 텍스처 없음)을 참조하고 있어서 상자가 새하얗게 나왔다.** 실제 머티리얼은 `Material/URP/M_Chest 01.mat`(`_BaseMap=T_Chest01_D`)인데 **이름에 공백이 있어서**(`M_Chest 01`) `FindAssets("Chest01")` 검색에 걸리지 않았다. 프리팹의 `MeshRenderer.sharedMaterial`을 이 머티리얼로 교체해 해결했다. 다른 상자(02~18)를 추가할 때도 같은 교체가 필요하다.
- 상자 메시는 뚜껑이 분리돼 있지 않은 단일 메시다. **여는 애니메이션은 불가능**하므로, 열림 피드백은 안내 문구가 "[E] 보급 상자 A 열기" -> "보급 상자 A (비어 있음)"로 바뀌는 것으로 대신했다.

#### 구조

- `LootContainer`(내용물 + `isEmpty` + `Loot(Inventory)`), `PlayerInteractor`(주변 탐색 + `E` 입력), `InteractionPromptUI`(안내 표시)로 나눴다. `PlayerInteractor`는 `currentTarget`만 공개하고 UI는 그것만 읽는다. 상호작용 컴포넌트가 직접 `Text`를 들고 있으면 §11.6의 "시스템은 UI를 모른다"가 깨진다.
- 탐색은 `Physics.OverlapSphere(transform.position, 2.5f)` + `GetComponentInParent<LootContainer>()`다. 레이어 마스크를 새로 만들지 않았다. 상자 수가 적고 레이어를 늘리면 기존 `whatIsTarget`(레이어 9) 설정과 얽힌다.
- 빈 상자도 `currentTarget`에 포함시킨다. 그래야 "비어 있음"을 안내할 수 있다. `Loot()`가 `isEmpty`로 막는다.
- 상자 프리팹은 **에셋 프리팹을 자식으로 둔 중첩 구조**다(`Loot Chest` 루트에 BoxCollider + LootContainer, 자식에 `SM_Chest01`). 좀비 때와 달리 언팩하지 않은 이유는 Animator/아바타 경로 문제가 없고 에셋 원본을 건드리지 않는 쪽이 낫기 때문이다.
- `BoxCollider`는 메시의 로컬 bounds(center 0,0.51,0.02 / size 1.27,1.02,0.58)를 그대로 썼다. 트리거가 아니라 솔리드라 플레이어가 통과하지 못한다.

#### NavMesh를 다시 베이크했다

- 상자는 솔리드 장애물이므로 NavMesh에서 파여야 한다. 안 그러면 좀비가 상자를 통과하거나 끼인다.
- 상자 3개(자식 포함)에 `NavigationStatic`을 주고 재베이크했다. **정점 16 -> 108, 삼각형 44**로 늘었고, 세 상자 위치 모두 `NavMesh.SamplePosition`이 실패(= 파였음)하는 것을 확인했다.
- `CLAUDE_HANDOFF.md`에 적어둔 "지형을 바꾸면 다시 베이크해야 한다"가 실제로 적용된 첫 사례다. 앞으로 씬에 정적 장애물을 놓을 때마다 이 절차를 반복할 것.

#### 검증 결과

- 배치: `보급 상자 A`(붕대3, 탄약 상자2), `무기 상자`(저격총1, 탄약 상자4), `보호구 상자`(군모1, 방탄조끼1, 전투바지1)를 `Loot Chests` 아래에 배치.
- 감지: 멀리 있을 때 `currentTarget=없음`, 1.40m 거리에서 `보급 상자 A`가 잡히고 안내가 `[E] 보급 상자 A 열기`로 뜬다.
- 열기: `Loot()` -> 묶음 2->4, 무게 5.00 -> 7.40(붕대 0.6 + 탄약 상자 1.8). 다시 열면 `False`, 내용 변화 없음. 안내가 `보급 상자 A (비어 있음)`으로 바뀐다.
- 상자 -> 장비 연결: 무기 상자에서 얻은 저격총을 장착하니 주무기가 저격총이 되고 **실제로 손에 `Sniper Gun(Clone)`이 들렸으며 오른손-그립 거리 0.00000**. 보호구 상자 3종 장착 시 총 방어력 20.
- 텍스처 교체 후 스크린샷으로 나무 상자 3개와 안내 문구가 정상 렌더링되는 것을 확인했다.
- `groundTruth.consoleErrors: 0`. 콘솔 버퍼에 잡힌 에러 1건은 게임 코드가 아니라 내가 머티리얼을 조회할 때 난 MCP 도구 타임아웃(`Failed to handle /api/exec request`)이다.

#### 남은 것

- **키 입력 자체는 자동 검증이 불가능하다.** MCP 파이프라인이 키보드 입력을 합성하지 못한다. `SetOpen`/`SelectSlot`/`Loot` 같은 코드 경로와 UI 버튼 클릭은 전부 직접 호출해 검증했지만, `I`/`O`/`K`/`E`/`1`/`2` 키 바인딩이 실제로 먹는지는 사용자가 플레이해서 확인해야 한다. `checklist.md`에 남겨뒀다.
- 새로 만든 UI 스크립트들이 기존 스타일을 따라 `FindObjectOfType`을 써서 CS0618 폐기 경고가 몇 건 늘었다. 기능 영향은 없고, 기존에 미뤄둔 `FindObjectOfType` 일괄 정리 작업에 같이 묶으면 된다.

## 2026-09-20 - M4 파밍과 거점 MVP 착수

### 사용자 결정과 내가 붙인 단서

- **채집은 무기 타격 방식으로 확정.** 나는 `E` 즉시 채집을 추천했다(M3에서 만든 `PlayerInteractor` 경로를 그대로 재사용할 수 있어서). 사용자가 타격 방식을 택했고, 선택지 설명에 내가 적어 보낸 단점(**총으로 벌목하게 되고 탄약을 소모한다**)을 그대로 수용한 것이다. 근접무기가 M8로 미뤄져 있어 지금은 총밖에 없다.
  - 되돌리기 비용은 작다. `ResourceNode`의 "파괴 시 드랍" 로직은 그대로 두고 진입점만 `OnDamage` -> `Interact`로 바꾸면 된다.
- **자유 배치로 확정.** 기획서 §11.3이 "초기 프로토타입에서 자유 배치와 비교합니다"라고 미정으로 남겨둔 항목이 이것으로 닫혔다. 그리드 스냅은 쓰지 않는다.
- **건설물 4종 확정** (벽/바리케이드/보관 상자/문). 기획서 §11.2의 "작업대 또는 기능성 오브젝트"는 제외됐다. 작업대는 제작 시스템이 붙어야 의미가 있는데 M4 완료 조건에 제작이 없다.

### 착수 전에 확인한 사실

- **`Gun.cs:114`가 `hit.collider.GetComponent<IDamageable>()`를 쓴다.** `GetComponentInParent`가 아니다. 자원 노드는 **로직과 콜라이더가 같은 GameObject에** 있어야 총알이 먹는다. M3의 상자(`GetComponentInParent`로 찾는 `PlayerInteractor`)와 규칙이 다르니 프리팹 구조를 헷갈리면 안 된다.
- `ItemData`를 M3에서 abstract로 뒀는데(소비/무기/방어구 3종만 필요했으므로) 순수 재료 아이템을 만들려면 해제해야 한다. `CLAUDE_HANDOFF.md`에 "abstract만 떼면 된다"고 적어둔 그대로다.
- 자원/건설물 에셋은 전부 `Apocalyptic_World`에 있다. 나무 4종, 바위 2종, 고철 후보(Dumpster/Can/Barrel), 벽 3종, 샌드백 4종 + 철조망 + 바리어, 게이트 도어 1종.

### M4에서 의도적으로 미룬 것 중 설명이 필요한 것

- **건설물은 NavMesh에 반영하지 않는다.** 런타임에 생기는 물체라 베이크로는 못 막는다. 정석은 `NavMeshObstacle`의 carving인데 AI Navigation 패키지 설정과 기존 레거시 `NavMeshBuilder` 베이크 방식이 얽힌다(M2에서 레거시 방식을 택했다). M4에서는 **물리 콜라이더로만 막는다** - 좀비는 경로를 그대로 계산하지만 실제로는 벽에 막혀 밀린다. 문 차단도 마찬가지다. 정식 처리는 M6에서 스트리밍과 함께 본다.
- 자원 노드의 채집 상태 저장은 M7이다. M4에서는 일정 시간 뒤 재생성으로 둔다.

### M4 1단계 구현 결과 (2026-09-20)

#### 콜라이더 위치가 이 단계의 핵심이었다

- `Gun.cs:114`가 `hit.collider.GetComponent<IDamageable>()`를 쓰므로(`GetComponentInParent`가 아니다) **자원 노드는 `ResourceNode`와 콜라이더가 같은 GameObject에 있어야** 총알이 먹는다.
- 에셋 프리팹(`SM_Veg_Tree_01` 등)은 자식에 자기 콜라이더를 갖고 있다. 그대로 두면 총알이 **자식 콜라이더에 먼저 맞고 거기엔 `IDamageable`이 없어서 아무 일도 안 일어난다.** 그래서 비주얼의 콜라이더를 전부 제거하고 루트에만 `BoxCollider`를 뒀다. 픽업 프리팹도 같은 이유로 자식 콜라이더를 제거했다.
- 이게 M3의 상자와 규칙이 다르다. 상자는 `PlayerInteractor`가 `GetComponentInParent`로 찾아서 자식 콜라이더여도 됐다. **새 프리팹을 만들 때 "총에 맞아야 하는가"를 먼저 물어야 한다.**

#### ResourceNode는 LivingEntity를 상속했다

- `LivingEntity`가 이 프로젝트의 유일한 피해 처리 기반이고 `Zombie`/`PlayerHealth`가 이미 쓴다. 자원 노드가 세 번째 사용자다. 내구도/사망 판정을 새로 짤 이유가 없다(`CLAUDE.md` §11.6의 "과도한 상속 트리를 만들지 않는다" 범위 안이라고 봤다).
- 채집되면 오브젝트를 파괴하지 않고 **콜라이더와 렌더러만 끈다.** 재생성 때 다시 켜면 되고 씬의 배치 정보가 유지된다.
- `ItemData`의 abstract를 해제했다. 목재/돌/고철은 효과가 없는 순수 재료라 서브클래스가 필요 없다. `[CreateAssetMenu(... "Material")]`을 붙였다.

#### 자원을 놓다가 좀비 스폰 지점을 막을 뻔했다

- 나무를 `(-14, 0, 2)`에 놓고 NavMesh를 재베이크했더니 **`Spawn Point 1`(-13, 0.5, 2)이 NavMesh에서 파여나갔다.** 그대로 뒀으면 좀비가 그 지점에 스폰되지 못한다.
- 나무를 `(-16, 0, 6)`으로 옮기고 다시 베이크해 스폰 지점 4개 전부 유효함을 확인했다. `CLAUDE.md` §11.7의 "시작 지점과 필수 경로가 막히지 않도록 연결성 검증을 우선한다"가 실제로 걸린 첫 사례다.
- **앞으로 씬에 정적 장애물을 추가하면 재베이크 후 스폰 지점/필수 경로를 반드시 샘플링해서 확인할 것.** 정점 수만 보면 놓친다.
- NavMesh 정점 변화: 상자만 있을 때 108 -> 자원 노드 7개 추가 후 392.

#### 검증 결과

- `hit.collider.GetComponent<IDamageable>()`가 `ResourceNode`를 반환하고 자식 콜라이더는 0개.
- 내구도: 나무 100에서 30씩 4번 -> 부서짐. 콜라이더/렌더러가 꺼지고 `목재 x5` 픽업이 0.81m 거리에 드랍.
- **실제 사격 검증**: 돌격소총(공격력 30)으로 바위를 1발 쏴서 내구 150 -> 120, 탄약 30 -> 29. 조준 레이가 `Rock Node`에 7.22m 거리에서 맞는 것도 확인했다.
- 줍기: 드랍된 목재를 밟아 인벤토리에 `목재 x5`, 무게 12.50kg(목재 1.5kg x 5 + 무기 5.0kg).
- 재생성: `respawnTime`을 3초로 준 나무가 3초 뒤 `dead=False`, 내구 100/100, 콜라이더·렌더러 복구. 60초짜리 노드도 검증 중 시간이 지나 실제로 되살아났다.
  - **함정**: `Die()`가 이미 코루틴을 시작한 뒤에 `respawnTime`을 바꿔도 소용없다. `WaitForSeconds`가 시작 시점 값을 캡처한다. 살아있을 때 먼저 바꿔야 한다.
- 텍스처: 나무/바위/고철 에셋 프리팹은 M3의 상자와 달리 URP 머티리얼을 제대로 물고 있어 교체가 필요 없었다. 스크린샷으로 확인.
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

### M4 2단계 구현 결과 (2026-09-20)

#### 구조

- `BuildableData`(정적 정의: 프리팹, 재료, 판정 상자) / `PlacedBuilding`(순수 데이터: id, 위치, 회전, 열림 여부) / `BaseBuildState`(설치 목록의 단일 소유자)로 나눴다. `PlacedBuilding`은 MonoBehaviour가 아니고 `instance` 필드만 `[NonSerialized]`라, M7에서 그대로 저장 DTO로 옮길 수 있다.
- 씬 오브젝트에서 자기 기록을 찾아야 하는 경우(문의 열림 상태 반영 등)를 위해 `PlacedBuildingLink`라는 얇은 연결고리 컴포넌트를 뒀다. 건설물 프리팹이 `PlacedBuilding`을 직접 들고 있으면 데이터와 씬 객체가 섞인다.
- 재료 차감은 `BuildableData.Pay(Inventory)`에 뒀다. `CanAfford`로 먼저 전부 확인한 뒤 차감하므로 **일부만 빠지는 일이 없다.** `Refund`도 미리 만들어뒀다(4단계 철거용).
- `BuildPlacer`는 UI를 모른다. `selected`/`isBuilding`/`canPlaceHere`와 `onStateChanged`만 공개하고 `BuildMenuUI`가 그것만 읽는다.

#### 좌클릭이 발사와 겹치는 문제

- 건설 모드의 좌클릭은 설치 입력인데 `PlayerShooter.Update`가 같은 `Fire1`로 총을 쏜다. `UIManager.isScreenOpen`은 화면이 열렸을 때만 참이라 건설 모드를 못 막는다(건설 중에는 이동도 조준도 해야 하므로 화면 게이트를 쓰면 안 된다).
- `PlayerShooter`가 `BuildPlacer.isBuilding`을 보고 발사/재장전을 건너뛰게 했다. 조건 하나 추가로 끝난다.

#### 버그 2개를 잡았다

**1. 조준선이 바닥에 닿지 않아 프리뷰가 뜨지 않았다.**
- 증상: 건설 모드에 들어가도 프리뷰가 비활성이고 `canPlaceHere=False`.
- 실제 측정: 카메라가 높이 2.22m에서 방향 (-0.03, -0.21, 0.98)로 거의 수평을 보고 있었다. 바닥(y=0)까지 가려면 2.22/0.21 = **10.5m**가 필요한데 `maxPlaceDistance`가 8m였다. 8m 안에서 충돌 0개.
- 해결: 조준선이 사정거리 안에서 바닥을 못 만나면, 사정거리 끝 지점에서 **아래로 다시 쏴서** 바닥을 찾는다(`TryFindGround`). 카메라 각도와 무관하게 항상 바닥에 놓인다.
- 같이 고친 것: 첫 레이캐스트가 플레이어 자기 몸에 맞을 수 있으므로 `Gun.cs`의 `TryGetClosestHit`처럼 자기 콜라이더를 건너뛰는 `TryRaycastIgnoringSelf`를 만들었다.

**2. 이미 지은 벽 위에 또 지어졌다 (겹침 판정이 안 먹었다).**
- 증상: 같은 자리에 두 번째 벽이 그대로 설치되고 재료도 또 빠졌다.
- 원인: `IsBlocked`에서 "딛고 선 바닥"이라고 생각한 `hit.collider`를 겹침 검사에서 제외했는데, **조준선이 바닥이 아니라 첫 번째 벽에 맞으면 그 벽이 예외 대상이 되어버린다.** 프리뷰 위치가 y=0.61(벽 옆면)이었던 게 단서였다.
- 해결: 바닥 예외를 **없앴다.** 판정 상자는 `checkCenter`로 접촉 지점보다 0.1 위에서 시작하므로 딛고 선 바닥에는 애초에 닿지 않는다. 예외가 필요 없었다.
- 벽 메시의 피벗이 x로 0.19 치우쳐 있어서 판정 상자 중심도 콜라이더 중심에 맞췄다. 메시 bounds를 그대로 믿고 (0, h/2, 0)에 두면 실제 벽과 어긋난다.

#### 검증 결과

- 재료 부족: `CanAfford=False`, `Pay=False`이고 **재료가 차감되지 않는다.** 건설 메뉴의 "건설" 버튼도 `interactable=false`, 이름이 붉게 표시된다.
- 재료 충족: 버튼이 활성화되고, 클릭하면 건설 모드 진입 + 메뉴가 닫힌다(`isScreenOpen=False`라 조준과 좌클릭이 먹는다).
- 프리뷰: 콜라이더 0개, 스크립트 0개, 반투명 머티리얼(알파 0.45). 설치 가능하면 초록 `(0.35, 1.00, 0.45)`, 불가능하면 빨강 `(1.00, 0.32, 0.28)`.
- 설치: 목재 30->20, 돌 15->10으로 **정확히 10/5만 차감**. `BaseBuildState`에 `id=wall pos=(0.61,0,2.70) rotY=0`이 기록된다.
- 겹침 거부: 같은 자리 재시도 시 프리뷰가 빨강, `TryPlace=False`, 건설물 수와 재료 모두 변화 없음.
- 회전: `previewRotationY=90`으로 설치하면 기록에 `rotY=90`이 남는다.
- 재료가 남아 있으면 건설 모드를 유지해 연속 설치가 가능하다.
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

### M4 3단계 구현 결과 (2026-09-20)

#### 상호작용을 인터페이스로 일반화했다

- M3에서 `PlayerInteractor`는 `LootContainer` 하나만 찾았다. 이제 대상이 루팅 상자 / 보관 상자 / 문 3종이라 `IInteractable`(`CanInteract` / `GetInteractionLabel` / `Interact`)을 만들고 셋 다 구현하게 했다.
- 상호작용 키(`E`)는 인터페이스에 넣지 않았다. 키는 `PlayerInteractor`의 설정이고, `InteractionPromptUI`가 `CanInteract`가 참일 때만 `[E]`를 앞에 붙인다. 빈 상자처럼 지금 다룰 수 없는 대상은 키 없이 상태만 보여준다.
- `GetComponentInParent<IInteractable>()`로 찾으므로 콜라이더가 자식에 있어도 된다. **자원 노드(`IDamageable`)와 규칙이 반대라는 점을 계속 기억할 것.**

#### 보관 상자는 Inventory를 재사용한다

- `StorageContainer`가 자기 내용물을 새로 관리하지 않고 **같은 오브젝트의 `Inventory` 컴포넌트**를 쓴다(`[RequireComponent]`). `Add`/`Remove`/`CountOf`/`onChanged`가 전부 그대로 필요했다. `maxWeight=500`으로 사실상 무게 제한을 걸지 않는다.
- 보관 상자는 UI를 모른다. `Interact`가 `static event onOpenRequested`를 쏘고 `StorageUI`가 구독해서 연다. 상자가 UI를 직접 열면 §11.6이 깨진다.
- `StorageUI`는 다른 화면과 달리 **토글 키로 열리지 않는다.** 상자에서만 열리고 `T` 키로는 닫기만 한다. `ScreenPanel.Update`를 오버라이드해 base를 호출하지 않는 방식으로 처리했다.
- 아이템 이동은 한 번에 한 묶음씩 통째로 옮긴다. 개수 선택 UI는 M4 완료 조건에 없다.

#### 문은 경첩을 돌린다

- `Door Building` 프리팹은 루트 > `Hinge` > 문짝 메시 구조다. **콜라이더를 `Hinge`에 붙여서** 문이 열리면 콜라이더도 같이 돌아간다. 열릴 때 콜라이더를 끄는 방식이 아니라 실제로 비켜나는 방식이다.
- 문짝 메시의 bounds가 `min=(-1.42, -1.02, -0.08)`로 원점 기준이 아니라, 경첩이 문짝 왼쪽 모서리·바닥에 오도록 문짝을 `(1.42, 1.02, 0.03)`만큼 밀어 붙였다.
- 열림 상태는 `PlacedBuildingLink.record.isOpen`에 반영된다. `Start`에서 기록된 값을 읽어 복원하므로 M7 불러오기가 바로 붙는다.
- **문틀이 없다.** 문짝만 있어서 옆으로 돌아갈 수 있다. 벽과 조합해서 쓰는 전제이고, 문틀 에셋을 찾거나 벽 사이에 끼우는 스냅은 M4 범위 밖이다.

#### 잡은 버그 2개 (둘 다 프리뷰 생성에서 나왔다)

**1. `Can't remove Inventory (Script) because StorageContainer (Script) depends on it`**
- `CreatePreview`가 프리뷰의 MonoBehaviour를 전부 `Destroy`하는데, `StorageContainer`가 `[RequireComponent(typeof(Inventory))]`라 의존하는 쪽이 남아 있는 동안 `Inventory` 제거가 거부된다. 콘솔에 에러만 남고 컴포넌트는 그대로 살아 있었다.
- 해결: 제거하지 않고 `enabled = false`로 끈다. 프리뷰는 동작만 멈추면 되지 컴포넌트를 지울 이유가 없었다.
- **이 에러는 eval 결과로는 전혀 안 보였다.** 프리뷰도 정상으로 보이고 설치도 됐다. `console_status`의 `groundTruth.consoleErrors`가 1로 바뀐 것만이 단서였다. M3의 IKHelperTool 건과 같은 패턴이다.

**2. 프리뷰가 자기 콜라이더에 막힐 수 있었다**
- 콜라이더도 `Destroy`로 지웠는데 `Destroy`는 프레임 끝에야 처리된다. 그 프레임 동안 프리뷰의 콜라이더가 살아 있어 `IsBlocked`의 `OverlapBox`에 잡히거나 조준 레이를 가로챌 수 있다. 측정해보니 실제로 `켜진콜라이더=1`이었다.
- 해결: 콜라이더도 `enabled = false`로 끈다. 즉시 반영되고 `OverlapBox`/`Raycast` 모두 꺼진 콜라이더를 무시한다.
- 이번 세션에서 `Destroy`의 지연 때문에 문제가 생긴 게 **세 번째**다(인벤토리 줄 중복, 프리뷰 콜라이더, 그리고 UI 공통 `ClearRows`). 런타임에 무언가를 즉시 없앤 것처럼 취급하면 안 된다.

#### 검증 결과

- 건설물 4종을 서로 떨어진 자리에 설치: 벽/바리케이드/문/보관 상자 전부 성공, 재료가 정확히 차감(목재 100->61, 돌 50->45, 고철 50->40).
- 문: `Interact`로 경첩이 0도 <-> 90도로 돌고 `record.isOpen`이 따라 바뀐다. 안내 문구도 "문 열기" <-> "문 닫기".
- 문 차단: 닫힌 문을 가로지르는 레이가 `Hinge` 콜라이더에 막히고(True), 열면 같은 경로가 통과한다(맞은 것 없음).
- 보관 상자: `E` 탐지 -> 화면 열림(제목 "보관 상자"), 돌 45개 넣기(내 무게 249->136.5kg), 꺼내기로 원복. 넣고 꺼낼 때 양쪽 목록이 같이 갱신된다.
- 화면 배타: 인벤토리가 열린 상태에서 상자를 열면 인벤토리가 닫힌다.
- 프리뷰 4종 전부 켜진 콜라이더 0, 켜진 스크립트 0. 설치된 실물은 전부 활성.
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

#### 검증 중 착각한 것 (기록용)

- 문 옆에서 상호작용 대상이 `LootContainer`로 잡혀 버그인 줄 알았는데, 문(6,0,3)을 M3에서 놓은 보급 상자(6,0,4) 바로 옆에 지어서 **상자가 실제로 더 가까웠다**(1.22m vs 1.70m). 탐지 로직은 정상이었다.
- 문 차단 테스트에서 레이가 `Player Character`에 맞았다. 플레이어가 레이 경로에 서 있었던 것이고, 비켜서니 정상적으로 `Hinge`에 맞았다.

### M4 4단계 구현 결과 (2026-09-20)

#### 철거

- `BuildPlacer`에 `X` 키 철거를 넣었다. **건설 모드가 아닐 때만** 받는다(건설 중 좌클릭은 설치가 우선이고, 철거까지 섞이면 오조작이 난다).
- 조준선 끝의 콜라이더에서 `GetComponentInParent<PlacedBuildingLink>()`로 기록을 찾는다. 문처럼 콜라이더가 자식(`Hinge`)에 있는 경우가 있어 부모까지 올라가야 한다.
- 철거하면 `BuildableData.Refund`로 재료를 전액 돌려준다(`refundOnDemolish` 기본 true). 실수로 잘못 지었을 때 손해가 없어야 자유 배치가 쓸 만하다.

#### 또 카메라 기준 거리 문제가 나왔다

- 증상: 벽 바로 앞(3m)에 서 있는데 `demolishTarget`이 계속 `없음`이었다.
- 실제 측정: 카메라가 `(-19.28, 2.28, -20.41)`, 플레이어가 `(-20, 0, -17)`로 카메라가 **3.4m 뒤**에 있다. 벽까지 카메라 기준 5.97m인데 `demolishDistance`가 4m라 레이가 닿지 못했다. 즉 실제 도달 범위는 플레이어 앞 0.6m뿐이었다.
- 해결: 레이 길이를 `카메라-플레이어 거리 + demolishDistance`로 늘리고, **사정거리는 플레이어와 맞은 지점 사이로 판정**한다. "플레이어로부터 4m 이내"라는 의도대로 동작한다.
- **2단계의 프리뷰 문제와 같은 원인이다.** 3인칭에서 카메라 기준 거리를 쓰면 항상 카메라-플레이어 간격만큼 손해를 본다. 앞으로 사정거리를 다루는 코드는 기준점이 카메라인지 플레이어인지 먼저 정할 것.
- 설치 쪽(`maxPlaceDistance`)도 같은 성질이지만, 2단계에서 넣은 "사정거리 끝에서 아래로 재투영" 보정 덕에 실제로는 잘 동작하고 검증도 끝나서 건드리지 않았다.

#### 저장 데이터 표현

- `BaseBuildState.ToJson()`이 `PlacedBuilding` 목록을 그대로 직렬화한다. `instance`가 `[NonSerialized]`라 자동으로 빠지므로, **나온 JSON이 곧 M7 저장 DTO의 형태**다.
- 실제 출력에서 4종이 `buildableId` / `position` / `rotationY` / `isOpen`만 남고 씬 참조가 전혀 섞이지 않는 것을 확인했다.
- 문을 열어두면 `"isOpen": true`가 그대로 찍힌다. `BuildableDoor.Start`가 기록된 값을 읽어 복원하므로 M7 불러오기는 `BaseBuildState.Place` 후 기록만 채워주면 된다.
- **M7에서 추가로 필요한 것**: 보관 상자의 내용물은 아직 이 DTO에 없다. `StorageContainer`가 들고 있는 `Inventory`를 `PlacedBuilding`에 붙이거나 별도 목록으로 저장해야 한다. M4 완료 조건이 "건설물이 저장 대상 데이터로 표현된다"까지라 여기서 멈췄다.

#### 검증 결과

- 4종 설치 후 `ToJson()`이 4개 항목을 정확히 출력. 문 열기 -> `isOpen: true` 반영.
- 철거: 안내 `[X] 벽 철거` 표시 -> 철거 성공 -> 기록 4->3, **목재 +10 / 돌 +5 전액 환급**, 씬 오브젝트 수와 기록 수가 3으로 일치, 철거 후 대상이 즉시 `없음`으로 초기화.
- 검증 중 만든 건설물이 에디트 모드 씬에 남지 않았음을 확인(0개).
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

## 2026-09-20 - M5 오토바이 착수

### 에셋이 두 개였고 성격이 정반대였다

- 이름만 보고 골랐으면 틀렸을 상황이다(`CLAUDE.md` §11.3).
  - `RetroStyleGames/.../P_RSG_Bike_B_Dirty_URP.prefab` - **이미 완성된 주행 리그.** Rigidbody + `Gadd420.BicycleVehicle`(모터/브레이크/조향/기울기/서스펜션) + WheelCollider 2개 + 콜라이더 17개 + CenterOfMass. URP 머티리얼 연결됨. 기획서 §15의 `Post Apocalyptic Motorcycle`이 이것이다.
  - `MotoInteractionAnimsFREE/Prefabs/SKM_Bike.prefab` - **Transform만 있는 껍데기.** 콜라이더/Rigidbody/WheelCollider 0개. 이 에셋의 값어치는 바이크가 아니라 탑승/주행 애니메이션 세트다.
- 사용자가 RSG 바이크를 택했다. 대신 `MotoInteractionAnimsFREE`의 애니메이션은 SKM_Bike 비율 기준이라 손/발이 정확히 맞지 않는다. 그래서 "앉은 포즈 하나만" 쓰기로 한 것이다.

### 서드파티를 수정하지 않고 주행을 제어할 수 있다

- `BikeController.cs:112~126`을 읽어보니 `Input_Manager`가 있으면 거기서, **없거나 비활성이면 `Input_Compat`으로 폴백**한다. 그리고 `Input_Compat.GetHorizontal/GetVertical`은 레거시 `Input.GetAxisRaw("Horizontal"/"Vertical")`을 그대로 읽는다(우리 `PlayerInput`과 같은 축).
- 따라서 **`BicycleVehicle` 컴포넌트의 `enabled`만 토글하면 된다.** 탑승 중에는 `PlayerMovement`가 게이팅되므로 같은 축을 공유해도 충돌하지 않는다. 에셋 코드를 한 줄도 건드리지 않는다(`Z_Attack`, `IKHelperTool` 때와 같은 방침).

### 라이더 포즈

- `AS_Idle_Riding.fbx`가 `animationType=Human`, 1.33초 **루프** 클립이다. Survivalist도 Humanoid라 리타게팅된다.
- **`Weapon Hold Arms` 레이어(weight 1, Human Arms Mask, Override)를 반드시 0으로 내려야 한다.** 안 그러면 라이더가 핸들이 아니라 소총을 쥔 팔 포즈로 앉아 있게 된다. M3에서 맨손 상태를 만들 때 겪은 것과 같은 계열의 문제다.

### 연료·내구도는 사용자가 범위 확대를 알고 선택했다

- 선택지 설명에 **"M5 완료 조건을 크게 넘어서고 수리 재료·UI까지 필요하다"**고 적어 보냈고 그대로 택했다. M5 완료 조건(`GAME_DESIGN.md` §20)에는 탑승/하차/주행, 카메라 전환, 저장 복원만 있다.
- 기획서 §21의 "오토바이 연료/내구도 도입 여부" 미정 항목이 이것으로 닫혔다.

### M5 1단계 구현 결과 (2026-09-20)

#### 구조

- `Motorcycle`(차량, `IInteractable`)과 `RiderControl`(플레이어)로 나눴다. 차량은 "누가 타고 있는가"만 알고, 플레이어의 조작·물리·애니메이션·카메라를 실제로 전환하는 책임은 `RiderControl`에 있다. 차량이 플레이어 내부 컴포넌트를 직접 켜고 끄면 경계가 무너진다.
- **서드파티 `BicycleVehicle`은 한 줄도 수정하지 않았다.** `Motorcycle.Awake`가 타입 이름으로 찾아서 `enabled`만 토글한다. 프리팹의 `Input_Manager`는 꺼두고 `Input_Compat` 폴백(레거시 `Horizontal`/`Vertical` 축)을 쓰게 했다.
- 탑승 중 게이팅은 컴포넌트 비활성으로 했다(`PlayerMovement`/`PlayerShooter`/`BuildPlacer`). `PlayerInput`에 조건을 더 넣지 않은 이유는 하차 후 원상복구가 단순하기 때문이다. `PlayerShooter`는 `OnDisable`에서 총을 숨기므로 라이더가 총을 든 채 타는 것도 자동으로 막힌다.
- `PlayerInteractor`는 끄지 않는다. 끄면 `E`로 내릴 수 없다. 대신 **타고 있는 동안에는 `currentTarget`을 차량으로 고정**해 주변 다른 상호작용이 끼어들지 않게 했다.
- 물리: 탑승 시 `Rigidbody.isKinematic = true` + 콜라이더 비활성. 안 그러면 플레이어 캡슐과 오토바이가 서로 밀어낸다.
- 카메라: `ThirdPersonCameraController.target`만 오토바이로 바꾼다. 카메라 코어는 손대지 않았다(`CLAUDE.md` §11.6).
- 애니메이터: `SurvivalistTPS.controller`에 `Mounted`(Bool) 파라미터와 `AS_Idle_Riding` 상태를 추가하고 `AnyState -> Mounted` / `Mounted -> Idle Walk Run Blend` 전환을 걸었다. **탑승 중 `Weapon Hold Arms` 레이어 가중치를 0으로 내린다** - 안 내리면 라이더가 소총 쥔 팔 포즈로 앉는다.

#### 측정 지표를 잘못 골라 한참 헤맸다 (기록용)

- 탑승 직후 "머리-엉덩이 1.58m, 렌더러 높이 2.80m"가 나와서 캐릭터가 늘어난 줄 알고 스케일·물리·클립을 차례로 의심했다. **전부 헛다리였다.**
  - 렌더러 높이 2.80m는 **총기와 총구 이펙트 렌더러**가 bounds에 섞여 나온 값이다. 실제 캐릭터는 부츠 `-0.06` ~ 모자 `1.88`로 1.89m, M3에서 검증한 값 그대로였다.
  - "머리-엉덩이"가 1.65m로 나온 건 **이 아바타가 `Hips`를 루트 근처(y≈0)에 매핑**하고 있어서다. 서 있을 때도 같은 값이 나온다. 애초에 의미 없는 지표였다.
  - 결정적 단서는 `Mounted=false`로 되돌려도 같은 값이 나온 것이었다. 탑승과 무관하다는 뜻이므로 그 시점에 지표를 의심했어야 했다.
- **교훈**: 캐릭터 치수는 `GetBoneTransform`이 아니라 **특정 메시 렌더러의 bounds**(부츠/모자)로 재야 한다. 이 프로젝트에는 Survivalist 본(`Hips`/`Left_Foot`)과 마네킹 본(`pelvis`/`foot_l`) 두 벌이 한 프리팹에 섞여 있어서 이름으로 찾는 것도 위험하다.
- 그리고 **숫자로 애매하면 그냥 측면에서 스크린샷을 찍는 게 빠르다.** 실제로 그렇게 해서 한 번에 원인을 봤다.

#### 좌석 위치는 측면 뷰로 맞췄다

- 처음 `Seat`를 렌더러 bounds 비율로 `(0, 0.77, -0.10)`에 뒀더니 라이더가 시트보다 높고 뒤에 떠 있었다.
- 오토바이 콜라이더에 `Seat` / `Fuel_Tank` / `Handlebar_Main`이라는 이름이 있어서 위에서 아래로 레이캐스트해 **실제 시트면이 월드 y≈1.28, 로컬 z -0.45~-0.15** 구간임을 찾았다.
- 최종값 `Seat local = (0, 0.45, -0.30)`, `Exit Point local = (-1.10, 0, -0.30)`. 이 값에서 엉덩이가 시트에 닿고 손이 핸들바에 온다.
- **다리가 약간 길게 내려온다.** Survivalist와 원본 마네킹의 다리 비율 차이에서 오는 리타게팅 아티팩트다. 사용자가 "앉은 포즈 하나만"을 택하면서 손/발 어긋남을 감수한 부분이고, 손·발 IK는 폴리싱 단계로 미뤄뒀다.

#### 검증 결과

- 탑승: 부모가 `Seat`, 로컬 위치 (0,0,0), 카메라 타깃 `Motorcycle`, `PlayerMovement`/`PlayerShooter`/`BuildPlacer` 전부 `False`, `isKinematic=True`, 콜라이더 `False`, `Mounted=True`, 팔 레이어 가중치 `0`, `BicycleVehicle.enabled=True`.
- 재생 클립이 `AS_Idle_Riding`인 것과 안내가 `[E] 오토바이에서 내리기`로 바뀌는 것 확인.
- 하차: 부모 해제, 위치 `(2.45, 0.00, -5.46)`으로 지면 위, 콜라이더·물리·조작 컴포넌트 전부 복구, 카메라 타깃 `Player Character`.
- 미탑승 상태에서 `BicycleVehicle.enabled=False`라 입력이 와도 오토바이가 움직이지 않는다.
- 오토바이가 물리로 자리를 잡은 뒤에도 기울기 0.1도로 넘어지지 않는다.
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

### M5 2단계 구현 결과 (2026-09-20)

#### 코드 변경이 없는 단계였다

- 주행은 1단계에서 이미 연결됐다. `Motorcycle`이 탑승/하차에 맞춰 `BicycleVehicle.enabled`만 토글하고, 에셋 컨트롤러가 `Input_Compat`(레거시 `Horizontal`/`Vertical` 축)으로 알아서 굴러간다. 2단계는 **그게 실제로 도는지 확인하는 단계**였고 새 코드가 필요 없었다.
- 오토바이는 런타임에 움직이는 물체라 `NavigationStatic`을 주지 않았다(`staticFlags=0` 확인). NavMesh를 다시 구울 필요도 없다. 건설물과 같은 이유로 좀비는 물리 콜라이더로만 막힌다.

#### 키 입력을 합성할 수 없어 검증 방식을 바꿔야 했다

- 처음엔 에셋의 `Input_Manager`에 값을 주입하려 했는데 **불가능했다.** `hzInput`/`vInput`이 `protected` 필드인데다 `Input_Manager.Update()`가 매 프레임 실제 입력으로 덮어쓴다. `BicycleVehicle`이 매니저를 쓰려면 `enabled`여야 하고, `enabled`면 `Update`가 돈다.
- 그래서 세 갈래로 나눠 검증했다.
  1. **게이팅** - 미탑승 `enabled=False`, 탑승 `True`, 하차 `False`. 입력이 와도 미탑승이면 움직이지 않는다.
  2. **주행 리그** - 컨트롤러를 잠시 끄고 뒷바퀴에 `motorTorque = 500`(컨트롤러의 `motorForce`와 같은 값)을 직접 걸었더니 오토바이가 `(4.04, 0, -4.00)`에서 `(-11.12, 0.16, -4.00)`까지 **15m를 주행**했고 라이더도 `Seat`에 붙은 채 함께 이동했다.
  3. **컨트롤러가 실제로 제어 중인지** - 컨트롤러를 켜고 입력이 없을 때 `rear.motorTorque`가 `0`으로 유지된다. 즉 컨트롤러가 매 프레임 값을 쓰고 있다.
- 남은 고리는 "실제 키 -> `Input_Compat` -> 토크"뿐이고 이건 에셋 자체 코드다. 다른 키 바인딩과 같은 성격이라 `checklist.md`에 수동 확인으로 남겼다.
- **주의**: 2번 테스트에서 생토크를 제한 없이 걸어서 뒷바퀴 rpm이 10339까지 치솟고 바퀴가 헛돌았다. 실제 컨트롤러는 브레이크/조향/기울기로 이걸 제어하므로 이 수치는 주행감의 지표가 아니다.

#### 검증 결과

- WheelCollider 2개 모두 `isGrounded=True`, radius 0.44.
- 컨트롤러 기본값: `motorForce=500`, `brakeForce=2000`, `maxSteeringAngle=45`, `maxLeanAngle=35`. 에셋 기본값을 그대로 쓴다(주행감 튜닝은 사용자 확인 후로 미룸).
- 정지 상태에서 기울기 0.1도로 넘어지지 않는다.
- 플레이 모드에서 오토바이가 움직여도 에디트 모드 씬 위치 `(4, 0.5, -4)`는 그대로다.
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

### M5 3단계 구현 결과 (2026-09-20)

#### 설계

- 연료와 내구도는 `Motorcycle`이 직접 들고 있다. 별도 상태 클래스를 만들지 않은 이유는 필드 두 개뿐이고 4단계에서 저장 DTO를 따로 만들 예정이라서다.
- **연료는 입력이 아니라 실제 이동 거리로 소모한다**(`fuelPerMeter`). 스로틀을 잡고 벽에 박혀 있어도 연료가 닳는 건 이상하다.
- `canDrive`(연료 > 0 && 내구 > 0)가 거짓이면 `BicycleVehicle`을 끈다. **타는 것 자체는 막지 않는다** - 탈 수는 있고 움직이지 않는다. 안내 문구가 `오토바이 타기 (연료 없음)` / `(고장)`으로 바뀌어 이유를 알려준다.
- 주유(`R`)와 수리(`F`)는 `PlayerInteractor`에 넣었다. `E`는 이미 타고 내리기에 쓰이고, 입력 처리는 입력 담당 컴포넌트에 있어야 한다. `Motorcycle`은 `Refuel(Inventory)`/`Repair(Inventory)`만 공개하고 키를 모른다.
- UI에는 `PlayerInteractor.serviceHint`(문자열)만 노출하고 `InteractionPromptUI`가 아랫줄에 붙인다. UI가 `Motorcycle` 타입을 알 필요가 없다.
- 휘발유통은 새 `ItemData`(8kg, 최대 5개)와 픽업 프리팹(`SM_Props_Can_02`)으로 만들었다. 수리는 기존 고철을 쓴다(5개당 내구 30 회복).

#### 콜라이더가 17개라 충돌 한 번에 내구도가 두 배로 깎였다

- 증상: 14m/s로 나무에 박았더니 내구도가 100 -> 54.4로 **45.6** 줄었다. 계산값은 `(14 - 4) * 2.5 = 25`다.
- 원인: **오토바이의 콜라이더가 17개**라 한 번의 충돌에서 `OnCollisionEnter`가 여러 번 호출된다. 콜라이더 쌍마다 따로 들어온다.
- 해결: `crashCooldown = 0.5초`를 두고 그 안에 들어온 추가 충돌은 무시한다. 재검증에서 감소량이 **23.5**로 계산값에 맞게 나왔다.
- 이 함정은 콜라이더가 하나뿐인 오브젝트에서는 절대 안 보인다. **복합 콜라이더를 가진 물체에 충돌 기반 로직을 붙일 때는 항상 쿨다운이나 프레임 가드를 생각할 것.**

#### 검증 결과

- 연료 0: `canDrive=False`, 안내 `오토바이 타기 (연료 없음)`, 탑승은 되지만 `BicycleVehicle.enabled=False`.
- 주유: 휘발유통 없으면 `False`(재료 미차감), 있으면 연료 0 -> 40, 통 2 -> 1, `canDrive=True`로 복구.
- 내구 0: 안내 `오토바이 타기 (고장)`, 탑승 시 주행 불가. 고철 5개로 수리해 내구 30 확보 후 `canDrive=True`.
- 수리: 고철 부족 시 `False`(미차감), 충분하면 내구 20 -> 50, 고철 12 -> 7.
- 주행 소모: 0.2m 이동에 연료 0.01 소모(`fuelPerMeter=0.06` 기준과 일치).
- 충돌: 14m/s 충돌로 내구 100 -> 76.5(감소 23.5).
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

### M5 4단계 구현 결과 (2026-09-20)

- `MotorcycleSaveData`(위치/회전Y/연료/내구/탑승 여부)를 순수 데이터 클래스로 만들고 `ToSaveData` / `LoadFromSaveData` / `ToJson`을 붙였다. M4의 `PlacedBuilding`과 같은 방식이라 M7에서 둘을 한 저장 파일에 묶기 쉽다.
- `LoadFromSaveData`는 연료/내구도를 `Mathf.Clamp`로 범위 안에 가둔다. 저장 파일이 손상되거나 `maxFuel`을 나중에 줄였을 때 이상한 값이 들어오는 걸 막는다.
- **탑승 상태는 복원하지 않는다.** `isRidden`은 기록만 하고, 실제로 다시 태우는 건 플레이어 복원 순서와 얽히므로 M7에서 처리한다.
- HUD(`VehicleHudUI`)는 `RiderControl.isRiding`과 차량의 공개 필드만 읽는다. 차량이나 라이더가 UI를 알지 않는다. 화면(인벤토리 등)이 열리면 숨긴다.

#### 검증 중 두 번 헛짚었다 (둘 다 내 기댓값이 틀린 쪽이었다)

- 저장/복원 왕복에서 연료가 62.5 대신 **62.37879**로, 위치도 어긋나게 나왔다. 버그로 보였지만 원인은 **eval 사이에 프레임이 지나면서 오토바이가 실제로 움직인 것**이었다. 연료는 이동 거리로 닳고(3단계 설계 그대로) 위치는 물리로 밀린다. 스냅샷은 그 시점의 실제 값을 정확히 담고 있었다.
- 같은 eval 안에서 저장 -> 훼손 -> 복원을 한 번에 돌리니 연료/내구/위치/회전이 전부 정확히 일치했다.
- **교훈**: 물리로 계속 움직이는 대상의 왕복 테스트는 프레임을 건너뛰지 말고 한 호출 안에서 끝내야 한다. M4의 `Refresh` 타이밍 건과 같은 계열이다.

#### 검증 결과

- JSON 덤프에 `position`/`rotationY`/`fuel`/`durability`/`isRidden`만 남고 씬 참조가 섞이지 않는다.
- 같은 호출 내 왕복: 연료 55.000, 내구 33.000, 위치 (2.500, 0.000, -6.500), 회전Y 200.00이 전부 그대로 복원됐다.
- 범위 밖 값(`fuel=9999`, `durability=-50`)을 넣으면 100 / 0으로 잘린다.
- HUD: 탑승 중 `연료 68 / 100     내구 42 / 100`으로 표시되고, 연료 0이면 `연료 없음`, 내구 0이면 `고장` 경고가 붙는다. 스크린샷으로 한글 표시 확인.
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

## 2026-09-20 - M6 시드 월드와 스트리밍 착수

### 에셋 치수가 설계를 거의 정해줬다

- `SM_Grounds_01` / `SM_Grounds_02` / `SM_Roads_03_Cross`가 **전부 정확히 50.2 x 50.2**다. 50m 격자를 고르면 에셋과 1:1로 맞는다. 사용자가 50m 청크를 택한 근거다.
- **다만 도로 조각들은 격자와 크기가 제각각이다.** `01_Straight`는 25.8 x 60.3, `02_L`은 33.1 x 33.0, `04_Curve`는 63.4 x 127.0이다. **Cross만 한 칸에 딱 맞는다.** 2단계에서 배치 오프셋이나 스케일 보정이 반드시 필요하다. 이걸 모르고 전부 50m 칸에 그냥 놓으면 도로가 끊기거나 겹친다.
- 건물도 크다. `SM_Building_01`이 69.7m 폭, `SM_Building_Low_01`은 높이가 104.6m다. **한 건물이 50m 칸을 넘긴다.** 도시 청크는 칸당 1동으로 제한하거나 축소해야 한다.
- 월드 에셋은 전부 URP `_BaseMap`이 연결돼 있다. M3의 상자 같은 머티리얼 교체는 필요 없다.

### School Scene은 프리팹이 아니다

- `Assets/TirgamesAssets/SchoolScene/`에 있고, 건물은 `SchoolSceneAbandoned.unity`(3.9MB) 안에 조립돼 있다. 프리팹 폴더에는 나무/벤치/보도블록 같은 조각만 있다.
- 그래서 "학교 프리팹을 가져다 놓으면 끝"이 아니라 **씬을 열어 계층을 골라 프리팹으로 뽑는 작업**이 필요하다. 라이트맵 의존 가능성도 확인해야 한다. 사용자가 이 비용을 보고도 School 추출을 택했다.

### NavMesh를 두 방식으로 나눠 쓴다

- `Unity.AI.Navigation` 패키지가 이미 있어 `NavMeshSurface`/`NavMeshModifier`를 바로 쓸 수 있다.
- **`Prototype.unity`는 레거시 `NavMeshBuilder` 방식을 그대로 둔다.** M2~M5를 전부 이 씬에서 검증했고, 지금 방식을 바꾸면 그 검증이 흔들린다. `NavMeshSurface`는 새 `World.unity`에서만 쓴다.
- 이 전환이 M4에서 남긴 "건설물이 런타임 생성물이라 NavMesh에 반영되지 않는다" 제약을 함께 푼다. 건설물에 `NavMeshObstacle`(carve)을 붙이면 좀비가 벽을 실제로 돌아간다.

### 400청크는 사용자가 비용을 알고 고른 것

- 선택지에 **"생성 시간과 메모리 부담 증가, 연결성 검사도 무거워진다"**고 적어 보냈고 그대로 택했다. 추천은 12x12(144청크)였다.
- 1단계에서 20x20 생성 시간을 실제로 재서 기록할 것. 느리면 그때 사용자에게 보고한다.

### M6 1단계 구현 결과 (2026-09-20)

#### 구조

- `ChunkType`(초원/도로/도시/산/POI) / `WorldChunkData`(순수 데이터) / `WorldGenerationSettings` / `WorldMap`(격자 + 좌표 변환) / `WorldGenerator`(static, 순수 계산) / `WorldConnectivityReport`로 나눴다.
- **`WorldGenerator`는 씬 오브젝트를 하나도 만들지 않는다.** 그래서 에디트 모드에서 Play 없이 바로 검증할 수 있었고, 2단계의 인스턴스화와 완전히 분리된다.
- **`UnityEngine.Random`을 쓰지 않았다.** 전역 상태라 호출 순서에 영향을 받아 결정론이 깨진다. `System.Random`을 시드로 직접 만들어 쓴다.
- 칸 시드(`chunkSeed`)는 **좌표에서 직접 해시로 뽑는다.** 순차 난수로 뽑으면 생성 순서가 바뀔 때 값이 달라지는데, 좌표 해시는 순서와 무관하게 같은 값이 나온다. 2단계에서 칸 안 세부 배치에 쓸 때 안전하다.
- 도로는 **격자로 깐다**(일정 간격의 가로줄 + 세로줄). 이렇게 하면 도로망이 구조적으로 항상 연결되므로 연결성 검사가 실패할 일이 거의 없다. 연결성은 그래도 별도로 검사한다(`CLAUDE.md` §11.7).
- POI는 **도로 자체를 덮지 않고 도로 옆 칸에만** 놓는다. 도로를 덮으면 길이 끊긴다. 서로 최소 격자 거리(`poiMinDistance`)만큼 떨어뜨린다.

#### 도로 비율이 너무 높아 기본값을 조정했다

- 처음 간격을 3~6으로 뒀더니 **400칸 중 도로가 144~190칸(36~48%)**이었다. 월드의 절반 가까이가 도로 타일이 된다.
- 간격을 4~8로 넓혀 재측정: 초원 34% / 도로 33% / 도시 25% / 산 7.6% / POI 0.8%. 산이 2~6%에서 7.6%로 늘어난 것도 부수 효과다(산은 도로에서 먼 칸에만 생기므로 도로가 줄면 산이 늘어난다).
- 이건 튜닝값이라 `WorldGenerationSettings`에서 언제든 바꿀 수 있다.

#### 400청크 생성 비용은 문제가 아니었다

- 사용자가 20x20(400청크)을 고를 때 내가 "생성 시간과 메모리 부담 증가"를 단점으로 적었는데, **실제 생성 시간은 0.0ms**였다(12회 평균). 순수 계산이라 400칸이 아무것도 아니다.
- **비용은 2단계 인스턴스화에 있다.** 400칸을 전부 씬에 올리면 부담이 크고, 그래서 스트리밍이 필요한 것이다. 1단계 결과로 "생성은 걱정할 필요 없고 동시 활성 청크 수만 관리하면 된다"는 게 확인됐다.

#### 콘솔에 남아 있던 에러 1,946건은 내 코드가 아니었다

- `Invalid worldAABB. Object is too large or too far away from the origin.` 이 1,946건 찍혀 있었다. 전부 **05:32:59**, 즉 M6 코드를 쓰기 전에 대형 프리팹 치수를 재던 시점이다. `groundTruth.consoleErrors`는 0이었다(콘솔에는 남아 있지 않음).
- 콘솔을 비우고 생성기만 20회 재실행해 **에러 0건**을 확인했다. 순수 계산 코드라 worldAABB 에러를 낼 수 없다.
- **다만 2단계 경고로 남겨둔다.** `SM_Building_Low_01`은 높이 104.6m, `SM_Roads_04_Curve`는 127m다. **1km 월드에서 원점에서 먼 곳에 이런 대형 콜라이더를 놓으면 실제로 worldAABB 문제가 날 수 있다.** 2단계에서 인스턴스화할 때 반드시 확인할 것.

#### 검증 결과

- 같은 시드 2회 -> 지문 일치, 시작 칸 일치 `(10, 10)`.
- 다른 시드 -> 지문 다름. **생성기 버전만 바꿔도 지문 다름**(`GeneratorVersion` 의도대로 동작).
- 칸 (7,13)의 `chunkSeed`가 두 번 생성에서 동일.
- 20개 시드 전부 연결성 통과(도로 100% 도달, POI 3/3 도달).
- 20x20 생성 시간 평균 0.0ms.
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

### M6 2단계 구현 결과 (2026-09-20)

#### 피벗이 제각각이라 좌표를 하드코딩하지 않았다

- 에셋 프리팹의 피벗이 **대부분 모서리 기준**이다. `SM_Grounds_02`의 bounds center가 `(-25.11, 0.47, -25.11)`이고, `SM_Roads_03_Cross`는 `(-12.92, 0, -25.11)`로 축마다 다르다. 좌표를 손으로 넣었으면 전부 어긋났을 것이다.
- 그래서 `WorldChunkBuilder.Place`가 **인스턴스화 후 실제 bounds를 읽어 보정값을 계산**한다. 회전·스케일이 적용된 뒤의 bounds를 쓰므로 어떤 프리팹이 와도 칸 중앙에 맞는다.
- 스케일도 하드코딩하지 않고 `FitScale`이 긴 축을 청크 크기에 맞춘다. **균등 스케일만 쓴다** - 비균등 스케일은 도로 텍스처가 늘어난다.
- 도로 조각 크기가 제각각인 문제(`Straight` 25.8x60.3, `L` 33x33, `Cross` 50.2x50.2)가 이 방식으로 자동 해결됐다. 긴 축을 50m에 맞추면 직선은 21x50, L은 50x50이 된다.

#### 건물이 120m짜리라 이중 제한을 걸었다

- `SM_Building_Low_02`는 높이가 **119.78m**, `SM_Building_Low_05`는 가로 60.78m다. 50m 칸에 그냥 넣으면 칸을 넘치고, 긴 축(높이) 기준으로 맞추면 오히려 더 커진다.
- 그래서 **가로/세로 제한(34m)과 높이 제한(40m) 중 더 빡빡한 쪽**을 따르게 했다. 검증에서 `SM_Building_Low_05`가 34.0 x 30.3 x 20.4로 들어갔다.
- 1단계에서 걱정했던 worldAABB 에러는 나지 않았다(에러 0건). 크기를 제한한 덕이 크다.

#### 버그 3개를 잡았다

**1. 카메라가 원점에서 600m를 보간해 따라왔다**
- 시드 월드의 시작 지점이 `(525, 1, 425)`인데 카메라가 씬 기본 위치 `(0, 1, -10)`에서 `Lerp`로 출발해, 몇 초간 엉뚱한 곳을 비췄다. 측정값 카메라 `(253.2, 2.1, 197.8)`.
- `ThirdPersonCameraController`에 `hasSnapped` 플래그를 넣어 **첫 프레임만 보간 없이 바로 붙게** 했다. 원점 근처에서 시작하는 `Prototype` 씬에서는 차이가 없다.

**2. 프레임이 아예 돌지 않았다 (`Time.time=0.000`, `frameCount=1`)**
- 스트리밍이 전혀 갱신되지 않아 한참 헤맸다. 원인은 코드가 아니라 **에디터가 포커스를 잃은 상태 + `Application.runInBackground = False`**였다. `isApplicationActive=False`, 실제 시간 82초가 지났는데 `frameCount`가 1이었다.
- 런타임에 `Application.runInBackground = true`로 켜니 즉시 `frameCount`가 2851로 뛰었다.
- **프로젝트 설정(Player Settings)은 건드리지 않았다.** 사용자 소유 파일이다. 앞으로 Play Mode 검증에서 시간이 안 흐르면 **먼저 `Time.frameCount`를 의심할 것.** `editor_status`는 `playing`이라고만 알려줘서 이걸 못 잡는다.

**3. 물체가 자기 콜라이더 위에 올라갔다**
- 바닥 타일이 평평하지 않아(`SM_Grounds_02`가 -0.09~1.04로 울퉁불퉁) y=0에 놓으면 뜨거나 파묻힌다. 그래서 아래로 레이캐스트해 실제 지면에 올리도록 바꿨다.
- 그런데 **레이캐스트가 방금 인스턴스화한 자기 자신의 콜라이더를 지면으로 잡았다.** 건물이 +27.19m, 나무가 +11.24m 떠올랐다 - 정확히 자기 키만큼이다.
- `SampleGroundHeight`에 `self`를 넘겨 자기 자식 히트를 건너뛰고, 바닥 타일(`Grounds`) 콜라이더만 지면으로 인정하게 했다. 이렇게 하면 앞서 놓인 나무 위에 다음 나무가 쌓이는 것도 막힌다.
- 수정 후 31개 오브젝트 전부 지면 오차 0.00m(도로만 의도된 -0.32m).

#### 검증 결과

- 시작 시 25청크 로드(반경 2 = 5x5). 3칸 이동하면 x 범위가 9~13으로 따라 이동하고 여전히 25개.
- **월드 모서리 `(0,0)`에서는 9개만 로드**된다(경계 밖을 올리지 않음).
- `loadedCount`와 실제 자식 수가 25로 일치(누수 없음).
- 지면 정렬: 나무/건물/바위 31개 전부 오차 0.00m.
- 건물 크기 제한: 34.0 x 30.3 x 20.4로 축소 확인.
- 스크린샷으로 도로 연결(연석·차선), 건물·나무 접지, 도로 위 풀 덮임 해소를 확인했다.
- `World.unity`를 빌드 설정 인덱스 3에 등록했다(`GameManager`의 씬 재시작이 동작하려면 필요).
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`. **1단계에서 경고로 남겼던 worldAABB 에러는 나지 않았다.**

#### 남은 시각적 아쉬움 (기능 영향 없음)

- 직선 도로가 21m 폭이라 50m 칸의 나머지는 풀밭이다. 도로 칸에 서 있어도 풀 위일 수 있다.
- `Roads_04_Curve`(63x127)와 `05_Hill`은 칸 크기와 너무 달라 쓰지 않았다. 커브는 `02_L`로 대체했다.
- T자 도로 조각이 에셋에 없어 세 갈래도 십자로 덮는다.

## 2026-09-20 - 좀비 탐지를 시야 + 청각 기반으로 변경

사용자 요청: "좀비는 플레이어를 발견하면 플레이어한테 이동하고 총소리 같은 큰소리를 들으면 그쪽으로 이동하게. 처음부터 플레이어를 추적하지 말고."

### 기존 동작의 문제

- `Physics.OverlapSphere(transform.position, 20f, whatIsTarget)` 하나로 끝이었다. **벽 너머든 등 뒤든 20m 안이면 무조건 발견**하고 즉시 추적했다. 스폰되자마자 플레이어에게 달려오는 게 이 때문이다.

### 바꾼 것

- **시야 기반 발견**: 거리(`sightRange` 15m) + 시야각(`sightAngle` 110도) + **시야 차단 레이캐스트**를 모두 통과해야 발견한다. 눈높이는 발끝이 아니라 가슴(+1.5m) 기준으로 잡았다.
- **청각**: `NoiseEvent`라는 정적 통로를 만들고 `Gun.Fire`가 발사 지점과 반경(`noiseRadius` 35m)을 알린다. 좀비는 `OnEnable`에서 구독해 듣는다.
  - **소리를 내는 쪽과 듣는 쪽이 서로를 모른다.** `Gun`은 좀비를 모르고 `Zombie`는 총을 모른다. 나중에 폭발음이나 차 소리도 `NoiseEvent.Emit` 한 줄이면 붙는다.
  - 소리를 들으면 **그 지점으로 가서 조사**한다(플레이어를 직접 쫓는 게 아니다). 도착하거나 `investigateGiveUpTime`(8초)이 지나면 포기하고 선다.
- **추적 포기**: 대상을 `loseSightTime`(4초) 동안 못 보면 놓친 것으로 보고, 마지막으로 본 자리를 조사한다. 잠깐 엄폐물 뒤로 숨는다고 바로 잊지는 않는다.
- **우선순위**: 눈에 보이면 소리보다 눈이 우선이다(조사 중이어도 발견하면 추적으로 전환).
- 걷는 애니메이션(`HasTarget`)은 추적 중이거나 조사 중일 때 재생한다.

### 걸린 것들

- **`OnEnable`을 `private`으로 선언해 부모의 `protected virtual OnEnable`을 가렸다.** 컴파일은 통과했지만 `protected override`가 맞다. `LivingEntity.OnEnable`이 체력/사망 상태를 초기화하므로 `base.OnEnable()`을 반드시 불러야 한다.
- **조사 지점이 NavMesh 밖일 수 있다.** 검증 중 플레이어를 지형 밖으로 순간이동시켰더니 조사 지점이 `y=-74.2`로 잡혔다. 실제 게임에서도 총소리가 건물 위 같은 곳에서 날 수 있다. `NavMesh.SamplePosition`으로 8m 안의 갈 수 있는 자리로 보정하고, 없으면 조사하지 않는다.

### 검증 중 착각한 것 (기록용)

- 좀비가 1.3m 앞에서 플레이어를 발견하지 못해 시야 로직 버그인 줄 알고 레이캐스트까지 뜯어봤는데, **플레이어가 앞선 테스트에서 이미 죽어 있었다**(`hp=0, dead=True`). 죽은 대상을 무시하는 건 의도한 동작이다.
  - **교훈**: 좀비 AI를 여러 번 테스트하면 플레이어가 죽는다. 판정이 이상하면 먼저 `player.dead`를 확인할 것.
- 콘솔 에러 1건(`Coroutine couldn't be started ... 'Assault Rifle Gun(Clone)' is inactive`)은 **죽은 플레이어의 총에 eval로 `Fire()`를 직접 호출해서** 난 테스트 아티팩트다. M2 노트에 같은 건이 이미 기록돼 있다. 깨끗한 실행에서는 에러 0건이다.

### 검증 결과

- **사거리 밖(25m) + 등짐**: 발견 안 함, 제자리 정지.
- **사거리 밖(25m) + 정면**: 발견 안 함(거리 조건).
- **사거리 안(12m) + 정면**: 발견 후 추적, 12m -> 0.4m까지 접근하고 공격(hp 100 -> 0).
- **사거리 안 + 등짐**: 발견 안 함(시야각 조건).
- **총소리(실제 `Gun.Fire()` 경로)**: 등 뒤 20m 좀비가 즉시 조사 상태로 전환, 조사 지점이 NavMesh 위(`y=0.1`)로 보정됨, 20m -> 3.9m까지 이동.
- **소리 -> 시야 전환**: 24.3m에서 소리를 쫓다가 시야에 들어오자 `조사중=False, 대상=있음`으로 전환.
- **추적 포기**: 대상을 치운 뒤 4초 지나자 대상 해제 + 마지막 위치 조사로 전환.
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

### 튜닝값 (전부 인스펙터 노출)

`sightRange` 15m / `sightAngle` 110도 / `loseSightTime` 4초 / `investigateStopDistance` 2m / `investigateGiveUpTime` 8초 / `Gun.noiseRadius` 35m.

### M6 3단계 구현 결과 (2026-09-20)

#### NavMeshSurface를 쓰려다 NavMeshBuilder로 갈아탔다

- 계획은 "청크마다 `NavMeshSurface` 하나"였는데 **쓸 수 없다.** `NavMeshSurface`는 각자 별도의 `NavMeshData`를 만들고, **분리된 NavMeshData끼리는 자동으로 이어지지 않는다.** 청크마다 두면 좀비가 청크 경계를 못 넘는다.
- 그래서 "로드된 영역 전체를 덮는 하나의 볼륨"으로 바꿨는데, 여기서 두 번 막혔다.
  1. **동기 베이크가 너무 느리다.** 250x250m(25청크) `BuildNavMesh()`가 **596ms**. 50m마다 이만큼 멈추면 오토바이로는 3초에 한 번씩 정지한다. 150x150m로 줄여도 202ms다.
  2. **`NavMeshSurface.UpdateNavMesh`는 볼륨이 움직이면 지오메트리를 다시 수집하지 않는다.** 플레이어가 150m 이동한 뒤 비동기 갱신을 돌렸더니 정점이 **1160 -> 122**로 떨어지고 발밑 NavMesh가 사라졌다. 같은 자리에서 동기 `BuildNavMesh()`를 부르면 1160이 나온다 - 이걸로 원인을 격리했다.
- 결론적으로 **저수준 `NavMeshBuilder`를 직접 쓴다.** `NavMeshBuilder.CollectSources`로 월드 bounds 안의 콜라이더를 모으고 `UpdateNavMeshDataAsync`로 비동기 갱신한다. 이게 Unity가 이동하는 NavMesh 볼륨에 대해 권장하는 방식이다. 씬에서 `NavMeshSurface` 컴포넌트는 제거했다.

#### 좌표계를 헷갈려 NavMesh가 통째로 비었다

- `NavMesh.AddNavMeshData(navMeshData)`로 데이터를 **원점에 identity로** 붙여놓고, 빌드 범위에는 **원점 기준 로컬 bounds**를 넘겼다. 수집원은 월드 좌표라 서로 어긋나 정점 0이 나왔다.
- 데이터가 원점 identity면 **로컬 좌표가 곧 월드 좌표**이므로 월드 bounds를 그대로 넘겨야 한다. 고친 뒤 1012정점.

#### 실행 순서 경쟁 상태

- 처음엔 `Start()`에서 구웠는데 `WorldStreamer.Start()`와 순서가 정해져 있지 않아 **청크가 지어지기 전에 구워 빈 NavMesh**가 나왔다.
- `WorldStreamer.onChunksChanged` 이벤트를 만들어 **청크가 다 올라온 뒤에만** 굽게 했다. 플레이어 위치만 보고 구우면 같은 문제가 반복된다.

#### 빌드하면 깨질 뻔한 문제를 잡았다

- 콘솔에 `RuntimeNavMeshBuilder: Source mesh ... does not allow read access. This will work in playmode in the editor but not in player` 가 **55건** 떴다.
- **런타임 NavMesh 베이크는 메시에 Read/Write 권한이 필요하다.** 에디터에서는 동작하지만 빌드하면 NavMesh가 안 만들어진다. `groundTruth.consoleErrors`를 안 봤으면 빌드 전까지 몰랐을 문제다.
- `Assets/Apocalyptic_World`의 모델 **132개 전부에 `isReadable = true`**를 켜고 재임포트했다.
  - **비용**: 메시가 CPU 메모리에도 남는다. M8 최적화 단계에서 실제로 필요한 메시만 남기는 걸 검토할 것.
  - 재임포트가 길어 MCP 호출이 한 번 타임아웃됐지만 작업은 완료됐다(132/132 확인).

#### 건설물 NavMeshObstacle - M4 제약 해소

- 건설물 프리팹 4종(벽/바리케이드/보관 상자/문)의 콜라이더가 있는 오브젝트에 `NavMeshObstacle`(carving, Box, `carveOnlyStationary`)을 붙이고 콜라이더 크기를 그대로 복사했다. 문은 콜라이더가 `Hinge`에 있어 거기에 붙었다.
- **M4에서 "건설물은 NavMesh에 반영되지 않는다"고 남겨둔 제약이 풀렸다.**

#### 검증 결과

- 첫 베이크: 정점 1012, 플레이어 발밑 NavMesh 유효.
- 150m 이동 -> 구운 중심이 (10,8) -> (13,8)로 따라오고 정점 1120, 발밑 유효. 한 칸 더 이동해도 (14,8)로 따라온다.
- 좀비가 시드 월드에서 12m -> 3.3m까지 추적, `pathStatus=PathComplete`.
- **벽 9장을 세우면** 경로가 꺾임 4개 / **17.4m**로 우회하고(직선 14.0m), **철거하면** 꺾임 3개 / **14.1m** 직선으로 돌아온다. 벽 자리가 NavMesh에서 파이는 것도 확인했다.
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

#### 남은 것

- 비동기 베이크 중에는 이전 NavMesh가 그대로 쓰인다. 빠르게 이동하면 아직 안 구워진 영역에 좀비가 설 수 없다. `chunkMargin`으로 로드 반경보다 좁게 구워 완충을 뒀다.
- 메시 Read/Write를 전부 켠 메모리 비용은 M8 최적화 사안이다.

### M6 4단계 구현 결과 (2026-09-20)

#### School 씬에서 건물 본체만 골라냈다

- `SchoolSceneAbandoned.unity`는 루트가 6개고 `SchoolScene` 하나에만 렌더러가 **2022개**다. `Exterior`(642) + `Interior`(1262) + 조명/프로브로 나뉜다. 통째로 가져오면 POI 하나에 2000개가 넘는 렌더러가 붙는다.
- `Exterior`의 자식 326개를 부피순으로 훑어 **`SchoolBuilding`(렌더러 58, 콜라이더 21, 80.1 x 9.3 x 58.2m)**을 찾았다. 나머지는 담장/벤치/보도블록/주변 건물이다.
- 머티리얼이 전부 `Universal Render Pipeline/Lit`이라 변환이 필요 없었다.
- **라이트맵 주의**: 58개 중 46개가 이 씬의 베이크된 라이트맵을 쓴다. 다른 씬에는 그 데이터가 없으므로 `lightmapIndex`를 끊고 저장했다(`Instantiate`가 이미 초기화해줘서 실제로 끊은 건 0개였다). 실시간 조명만으로 렌더된다.

#### POI를 칸에 맞춰 줄이면 "대형"이 아니게 된다

- 처음 규칙은 `FitScale(poiPrefab, chunkSize * 0.9)` = 45m였다. 80m 학교를 45m로 줄이면 **도시 건물(가로 34m, 높이 40m)보다 작아져** 랜드마크가 되지 않는다.
- 그래서 두 가지를 바꿨다.
  - `ChunkLibrary.poiFootprint`(기본 85m)를 따로 두고 칸 크기와 분리했다. 학교가 자연 비율(85.0 x 9.8 x 61.8m)로 선다.
  - **생성기가 POI 주변 8칸을 초원으로 비운다**(`ClearAroundPoi`). 한 칸을 넘치는 POI가 건물이나 바위와 겹치지 않는다.
- **도로는 비우지 않는다.** 비우면 길이 끊겨 연결성 검사가 깨진다. 검증에서 POI 남쪽에 도로가 그대로 남아 있는 것을 확인했다.
- 생성 규칙이 바뀌었으므로 `generatorVersion`을 **2로 올렸다**. 같은 시드라도 v1과 v2는 다른 월드가 나온다(확인함).

#### 검증 결과

- v2 연결성: 10개 시드 전부 통과(도로 128/128, POI 3/3 도달).
- v2 결정론: 같은 시드 2회 지문 일치, v1과는 다름.
- POI 주변 배치: POI 칸 주변이 `Plain`으로 비워지고 남쪽 도로 3칸은 그대로 `Road`.
- 씬 로드: `School POI(Clone)` 85.0 x 9.8 x 61.8m, 콜라이더 21개, 바닥 y=-0.28로 접지.
- NavMesh 정점이 약 1000에서 **1939**로 늘어 학교 지오메트리가 수집됐다.
- 스크린샷으로 학교가 도로 건너편 대형 랜드마크로 서 있는 것을 확인했다.
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

#### 남은 것

- `Interior`(렌더러 1262)는 가져오지 않았다. 학교 내부 탐험이 필요해지면 별도 POI로 다루거나 내부 진입 시에만 로드해야 한다.
- 라이트맵이 없어 학교가 다소 평평하게 보인다. 조명 폴리싱은 M8 사안이다.

## 2026-09-20 - 학교 내부 탐험 추가

사용자 요청: "학교 내부 탐험도 필요해". M6 4단계에서 `Interior`(렌더러 1262)를 제외했던 것을 되돌렸다.

### 내부는 별도 공간이 아니라 제자리에 있었다

- `Interior` 트랜스폼의 **위치가 `(-68, -15, 27.9)`**라 처음엔 건물과 동떨어진 별도 공간(텔레포트 방식)인 줄 알았다.
- 실제로 월드 bounds를 재보니 **`SchoolBuilding`과 중심 차이가 `(0.6, -0.6, 0.0)`밖에 안 됐다.** 트랜스폼 원점이 임의의 자리에 있고 자식들이 그걸 상쇄하고 있을 뿐, 내부는 건물 안 제자리에 있다.
- **교훈**: 트랜스폼 위치만 보고 판단하면 안 된다. 렌더러 bounds로 실제 공간 관계를 확인해야 한다.
- 그래서 프리팹을 만들 때 **원본의 상대 위치(`interior.position - school.position` = `(-82.96, -15.00, 27.94)`)를 그대로 보존**했다. 이 값이 틀어지면 내부가 건물 밖으로 어긋난다.

### 내부 구성

- 렌더러 1262, **콜라이더 1065**(완전히 걸어다닐 수 있다), 전부 `Universal Render Pipeline/Lit`.
- 교실 7개(Class1-2/1-3/1-4/2-1/2-2, ClassComputer, ClassDirector, ClassTeachers), 복도 2개, 급식실, 강당, 체육실 3개, 화장실, 문 41개, 잔해.
- **1226/1262가 베이크 라이트맵에 의존**한다. `DayLightsInterior`는 Light가 0개고 라이트맵용 발광 메시 115개뿐이라 가져와도 소용없다.
  - 걱정했던 것과 달리 **스카이박스 앰비언트만으로 충분히 밝았다.** 스크린샷으로 확인했다. 조명 폴리싱은 M8 사안이다.
- 합친 프리팹: 렌더러 1320, 콜라이더 1086, 크기 81.4 x 9.4 x 58.2.

### 실내에서 3인칭 카메라가 벽을 뚫었다

- 좁은 복도에서 카메라가 플레이어 뒤 3.5m에 있으니 벽을 관통해 바깥이 보였다. 실내 탐험에는 치명적이다.
- `ThirdPersonCameraController`에 **구 캐스트 기반 벽 회피**를 넣었다. 피벗에서 목표 지점까지 `SphereCastAll`로 굴려 막히면 그 앞까지만 물러난다. 플레이어 자신과 들고 있는 물건은 건너뛴다.
- `collisionRadius`(0.25) / `minCollisionDistance`(0.4)로 조절한다. 원점 근처에서 시작하는 `Prototype` 씬에는 영향이 없다.

### 실내외 NavMesh가 끊겨 있었다 - 문턱 높이 문제

- 바깥에서 복도까지 경로가 **`PathPartial`**(62.8m까지 가고 못 들어감)이었다. 좀비가 학교 안으로 못 따라온다는 뜻이다.
- x축을 따라 훑어보니 `OOOO....OOOOOOOOOO`로 건물 앞 약 16m 구간에서 NavMesh가 끊겼다.
- 원인은 **`agentClimb`(오를 높이) 기본값 0.4**였다. 학교 출입구 문턱이 이보다 높아 NavMesh가 이어지지 않았다.
- `WorldNavMeshBaker.agentClimb`를 두고 **0.75**로 올리니 즉시 **`PathComplete`(88.7m)**가 됐다. 베이크 시간은 245ms로 비슷하다.
- **이건 추측이 아니라 실험으로 잡았다.** 같은 자리에서 설정만 바꿔 다시 구워 `PathPartial -> PathComplete`를 확인했다.

### 검증 결과

- 합친 프리팹이 씬에 로드되고 내부가 건물에 정확히 맞물린다(스크린샷 확인).
- 카메라가 복도에서 벽 안쪽에 머문다(수정 전에는 바깥이 보였다).
- 실외 -> 실내 경로 `PathComplete`, 꺾임 7개, 88.7m.
- **좀비가 실제로 학교 안까지 들어왔다**: 총소리로 유인하니 64.6m -> 25.4m로 접근하고 z=177.1(건물 내부 범위 162~204)에 도달.
- NavMesh 수집원이 48 -> 600으로, 정점이 약 1900 -> 3806으로 늘었다.
- `compilationFailed: false`, `groundTruth.consoleErrors: 0`.

### 남은 것

- POI 하나에 렌더러 1320개가 붙는다. 지금은 POI가 서로 5칸 이상 떨어져 있어 동시에 한 개만 로드되지만, **M8 최적화에서 거리 기반 내부 비활성화를 검토할 것.**
- 내부에 조명이 없어 라이트맵이 있던 원본보다 평평하다.

## 2026-09-20 - M7 저장 시스템 착수

### 착수 전에 발견한 것: World.unity가 비어 있었다

- 저장 시스템을 만들기 전에 저장 대상이 있는지 확인했더니 **`World.unity`에 게임플레이 콘텐츠가 하나도 없었다.** `Motorcycle` 0, `LootContainer` 0, `ResourceNode` 0, `StorageContainer` 0, `ZombieSpawner` 0, `ItemSpawner` 0.
- M6에서 `World.unity`를 새로 만들면서 플레이어/HUD/매니저/스트리머만 넣었고, "스포너의 청크 연동"은 M8로 미뤘기 때문이다. 콘텐츠는 전부 `Prototype.unity`에만 있다.
- 이대로 M7을 하면 **기획서 M7 완료 조건의 "오토바이 복원"을 채울 수 없다.** 사용자에게 선택지를 주고 **"먼저 World에 콘텐츠를 붙인다"**로 확정했다.
- **교훈**: 저장 시스템처럼 다른 시스템을 모으는 작업은 착수 전에 대상이 실제로 존재하는지부터 세어볼 것. 코드를 먼저 썼으면 저장할 게 없다는 걸 나중에 알았을 것이다.

### 확정된 설계 결정

- 슬롯 화면은 **게임 내 `ScreenPanel`**. 기존 인벤토리/장비/상태/건설/보관함과 같은 구조라 화면 배타(한 번에 하나만 열림)가 자동으로 적용된다. 타이틀 씬은 만들지 않는다.
- 자동저장은 **일정 시간마다**(기본 5분), 수동 저장과 같은 슬롯의 **다른 칸**에 기록해 수동을 덮어쓰지 않는다.

### 미리 정리한 기술적 제약

- **id -> 에셋 레지스트리가 없다.** `ItemData.itemId`와 `BuildableData.buildableId`는 있지만 id로 에셋을 되찾을 방법이 없어 불러오기가 불가능하다. `SaveRegistry`(ScriptableObject)를 만들어야 한다.
- **청크 콘텐츠의 상태는 청크 오브젝트에 두면 안 된다.** 스트리밍으로 청크가 내려가면 같이 사라진다. `WorldRuntimeState`가 채집/루팅 상태를 청크 밖에서 들고 있어야 한다(`CLAUDE.md` §11.6).
- 이미 준비된 것: `BaseBuildState.ToJson()`, `Motorcycle.ToSaveData()/LoadFromSaveData()`, 월드는 시드 + 생성기 버전만으로 재현.

### M7 1단계 — 청크 콘텐츠 배치 (2026-09-20)

- **배치는 칸 시드, 상태는 칸 밖.** `WorldChunkBuilder`가 칸 시드 난수로 자원/상자를 놓고, 배치 순번이 그대로 id(`11_10_res_0`)가 된다. 채집/루팅 여부는 `WorldRuntimeState`가 id로 들고 있으므로 청크가 내려갔다 올라와도 이어진다. 저장 DTO(`WorldRuntimeSaveData`)는 남은 재생성 시간을 절대 시각이 아닌 상대값으로 담는다. 불러오는 시점의 `Time.time`은 저장 시점과 다르기 때문이다.
- **복원은 `Start`에서 한다.** `LivingEntity.OnEnable`이 `dead`와 체력을 되돌리므로 그보다 늦은 `Start`에서 채집 상태를 다시 적용해야 한다. 청크 빌더가 `Instantiate` 직후에 id를 채우는데, `Awake`/`OnEnable`은 그 전에 끝나고 `Start`는 다음 프레임이라 순서가 맞는다.
- **지붕/바위 위에 얹히지 않게 거부 샘플링을 쓴다.** 후보 지점 바로 위에서 내려봤을 때 먼저 닿는 것이 바닥 타일(`Grounds`)일 때만 받아들이고, 아니면 최대 8번 다시 뽑는다. 놓을 때마다 `Physics.SyncTransforms()`를 불러야 앞서 놓은 것 위에 겹치지 않는다.
- **시작 청크 중앙은 비워야 한다.** 첫 검증에서 자원 노드가 스폰 지점 0.6m 옆에 생겼다. `WorldStreamer.startClearRadius`(4m)를 시작 청크에만 넘겨 가장 가까운 콘텐츠가 12.0m로 밀렸다.
- **상자 프리팹의 내용물이 비어 있었다.** `Loot Chest.prefab`의 `contents`가 0개라 열어도 얻는 것이 없었다. Ammo Box 1 / Bandage 1 / Material Scrap 2를 넣었다. 칸별로 다른 전리품이 나오게 하려면 루팅 테이블이 필요하지만 지금 범위가 아니다.
- **비용.** 칸당 1.36ms(25칸 33.9ms). 주요 비용은 여전히 NavMesh 베이크다.
- 검증: 노드 41개/상자 18개 배치, 모두 바닥과 정확히 맞닿음(차이 0.00m), 청크 재생성 후 동일 좌표, 채집/루팅 상태 유지, 재생성 시간이 지나자 부활, `consoleErrors: 0`.

### M7 2단계 — 오토바이와 좀비 스폰 (2026-09-20)

- **교재의 `ZombieSpawner`를 World 씬에 쓰지 않는다.** 고정 스폰 지점 + 무한 웨이브 방식이라 스트리밍 월드에서는 플레이어가 멀어지면 아무 일도 안 일어난다. 사용자 결정으로 `WorldZombieSpawner`를 새로 만들어 플레이어 주변 25~45m 고리의 NavMesh 위에 최대 8마리를 유지하고, 90m 넘게 멀어진 좀비는 지운다. Prototype 씬의 기존 스포너는 그대로 둔다.
- **`ItemSpawner`는 World 씬에 넣지 않는다.** 플레이어 근처에 아이템을 뿌리고 5초 뒤 지우는 방식은 파밍/생존 설계와 충돌하고 저장하기도 애매하다 (사용자 결정).
- **좀비는 저장 대상이 아니다.** 불러오면 스포너가 다시 채우면 되므로 3단계 저장 DTO에 넣지 않는다.
- 검증: 오토바이를 시작 지점 8m 옆에 놓았고 청크가 올라온 뒤 지면(y=-0.28)으로 안정했다. 시드 월드에서 탑승/하차가 동작하고, 좀비는 8/8로 유지되며 모두 NavMesh 위에 섬다. 275m 순간이동 후 기존 8마리가 정리되고 새 고리에 다시 생겼다. `consoleErrors: 0`.

### M7 3단계 — 저장 데이터 모델과 파일 입출력 (2026-09-20)

- **저장에는 에셋 참조 대신 id만 넣는다.** `SaveRegistry`(ScriptableObject)가 `itemId`/`buildableId`를 에셋으로 되돌린다. 현재 아이템 13개, 건설물 4개이고 중복이나 빈 id는 없다. 새 아이템/건설물을 만들 때 여기에 등록하지 않으면 불러오기에서 조용히 사라진다.
- **슬롯마다 파일 두 개.** `slot{N}.json`(수동)과 `slot{N}.auto.json`(자동)을 나눠 `CLAUDE.md` §11.8의 “수동과 자동을 구분”을 만족시킨다. 경로는 `Application.persistentDataPath/Saves`다.
- **원자적 저장.** `.tmp`에 먼저 쓰고 기존 파일을 지운 뒤 옮긴다. 실패하면 `.tmp`를 지우고 false를 돌려준다. 검증은 기존 파일을 읽기 전용으로 만들어 교체를 실패시켜 했고, 원본 내용이 그대로 남고 임시 파일도 안 남았다.
- **이 단계 검증은 Play Mode가 아니라 에디터 상태에서 돌렸다.** 파일 입출력과 `JsonUtility`는 플레이 여부와 무관하고, 실제 씬 상태 수집/복원은 4단계에서 Play Mode로 검증한다.
- **좀비는 저장하지 않는다.** `SaveData`에 좀비 항목이 없다. 불러오면 `WorldZombieSpawner`가 다시 채운다.
- 검증 후 시험 저장 파일은 지우고 콘솔을 비웠다 (`consoleErrors: 0`).

### M7 4단계 — 수집과 복원 (2026-09-20)

- **`SaveManager`가 수집/복원을 혼자 맡는다.** 각 시스템은 저장을 모르고, `SaveManager`가 공개 API로만 읽고 쓴다. 기존 컴포넌트에는 꼭 필요한 것만 더했다. `LivingEntity.SetHealth`(체력을 그대로 되돌리기 위해. `RestoreHealth`는 더하기만 된다), `PlayerHealth.SetHealth` 오버라이드(체력 슬라이더 갱신), `GameManager.score` 공개 + `SetScore`, `WorldStreamer.Regenerate`.
- **순서가 중요하다.** ① 플레이어를 먼저 옮기고 ② 시드가 다르면 월드를 재생성해야 새 위치 주변 청크가 올라온다. 채집/루팅 상태는 청크가 다시 올라오기 전에 되돌려야 반영된다. 장비는 인벤토리 안 아이템을 가리키므로 인벤토리를 먼저 채운 뒤 장착한다.
- **문 열림 상태는 기록을 통해 전달된다.** `BaseBuildState.Place` 직후에 `record.isOpen`을 써놓으면 다음 프레임 `BuildableDoor.Start`가 읽어 반영한다. 검증에서 복원 뒤 경첩 각도가 실제로 90도였다.
- **보관 상자 내용물은 건설물 순번으로 연결한다.** 상자에 별도 id를 두지 않고 `SaveData.buildings` 안의 순번을 쓴다. 복원은 같은 순서로 다시 세우므로 순번이 맞는다.
- **탑승 상태로 저장해도 불러올 때는 내린 상태로 시작한다.** 탑승 중 복원은 부모 관계와 입력 잠금까지 되살려야 해서 복잡하고 얻는 것이 적다.
- 검증: 상태 변경 → 저장 → 훼손 → 불러오기에서 체력 70, 위치/방향, 아이템 6종, 장비 2칸, 건설물 2개, 상자 안 mat_scrap x7, 오토바이 55/70, 채집/루팅 1개씩이 모두 일치했다. 시드 777 저장을 불러오자 지문이 바뀐고 청크 25칸이 재구성됐다. `consoleErrors: 0`.

### M7 5단계 — 슬롯 UI와 자동저장 (2026-09-20)

- **자동저장은 자동 칸에만 쓴다.** `slot{N}.auto.json`을 갱신하고 수동 파일은 건드리지 않는다. 대상 슬롯은 마지막으로 수동 저장하거나 불러온 슬롯(`SaveManager.currentSlot`, 기본 0)이다.
- **주기는 ‘다음 시각’이 아니라 ‘마지막 저장 시각’을 기준으로 재산한다.** 다음 시각을 미리 잡아두면 `autoSaveInterval`을 런타임에 바꿔도 한 번은 옮 주기로 돌아 검증도 못 한다. 실제로 이 문제를 먼저 만나고 바꿨다.
- **오버레이 캔버스는 게임뷰 캐프처에 안 찍힌다.** UI 배치를 눈으로 확인하려면 캔버스를 잠시 `ScreenSpaceCamera`로 바꿔야 한다. 이때 바꿔놓은 값은 플레이 종료 시 사라진다.
- **플레이 중에 고친 씬 값은 저장되지 않는다.** 패널 크기와 줄 간격을 플레이 중에 고쳐두고 잊어 한 번 날렸다. 씬 값은 반드시 에디터 상태에서 다시 적용하고 저장해야 한다.
- **버튼은 중심 기준이다.** 오른쪽 앵커에 `anchoredPosition.x = -12`를 주면 폭의 절반만큼 패널 밖으로 튀어나온다. 포함 폭을 계산해 -63/-177/-291/-405로 배치했다.
- 검증: 슬롯 10줄 표시, 저장 버튼으로 수동 저장, 3초 주기 자동저장이 자동 칸에만 기록(수동 파일 불변), 불러오기 버튼으로 인벤토리와 위치 복원 및 화면 자동 닫힘, 삭제 버튼으로 수동+자동 둘 다 제거, 빈 슬롯 버튼 비활성. `consoleErrors: 0`.

### M8-UI 착수 — 팰월드풍 UI 리뉴얼 (2026-09-20)

- **조사 결과 하나가 계획을 바꿨다.** 팰월드 기본 인벤토리는 격자가 아니라 세로 목록이다(격자는 Better Inventory UI 모드가 넣는 기능). 그래서 줄 목록 구조를 버리고 격자로 갈 필요가 없고, 줄의 외형과 창 구성만 바꾸면 된다.
- **사용자가 한 창 + 탭 통합을 택했다.** `CLAUDE.md` §11.1의 "인벤토리, 장비, 상태 화면은 서로 분리합니다"와 정면으로 충돌하므로, 2단계에서 규칙 문서를 갱신한다. 코드보다 문서가 오래된 상태로 두지 않는다(§11.5).
- **아이콘이 UI의 전제다.** 13종 전부 `icon`이 비어 있어서 아이콘부터 만든다. 7종은 `worldPrefab`, 3종은 무기 프리팹이 있고, 방어구 3종은 모델이 아예 없어 색 타일로 간다.
- **아이콘 생성기는 `Assets/Editor/`에 둔다.** 런타임 어셈블리에 `UnityEditor`가 섞이면 빌드가 깨진다.

### M8-UI 1단계 — 테마와 아이콘 (2026-09-20)

- **아이콘은 `AssetPreview.GetAssetPreview`로 뽑는다.** URP에서 직접 카메라를 돌려 렌더하는 것보다 안전하다. 다만 **첫 호출은 항상 null**을 주고 내부적으로 굽기 시작하므로, 여러 번 나눠 호출해야 한다. MCP eval은 메인 스레드 5초 제한이 있어 `maxCount`로 한 번에 4~5개씩 끊어 돌렸다.
- **미리보기 배경은 투명이 아니라 회색이다.** 전역 색 키로 지우면 물체 안의 비슷한 회색까지 뚫린다. 테두리에서 시작하는 채우기(flood fill)로 배경에 연결된 영역만 지운다. 허용 오차 24는 나무 상자의 밝은 윗면까지 먹어서 10으로 낮췄다.
- **미리보기는 조명이 약해 어둡다.** 어두운 패널 위에서 안 보여서 감마 0.62로 들어올린다.
- **생성기는 `Assets/Editor/`에 둔다.** `UnityEditor`를 참조하므로 런타임 어셈블리에 있으면 빌드가 깨진다.
- 모델이 없는 방어구 3종은 슬롯별 색의 둥근 타일로 대체했다. 나중에 실제 아이콘이 생기면 `ItemData.icon`만 바꾸면 된다.

### M8-UI 2단계 — 통합 창과 탭 (2026-09-20)

- **HUD Canvas는 프리팹 인스턴스라 씬에서 자식을 옮길 수 없다.** 이름 변경 같은 오버라이드는 되지만 재부모화는 조용히 실패한다(이름만 바뀌고 자리는 그대로였다). 그래서 `PrefabUtility.LoadPrefabContents`로 **프리팹 자체를 고쳤다**. 덕분에 `Prototype.unity`도 같은 UI를 받는다.
- **M7에서 씬에만 추가했던 저장 화면을 프리팹으로 옮겼다.** 씬 오버라이드(추가 오브젝트 2개 + 추가 컴포넌트 1개)를 지우고 `RevertPrefabInstance`로 인스턴스를 프리팹과 맞췄다. UI는 이제 한 곳에서만 관리한다.
- **탭은 `TabView`, 창은 `CharacterScreenUI`.** 여닫기·커서·단축키·탭 전환은 창이 갖고, 탭은 `Refresh()`로 자기 내용만 그린다. 기존 세 화면은 `ScreenPanel` -> `TabView`로 바꾸고 `panel` 대신 `root`를 본다.
- **`ScreenPanel`의 "한 번에 한 화면" 규칙은 그대로다.** 캐릭터 창을 열면 저장/건설/보관 화면이 닫히는 것을 확인했다.
- 검증: 탭 버튼 3개 생성, 클릭 전환, `I`/`O`/`K` 매핑, 장비 슬롯 6줄, 상태 수치 갱신, 저장 슬롯 10줄, 커서 잠금 복귀.

### M8-UI 3단계 — 팰월드풍 인벤토리 탭 (2026-09-20)

- **선택은 순번이 아니라 아이템 정의로 기억한다.** 사용/버리기를 하면 목록을 다시 그리면서 순번이 밀린다. `ItemData`로 들고 있으면 개수가 줄어도 같은 줄이 선택된 채로 남고, 다 떨어지면 자동으로 선택이 풀린다.
- **`Mask` + 알파 0 이미지로 스크롤 뷰포트를 만들면 내용이 통째로 사라진다.** 스텐실을 쓰는 `Mask`는 그래픽이 실제로 그려져야 하는데 알파가 0이라 아무것도 통과하지 못했다. `RectMask2D`로 바꾸니 바로 나왔다. 화면에는 "줄이 7개 있다"고 나오는데 눈에는 안 보이는 상태라, 스크린샷을 안 찍었으면 못 잡았을 문제다.
- **레이아웃 값은 다음 프레임에 확정된다.** `Refresh()` 직후 같은 프레임에 `rect.size`를 재면 0이 나온다. 검증은 한 프레임 뒤에 해야 한다.
- **막대는 스프라이트 없이 앵커로 채운다.** `Image.type = Filled`는 스프라이트가 있어야 동작한다. `anchorMax.x`를 비율로 두면 기본 사각형만으로도 막대가 된다.
- 무게 막대 색은 80%에서 노랑, 100%에서 빨강으로 바뀐다(`UiTheme.BarColorForFill`).
- 참고: `LivingEntity.RestoreHealth`는 상한을 두지 않아 붕대를 쓰면 체력이 최대치를 넘는다(60 -> 110). 교재 코드 그대로이고 이번 작업 범위가 아니라 두었다.

### M8-UI 4단계 — 나머지 화면 통일 (2026-09-20)

- **줄 프리팹 하나(`Item Row`)를 다섯 곳에서 재사용한다.** 인벤토리, 장비 슬롯, 장비 후보, 보관 상자 양쪽, 건설 메뉴가 같은 줄을 쓴다. `Count` 칸의 글자만 바꿔 역할을 표시한다(개수 / 해제 / 장착 / 넣기 / 꺼내기 / 건설). 새 목록이 필요하면 프리팹을 또 만들지 말고 이 줄을 쓸 것.
- **버튼 대신 줄 전체를 누르게 했다.** 팰월드도 줄을 선택해 동작한다. 줄 안에 작은 버튼을 두는 것보다 조준이 쉽고, 프리팹도 단순해진다.
- **건설물에도 아이콘이 필요했다.** `BuildableData.icon`을 추가하고 아이콘 생성기가 건설물 프리팹도 찍게 했다(`build_<id>.png`).
- **저장 슬롯 요약에서 시드를 뺐다.** 줄 폭(318px)보다 길어 글자가 잘렸다. 대신 저장 시각(MM-dd HH:mm)을 넣어 278px로 맞췄다. 폭은 `TextGenerator.GetPreferredWidth`로 실제 측정해 확인했다.
- 검증: 장착(방어 12)/해제, 상자 넣기(목재 20개)/꺼내기(붕대 2개)와 무게 갱신, 건설 메뉴의 재료 부족 표시(문), 저장 슬롯 10줄.

### M8-UI 5단계 — HUD (2026-09-20)

- **체력바는 플레이어 프리팹의 캔버스에 그대로 둔다.** `PlayerHealth.healthSlider`가 슬라이더를 직접 참조하고 값을 넣으므로, HUD Canvas로 옮기면 씬마다 참조를 다시 이어야 한다. 대신 슬라이더를 좌하단으로 옮기고 색만 테마에 맞췄고, 수치는 `HealthBarLabel`이 **슬라이더 값만 읽어** 겹쳐 그린다(PlayerHealth를 모르게).
- **우하단 `WeaponHudUI`**는 `PlayerShooter.gun`의 탄약과 장비 슬롯의 무기 이름/아이콘을 읽는다. 탄창이 비면 숫자가 빨갛게 변한다.
- **차량 HUD는 글자에서 막대로 바꿨다.** 연료/내구도가 25% 아래면 빨강, 50% 아래면 노랑이다(`UiTheme.BarColorForRemaining`).
- **상호작용 안내에 배경 배지를 붙였다.** 문구가 없을 때 빈 배지가 남지 않도록 `InteractionPromptUI`가 배경까지 껐다 켠다.
- `Prototype.unity`도 같은 HUD 프리팹을 쓰므로 참조가 살아 있는지 확인했다(8개 화면 전부 연결됨).

### 세션 마무리 정리 (2026-09-20)

- **체크리스트 편집 사고 한 건.** M8 3단계 항목을 줄 번호로 밀어넣다가 `### 3단계.`를 파일 앞쪽에서 먼저 찾는 바람에 **M3의 장비 시스템 섹션을 덮어썼다.** `git show a6694db:checklist.md`로 원본을 꺼내 복구했다. 이런 편집은 마일스톤 헤더(`## 2026-09-20 - M8-UI`)를 먼저 찾고 그 안에서만 검색해야 한다.
- 파이썬으로 한글을 유니코드 이스케이프로 넣다가 비슷한 글자(팹/팽, 뼈/뎼)를 여러 번 틀렸다. 앞으로는 이스케이프로 쓰지 말고 파일에 있는 올바른 표기를 복사하거나, `chr(0xAC00 + (초성*21+중성)*28 + 종성)`으로 합성해 확인할 것.
- 남은 미완료 항목 13개는 전부 **사용자 수동 확인**이 필요한 것들이다(입력 합성 불가). 코드로 검증 가능한 항목은 남아 있지 않다.


### 안정화 착수 - 체력 상한과 폐지 API (2026-09-20)

- RestoreHealth는 저장 복원용 SetHealth와 달리 상한을 적용하지 않아, 붕대 사용 시 체력이 최대치를 넘었다. 회복 경로에서만 startingHealth를 상한으로 제한한다.
- 기존 FindObjectOfType<T>() 다섯 곳은 모두 단일 씬 객체를 찾는 용도다. 기존 탐색 의미를 유지하는 FindFirstObjectByType<T>()로 치환한다.
- 검증: 강제 재컴파일은 실패 없이 완료했다. 임시 LivingEntity에서 60 + 50 회복은 100으로 제한되고, 사망 체력 0은 회복되지 않았다. Assets/Scripts의 FindObjectOfType<T>() 호출은 0개다. 외부 GrenadeSystem 예제의 camera 멤버 숨김 경고 1개만 남았다.

### M9 착수 - 게임 시작 흐름 (2026-09-21)

- 기존 `SaveSlotUI`는 플레이 씬의 `SaveManager`를 직접 사용하므로 타이틀에 재사용하지 않는다. 타이틀은 `SaveSystem.ListSlots()`로 저장 유무와 요약만 읽고, 선택한 슬롯 번호와 자동 저장 여부만 월드 씬으로 전달한다.
- 실제 데이터 복원은 기존 `SaveManager.Load` 경로를 그대로 쓴다. 타이틀이 플레이어·월드·건설물을 알게 하지 않아 저장 시스템의 도메인 경계를 유지한다.

### M9 진행 - 타이틀과 저장 선택 연결 (2026-09-21)

- `Title.unity`를 빌드 첫 씬으로 추가하고 `TitleMenuUI`가 런타임에 Canvas, 메뉴, 10개 슬롯 스크롤 목록을 만든다. 저장이 하나도 없으면 이어하기 버튼은 비활성이다.
- 타이틀은 `StartupLoadRequest`에 슬롯 번호와 자동 여부만 남긴 뒤 `World.unity`를 연다. `SaveManager.Start`는 한 프레임 뒤 이 요청을 소비해 기존 `Load`를 호출하므로 WorldStreamer의 초기화가 먼저 끝난다.
- World 첫 프레임에서 `IKHelperTool`이 이펙터 할당 전 실행되던 예외를 발견했다. `PlayerShooter.Awake`에서 먼저 IK를 끄고, 기존 무기 생성 경로에서만 다시 켜도록 고쳤다.
- 검증에서 새 게임은 Title -> World로 전환됐고 이어하기는 슬롯 10개를 만들었다. 이 컴퓨터에는 기존 저장이 없어 수동/자동 실제 복원은 검증하지 않았다.
- World 전환 후 RuntimeNavMeshBuilder가 읽기 불가 메시(`SM_Grounds_02_LOD0`, 도로, 건물)를 오류로 기록한다. 에디터에서는 동작해도 플레이어 빌드에서 실패할 수 있는 기존 월드 에셋 설정 문제라 M9 커밋 전에 별도 해결이 필요하다.

### M9 안정화 착수 - 월드 런타임 NavMesh 메시 읽기 (2026-09-21)

- `WorldNavMeshBaker`는 `PhysicsColliders`로 수집한다. RuntimeNavMeshBuilder 오류는 베이커 알고리즘이 아니라 수집된 모델 메시의 Read/Write 비활성화에서 발생한다.
- 오류에 실제로 나온 모델 임포터만 `isReadable`로 바꾼다. 모든 모델을 일괄 변경해 메모리를 불필요하게 늘리지 않는다.

### M9 안정화 완료 - 월드 런타임 NavMesh 메시 읽기 (2026-09-21)

- Read/Write Enabled를 켠 모델은 네 개다. `SM_Grounds_02.FBX`, `SM_Roads_01_Straight.FBX`, `SM_Building_Low_03.FBX`, `SM_Building_Broken_01.FBX`.
- Title에서 새 게임으로 World를 열어 재검증했다. RuntimeNavMeshBuilder 읽기 오류는 0개이고, NavMesh는 수집원 167개·정점 1885개·삼각형 833개로 생성됐다.
- Read/Write는 런타임 메모리 비용이 있으므로, 새 월드 청크 모델을 NavMesh 수집 대상으로 추가하면 같은 오류가 다시 나타나는지 확인하고 필요한 모델만 켠다.

### M10 착수 - 루팅 테이블 (2026-09-21)

- 사용자는 M9의 저장 불러오기 수동 검증을 나중으로 미루고 루팅 테이블을 먼저 진행하라고 했다.
- 월드 상자는 `WorldRuntimeState`에 열린 ID만 남긴다. 따라서 상자 ID별로 독립된 결정론적 난수를 써서 내용을 만들면, 저장 파일에 아이템 목록을 중복 저장하지 않아도 재생성 결과와 열린 상태가 일치한다.
- 청크 유형이 현재 월드 콘텐츠 성격을 표현하는 가장 작은 경계다. 이번 단계는 유형별 테이블 하나를 `ChunkContentRule`에 연결하고, POI는 POI 유형 테이블로 취급한다.

### M10 완료 - 유형별 결정론적 루팅 테이블 (2026-09-21)

- `LootTableData`는 `rolls` 횟수만큼 유효한 항목의 가중치 합에서 하나를 뽑고, 동일 아이템이 여러 번 뽑히면 한 `LootEntry`로 합친다. `ItemData` 참조만 보관하므로 정적 정의 데이터 경계를 넘지 않는다.
- 상자 내용물용 난수는 청크 시드와 상자 순번만 해싱해 만든다. 자원/상자 배치가 지형 충돌로 재시도해 공유 난수를 더 소비해도 이미 배치된 같은 순번 상자의 내용물은 바뀌지 않는다.
- 초원은 목재·돌 위주 1롤, 도로는 고철·탄약·회복품 위주 1롤, 도시는 2롤, 산은 돌 위주 1롤, POI는 탄약·회복품·장비를 포함한 3롤로 설정했다. 무기와 방탄조끼는 POI에만 낮은 가중치로 넣었다.
- 에디터에서 청크 유형 다섯 개 모두 테이블 참조가 있는지 확인하고, 같은 청크 시드와 순번으로 두 번 롤링해 결과가 완전히 같은지 확인했다. 컴파일 오류와 콘솔 오류는 0개다. 실제 플레이어가 World 상자를 열어 인벤토리로 받는 수동 검증은 사용자 요청에 따라 보류한다.

### M11 착수 - 전투 콘텐츠 확장 1단계 (2026-09-21)

- 사용자는 총기 확대와 활·석궁·근접무기 추가를 요청했고, `WeaponsPack (LowPoly)`도 참고하라고 했다.
- 실제 설치 자산을 확인했다. LowPoly 팩은 총기 프리팹 다수를, Free medieval weapons는 활·화살·검·도끼를, Crusader_Castle은 활·석궁·볼트·검·도끼·망치·철퇴·창을 제공한다.
- 무기 전부를 만들면 검증 불가능한 대형 변경이 된다. 이번 단계는 SMG, 활, 석궁, 한손검 하나씩으로 사격·투사체·근접의 세 방식을 완성하고, 이후 무기는 같은 데이터/프리팹 경로를 재사용한다.
- 컴파일 중 Git에서 무시되는 `Assets/ExplosiveLLC` 외부 패키지가 전역 `PlayerInput` 구조체와 Unity 6.3에서 허용되지 않는 `[SerializeField] public struct` 선언을 가져온 것을 확인했다. 프로젝트 쪽 입력 컴포넌트는 `ZombiePlayerInput`으로 바꿔 이름 충돌을 피한다. 외부 패키지 호환 수정은 사용자 파일을 건드리므로 별도 승인 뒤에만 한다.

### M11 완료 - 전투 콘텐츠 확장 1단계 (2026-09-21)

- `EquippedWeapon` 계약으로 기존 `Gun`, 새 `ProjectileWeapon`, 새 `MeleeWeapon`을 묶고, `PlayerShooter`와 HUD가 이 계약만 사용하도록 전환했다. 기존 총기 아이템·프리팹 구조는 유지했다.
- LowPoly `American Light SMG`, Crusader Castle `Bow_01`, `Crossbow_01`, `Shortsword_01`, `Arrow_01`, `Crossbow_Bolt_01`을 장착용 래퍼 프리팹으로 만들었다. 활과 석궁은 실물 투사체와 각 탄약을 소모하고, 검은 전방 부채꼴의 가장 가까운 `LivingEntity` 하나만 공격한다.
- `Save Registry`는 아이템 19개와 중복 ID 0개, POI 루팅 테이블은 13개 엔트리로 검증했다. World Play Mode에서 SMG·활·석궁·한손검이 각각 `Gun`·`ProjectileWeapon`·`ProjectileWeapon`·`MeleeWeapon`으로 실제 장착되었고, 활 재장전 뒤 화살 투사체 1개 생성도 확인했다.
- 새 Archer·Crossbow·Swordsman 애니메이션 컨트롤러는 현재 Survivalist 플레이어 리그 및 파라미터 구조와 달라 이번 단계에서 직접 교체하지 않았다. 다음 애니메이터 레이어/리타게팅 단계에서 활 당김, 석궁 장전, 검 휘두르기 클립을 연결한다.
- 사용자가 추가한 `Assets/ExplosiveLLC`의 패키지 간 전역 타입 충돌은 외부 에셋의 호환 보정으로 컴파일을 복구했다. 이 폴더는 사용자 추가분이므로 이번 전투 기능 커밋에는 포함하지 않는다.
