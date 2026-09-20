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

## 2026-09-20 - 캐릭터 파묻힘 회귀와 시체 공격 버그 수정

- [x] 파묻힘 원인을 실제 측정으로 특정한다 (루트/콜라이더는 정상, 비주얼만 약 1.09 아래).
- [x] `SurvivalistTPS.controller`의 Base Layer 모션 8개가 전부 NULL인 것을 확인한다.
- [x] 깨진 GUID 7개가 내가 삭제한 StarterAssets 애니메이션 FBX임을 확인한다.
- [x] Asset Store 캐시의 원본 패키지에서 해당 FBX 7개만 골라 원래 경로로 복구한다 (Editor 폴더는 복구하지 않아 Cinemachine 에러 재발을 막는다).
- [x] 컨트롤러를 더티 처리해 런타임 데이터를 재빌드하고 Base Layer 가중치 0을 1로 정상화한다.
- [x] Play Mode에서 발 높이와 이동 블렌드(Idle/Walk_N/Run_N)를 검증한다.
- [x] `Zombie.cs`의 `OnTriggerStay`에 `!attackTarget.dead` 조건을 추가해 시체를 계속 때리지 않게 한다.
- [x] Play Mode에서 사망 시 체력이 정확히 0에서 멈추고 좀비가 Idle로 돌아가는지 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 검증된 변경을 로컬 커밋하고 원격 `main`에 push한다.

## 2026-09-20 - M3 루팅과 캐릭터 관리

### 착수 준비

- [x] 인벤토리 방식(무게제), 기존 아이템 처리(인벤토리 전환), 장비 슬롯 구성(무기 3 + 방어구 3)을 사용자에게 확인한다.
- [x] 방어구 비주얼 가능 여부를 실제 메시 파츠로 검증한다 (하체 맨다리 메시 없음 -> 전부 수치만으로 확정).
- [x] 방어 계산 규칙을 확정한다 (`Max(1, damage - armor)`).
- [x] `plan.md`에 목표/결정/완료 조건/구현 순서/제외 범위를 기록한다.
- [x] `CLAUDE_HANDOFF.md`를 M2 완료 시점 기준으로 갱신한다.

### 1단계. 아이템 데이터 + 무게제 인벤토리 코어

- [x] `ItemData`(base) ScriptableObject를 만든다 (id/이름/아이콘/개당 무게/최대 스택/설명).
- [x] `ConsumableItemData` / `WeaponItemData` / `ArmorItemData` 3종 서브클래스를 만든다.
- [x] `EquipmentSlot` enum을 만든다 (주무기/보조무기/근접/머리/상체/하체).
- [x] 런타임 상태 `ItemStack`(ItemData 참조 + 개수)을 만든다.
- [x] `Inventory`를 만든다 (무게 합산, 스택 병합, 추가/제거/사용, `OnChanged` 이벤트, UI 무지).
- [x] `PlayerMovement`에 무게 초과 시 이동속도 배율을 연결한다.
- [x] `Player Character.prefab`에 `Inventory`를 붙인다.
- [x] Play Mode `eval`로 추가/스택/제거/무게 합산/초과 배율을 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 2단계. 기존 아이템 3종 전환 + 인벤토리 UI

- [x] 실제 `ItemData` 에셋을 만든다 (붕대, 탄약, 동전 최소 3종).
- [x] `AmmoPack`/`HealthPack`/`Coin`을 `WorldItem` 하나로 통합한다.
- [x] `PlayerHealth.OnTriggerEnter`를 즉시 사용에서 인벤토리 추가로 바꾼다.
- [x] `ItemSpawner.items`와 기존 픽업 프리팹 3종을 새 `WorldItem` 방식으로 갱신한다.
- [x] 인벤토리 화면(목록, 무게 표시, 사용, 버리기)을 만들고 `I` 키로 연다.
- [x] Play Mode에서 줍기 -> 인벤토리 적재 -> 사용 -> 체력/탄약/점수 변화를 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 3단계. 장비 시스템 6슬롯 + 장비 UI + 방어 계산

