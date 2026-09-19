# Prototype 씬 작업 체크리스트

- [x] 기존 작업 기록을 초기화하고 현재 Git 상태를 확인한다.
- [x] `Assets/Scenes/Prototype.unity` 작업 씬을 생성한다.
- [x] Unity Editor에서 새 씬의 기본 구성과 활성 상태를 확인한다.
- [x] 다음 기능 범위와 완료 조건을 정한다.

## 2026-09-18 - TPS 백뷰와 ADS

- [x] 기본 시점을 TPS 백뷰/숄더뷰로, 우클릭 시점을 ADS로 결정한다.
- [x] 완료 조건과 구현 순서를 `plan.md`에 기록한다.
- [x] TPS 카메라 리그와 ADS 전환 컴포넌트를 구현한다.
- [x] `Prototype`에 플레이어와 최소 테스트 공간을 구성한다.
- [x] Unity Play Mode에서 기본 숄더뷰 초기화와 콘솔 오류를 검증한다.
- [ ] Unity Editor에서 우클릭 ADS 전환과 이동·사격을 수동 확인한다.
- [x] 검증된 변경을 로컬 커밋한다.

## 2026-09-18 - Survivalist 애니메이션과 양손 IK

- [x] Survivalist Animator의 컨트롤러와 파라미터를 확인한다.
- [x] 이동·점프·지면 상태를 Survivalist Animator에 전달한다.
- [x] 총기 양손 IK를 Survivalist Animator에 적용한다.
- [x] Unity Play Mode에서 애니메이션·손 위치·콘솔을 검증한다.
- [x] 검증된 변경을 로컬 커밋하고 원격에 push한다.

## 2026-09-18 - IK Helper Tool 왼손 그립

- [x] IK Helper Tool 문서와 소총 예제 구성을 확인한다.
- [x] 직접 왼손 IK를 IK Helper Tool 컴포넌트로 교체한다.
- [x] 효과기와 스위치를 Survivalist 프리팹에 연결한다.
- [x] Unity Play Mode에서 양손 그립과 콘솔을 검증한다.
- [x] 검증된 변경을 로컬 커밋하고 원격에 push한다.

## 2026-09-18 - Claude 인수인계

- [x] 현재 TPS·Survivalist·총기 그립 상태와 다음 작업을 인수인계 메모에 기록한다.
- [x] 인수인계 메모를 로컬 커밋하고 원격에 push한다.

## 2026-09-18 - Survivalist 총기 피벗 보정

- [x] 오른손·총기 피벗·총구 위치를 확인한다.
- [x] 오른손 그립 기준 총기 피벗 계산으로 교체한다.
- [x] Unity Play Mode에서 총구 위치와 콘솔을 검증한다.
- [x] 검증된 변경을 로컬 커밋하고 원격에 push한다.
- [x] 원격 `main` 브랜치로 push한다.

## 2026-09-18 - TPS 스트레이프와 Survivalist 플레이어

- [x] 기획서와 실제 에셋을 대조해 `PlayerArmature.prefab`을 3인칭 플레이어 비주얼로 선택한다.
- [x] 수평 입력 기반 스트레이프 이동을 구현한다.
- [x] 임시 여성 메시를 Survivalist 비주얼로 교체한다.
- [x] Unity Play Mode에서 전후·좌우 이동과 비주얼을 검증한다.
- [x] 검증된 변경을 로컬 커밋한다.

## 2026-09-18 - 소총 조준 자세 (힙 파이어 -> ADS)

- [x] `SurvivalistTPS.controller`에 `Aiming` Bool 파라미터와 `Rifle Aim Blend` 상태(IK@RifleIdle/IK@RifleRun 블렌드)를 추가한다.
- [x] `Idle Walk Run Blend` <-> `Rifle Aim Blend` 전환과 조준 중 점프/낙하 전환을 연결한다.
- [x] `PlayerMovement`가 `ThirdPersonCameraController.isAiming`을 읽어 `Aiming` 파라미터를 갱신하도록 연결한다.
- [x] Play Mode에서 `PlayerMovement`를 일시 비활성화하고 `Aiming` 파라미터를 직접 토글해 총구·손 위치를 측정해 검증한다.
- [x] `recompile_status`/`console_status`에 신규 오류가 없음을 확인한다.
- [x] 검증된 변경을 로컬 커밋한다.
- [ ] Unity Editor에서 실제 마우스 우클릭 ADS로 육안 확인한다 (Pipeline은 마우스 입력 합성 불가, 수동 확인 필요).

