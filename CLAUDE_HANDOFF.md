# Claude 인수인계 메모

다른 컴퓨터/세션에서 이어서 작업하기 위한 현재 상태 스냅샷입니다. 오래되면 다시 갱신하세요.

## 시작 상태

- 기준 커밋은 `92a8b31 저격총/돌격소총 무기 교체 시스템 구현`이며 `main`과 `origin/main`은 동기화되어 있다.
- 사용자 소유의 미커밋 변경(항상 존재, 되돌리거나 커밋하지 않는다): `ProjectSettings/ProjectSettings.asset`, `ProjectSettings/ShaderGraphSettings.asset`, `.vsconfig`.
- 작업 씬은 `Assets/Scenes/Prototype.unity`, 플레이어 프리팹은 `Assets/Prefabs/Player Character.prefab`이다. `TPS Player.prefab` 계열은 전부 삭제되고 처음부터 재구성되었으므로 더 이상 존재하지 않는다.

## 현재 구현 (2026-09-18 기준)

### 카메라/이동
- 3인칭 숄더뷰(`ThirdPersonCameraController`, `Main Camera`에 부착)를 사용 중이다. 우클릭(Fire2)으로 ADS 거리/FOV만 전환하며, 조준 자세 자체는 애니메이션으로 바꾸지 않는다(아래 "확정된 설계" 참고).
- `PlayerMovement.cs`가 `Vertical`/`Horizontal` 입력을 로컬 forward/right로 합성해 스트레이프 이동을 처리한다.
- 카메라는 2026-09-20에 사용자가 **3인칭으로 확정**했고 `GAME_DESIGN.md` §5.1/§5.2, `CLAUDE.md` §11.1, `AGENTS.md`도 여기에 맞춰 갱신했다. 1인칭은 더 이상 검토 대상이 아니다.

### 플레이어 캐릭터
- 루트 `Player Character` 아래 `Survivalist Visual/Geometry/SK_Military_Survivalist`가 실제 렌더 모델이며, 이 자식의 `Animator`(`Assets/Animations/SurvivalistTPS.controller` 사용)만 활성화되어 있다. 루트 자체에는 컨트롤러 없는 **비활성 더미 Animator**가 있는데, 이는 `PlayerHealth`/`PlayerMovement`의 `GetComponent<Animator>()` 호출이 예외를 던지지 않게 하기 위함이다(지우지 말 것).
- `SurvivalistTPS.controller`의 Base Layer는 `Idle Walk Run Blend`/`InAir`/`JumpLand`/`JumpStart` 4개 상태만 가진다(`Aiming` 파라미터나 조준 전용 상태는 의도적으로 제거됨 - 아래 참고). 별도 레이어 `Weapon Hold Arms`(Human Arms Mask, Override)가 항상 두 손으로 총을 쥔 포즈(`HumanM@WeaponHold_Rifle01`)를 덮어써서 힙파이어/조준 여부와 무관하게 자연스러운 파지를 유지한다.
- `SurvivalistWeaponIK.cs`가 매 `OnAnimatorIK` 프레임마다 오른손 본 위치/회전 기준으로 `gunPivot`을 역산해 배치한다(오른손이 총을 실제로 구동). 왼손은 `KevinIglesias.IKHelperTool`(에셋)이 `IK Left Hand Effector`로 보조 IK로 맞춘다.

### 확정된 설계: 조준 시 전신 포즈를 바꾸지 않는다
- 과거에 `Aiming` Bool + `Rifle Aim Blend` 상태로 조준 시 상체를 앞으로 기울이는 자세를 시도했으나, 척추 각도가 바뀌면서 오른손의 월드 회전이 달라져 총이 대각선 아래로 처지는 버그가 있었다(`Weapon Hold Arms` 오버라이드 레이어는 팔/손만 가리고 척추는 가리지 못하기 때문). 이 파라미터/상태는 완전히 제거했고, 조준은 순수하게 카메라 줌(`ThirdPersonCameraController`)만으로 표현한다. 다시 전신 조준 포즈를 넣으려면 이 트레이드오프를 먼저 검토할 것.