- [x] 아이콘 + 이름 + 개수 + 무게로 된 줄 프리팽을 만든다 (`Item Row.prefab`).
- [x] 줄을 선택하면 강조되고 오른쪽 상세 패인에 설명과 동작 버튼이 나온다.
- [x] 무게를 막대 그래프로 보여주고 과적이면 색이 바뀜다 (초록 -> 노랑 -> 빨강).
- [x] 목록을 스크롤할 수 있게 한다 (`ScrollRect` + `RectMask2D`).
- [x] 선택 유지: 사용/버리기 후에도 같은 아이템이 선택된 채로 남는지 확인한다.
- [x] 사용/장착/버리기가 상세 패인에서 동작하는지 검증한다 (붕대 사용 체력 60->110, 방탄조끼 장착 방어 12, 돌 4->3).
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [ ] 커밋하고 원격 `main`에 push한다.

### 4단계. 상태 화면

- [x] 상태 화면(체력, 총 방어력, 총 무게/최대 무게, 장착 무기 공격력)을 만들고 `K` 키로 연다.
- [x] 인벤토리/장비 변경 시 수치가 따라 갱신되는지 Play Mode에서 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 5단계. 상자 루팅

- [x] `LootContainer`를 만든다 (내용물 목록, 열림 상태, 인벤토리로 옮기기).
- [x] 상호작용 입력(`E`)과 대상 감지를 `PlayerInput`/상호작용 컴포넌트에 추가한다.
- [x] `Ditag Design/Mesh Pack/Chest 01`의 모델로 상자 프리팹을 만든다.
- [x] `Prototype` 씬에 상자를 배치한다.
- [x] Play Mode에서 열기 -> 인벤토리 적재 -> 빈 상자 재개봉 불가를 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### M3 마무리 - 사용자 수동 확인이 필요한 항목

- [ ] 키 입력 확인: `I`(인벤토리) / `O`(장비) / `K`(상태) / `E`(상자 열기) / `1`,`2`(주무기·보조무기 전환).
      MCP 파이프라인은 키보드 입력을 합성할 수 없어 자동 검증이 불가능하다.
      코드 경로(`SetOpen`, `SelectSlot`, `LootContainer.Loot`)와 버튼 클릭은 전부 검증했고, 남은 것은 키 바인딩 그 자체뿐이다.
- [ ] 화면이 열린 동안 마우스로 시점/캐릭터가 돌지 않는지 육안 확인.

## 2026-09-20 - M4 파밍과 거점 MVP

### 착수 준비

- [x] 채집 방식(무기 타격), 배치 방식(자유 배치), 건설물 4종(벽/바리케이드/보관 상자/문)을 사용자에게 확인한다.
- [x] 사격 데미지 전달 경로(`hit.collider.GetComponent<IDamageable>()`)를 확인한다.
- [x] 자원/건설물로 쓸 에셋이 실제로 있는지 확인한다.
- [x] `plan.md`에 목표/결정/완료 조건/구현 순서/제외 범위를 기록한다.

### 1단계. 재료 아이템과 자원 채집

- [x] `ItemData`의 abstract를 해제해 순수 재료 아이템을 만들 수 있게 한다.
- [x] 재료 `ItemData` 에셋 3종(목재/돌/고철)을 만든다.
- [x] 재료 픽업 프리팹 3종을 만들고 `ItemData.worldPrefab`에 연결한다.
- [x] `ResourceNode : LivingEntity`를 만든다 (내구도, 파괴 시 드랍, 재생성).
- [x] 자원 노드 프리팹 3종(나무/바위/고철)을 만든다 (콜라이더와 로직을 같은 오브젝트에).
- [x] 자원 노드 머티리얼이 텍스처를 제대로 물고 있는지 확인한다.
- [x] `Prototype` 씬에 자원 노드를 배치하고 NavMesh를 다시 베이크한다.
- [x] Play Mode에서 사격 -> 내구도 감소 -> 파괴 -> 드랍 -> 줍기 -> 인벤토리 적재를 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 2단계. 건설 배치 시스템 + 벽 1종

- [x] `BuildableData`(id/이름/프리팹/필요 재료/판정 크기)를 만든다.
- [x] `PlacedBuilding`(순수 데이터)과 `BaseBuildState`(설치 목록 소유자)를 만든다.
- [x] `BuildPlacer`를 만든다 (프리뷰 생성, 바닥 레이캐스트, 휠 회전, 겹침 판정, 재료 소모, 설치).
- [x] `BuildMenuUI`를 만들고 `B` 키로 연다 (건설물 목록과 필요 재료 표시).
- [x] 벽 `BuildableData`와 건설물 프리팹을 만든다.
- [x] Play Mode에서 재료 부족 거부 / 충분 시 설치 / 재료 정확히 차감 / 겹침 거부를 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 3단계. 나머지 건설물 3종

