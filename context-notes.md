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

## 2026-09-18 - 캐릭터 교체 + 무기 적용 + 자세 세팅 (Claude)

사용자 요청: "캐릭터를 바꾸고 무기들 에셋도 적용시킨다음 애니메이션 에셋을 참고 해서 자세같은걸 세팅". 실제 존재하는 에셋을 먼저 확인(`get_import_settings`로 avatar 타입 확인 등)한 뒤 진행. 사용자가 고른 옵션: 캐릭터 = Survivalist, 무기 = 권총 + 소총 2종, 자세 = 이동+전투 전부 한번에.

### 1. 캐릭터 교체 (Woman -> Survivalist)

- `Assets/Survivalist/Prefab/Survivalist (2).prefab`을 씬에 배치 후 `Player Character`의 자식으로 재배치. 이 프리팹은 자체 `Animator`(avatar만 있고 controller는 없음)와 7개의 `SkinnedMeshRenderer`(머리/바지/셔츠/조끼x2/장갑/모자 - 모듈식 장비 파츠)를 가짐.
- 중복 `Animator`를 피하기 위해 인스턴스의 `Animator` 컴포넌트는 제거하고, `Player Character`(원래 있던 루트 Animator, `ShooterAnimator.controller` 보유)의 `avatar` 필드만 Survivalist의 avatar로 교체. 기존 `Woman` 자식은 삭제.
- Survivalist가 기존 Woman보다 커서(약 1.92m vs 1.62m) `CapsuleCollider` 높이를 1.5->1.9, 중심을 0.75->0.95로, 1인칭 카메라(`FirstPerson Cam`) 로컬 Y를 1.5->1.78로 조정.
- `CameraRigController.bodyRenderer`(단일 `Renderer`)는 `bodyRoot`(`GameObject`)로 리팩터링해 Survivalist의 여러 렌더러를 한번에 켜고 끄도록 함(`SetActive`).
- 검증: Play 모드에서 사망 애니메이션이 새 스켈레톤에 정상 리타겟됨을 확인(`Animator.avatar.isHuman == true`), 좀비를 정리한 뒤 Aim Idle 자세로 서 있는 모습을 스크린샷으로 확인.

### 2. 애니메이션 재타겟팅 (Human Soldier Animations 2.0 FREE)

- `Assets/Kevin Iglesias/Human Animations`가 실제 존재함을 확인(Male/Female, Combat/Idles/Movement 카테고리). FBX 임포터의 `animationType`이 Woman/Survivalist/HumanM 전부 `Human`이라 Mecanim 제네릭 리타겟팅이 가능함을 먼저 확인.
- FBX 안의 `AnimationClip` 서브에셋은 `find_assets` 도구로는 검색되지 않음(메인 타입만 인덱싱하는 것으로 추정) — `AssetDatabase.LoadAllAssetsAtPath` + `OfType<AnimationClip>()`으로 직접 찾아야 했음. 클립 이름은 파일명과 동일(`HumanM@Rifle_Aim01` 등).
- `ShooterAnimator.controller`의 클립을 코드로 교체 (블렌드 트리/상태 자체는 그대로 두고 `motion`만 교체, 파라미터 `Move`/`Reload`/`Die` 이름도 그대로라 `PlayerMovement`/`PlayerShooter`/`PlayerHealth`는 전혀 수정하지 않음):
  - `Movement Tree`(param `Move`, 5분기): -1/1 -> `HumanM@Run01_Forward`, -0.5/0.5 -> `HumanM@Walk01_Forward`, 0 -> `HumanM@MilitaryIdle01`. (기존에도 음수/양수 구간에 같은 클립을 재사용하던 패턴을 그대로 유지 - 후진 시 방향별 클립을 쓰지 않음)
  - Base layer `Die` -> `HumanM@Death01`.
  - Upper Body layer `Aim Idle` -> `HumanM@Rifle_Aim01`, `Reload` -> `HumanM@Rifle_Reload01` (장착 무기가 소총이므로 Rifle 카테고리 선택. 권총 장비 시 `HumanM@Gun_Aim01`/`Gun_Reload01`로 바꾸면 됨).
- IK Helper Tool(Kevin Iglesias)도 존재를 확인했으나 이번엔 도입하지 않음: 이 도구는 `OnAnimatorIK`를 직접 구현해서 손마다 별도 컴포넌트가 필요하고 `iKSwitch`로 가중치를 애니메이션 커브로 페이드하는 방식이라, 기존 `PlayerShooter`의 단순 IK 코드와 같은 `AvatarIKGoal`을 동시에 건드리면 충돌한다. 기존 코드가 이미 정상 작동하므로 교체하지 않고 그대로 둠 - 필요해지면 `PlayerShooter`의 IK를 걷어내고 IK Helper Tool로 전환하는 별도 작업으로 분리해야 함.