## 2026-09-18 - 총기 파지 프리팹 재구성

- [x] `TPS Player.prefab`을 백업한다 (`TPS Player (Backup 0918).prefab`).
- [x] `Survivalist Visual`을 `PlayerArmature.prefab` 원본에서 새로 인스턴스화하고, 불필요한 StarterAssets 이동 컴포넌트를 제거한다.
- [x] `Human Soldier Animations` 에셋의 `Weapon Hold Arms`(Human Arms Mask, Override) 레이어를 `SurvivalistTPS.controller`에 추가해 항상 두 손으로 총을 쥔 자세를 적용한다.
- [x] Play Mode 스크린샷으로 그립을 검증하고, 오른손-그립/왼손-이펙터 거리가 모두 0.00000임을 확인한다.
- [x] 재구성된 인스턴스를 `TPS Player.prefab`에 저장한다.
- [x] `recompile_status`/`console_status`에 신규 오류가 없음을 확인한다.

## 2026-09-18 - 플레이어 캐릭터 프리팹 완전 재작성

- [x] 사용자가 기존 플레이어 프리팹(`TPS Player`, `TPS Player (Backup 0918)`, `Player Character`, `PlayerTestRig`, `Gun`)을 모두 삭제하고 처음부터 다시 만들 것을 요청.
- [x] 루트에 Rigidbody/CapsuleCollider/AudioSource/PlayerInput/PlayerMovement/PlayerHealth/PlayerShooter를 새로 구성한다(원본 초기 커밋의 캡슐 치수·리지드바디 제약을 그대로 사용).
- [x] 총은 삭제되지 않고 남아있던 `Assets/Prefabs/Weapons/Pistol Gun.prefab`(Left/Right Handle, Fire Position 기본 리깅 포함)을 재사용하고 `Pistol Data.asset`을 연결한다.
- [x] `Survivalist Visual`을 `PlayerArmature.prefab` 원본에서 다시 인스턴스화하고 `SurvivalistTPS.controller`(Weapon Hold Arms 레이어 포함) + `SurvivalistWeaponIK` + `IKHelperTool`을 연결한다.
- [x] 루트에 비활성 `Animator` 더미를 추가해 `PlayerHealth`/`PlayerMovement`의 `GetComponent<Animator>()` 참조가 예외를 던지지 않도록 한다.
- [x] 체력 슬라이더 UI(Canvas+Slider)를 새로 만들고 `PlayerHealth`에 연결한다(사망/피격/아이템 습득 사운드는 원본 초기 커밋과 동일한 클립 재사용).
- [x] Play Mode에서 오른손-그립/왼손-이펙터 거리 0.00000, 카메라 자동 타겟팅, 체력 슬라이더 렌더링을 스크린샷으로 확인한다.
- [x] `Assets/Prefabs/Player Character.prefab`로 저장하고 `recompile_status`/`console_status`에 신규 오류 없음을 확인한다.
- [x] 사용자 피드백("총을 대각선으로 들고 있다")에 따라 `Gun/Right Handle`의 로컬 회전을 재계산해 총이 항상 플레이어 정면·수평을 향하도록 보정한다.

## 2026-09-18 - 조준 시 총 처짐 / 총구 이펙트 위치 / 탄피 이펙트 제거

- [x] 조준(Aiming) 시 총이 대각선 아래로 처지는 문제의 원인(전신 포즈 전환이 팔 오버라이드가 가리지 않는 척추 각도를 바꿔 오른손 월드 회전이 달라짐)을 확인한다.
- [x] `SurvivalistTPS.controller`에서 이제 쓸모없어진(오히려 버그를 유발하는) `Aiming` 파라미터, `Rifle Aim Blend` 상태와 관련 전환을 전부 제거한다.
- [x] `PlayerMovement.cs`에서 더 이상 존재하지 않는 `Aiming` 파라미터를 갱신하던 코드와 카메라 참조를 제거한다.
- [x] `MuzzleFlashEffect`가 실제 총구(`Fire Position`)와 다른 위치에 있던 문제를 확인하고 위치/회전을 `Fire Position`에 맞춘다.
- [x] `Gun.cs`에 `shellEjectEffect` null 체크를 추가하고, 플레이어 총 인스턴스에서 `ShellEjectEffect` 오브젝트를 제거한다.
- [x] Play Mode에서 조준 시에도 총이 수평을 유지하는지, 발사 시 총구 이펙트가 정확한 위치에서 나오는지, 탄피 이펙트 제거로 인한 오류가 없는지 확인한다.