### 총기 시스템
- `Gun.cs`/`GunData.cs`: 발사/재장전/탄퍼짐(`minSpread`/`maxSpread`/`spreadIncrement`/`spreadRecoverSpeed`)을 갖는다. 조준 레이는 항상 화면 중앙(뷰포트 0.5,0.5) 기준이며 카메라 축 기준으로 탄퍼짐을 적용한다.
- `Assets/Scripts/SpreadCrosshair.cs`: 탄퍼짐 비율(0~1)에 따라 4방향으로 벌어지는 조준점 UI. `Too Many Crosshairs` 에셋의 전역 `Crosshair` 클래스와 이름이 겹쳐서 `SpreadCrosshair`로 명명했다.
- **무기 교체 시스템(이번 세션 신규)**: `PlayerShooter.cs`가 `weaponPrefabs`(GameObject 배열, 0=Pistol/1=Sniper/2=Assault Rifle) + `EquipWeapon(int)`를 갖는다. `Start()`에서 `EquipWeapon(0)`으로 최초 장착부터 처리하므로, 프리팹에는 정적 `Gun` 자식이 더 이상 없다 - 항상 런타임 인스턴스화. `PlayerInput.cs`의 숫자키(1~9, `selectWeaponIndex`)로 전환한다.
  - 무기 교체 시 `gun`/`leftHandMount`/`rightHandMount`를 새 인스턴스로 갱신하고, `SurvivalistWeaponIK.RecalibrateGrip()`(공개 메서드로 분리됨)을 다시 호출해 그립 오프셋 캐시를 새 무기 기준으로 갱신한다. `IK Left Hand Effector`도 새 무기의 `Left Handle` 아래 매번 새로 생성한다.
  - `Assets/Prefabs/Weapons/Pistol Gun.prefab`, `Sniper Gun.prefab`, `Assault Rifle Gun.prefab`(신규, WeaponsPack (LowPoly)의 American AssaultRifle 모델) 세 개가 이 컨벤션(자식: `Left Handle`/`Right Handle`/`Fire Position`/`MuzzleFlashEffect`/`ShellEjectEffect`(선택)/`Model`)을 따른다. 새 무기를 추가할 때는 이 컨벤션을 그대로 따르고, `Right Handle.localRotation`은 세 프리팹이 공유하는 보정값 `(0.19779, -0.81233, -0.49433, 0.23800)`을 그대로 복사하면 된다(캐릭터 팔 포즈에만 의존, 무기 모델과 무관하게 재사용 가능하다는 것을 수학적으로 확인함).
  - **알려진 사소한 잔여 오차**: 오른손 그립은 항상 정확히 0으로 맞지만, 왼손은 무기별로 최대 2~3cm 정도 어긋날 수 있다(팔 포즈 애니메이션의 실제 도달 범위와 `Left Handle` 좌표가 완벽히 일치하지 않기 때문). 조준 방향에는 영향 없음. 필요하면 폴리싱 단계에서 무기별 `Left Handle` 좌표를 미세 조정할 것.

### UI
- `HUD Canvas.prefab`에는 이제 `Crosshair`만 있다(게임오버/웨이브/스코어/탄약 텍스트는 사용자 요청으로 전부 삭제됨). `UIManager.cs`도 `UpdateCrosshairSpread` 하나만 담당한다.
- `GameManager.cs`의 `score`/`isGameover`는 UI 호출 없이 상태만 유지한다(`isGameover`는 `PlayerInput`이 입력 게이팅에 사용하므로 삭제 금지).

## 검증 기준

- `recompile_status`가 `compilationFailed: false`, `errors: []`.
- `console_status`의 `groundTruth.consoleErrors == 0`.
- Play Mode에서 `PlayerShooter.EquipWeapon(0/1/2)`를 각각 호출한 뒤(같은 eval 안에서 연달아 부르지 말고, **별도의 도구 호출로 나눠서** 최소 한 프레임 이상 시간을 준 뒤) 오른손 본과 `rightHandMount.position` 거리가 0.00000이어야 한다. (같은 프레임에서 바로 측정하면 `gunPivot`이 이전 무기 기준 값으로 남아있어 오차가 커 보이는 함정이 있음 - IK는 `OnAnimatorIK` 콜백에서만 갱신됨.)
- 총구 이펙트(`MuzzleFlashEffect`)와 `Fire Position`의 월드 거리는 항상 0.00000이어야 한다(로컬 좌표를 맞춰뒀으므로 자동으로 성립).

## 권장 다음 작업

`GAME_DESIGN.md` §23/§20 마일스톤 순서 기준으로:

1. **근접무기(멜리) 전투**: `Crusader Weapon`/`Free medieval weapons` 에셋 사용. 발사가 아닌 스윙/히트박스 기반의 완전히 다른 전투 방식이 필요하다(이번 세션에서 사용자가 총기류를 먼저 요청해 의도적으로 범위에서 제외함). `IDamageable`은 재사용 가능하지만 애니메이션/입력/판정은 새로 설계해야 한다.
2. **루팅/인벤토리(M3)**: 상자 열기, 인벤토리 화면, 장비 화면에서 무기 장착/해제. 지금의 `weaponPrefabs` 배열은 하드코딩된 3종 고정 목록이라, 실제 인벤토리 시스템이 들어오면 "보유한 무기 목록"으로 대체해야 한다.
3. (완료) `GAME_DESIGN.md` §5.1의 시점 서술을 3인칭 확정으로 갱신했다.

## 최근 커밋

- `92a8b31 저격총/돌격소총 무기 교체 시스템 구현`
- `ec62add UI 삭제 작업 체크리스트/컨텍스트 노트 기록`
- `57d16eb 사용하지 않는 게임오버/웨이브/스코어/탄약 UI 제거`
- `d93d1e7 조준점, 탄퍼짐, 탄퍼짐에 따른 조준점 벌어짐 구현`
- `03804a6 조준 시 총 처짐, 총구 이펙트 위치, 탄피 이펙트 제거`