### 3. 무기 교체 및 추가 (WeaponsPack (LowPoly))

- 기존 placeholder Uzi를 `Assets/WeaponsPack (LowPoly)/FBXs/American Light AssaultRifle.fbx`로 교체. 이 에셋들은 `.prefab`이 아니라 FBX라서 `instantiate_prefab` MCP 도구가 거부함(`is not a prefab asset`) - `PrefabUtility.InstantiatePrefab`를 `eval`로 직접 호출해 우회.
- FBX 원본은 로컬 X축을 따라 길게 뻗어 있어(기존 Uzi 구조는 로컬 Z축이 총열 방향) 그대로 넣으면 총이 옆으로 누움. 모델 자식에 Y축 +90도 회전을 줘서 Z축 방향으로 맞춤. 회전 후 실제 바운즈를 다시 측정해 `Fire Position`/`Left Handle`/`Right Handle`을 새 좌표에 맞게 재배치 (원점=총 뒤쪽/그립 부근, +Z가 총구 방향이 되는 패턴을 세 무기 모두에 동일하게 적용).
- 기존 `Gun Data.asset`(Uzi 스탯)을 `Assault Rifle Data.asset`으로 이름 변경(GUID 유지되어 `Gun.gunData` 참조 안 끊김)하고 값만 소총에 맞게 조정(damage 25->30, ammo 100->120, mag 25->30, fireRate 0.12->0.10, reload 1.8->2.0).
- 같은 패턴으로 `Pistol Gun.prefab`/`Sniper Gun.prefab`(`Assets/Prefabs/Weapons/`)과 각각의 GunData(`Pistol Data.asset`: 데미지 낮고 연사 빠름, `Sniper Data.asset`: 데미지 높고 연사 느림/재장전 김)를 준비. **이 두 무기는 아직 플레이어에게 장착되어 있지 않음** - 인벤토리/장비 교체 시스템(M3)이 없어서 장착할 방법이 없기 때문. 씬 밖에서 임시로 조립한 뒤 `create_prefab`으로 저장하고 씬에서는 삭제했다.
- 검증: 소총으로 교체된 상태에서 Play 모드로 들어가 Aim 자세를 취하고 있는 캐릭터가 총을 정상적으로 쥐고 있는 모습을 스크린샷으로 확인, 콘솔 에러 없음. Pistol/Sniper 두 프리팹은 좌표만 비슷한 패턴으로 추정 배치했고 실제 화면으로 개별 검증하지는 못했다 (시간 제약).

### 남은 위험 / 다음 작업

- Pistol Gun / Sniper Gun 프리팹의 Fire Position/Handle 좌표는 추정치라 실제 장착해서 눈으로 확인 필요.
- Walk/Run 블렌드 애니메이션이 실제로 자연스러운지 확인 안 됨 (Idle/Aim/Die만 캡처로 확인).
- 무기별로 다른 Aim/Reload 포즈(Rifle vs Gun 카테고리)를 자동으로 바꿔주는 로직은 아직 없음 - 인벤토리/장비 시스템을 만들 때 함께 설계해야 함.
- IK Helper Tool 도입은 보류.

## 2026-09-18 - 1인칭 테스트 씬 생성 + 카메라 스택 검증 결과 정정 (Claude)

사용자가 "1인칭 테스트 맵을 만들어줘 잘되어있는지 확인하게"라고 요청. `Assets/Scenes/FirstPersonTest.unity` 신규 생성.

### 씬 구성 방법