## 2026-09-18 - 조준점, 탄퍼짐, 탄퍼짐에 따른 조준점 벌어짐

- [x] `GunData`에 탄퍼짐 관련 수치(`minSpread`, `maxSpread`, `spreadIncrement`, `spreadRecoverSpeed`)를 추가한다.
- [x] `Gun.cs`에서 발사할 때마다 탄퍼짐이 늘어나고 시간이 지나면 회복되도록 구현하고, 실제 발사 방향에 탄퍼짐을 반영한다.
- [x] 탄퍼짐 비율(0~1)을 `Gun.spreadRatio`로 외부에 노출한다.
- [x] 탄퍼짐에 따라 4방향으로 벌어지는 조준점 UI(`SpreadCrosshair.cs`)를 만들고 `HUD Canvas.prefab`에 배치한다.
- [x] `UIManager`/`PlayerShooter`를 통해 매 프레임 탄퍼짐 비율을 조준점 UI에 전달한다.
- [x] `HUD Canvas` 프리팹 인스턴스를 `Prototype` 씬에 배치한다(조준점 UI가 실제로 보이도록).
- [x] Play Mode에서 발사 시 탄퍼짐 수치와 조준점 UI 좌표가 함께 갱신되는지 확인한다.

## 2026-09-18 - 쓸모없는 UI 삭제 (게임오버/웨이브/스코어/탄약)

- [x] 삭제 대상 범위를 사용자에게 확인한다(게임오버 UI, 웨이브 텍스트, 스코어 텍스트, 탄약 수 텍스트 전부 + 관련 코드 정리).
- [x] `UIManager.cs`를 조준점 갱신만 담당하도록 재작성한다(ammoText/scoreText/waveText/gameoverUI 필드와 관련 메서드 제거).
- [x] `GameManager.cs`의 `AddScore`/`EndGame`에서 UI 갱신 호출을 제거하고 `score`/`isGameover` 상태는 유지한다.
- [x] `ZombieSpawner.cs`의 `UpdateUI()` 호출과 메서드를 제거한다.
- [x] `PlayerShooter.cs`의 `UpdateUI()`에서 탄약 텍스트 갱신 호출만 제거하고 조준점 갱신은 유지한다.
- [x] `HUD Canvas.prefab`에서 `Ammo Display`, `Score Text`, `Enemy Wave Text`, `Gameover UI` 자식 오브젝트를 삭제하고 저장한다.
- [x] `recompile_status`/`console` 로그로 신규 오류 없음과 4개 오브젝트 삭제·`Crosshair`만 남은 것을 확인한다.
- [x] 검증된 변경을 로컬 커밋하고 원격 `main`에 push한다.

## 2026-09-18 - 다른 무기(저격총/돌격소총)로 무기 교체 시스템 구현

- [x] 사용자에게 대상 무기군(총기류/근접무기)과 작업 목표(IK 확인용 vs 실제 교체 시스템)를 확인한다 -> 총기류 우선, 실제 교체 시스템 구현으로 확정.
- [x] `Pistol Gun.prefab`/`Sniper Gun.prefab`에 남아있던 템플릿 잔재(Right Handle 회전 미보정, MuzzleFlashEffect가 Fire Position과 어긋남)를 공용 에셋 레벨에서 수정한다.
- [x] WeaponsPack (LowPoly)의 American AssaultRifle 모델로 `Assault Rifle Gun.prefab`을 새로 만들고 Fire Position/Left/Right Handle 좌표를 실측해 배치한다.
- [x] `PlayerShooter`에 `weaponPrefabs` 배열과 `EquipWeapon(int)`을 추가해 gunPivot 아래에 총을 동적으로 생성/교체하고 gun/leftHandMount/rightHandMount/IK 이펙터를 매번 다시 연결한다.
- [x] `SurvivalistWeaponIK`의 그립 보정 로직을 `RecalibrateGrip()`으로 분리해 무기 교체 시마다 다시 호출할 수 있게 한다.
- [x] `PlayerInput`에 숫자키(1~9) 기반 무기 선택 입력(`selectWeaponIndex`)을 추가한다.
- [x] `Player Character.prefab`의 정적 `Gun` 자식을 제거하고 `weaponPrefabs`에 3종을 등록한다.
- [x] Play Mode에서 Pistol/Sniper/Assault Rifle 각각으로 `EquipWeapon()`을 호출해 오른손 그립 거리, 총구 이펙트 정렬, gunPivot 방향을 검증한다.
- [x] 검증된 변경을 로컬 커밋하고 원격 `main`에 push한다.