- [x] 바리케이드 `BuildableData`와 프리팹을 만든다.
- [x] 보관 상자를 만든다 (`StorageContainer` + 넣기/꺼내기 화면).
- [x] 문을 만든다 (`BuildableDoor` 열림/닫힘, `E` 상호작용, 닫힘 시 물리 차단).
- [x] Play Mode에서 각 건설물의 고유 동작을 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 4단계. 철거와 저장 데이터 표현

- [x] 철거 입력을 만들고 `BaseBuildState`에서 제거한다.
- [x] 설치된 건설물 전체를 `PlacedBuilding` 목록으로 덤프해 M7 저장 DTO로 옮길 수 있는 형태임을 보인다.
- [x] Play Mode에서 설치 -> 철거 -> 목록 반영을 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### M4 마무리 - 사용자 수동 확인이 필요한 항목

- [ ] 키 입력 확인: `B`(건설 메뉴) / `X`(철거) / `E`(문·보관 상자) / 휠(프리뷰 회전) / 좌클릭(설치) / 우클릭·ESC(취소).
      MCP 파이프라인은 키보드·마우스 입력을 합성할 수 없다. 코드 경로(`Select`/`TryPlace`/`Demolish`/`Interact`)와 UI 버튼은 전부 검증했고 남은 것은 입력 바인딩 자체뿐이다.
- [ ] 자원을 실제로 쏴서 부수는 조작감 확인 (총으로 벌목하는 방식이 어색하지 않은지).
- [ ] 건설 프리뷰가 조준을 따라 자연스럽게 움직이는지 육안 확인.

## 2026-09-20 - M5 오토바이

### 착수 준비

- [x] 오토바이 에셋(RSG 바이크), 라이더 표현(앉은 포즈만), 연료·내구도(둘 다)를 사용자에게 확인한다.
- [x] 두 오토바이 에셋의 실제 구조(물리 리그 유무, 동봉 스크립트)를 확인한다.
- [x] `BicycleVehicle`의 입력 폴백 경로를 확인한다 (서드파티 수정 없이 제어 가능한지).
- [x] 라이더 포즈 클립과 `SurvivalistTPS.controller` 레이어 구조를 확인한다.
- [x] `plan.md`에 목표/결정/완료 조건/구현 순서/제외 범위를 기록한다.

### 1단계. 탑승과 하차

- [x] `Motorcycle`을 만든다 (`IInteractable`, 탑승 상태, 좌석/하차 지점).
- [x] 오토바이 프리팹을 만든다 (RSG URP 프리팹 + 좌석/하차 지점 + 로직).
- [x] `SurvivalistTPS.controller`에 `Mounted` 파라미터와 앉은 포즈 상태를 추가한다.
- [x] 탑승 중 `Weapon Hold Arms` 레이어 가중치를 0으로 내린다.
- [x] 탑승 중 `PlayerInput`/`PlayerMovement`/`PlayerShooter`/`BuildPlacer` 입력을 막는다.
- [x] 카메라 타깃을 오토바이로 전환하고 하차 시 되돌린다.
- [x] `Prototype` 씬에 오토바이를 배치한다.
- [x] Play Mode에서 탑승/하차, 입력 차단·복구, 카메라 전환, 라이더 포즈를 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 2단계. 주행

- [x] 탑승 중에만 `BicycleVehicle`이 동작하도록 활성/비활성을 연동한다.
- [x] 오토바이가 NavMesh 장애물로 잡히지 않는지 확인하고 필요하면 재베이크한다.
- [x] Play Mode에서 주행 리그가 실제로 움직이는지 검증한다 (키 입력 합성 불가라 토크 직접 인가로 대체, 실제 키 주행은 사용자 수동 확인).
- [x] 미탑승 상태에서 입력이 오토바이를 움직이지 않는지 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 3단계. 연료와 내구도

- [x] `Motorcycle`에 연료/내구도와 소모 규칙을 넣는다.
- [x] 연료 0 또는 내구 0이면 주행이 막히도록 한다.
- [x] 충돌 시 내구도가 감소하도록 한다.
- [x] 연료통 `ItemData`와 픽업을 만들고 보충 경로를 붙인다.
- [x] 고철로 수리하는 경로를 붙인다.
- [x] Play Mode에서 소모/고갈/보충/수리를 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 4단계. 저장 데이터 표현과 HUD