- Main.unity를 건드리지 않기 위해 additive로 새 씬을 열고, Main에서 `Player Character`/`Main Camera`/`Follow Cam`을 임시 부모(`TestPlayerRigRoot`) 밑에 모아 `Assets/Prefabs/PlayerTestRig.prefab`으로 저장. 이렇게 프리팹으로 묶어야 세 오브젝트 사이의 컴포넌트 참조(예: `CameraRigController`가 `Follow Cam`/`FirstPerson Cam`을 가리키는 것)가 인스턴스화 시 올바르게 리매핑된다. 그냥 각각 따로 `Instantiate`하면 참조가 원본(Main 씬)을 계속 가리켜서 못 쓰게 됨.
- 프리팹을 새 씬에 인스턴스화한 뒤, `Player Character`만 스폰 지점(0, 0.05, -2)으로 재배치 (카메라들은 `Follow`/`LookAt` 참조로 자동 추적하므로 별도 이동 불필요).
- `EventSystem`/`HUD Canvas`/`Game Manager`는 서로 다른 씬 간 참조가 없는 독립 오브젝트라 그냥 `Instantiate`로 복제.
- Main.unity는 이 작업 동안 저장하지 않았고(`save_scene`을 Main 경로로 호출하지 않음), 마지막에 `open_scene(FirstPersonTest, additive:false)`로 전체를 언로드해 임시 편집 내용이 디스크에 전혀 반영되지 않도록 함 (`git status`로 Main.unity 무변경 확인).
- 테스트용 오브젝트: 바닥 Plane, 방향광, 2m/5m/10m 거리 마커 큐브, 실제 캐릭터 키(1.9m) 비교용 기둥, 사격 연습용 캡슐 타겟 2개(`Target A`/`Target B`, IDamageable은 구현하지 않아 데미지는 안 들어가고 총알 궤적/명중 위치 확인용). 좀비/스포너는 넣지 않음 - 지난 세션에서 방치된 플레이어가 좀비에게 죽어 테스트를 방해한 적이 있어서, 이번 맵은 순수하게 카메라/이동/무기 확인용으로 좀비 위협 없이 구성.

### 중요: 이전 세션의 "카메라 스택 검증 불가" 결론을 정정함

- 이전 세션(1인칭 무기 뷰모델 작업)에서는 `capture_game_view`가 `Camera.Render()`를 직접 호출하는 방식이라 URP Base+Overlay 카메라 스택 합성이 캡처에 반영되지 않는다고 결론 내렸었음.
- 이번 테스트 씬에서 Play 모드로 들어가 이런저런 확인 작업을 좀 진행한 뒤(즉 몇 프레임 이상 지난 시점에) 같은 `capture_game_view`(source 기본값 `camera`)로 1인칭 모드를 캡처했더니 **총이 포스트 프로세싱 왜곡 없이 정상적으로 화면에 나타남**을 확인했다. 이전 실패는 카메라 스택 자체의 문제가 아니라, 모드 전환 직후 너무 빨리 캡처해서(또는 에디터가 포커스를 잃어 프레임이 거의 진행되지 않아서) 스택이 아직 합성되지 않은 상태를 캡처했던 것으로 보인다.
- 결론: **1인칭 전용 무기 카메라 스택(Base+Overlay)은 정상 작동한다.** 이전 context-notes의 "자동화 도구로 검증 불가"는 정정. 다만 여전히 모드 전환 직후 1~2프레임 이내의 상태는 캡처가 불안정할 수 있으니, 검증 시 전환 후 약간의 프레임이 지난 뒤 캡처하는 것을 권장.

### 남은 사람 확인 사항

- 이 문서 작업은 Unity 에디터에서 `Assets/Scenes/FirstPersonTest.unity`를 열고 Play를 누르면 바로 테스트 가능. WASD류 이동은 없고 기존 탑다운 이동(전후진+제자리 회전)과 `C` 키로 1인칭/3인칭 전환.
- Build Settings에 이 씬을 추가하려 시도했으나(`add_scene_to_build`) `ProjectSettings/EditorBuildSettings.asset` 파일에 실제로 반영되지 않는 것을 확인함 (에디터 메모리상에는 등록되지만 디스크 저장이 안 됨) - 급하지 않아 더 파고들지 않음. 필요하면 Unity 에디터에서 File > Build Settings로 직접 추가할 것. → 이후 다른 작업(Play 진입 등) 중에 자동으로 디스크에 반영된 것을 확인함(에디터가 지연 저장하는 것으로 보임).

## 2026-09-18 - 1인칭 시점을 실제 FPS 방식(마우스룩)으로 개선 (Claude)

사용자 요청: "1인칭 시점을 인터넷에 검색해서 총게임 1인칭 시점으로 고쳐봐". 웹 검색으로 Unity FPS 카메라의 표준 패턴을 확인: **마우스 X로 몸통(또는 카메라 부모) Yaw 회전 + 마우스 Y로 카메라만 Pitch 회전(각도 클램프) + 커서 잠금/숨김**. (참고: Unity Discussions "FPS MouseScript", 구 Standard Assets `MouseLook.cs`)

### 문제였던 것

