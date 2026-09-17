# context-notes.md

작업 결정과 인계 기록. 시간순으로 append.

## 2026-09-18 - M0 진단 (Claude)

### 확인한 사실

- Unity 에디터 버전 `6000.3.23f1` (`ProjectSettings/ProjectVersion.txt`).
- 렌더 파이프라인은 URP 커스텀 파이프라인이 `GraphicsSettings.asset`에 할당되어 있음.
- `ProjectSettings.asset`의 `activeInputHandler: 2` (레거시 Input Manager + 신규 Input System 병행). 단, 현재 코드(`PlayerInput.cs`)는 레거시 `Input.GetAxis`/`Input.GetButton`만 사용 중이며 신규 Input System 패키지(`com.unity.inputsystem 1.20.0`)는 아직 코드에서 쓰이지 않음.
- `com.unity.cinemachine 3.1.7`, `com.unity.ai.navigation 2.0.14`가 패키지에 포함되어 있어 카메라 전략 교체와 NavMesh AI 확장에 바로 사용 가능.
- `Assets/Scripts`의 14개 스크립트는 교재 PART 06 탑다운 좀비 서바이버 예제와 동일한 구조. 카메라 독립 코어, 인벤토리/장비/상태 UI, 저장, 오토바이, 시드 월드 관련 코드는 전혀 없음.
- 대형 서드파티 에셋 폴더(`Apocalyptic_World`, `Survivalist`, `Zombie`, `WeaponsPack (LowPoly)` 등)는 `.gitignore`에 등록되어 로컬에만 존재하고 git에는 커밋되지 않음. 코드/설정만 git으로 관리됨.
- 최소 플레이 씬은 `Assets/Scenes/Main.unity`로 확인됨 (교재 데모 씬).

### 발견한 이슈

- `Assets/Scripts/Zombie.cs:55` `Setup()` 메서드에서 `health = zombieData.damage;`로 되어 있어 좀비 체력이 `damage` 값으로 잘못 설정됨. 원래 의도는 `health = zombieData.health;`로 추정. 아직 수정하지 않음 (다음 작업에서 수정 예정).
- `checklist.md`, `context-notes.md`가 저장소에 없었음 -> 이번에 생성.

### 다음 작업 순서 (사용자 지시)

1. `checklist.md`/`context-notes.md` 생성 (완료).
2. M1 수직 슬라이스 상태를 Unity 에디터에서 실제로 확인 (컴파일/콘솔 에러/Play 모드) (완료).
3. `Zombie.Setup()` 버그 수정 + 카메라 독립 구조(`FirstPerson`/`ThirdPerson` 전략) 설계 착수 (완료).

### 결정 사항

- 카메라 분리는 GAME_DESIGN.md 5.2절 원칙(이동/전투/인벤토리/상호작용은 카메라 모드와 분리, `FirstPerson`/`ThirdPerson`을 교체 가능한 전략/프리팹으로 구성, 조준 입력은 공용 `AimContext`로 전달)을 따른다.

## 2026-09-18 - Unity 에디터 실제 검증 및 컴파일 오류 수정 (Claude)

### 발견한 문제

- `mcp__unity-editor-mcp__recompile` 실행 결과 프로젝트가 이미 컴파일 오류 상태였음. 원인은 `Assets/Survivalist/StarterAssets/Editor/StarterAssetsDeployMenu.cs`가 `using Cinemachine;`(구 2.x 네임스페이스)를 사용하는데, 실제 설치된 패키지는 `com.unity.cinemachine 3.1.7`이라 타입이 `Unity.Cinemachine` 네임스페이스로 이동했기 때문.
- 이 `using` 구문은 `STARTER_ASSETS_PACKAGES_CHECKED` 스크립팅 정의로 가드되어 있었고, 같은 이름의 `[InitializeOnLoadMethod]`(`PackageChecker.CheckPackage()`)가 도메인 리로드마다 이 정의가 없으면 자동으로 다시 설정함. 그래서 `ProjectSettings.asset`에서 정의만 지워도 다음 리로드 때 되돌아가 컴파일이 다시 깨짐 (실제로 재현됨).
- 근본 수정은 `StarterAssetsDeployMenu.cs`의 `using Cinemachine;`을 `using Unity.Cinemachine;`으로 변경. 해당 파일이 참조하는 `CinemachineBrain`, `CinemachineVirtualCamera`는 `Unity.Cinemachine` 네임스페이스에 Deprecated 상태로 남아있어 타입 자체는 존재함 (`Library/PackageCache/com.unity.cinemachine@.../Runtime/Deprecated/CinemachineVirtualCamera.cs`, `Runtime/Behaviours/CinemachineBrain.cs` 확인).
- 씬(`Assets/Scenes/Main.unity`)의 `Follow Cam` 오브젝트가 사용 중인 `CinemachineVirtualCamera` 컴포넌트도 같은 이유로 런타임에는 정상 동작함(Deprecated 타입이 여전히 존재하므로). 다만 이 타입은 언젠가 최신 `CinemachineCamera`로 마이그레이션이 필요할 수 있음 (지금 당장 급하지 않음, 폴리시 단계에서 재검토).
- 수정 후 `recompile_status`로 컴파일 성공 확인, Play 모드 진입 후 콘솔 에러/경고 0건, 게임 뷰 캡처로 플레이어/좀비/이펙트가 정상 렌더링됨을 확인.