- [x] `MotorcycleSaveData`와 JSON 덤프를 만든다.
- [x] 탑승 중 연료/내구도를 HUD에 표시한다.
- [x] Play Mode에서 상태 변경이 덤프와 HUD에 반영되는지 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### M5 마무리 - 사용자 수동 확인이 필요한 항목

- [ ] 키 입력 확인: `E`(타기/내리기) / `R`(주유) / `F`(수리) / `WASD`(주행).
      MCP 파이프라인은 키 입력을 합성할 수 없다. 코드 경로(`Mount`/`Dismount`/`Refuel`/`Repair`)와 주행 리그는 전부 검증했고 남은 것은 입력 바인딩 자체뿐이다.
- [ ] 실제 주행감 확인 (에셋 기본값 `motorForce=500`, `brakeForce=2000`, `maxSteeringAngle=45`, `maxLeanAngle=35`). 너무 빠르거나 느리면 알려주면 조정한다.
- [ ] 라이더 자세 육안 확인 (다리가 약간 길게 내려오는 리타게팅 아티팩트가 거슬리는지).

## 2026-09-20 - M6 시드 월드와 스트리밍

### 착수 준비

- [x] 씬 구성(새 `World.unity`), 청크/월드 크기(50m x 20x20), NavMesh 방식(`NavMeshSurface`), POI(School 추출)를 사용자에게 확인한다.
- [x] 월드 에셋의 실제 치수와 연결 타입을 확인한다 (Grounds/Cross가 50.2m로 일치).
- [x] `NavMeshSurface`/`NavMeshModifier` 사용 가능 여부를 확인한다.
- [x] `School Scene`이 프리팹이 아니라 씬이라는 것을 확인한다.
- [x] `plan.md`에 목표/결정/완료 조건/구현 순서/제외 범위를 기록한다.

### 1단계. 결정론적 시드 생성기와 연결성 검증

- [x] `ChunkType` enum을 만든다 (도시/도로/산/초원/POI).
- [x] `WorldChunkData`(순수 데이터: 좌표/타입/회전/청크 시드)를 만든다.
- [x] `WorldGenerator`를 만든다 (`WorldSeed` + `GeneratorVersion` -> 결정론적 청크 맵).
- [x] 연결 규칙과 가중치로 청크 타입을 배치한다.
- [x] 시작 지점 기준 도로 연결성 검사를 만든다.
- [x] Play Mode에서 같은 시드 = 같은 맵, 다른 시드 = 다른 맵을 검증한다.
- [x] 연결성 검사가 통과하는지와 20x20 생성 시간을 측정한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 2단계. 청크 프리팹과 스트리밍, World.unity

- [x] 청크 타입별 프리팹을 만든다 (초원/도로/도시/산).
- [x] 도로 조각이 50m 격자에 맞도록 배치 오프셋/스케일을 보정한다 (하드코딩 대신 bounds 기반 자동 보정).
- [x] `WorldStreamer`를 만든다 (플레이어 반경 기준 활성/비활성).
- [x] `World.unity`를 만들고 플레이어/HUD/매니저를 배치한다.
- [x] 빌드 설정에 `World.unity`를 등록한다.
- [x] Play Mode에서 이동에 따른 청크 활성/비활성과 활성 개수를 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 3단계. NavMeshSurface 전환

- [x] 런타임 NavMesh를 만든다 (`NavMeshSurface`는 볼륨 이동 시 재수집을 하지 않아 저수준 `NavMeshBuilder`로 대체).
- [x] 건설물 프리팹 4종에 `NavMeshObstacle`(carve)을 추가한다.
- [x] Play Mode에서 좀비가 새 지형을 추적하는지 검증한다.
- [x] 벽을 세웠을 때 좀비 경로가 실제로 바뀌는지 검증한다 (M4에서 남긴 제약 해소).
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### 4단계. 대형 POI (School)

- [x] `SchoolSceneAbandoned.unity`에서 학교 건물 계층을 확인한다.
- [x] 라이트맵/머티리얼 의존성을 확인하고 프리팹으로 추출한다.
- [x] POI 청크로 편입하고 시드에 따라 배치되게 한다.
- [x] Play Mode에서 POI 배치와 도로 연결을 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

## 2026-09-20 - 좀비 탐지 방식 변경 (시야 + 청각)