기존 1인칭 카메라(`FirstPerson Cam`)는 `Player Character`의 자식으로 붙어서 몸통 회전을 그대로 따라가기만 했음. 몸통 회전은 `PlayerMovement.Rotate()`가 키보드 `Horizontal` 축으로만 처리해서, 마우스를 움직여도 시점이 전혀 돌아가지 않았다 - 사실상 "1인칭 카메라가 달린 탑다운 조작"이었지 FPS 시점이 아니었음.

### 구현

- `PlayerMovement.cs`: `useMouseLook`(bool), `mouseYawSpeed`(float) 필드 추가. `Rotate()`에서 `useMouseLook`이 true면 `Input.GetAxis("Mouse X") * mouseYawSpeed`를, false면 기존 키보드 기반 회전량을 사용 - 리지드바디에 적용하는 나머지 로직은 동일해서 두 모드가 서로 다른 회전 "소스"만 갖도록 최소 침습적으로 수정.
- `Assets/Scripts/Camera/FirstPersonLook.cs`(신규): `FirstPerson Cam`에 부착. 마우스 Y로 `pitch`를 누적하고 -80~80도로 클램프한 뒤 로컬 회전에 반영. `OnEnable()`에서 pitch를 0으로 초기화해 1인칭 진입 시 항상 정면을 보게 함.
- `CameraRigController.SetMode()`: 모드 전환 시 `playerMovement.useMouseLook`, `firstPersonLook.enabled`, `Cursor.lockState`/`Cursor.visible`을 함께 전환 (1인칭 = Locked + 숨김, 3인칭 = None + 표시).
- `FirstPerson Cam`(Cinemachine Lens)과 `FirstPersonWeaponCamera`(오버레이 Camera) 시야각을 기존 40도에서 75도로 확대 - 40도는 상당히 좁아(줌인된 느낌) 일반적인 FPS 시야각(60~90도 사이)에 맞지 않았음.
- 총 조준 방향: `Gun Pivot`은 1인칭에서 `firstPersonWeaponMount`(FirstPerson Cam의 자식)의 월드 회전을 그대로 따라가도록 이미 구현되어 있었기 때문에, 카메라가 피치로 위아래를 보면 총도 자동으로 같은 방향을 향하게 된다 - 별도 수정 없이 "보는 곳을 쏜다"가 성립함.

### 검증의 한계

- `Input.GetAxis("Mouse X"/"Mouse Y")`는 실제 OS 마우스 이동을 읽기 때문에 자동화 도구로는 입력 자체를 시뮬레이션할 수 없음. Play 모드에서 컴포넌트 활성화 상태(`useMouseLook=True`, `firstPersonLook.enabled=True`, `Cursor.lockState=Locked`)와 FOV 값(75)이 올바르게 적용됐음은 코드로 확인했고, 스크린샷으로 시야각이 넓어진 것도 확인했다. **실제로 마우스를 움직였을 때 감도/반응이 자연스러운지는 사람이 직접 플레이해서 확인해야 함.** 감도가 안 맞으면 `FirstPersonLook.mouseSensitivity`(카메라 상하)와 `PlayerMovement.mouseYawSpeed`(몸통 좌우)를 조정.
- `FirstPersonTest.unity`용 `PlayerTestRig.prefab`도 Main.unity와 동일하게 갱신했다 (그룹핑 후 프리팹 덮어쓰기 방식, 작업 후 Main 씬 계층은 메모리상에서도 즉시 원상 복구해 실수로 저장되는 일이 없도록 함).

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

## 2026-09-18 - 실제 FPS 방식 1인칭 전용 뷰모델 카메라 스택 설계 (Claude)

앞선 세션에서 확인한 "근접 포스트 프로세싱 왜곡" 문제를 해결하기 위해, 총을 메인 카메라와 별도로 그리는 URP 카메라 스택(Base + Overlay) 구조로 전환.

### 구현 내용

- 새 레이어 `FirstPersonWeapon`(인덱스 8) 추가.
- `Main Camera`의 자식으로 `FirstPersonWeaponCamera` 생성. `Camera` + `UniversalAdditionalCameraData` 컴포넌트를 붙이고 다음과 같이 설정:
  - `Camera`: `clearFlags = Depth only`, `cullingMask = FirstPersonWeapon 레이어만`, `nearClipPlane = 0.01`, `farClipPlane = 10`, `fieldOfView = 40`.
  - `UniversalAdditionalCameraData`: `renderType = Overlay`, `renderPostProcessing = false`(포스트 프로세싱 미적용 - 근접 왜곡 문제의 핵심 해결책), `renderShadows = false`.
  - 로컬 트랜스폼은 `Main Camera`에 대해 항등(0,0,0)이라 `CinemachineBrain`이 `Main Camera`를 옮길 때마다 자동으로 같이 따라간다.