### 결정 사항

- `ProjectSettings.asset`의 스크립팅 정의 변경은 Unity 에디터 자체의 `PlayerSettings` API(`UnityEditor.PlayerSettings.SetScriptingDefineSymbols`)를 통해 적용함 (파일을 텍스트로 직접 수정하는 대신, 에디터가 살아있는 상태에서 API로 반영하고 재컴파일까지 확인). 이후 에디터가 이 파일을 다시 직렬화하면서 작업 중 외부 diff가 감지되었는데, 이는 에디터의 정상 동작이므로 되돌리지 않음.
- `Zombie.cs`의 `health = zombieData.damage` 오탈자를 `health = zombieData.health`로 수정.

## 2026-09-18 - 1인칭/3인칭 카메라 분리 구조 추가 (Claude)

### 구현 내용

- `Assets/Scripts/Camera/CameraRigController.cs` 신규 추가. `ThirdPersonCamera`/`FirstPersonCamera` 두 개의 가상 카메라 GameObject 참조를 받아 `SetActive`로 하나만 활성화하는 방식으로 전환한다 (Cinemachine 3.x의 `PrioritySettings` 구조체를 직접 다루지 않고 활성/비활성으로 단순화).
- 씬에 `Player Character` 자식으로 `FirstPerson Cam` GameObject를 추가하고 `CinemachineCamera`(신규 API, 3.x) 컴포넌트를 부착. 로컬 위치 `(0, 1.5, 0.15)`는 `CapsuleCollider` 높이(1.5, center y=0.75)를 기준으로 머리 근처 눈높이로 추정한 값. 별도의 Body/Aim 컴포넌트 없이 플레이어 트랜스폼에 그대로 종속되어 회전한다 (탱크 컨트롤 회전 방식과 자연스럽게 맞음).
- 기존 `Follow Cam`(`CinemachineVirtualCamera`, 탑다운에 가까운 고정 오프셋 추적 카메라)을 `ThirdPersonCamera`로 그대로 재사용.
- `CameraRigController`는 `Main Camera`(기존 `CinemachineBrain` 보유 오브젝트)에 부착. 기본 키는 `C`로 전환하도록 구현했지만, 실제 전환 검증은 Play 모드에서 `eval`로 `SetMode()`를 직접 호출해 확인함 (자동화 도구가 실제 키 입력 시뮬레이션을 지원하지 않음 - 사람이 플레이할 때 `C` 키로 실제 작동하는지는 아직 수동 확인 필요).

### 확인한 사실

- Play 모드에서 `SetMode(FirstPerson)` 호출 시 게임 뷰가 즉시 1인칭 눈높이 시점으로 전환되고, `SetMode(ThirdPerson)` 호출 시 원래의 탑다운형 추적 시점으로 복귀함을 캡처로 확인. 전환 중 콘솔 에러/경고 0건.
- 1인칭 모드에서 캐릭터 본체(스킨드 메시)를 가리는 처리는 하지 않았음 - 카메라가 눈높이에서 앞을 보므로 현재는 크게 문제되지 않지만, 무기 모델이 1인칭 시점에서 어떻게 보이는지는 아직 확인하지 않음 (GAME_DESIGN.md 5.2절이 명시한 "1인칭에서는 무기 모델과 손 애니메이션이 별도 구성이 필요한지 프로토타입에서 확인" 항목은 다음 작업으로 남김).

### 의도적으로 하지 않은 것 (범위 최소화)

- `AimContext` 같은 공용 조준 입력 추상화는 아직 만들지 않음. 현재 `PlayerMovement`는 카메라와 무관하게 탱크 컨트롤(전후진 + 제자리 회전)로 동작하고 있어 카메라 모드 전환이 이동 로직에 영향을 주지 않는다. 카메라 시점별로 실제 조작 방식(마우스 시점 회전, 전략적 조준 등)을 다르게 가져가기로 결정하기 전까지는 `AimContext` 도입을 보류한다.
- `PlayerMovement`/`Gun`/`Zombie` 등 게임플레이 스크립트는 전혀 수정하지 않았다 (버그 수정 1건 제외). 카메라 시스템은 완전히 별도 파일/컴포넌트로 추가되어 GAME_DESIGN.md 5.2절의 "이동/전투/인벤토리/상호작용은 카메라 모드와 분리" 원칙을 그대로 만족한다.

### 다음 작업 후보

- 사람이 직접 Play 모드에서 `C` 키를 눌러 전환되는지 확인 (자동화 도구로는 키 입력 시뮬레이션 불가).
- 1인칭 시점에서 총 모델/손 위치가 자연스러운지 확인하고, 필요하면 1인칭 전용 무기 마운트를 검토.
- `Gun`/`PlayerShooter`의 실제 발사/재장전 입력 테스트, `Zombie`의 추적/공격/사망 시퀀스 테스트 (이번 세션에서는 컴파일과 씬 배치만 확인함).