- [x] 기존 탐지(`OverlapSphere` 20m, 벽 무시, 즉시 추적)의 문제를 확인한다.
- [x] `NoiseEvent`(정적 소리 전파 통로)를 만든다.
- [x] `Gun.Fire`가 총소리를 `NoiseEvent`로 알리게 한다.
- [x] `Zombie`를 시야 기반 발견(거리 + 시야각 + 시야 차단)으로 바꾼다.
- [x] 소리를 들으면 그 지점으로 가서 조사하게 한다.
- [x] 대상을 일정 시간 놓치면 추적을 포기하고 마지막 위치를 조사하게 한다.
- [x] 조사 지점이 NavMesh 밖이면 갈 수 있는 자리로 보정한다.
- [x] Play Mode에서 사거리 밖/등 뒤/정면/총소리/추적 포기/공격을 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

### M6 마무리 - 사용자 수동 확인이 필요한 항목

- [ ] `World.unity`를 직접 플레이해 이동/스트리밍/주행 체감을 확인한다.
      **에디터 창이 포커스를 잃으면 게임이 멈춘다**(Run In Background 꺼짐). 필요하면 Player Settings에서 켤 것 - 사용자 소유 설정이라 건드리지 않았다.
- [ ] 시드를 바꿔가며(`World Streamer > settings > worldSeed`) 월드가 달라지는지 확인한다.
- [ ] 도로 폭(21m)과 50m 칸의 빈 풀밭 비율이 괜찮은지 판단한다.

## 2026-09-20 - 학교 내부 탐험

- [x] `Interior`가 건물과 같은 자리에 맞물리는지(별도 공간인지) 확인한다.
- [x] 내부의 콜라이더/셰이더/라이트맵 의존성을 확인한다.
- [x] `School POI.prefab`을 외부 + 내부로 다시 만든다 (원본 상대 위치 보존).
- [x] 실내에서 카메라가 벽을 뚫지 않도록 카메라 충돌을 넣는다.
- [x] 바깥에서 실내까지 NavMesh가 이어지는지 확인하고 끊기면 고친다.
- [x] Play Mode에서 좀비가 학교 안까지 쫓아오는지 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다.

## 2026-09-20 - M7 저장 시스템

### 착수 준비

- [x] `World.unity`에 저장 대상 콘텐츠가 있는지 확인한다 (오토바이/상자/자원/스포너 전부 0개).
- [x] 기준 씬(World + 콘텐츠 먼저), 슬롯 UI(게임 내 ScreenPanel), 자동저장(시간 간격)을 사용자에게 확인한다.
- [x] `plan.md`에 목표/결정/완료 조건/구현 순서/제외 범위를 기록한다.

### 1단계. 청크 콘텐츠 배치

- [x] `ChunkLibrary`에 자원 노드/루팅 상자 프리팹과 청크 타입별 개수를 넣는다.
- [x] `WorldChunkBuilder`가 칸 시드로 자원/상자를 결정론적으로 배치한다.
- [x] 시작 청크 중앙(스폰 지점) 주변 4m를 비워둔다 (가장 가까운 콘텐츠 12.0m).
- [x] `WorldRuntimeState`를 만들어 채집/루팅 상태를 청크 밖에 보관한다.
- [x] 청크가 다시 올라올 때 저장된 상태를 반영한다.
- [x] Play Mode에서 같은 칸의 배치가 항상 같은지 검증한다 (청크 재생성 후에도 동일 좌표 574.41/-0.32/524.94).
- [x] Play Mode에서 청크를 내렸다 올려도 채집/루팅 상태가 유지되는지 검증한다 (복원 직후 dead=True/잔여 재생성 10.1초, 시간이 지나자 부활).
- [x] 청크 생성 비용을 측정한다 (25칸 33.9ms, 칸당 1.36ms).
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다. (b2ae181)

### 2단계. 오토바이와 스포너 배치

- [x] `World.unity` 시작 지점 근처에 오토바이를 배치한다 (스폰에서 8m).
- [x] 좀비 스폰을 붙인다. 고정 스폰 지점 대신 `WorldZombieSpawner`로 플레이어 주변 유지 방식을 쓴다 (사용자 결정).
- [x] `ItemSpawner`는 World 씬에 넣지 않는다. 상자 루팅과 자원 채집이 공급원이다 (사용자 결정).
- [x] Play Mode에서 시드 월드 탑승과 좀비 스폰을 검증한다 (탑승/하차 성공, 좀비 8/8 유지, 모두 NavMesh 위 25.7~43.6m).
- [x] 플레이어가 멀리 이동하면 멀어진 좀비가 정리되는지 검증한다 (275m 이동 후 기존 8마리 제거, 새 고리에 재생성).
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다. (95da256)