## 2026-09-20 - 다른 PC 인계 동기화와 에셋 검증

- [x] `origin/main`을 pull해 다른 PC의 작업을 이어받는다(clone 불필요 - 같은 원격의 깨끗한 저장소였음).
- [x] `GAME_DESIGN.md` §15의 보유 에셋 19종이 실제 `Assets/` 아래에 전부 있는지 폴더 단위로 대조한다.
- [x] Unity 컴파일 상태를 확인하고 실패 원인을 실제 콘솔 에러로 특정한다.
- [x] StarterAssets의 Cinemachine 네임스페이스 컴파일 에러를 해소한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`으로 재검증한다.
- [x] 검증된 문서 변경을 로컬 커밋하고 원격 `main`에 push한다.

## 2026-09-20 - 카메라 시점 3인칭 확정 문서 갱신

- [x] 사용자가 카메라를 3인칭으로 확정. 문서에 남아있던 "1인칭 확정"/"미확정" 서술 위치를 전부 찾는다.
- [x] `GAME_DESIGN.md` 헤더, §1, §5.1, §5.2, §15, §16, M1 완료 조건을 3인칭 확정으로 갱신한다.
- [x] `CLAUDE.md` §11.1과 `AGENTS.md`(핵심 방향, 구현 우선순위)를 3인칭 확정으로 갱신한다.
- [x] `CLAUDE_HANDOFF.md`의 문서-코드 불일치 경고와 미결 항목을 해소 상태로 갱신한다.
- [x] 남은 1인칭/미확정 서술이 없는지 grep으로 재확인한다.
- [x] 1인칭 확정으로 죽은 코드가 된 파일을 삭제하지 않고 목록만 보고한다.
- [x] 검증된 문서 변경을 로컬 커밋하고 원격 `main`에 push한다.

## 2026-09-20 - M2 전투 수직 슬라이스 (Zombie 에셋 Zombie3)

- [x] 좀비 모델(Zombie3)과 애니메이션 구성(에셋 클립 + 공격 포함)을 사용자에게 확인한다.
- [x] `plan.md`에 목표/완료 조건/구현 순서/제외 범위를 기록한다.
- [x] `Player Character`의 레이어를 `Player`(9)로 변경한다 (현재 Default라 좀비가 탐지 못 함).
- [x] `Test Ground`에 Navigation Static을 설정하고 NavMesh를 베이크한다.
- [x] `Zombie3` 기반 좀비 프리팹을 만들고 NavMeshAgent/AudioSource/콜라이더/Zombie.cs/BloodSprayEffect를 구성한다.
- [x] 에셋 클립(`Z_Idle`/`Z_Run_InPlace`/`Z_Attack`/`Z_FallingForward`)으로 `HasTarget`/`Die`/`Attack` 애니메이터 컨트롤러를 만든다.
- [x] `Zombie.cs`의 `OnTriggerStay`에 `SetTrigger("Attack")`를 추가한다.
- [x] `GameManager`/`ZombieSpawner`/`ItemSpawner`/`Spawn Points`를 `Prototype` 씬에 배치하고 참조를 연결한다.
- [x] `GameManager`에 게임오버 상태에서의 씬 재시작 입력을 추가한다 (UI는 다시 만들지 않는다).
- [x] Play Mode에서 탐지/추적/공격/사망/드랍/재시작 전체 루프를 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 검증된 변경을 로컬 커밋하고 원격 `main`에 push한다.
