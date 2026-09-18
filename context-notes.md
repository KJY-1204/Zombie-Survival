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