## 2026-09-18 - 1인칭 무기 구성 재검토 (Claude)

GAME_DESIGN.md 5.2절이 명시한 "1인칭에서는 무기 모델과 손 애니메이션이 별도 구성이 필요한지 프로토타입에서 확인"을 실제로 진행. 결론부터: **필요하다는 것이 확인됨.** 기존 3인칭 IK 기반 총 리그를 그대로 1인칭에 재사용하는 방식은 문제가 많다.

### 구현 내용

- `PlayerShooter.cs`에 `firstPersonWeaponMount`(Transform)와 `useFirstPersonMount`(bool) 필드 추가. `OnAnimatorIK()`에서 `useFirstPersonMount`가 true면 `gunPivot`을 팔꿈치 IK 힌트 대신 이 마운트의 위치/회전으로 맞춘다.
- `CameraRigController`가 카메라 모드 전환 시 `playerShooter.useFirstPersonMount`와 `bodyRenderer.enabled`(1인칭에서 몸통 숨김)를 함께 갱신하도록 확장.
- 씬에 `Player Character/FirstPerson Cam/FirstPersonWeaponMount` 빈 오브젝트 추가, 로컬 위치 `(0.15, -0.06, 0.4)`로 설정 (카메라 기준 우측 하단 전방).

### 발견한 문제 (원인 조사 포함)

1. **Animator 컬링으로 인한 IK 정지** - `Player Character`의 `Animator.cullingMode`가 기본값 `Cull Update Transforms`였음. 1인칭에서 몸통 `SkinnedMeshRenderer`(`Woman`)를 숨기자 Animator가 "화면에 안 보인다"고 판단해 트랜스폼/IK 갱신 자체를 멈춰버렸고, `OnAnimatorIK()`가 더 이상 호출되지 않아 총이 마지막 위치에 얼어붙었다. `AlwaysAnimate`로 변경해 해결 (씬에 저장됨).
2. **디버깅 중 거짓 단서** - 위 문제를 조사하는 동안 실제로는 플레이어가 좀비에게 계속 공격받아 사망한 상태(`PlayerHealth.dead = true`)였고, `PlayerHealth.Die()`가 `PlayerShooter.enabled = false`로 만들면서 `Gun` 오브젝트 전체가 비활성화됐던 것도 총이 안 보인 이유 중 하나였다. `OnAnimatorIK`는 컴포넌트가 비활성화되면 호출되지 않으므로 이 상태에서는 마운트 좌표를 아무리 바꿔도 반영되지 않았다. 테스트 시 좀비를 미리 제거/스포너 비활성화하지 않으면 이런 오탐이 재발할 수 있다.
3. **여전히 남은 문제 - 근접 프레이밍이 매우 예민함** - 위 두 문제를 모두 해결하고 `Gun`이 활성 상태, `useFirstPersonMount=true`, `GeometryUtility.TestPlanesAABB`로 카메라 프러스텀 안에 있음까지 코드로 확인했음에도, 실제 캡처한 화면에는 총이 뚜렷하게 보이지 않았다(빈 화면이거나 알아보기 힘든 흐릿한 얼룩). 카메라에서 0.3~1m 거리는 War FX/포스트 프로세싱 스택(비네트, 크로마틱 애버레이션, 블룸으로 추정)이 강하게 걸리는 구간이라 근접한 작은 오브젝트가 왜곡되거나 묻히는 것으로 보인다. 즉 좌표 계산이 맞아도 "눈으로 확인 가능한" 결과가 나오지 않았다.

### 결론 및 권장 사항

- 3인칭용 IK 총 리그(`Gun Pivot`이 팔꿈치를 따라가는 구조)를 그대로 가져와 1인칭 마운트에 스냅시키는 지금 방식은 근본적으로 근접 화면비/포스트 프로세싱과 맞지 않아 프로토타입 수준을 넘기 어렵다.
- 실제 FPS 게임들처럼 **1인칭 전용 무기 뷰모델을 별도로 두는 것**(카메라에 고정된 작은 전용 오브젝트, 가능하면 별도 카메라 스택/레이어로 포스트 프로세싱 영향을 줄이거나 다르게 적용)을 다음 단계로 검토해야 한다.
- 이 작업은 이번 세션 범위를 벗어나는 별도 수직 슬라이스로 분리하는 것을 권장 (M1 범위를 벗어나 카메라/무기 렌더링 아키텍처를 다시 설계해야 함).
- 코드 인프라(`useFirstPersonMount`, `firstPersonWeaponMount`, 몸통 숨김)는 재사용 가능한 형태로 남겨두었으니, 별도 뷰모델을 붙일 때 이 스위치를 그대로 활용할 수 있다.

### 씬에 저장된 변경 사항

- `Player Character/FirstPerson Cam/FirstPersonWeaponMount` 위치 `(0.15, -0.06, 0.4)`.
- `Player Character`의 `Animator.cullingMode = AlwaysAnimate`.
- `PlayerShooter.firstPersonWeaponMount` -> 위 마운트, `CameraRigController.playerShooter`/`bodyRenderer` 참조 연결.