### 3단계. 저장 데이터 모델과 파일 입출력

- [x] `SaveRegistry`(id -> ItemData/BuildableData)를 만든다 (아이템 13개/건설물 4개, 중복·빈 id 없음).
- [x] `SaveData` 계열 순수 데이터와 `SaveVersion`을 만든다.
- [x] `SaveSystem`을 만든다 (슬롯 파일 경로, 원자적 저장, 목록 조회, 삭제).
- [x] 쓰기/읽기 왕복을 검증한다 (시드·플레이어·인벤토리·장비·건설물·보관상자·오토바이·채집/루팅 전부 일치).
- [x] 저장 실패가 기존 저장을 손상시키지 않는지 검증한다 (읽기 전용 파일로 교체 실패 유도 → 반환 false, 원본 그대로, 임시 파일 잔류 없음).
- [x] 10슬롯 목록과 수동/자동 구분, 삭제를 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [ ] 커밋하고 원격 `main`에 push한다.

### 4단계. 수집과 복원

- [x] 플레이어 위치/체력/점수를 저장/복원한다 (`LivingEntity.SetHealth`, `GameManager.SetScore` 추가).
- [x] 인벤토리와 장비를 저장/복원한다 (장비는 인벤토리를 가리키므로 인벤토리를 먼저 되돌린다).
- [x] 건설물을 저장/복원한다 (보관 상자 내용물과 문 열림 상태 포함).
- [x] 오토바이를 저장/복원한다.
- [x] 루팅/채집 상태와 월드 시드를 저장/복원한다.
- [x] 시드가 다른 저장을 불러오면 월드를 다시 만든다 (`WorldStreamer.Regenerate`).
- [x] Play Mode에서 상태 변경 -> 저장 -> 훼손 -> 불러오기 왕복을 검증한다 (체력 70, 위치/방향, 아이템 6종, 장비 2칸, 건설물 2개, 상자 안 mat_scrap x7, 오토바이 55/70, 채집/루팅 1개씩 모두 일치).
- [x] 다른 시드(777) 저장을 불러오면 월드가 재생성되는지 검증한다 (지문 변경, 청크 25칸 재구성, 연결성 통과).
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다. (2227583)

### 5단계. 슬롯 UI와 자동저장

- [x] 10개 슬롯 화면(`ScreenPanel`)을 만들고 `ESC`로 열고 닫는다 (다른 화면이 열려 있으면 그쪽을 먼저 닫는 것까지 확인).
- [x] 각 슬롯의 수동/자동저장 칸을 구분해 표시한다.
- [x] 저장/불러오기/자동 불러오기/삭제 버튼을 연결한다 (빈 슬롯은 불러오기와 삭제가 꺼진다).
- [x] 일정 시간마다 자동저장한다 (기본 300초, 마지막으로 쓰거나 불러온 슬롯의 자동 칸에만 쓴다).
- [x] 자동저장이 수동 저장을 덮지 않는지 검증한다 (3초 주기로 돌려 자동 파일만 생기고 수동 파일 내용은 불변).
- [x] Play Mode에서 슬롯 목록 10개와 저장/불러오기/삭제 버튼을 실제로 눌러 검증한다.
- [x] 패널 배치가 화면을 넘치지 않는지 검증한다 (줄 높이 50, 마지막 줄과 상태줄 간격 31px, 버튼이 패널을 벗어나지 않음).
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [x] 커밋하고 원격 `main`에 push한다. (ebe5f24)

## 2026-09-20 - M8-UI 팰월드풍 UI 리뉴얼

### 착수 준비

- [x] 팰월드 UI 조사(기본 인벤토리는 목록, HUD는 좌하단 체력/우하단 무기).
- [x] 아이템 아이콘 현황 확인(13개 전부 비어 있음).
- [x] 화면 구성(한 창 + 탭), 아이콘(자동 생성), 범위(전체 + HUD)를 사용자에게 확인.
- [x] `plan.md`에 목표/결정/완료 조건/구현 순서/제외 범위를 기록한다.

### 1단계. 테마와 아이콘 기반