- `Main Camera`의 `Camera.cullingMask`에서 `FirstPersonWeapon` 레이어를 제외 (`~(1<<8)`), `UniversalAdditionalCameraData.cameraStack`에 `FirstPersonWeaponCamera`를 추가.
- `CameraRigController`에 `weaponViewModelTarget`(Gun 오브젝트), `firstPersonWeaponLayer`/`defaultWeaponLayer` 필드 추가. 모드 전환 시 `Gun`과 모든 자식(메시/이펙트)의 레이어를 재귀적으로 `FirstPersonWeapon` <-> `Default`로 전환하는 `SetLayerRecursively()` 구현.
- 이렇게 하면 3인칭에서는 메인 카메라가 `Default` 레이어의 총을 그대로 그리고(기존과 동일), 1인칭에서는 메인 카메라가 그 총을 그리지 않는 대신 `FirstPersonWeaponCamera`가 `FirstPersonWeapon` 레이어로 옮겨간 같은 총을 포스트 프로세싱 없이 오버레이로 그린다. 총 오브젝트(`Gun.cs`, 탄약, 발사 로직, 이펙트)는 하나만 존재하며 복제하지 않았다.

### 검증 관련 중요한 발견 - 자동 스크린샷 도구의 한계

- `set_component_properties`로 `Camera.m_ClearFlags`에 유효 값으로 안내된 문자열 `"Depth only"`를 그대로 넣었는데도 실제로는 `"Don't Clear"`(Nothing)로 저장되는 버그를 발견함. `UnityEditor.EditorUtility.SetDirty` 후 `eval`로 `camera.clearFlags = CameraClearFlags.Depth`를 직접 대입하는 방식으로 우회함. (Claude Code에 제품 피드백으로 별도 보고함)
- 카메라 스택/레이어/프러스텀 설정을 `eval`로 전부 코드 레벨에서 검증(`GeometryUtility.TestPlanesAABB`, `WorldToViewportPoint`, 레이어/활성 상태 등)했고 전부 정상이었음에도, `capture_game_view` 도구(`source: "camera"`와 `source: "screen"` 둘 다)로 캡처한 화면에는 오버레이 카메라가 그리는 내용이 전혀 나타나지 않음. 오버레이 카메라의 `cullingMask`를 일부러 전체로 넓히고 배경을 빨간색 단색으로 바꿔도 캡처 결과에 전혀 반영되지 않아 재확인함.
- 콘솔 스택 트레이스를 보면 `capture_game_view`는 `source` 옵션과 무관하게 내부적으로 `UnityEngine.Camera:Render()`를 직접 호출해 캡처하는 것으로 보임. **URP는 카메라 스택(Base+Overlay) 합성을 `Camera.Render()` 수동 호출로는 처리하지 않고, 매 프레임 SRP의 `RenderPipeline.Render(context, cameras[])` 경로로만 처리**하는 것으로 알려져 있음. 즉 이번 세션에서 만든 카메라 스택 구조 자체는 (레이어/컬링마스크/스택 리스트/직렬화 필드 등 모든 설정이 코드로 확인된 것처럼) 정상일 가능성이 높지만, **이 자동화 도구로는 실제로 화면에 나오는지 최종 확인이 불가능**했다.

### 남은 작업 (사람의 확인 필요)

- **사람이 직접 Unity 에디터에서 Play를 눌러** 1인칭 모드로 전환했을 때 총이 `FirstPersonWeaponCamera`를 통해 실제로 화면에 나타나는지 육안으로 확인해야 함. 자동화 도구로는 이 부분을 검증할 수 없었다.
- 만약 실제로도 안 보인다면 다음을 의심할 것: (1) `FirstPersonWeaponMount` 위치/각도가 여전히 화면 밖일 가능성 (이번 좌표는 계산상으로만 확인됨), (2) URP 파이프라인 에셋 자체에서 카메라 스택을 막는 별도 설정, (3) Cinemachine 3.x와 카메라 스택 조합 관련 알려진 이슈.
- 잘 보인다면 다음 다듬기 후보: 뷰모델 전용 FOV/위치 미세 조정, 1인칭에서 총구 화염/탄피 이펙트가 자연스러운지 확인, 발사/재장전 시 실제 입력으로 테스트.