- [x] `UiTheme`에 색/여백/글자 크기를 모은다.
- [x] 에디터 전용 아이콘 생성기를 만든다 (`Assets/Editor/ItemIconGenerator.cs`).
- [x] 13종 아이템의 `icon`을 채운다 (미리보기 10종 + 방어구 색 타일 3종, 128x128).
- [x] 미리보기의 회색 배경을 테두리 채우기로 지우고 감마 보정으로 밝힌다.
- [x] 생성한 스프라이트의 임포트 설정(Sprite, 투명 배경)을 확인한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [ ] 커밋하고 원격 `main`에 push한다.

### 2단계. 통합 창과 탭

- [x] `TabView` 공통 뎼대를 만들고 인벤토리/장비/상태를 탭으로 옮긴다.
- [x] `CharacterScreenUI`가 여닫기/커서/단축키/탭 전환을 담당한다.
- [x] HUD Canvas **프리팽을** 고쳐 두 씬이 같은 UI를 쓰게 한다 (씬에서는 프리팽 인스턴스 자식을 옮길 수 없다).
- [x] M7에서 씬에만 넣었던 저장 화면도 프리팽으로 옥기고 중복 오버라이드를 정리한다.
- [x] `I`/`O`/`K`가 같은 창을 각 탭으로 여는지 확인한다 (탭 버튼 3개, 전환 동작).
- [x] 기존 동작(장비 슬롯 6줄, 상태 수치, 저장 슬롯 10줄, 건설 메뉴)이 그대로인지 확인한다.
- [x] 한 번에 한 화면만 열리는 규칙과 커서 잠금이 유지되는지 확인한다.
- [x] `CLAUDE.md` §11.1과 `GAME_DESIGN.md` §10의 "화면 분리" 규칙을 갱신한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [ ] 커밋하고 원격 `main`에 push한다.

### 3단계. 팰월드풍 인벤토리 탭

- [ ] 아이콘 + 이름 + 개수 + 무게로 된 줄 팹을 만든다.
- [ ] 줄을 선택하면 강조되고 오른쪽 상세 패인에 설명과 동작 버튼이 나온다.
- [ ] 무게를 막대 그래프로 보여주고 과적이면 색이 바뀐다.
- [ ] 선택 유지: 사용/버리기 후에도 목록이 튀지 않는지 확인한다.
- [ ] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [ ] 커밋하고 원격 `main`에 push한다.

### 4단계. 나머지 화면 통일

- [x] 장비 탭을 슬롯 6줄 + 장착 후보 목록 두 열로 바꿈다 (줄을 누르면 장착/해제).
- [x] 상태 탭을 체력 막대 + 능력치/장착 카드로 바꿈다.
- [x] 보관 상자 화면을 같은 테마로 바꿈다 (양쪽 목록 + 무게 막대, 줄 클릭으로 이동).
- [x] 건설 메뉴를 같은 테마로 바꿈다 (아이콘 추가, 재료 부족은 붉게).
- [x] `BuildableData.icon`을 추가하고 건설물 4종 아이콘을 생성한다.
- [x] 저장 슬롯 화면을 같은 테마로 바꾸고 요약 문구가 잘리지 않게 줄인다 (278px < 318px).
- [x] Play Mode에서 장착/해제, 상자 넣기/꺼내기, 건설 선택을 검증한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [ ] 커밋하고 원격 `main`에 push한다.

### 5단계. HUD

- [x] 체력바를 좌하단으로 옮기고 팔월드풍으로 바꿈다 (슬라이더 유지 + 수치 겹침).
- [x] 우하단에 장착 무기와 탄약을 표시한다 (`WeaponHudUI`).
- [x] 상호작용 안내를 배지 형태로 바꾸고 비어 있을 때 배경까지 감춘다.
- [x] 건설 안내를 상단 중앙으로 옮기고 강조색을 쓴다.
- [x] 차량 상태(연료/내구도)를 막대로 바꾸고 값에 따라 색이 변한다.
- [x] Play Mode에서 값이 실제로 바뀌는지 검증한다 (체력 72/100, 탄약 30/120, 연료 32/100 노랑, 내구 18/100 빨강).
- [x] `Prototype.unity`도 같은 프리팽을 쓰므로 참조가 깨지지 않았는지 확인한다.
- [x] `compilationFailed: false`, `consoleErrors: 0`을 확인한다.
- [ ] 커밋하고 원격 `main`에 push한다.
