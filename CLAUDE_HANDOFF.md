# Claude 인수인계 메모

다른 컴퓨터/세션에서 이어서 작업하기 위한 현재 상태 스냅샷입니다. 오래되면 다시 갱신하세요.

## 시작 상태

- 기준 커밋은 `85587e9 캐릭터 파묻힘 회귀 복구와 시체 공격 버그 수정`이며 `main`과 `origin/main`은 동기화되어 있다. 원격은 `https://github.com/KJY-1204/Zombie-Survival.git`이다.
- 사용자 소유의 미커밋 변경(있다면 되돌리거나 커밋하지 않는다): `ProjectSettings/ProjectSettings.asset`, `ProjectSettings/ShaderGraphSettings.asset`, `.vsconfig`.
- 작업 씬은 `Assets/Scenes/Prototype.unity`(Build Settings 인덱스 2), 플레이어 프리팹은 `Assets/Prefabs/Player Character.prefab`, 좀비 프리팹은 `Assets/Prefabs/Zombie Character.prefab`이다.
- 교재 씬 `Assets/Scenes/Main.unity`와 교재 `Assets/Prefabs/Zombie.prefab`은 참조 관계가 남아 있어 그대로 두었다. 건드리지 말 것.

## 마일스톤 진행도

- M0 프로젝트 진단/에셋 검증 - 완료.
- M1 플레이어와 카메라 프로토타입 - 완료.
- M2 전투 수직 슬라이스 - 완료(2026-09-20).
- M3 루팅과 캐릭터 관리 - 미착수. `Assets/Scripts`에 인벤토리/장비/상자 계열 스크립트가 아직 하나도 없다.

## 현재 구현

### 카메라/이동
- 3인칭 숄더뷰(`ThirdPersonCameraController`, `Main Camera`에 부착)를 사용 중이다. 우클릭(Fire2)으로 ADS 거리/FOV만 전환하며, 조준 자세 자체는 애니메이션으로 바꾸지 않는다(아래 "확정된 설계" 참고).
- `PlayerMovement.cs`가 `Vertical`/`Horizontal` 입력을 로컬 forward/right로 합성해 스트레이프 이동을 처리한다.
- 카메라는 2026-09-20에 사용자가 **3인칭으로 확정**했고 `GAME_DESIGN.md` §5.1/§5.2, `CLAUDE.md` §11.1, `AGENTS.md`도 여기에 맞춰 갱신했다. 1인칭은 더 이상 검토 대상이 아니다.

### 플레이어 캐릭터
- 루트 `Player Character` 아래 `Survivalist Visual/Geometry/SK_Military_Survivalist`가 실제 렌더 모델이며, 이 자식의 `Animator`(`Assets/Animations/SurvivalistTPS.controller` 사용)만 활성화되어 있다. 루트 자체에는 컨트롤러 없는 **비활성 더미 Animator**가 있는데, 이는 `PlayerHealth`/`PlayerMovement`의 `GetComponent<Animator>()` 호출이 예외를 던지지 않게 하기 위함이다(지우지 말 것).
- 루트 레이어는 **9(`Player`)여야 한다.** `Zombie.whatIsTarget`이 512(레이어 9)라 Default로 돌아가면 좀비가 플레이어를 영원히 탐지하지 못한다. 2026-09-20에 이 값이 0으로 빠져 있던 것을 고쳤다.
- `SurvivalistTPS.controller`의 Base Layer는 `Idle Walk Run Blend`/`InAir`/`JumpLand`/`JumpStart` 4개 상태만 가진다(`Aiming` 파라미터나 조준 전용 상태는 의도적으로 제거됨 - 아래 참고). 별도 레이어 `Weapon Hold Arms`(Human Arms Mask, Override)가 항상 두 손으로 총을 쥔 포즈(`HumanM@WeaponHold_Rifle01`)를 덮어써서 힙파이어/조준 여부와 무관하게 자연스러운 파지를 유지한다.
- `SurvivalistWeaponIK.cs`가 매 `OnAnimatorIK` 프레임마다 오른손 본 위치/회전 기준으로 `gunPivot`을 역산해 배치한다(오른손이 총을 실제로 구동). 왼손은 `KevinIglesias.IKHelperTool`(에셋)이 `IK Left Hand Effector`로 보조 IK로 맞춘다.

### 확정된 설계: 조준 시 전신 포즈를 바꾸지 않는다
- 과거에 `Aiming` Bool + `Rifle Aim Blend` 상태로 조준 시 상체를 앞으로 기울이는 자세를 시도했으나, 척추 각도가 바뀌면서 오른손의 월드 회전이 달라져 총이 대각선 아래로 처지는 버그가 있었다(`Weapon Hold Arms` 오버라이드 레이어는 팔/손만 가리고 척추는 가리지 못하기 때문). 이 파라미터/상태는 완전히 제거했고, 조준은 순수하게 카메라 줌(`ThirdPersonCameraController`)만으로 표현한다. 다시 전신 조준 포즈를 넣으려면 이 트레이드오프를 먼저 검토할 것.

### 총기 시스템
- `Gun.cs`/`GunData.cs`: 발사/재장전/탄퍼짐(`minSpread`/`maxSpread`/`spreadIncrement`/`spreadRecoverSpeed`)을 갖는다. 조준 레이는 항상 화면 중앙(뷰포트 0.5,0.5) 기준이며 카메라 축 기준으로 탄퍼짐을 적용한다.
- `Assets/Scripts/SpreadCrosshair.cs`: 탄퍼짐 비율(0~1)에 따라 4방향으로 벌어지는 조준점 UI. `Too Many Crosshairs` 에셋의 전역 `Crosshair` 클래스와 이름이 겹쳐서 `SpreadCrosshair`로 명명했다.
- **무기 교체 시스템**: `PlayerShooter.cs`가 `weaponPrefabs`(GameObject 배열, 0=Pistol/1=Sniper/2=Assault Rifle) + `EquipWeapon(int)`를 갖는다. `Start()`에서 `EquipWeapon(0)`으로 최초 장착부터 처리하므로, 프리팹에는 정적 `Gun` 자식이 더 이상 없다 - 항상 런타임 인스턴스화. `PlayerInput.cs`의 숫자키(1~9, `selectWeaponIndex`)로 전환한다.
  - 무기 교체 시 `gun`/`leftHandMount`/`rightHandMount`를 새 인스턴스로 갱신하고, `SurvivalistWeaponIK.RecalibrateGrip()`(공개 메서드로 분리됨)을 다시 호출해 그립 오프셋 캐시를 새 무기 기준으로 갱신한다. `IK Left Hand Effector`도 새 무기의 `Left Handle` 아래 매번 새로 생성한다.
  - `Assets/Prefabs/Weapons/Pistol Gun.prefab`, `Sniper Gun.prefab`, `Assault Rifle Gun.prefab`(WeaponsPack (LowPoly)의 American AssaultRifle 모델) 세 개가 이 컨벤션(자식: `Left Handle`/`Right Handle`/`Fire Position`/`MuzzleFlashEffect`/`ShellEjectEffect`(선택)/`Model`)을 따른다. 새 무기를 추가할 때는 이 컨벤션을 그대로 따르고, `Right Handle.localRotation`은 세 프리팹이 공유하는 보정값 `(0.19779, -0.81233, -0.49433, 0.23800)`을 그대로 복사하면 된다(캐릭터 팔 포즈에만 의존, 무기 모델과 무관하게 재사용 가능하다는 것을 수학적으로 확인함).
  - **M3 착수 시 주의**: `weaponPrefabs`는 하드코딩된 3종 고정 배열이다. 인벤토리/장비 시스템이 들어오면 "보유한 무기 목록"으로 대체해야 한다.
  - **알려진 사소한 잔여 오차**: 오른손 그립은 항상 정확히 0으로 맞지만, 왼손은 무기별로 최대 2~3cm 정도 어긋날 수 있다(팔 포즈 애니메이션의 실제 도달 범위와 `Left Handle` 좌표가 완벽히 일치하지 않기 때문). 조준 방향에는 영향 없음. 필요하면 폴리싱 단계에서 무기별 `Left Handle` 좌표를 미세 조정할 것.

### 좀비와 전투 루프 (M2, 2026-09-20 신규)
- `Prototype.unity`에 `Game Manager` / `Spawn Points` / `Zombie Spawner` / `Item Spawner`를 배치했다. 이름 규칙은 교재 `Main.unity`를 그대로 따랐다.
- **NavMesh**: `Test Ground`에 `StaticEditorFlags.NavigationStatic`을 주고 레거시 `NavMeshBuilder`로 베이크했다(`Assets/Scenes/Prototype/NavMesh.asset`, 48.66 x 48.66). AI Navigation 패키지의 `NavMeshSurface` 방식이 아니라 프로젝트에 이미 있던 방식을 따랐다. **지형을 바꾸면 다시 베이크해야 한다.**
- **좀비 프리팹** `Assets/Prefabs/Zombie Character.prefab`: `Zombie` 에셋의 `Zombie3.prefab`을 인스턴스화 후 완전 언팩해 루트로 삼고 로직 컴포넌트를 붙였다. 콜라이더/NavMeshAgent 수치는 교재 `Zombie.prefab`에서 그대로 가져왔다. `applyRootMotion=false`(켜면 NavMeshAgent 이동과 충돌).
  - `Zombie3`를 고른 이유는 SkinnedMeshRenderer가 **1개**뿐이기 때문이다. `Zombie.cs:48`의 `GetComponentInChildren<Renderer>()`가 첫 렌더러 하나에만 `skinColor`를 입히므로, 렌더러가 여러 개인 `Zombie1`(14개, 절단용)/`Zombie2`(2개)는 색이 일부 부위에만 먹는다.
  - **자식 순서 주의**: `BloodSprayEffect`는 반드시 **마지막 자식**이어야 한다. 앞에 오면 `GetComponentInChildren<Renderer>()`가 파티클 렌더러를 먼저 잡아 `skinColor`가 본체가 아닌 파티클에 적용된다.
- **애니메이터** `Assets/Animations/Zombie Character.controller`: 파라미터 `HasTarget`(Bool)/`Attack`(Trigger)/`Die`(Trigger), 상태 Idle(`Z_Idle`)/Move(`Z_Run_InPlace`)/Attack(`Z_Attack`)/Die(`Z_FallingForward`).
  - 이동 클립은 반드시 `_InPlace` 변종을 쓴다. `Z_Run`/`Z_Walk`는 루트 모션이 들어 있어 NavMeshAgent와 충돌한다.
  - `Z_Attack`이 에셋에서 `loopTime=true`로 들어와 있다. 서드파티 에셋을 수정하지 않기 위해, Attack -> Idle/Move 전환에 `hasExitTime=true, exitTime=0.9`를 줘서 한 번만 재생되고 빠져나오게 했다.
- **게임오버와 재시작**: `GameManager.cs`가 `Update()`에서 게임오버 상태일 때 `R` 키로 `SceneManager.LoadScene(buildIndex)`를 호출한다. 재시작 입력을 `PlayerInput`이 아니라 `GameManager`에 둔 이유는 `PlayerInput.cs:26`이 게임오버 상태에서 모든 입력을 차단하기 때문이다. **게임오버 UI는 사용자가 의도적으로 삭제했으므로 다시 만들지 않는다.**

### UI
- `HUD Canvas.prefab`에는 이제 `Crosshair`만 있다(게임오버/웨이브/스코어/탄약 텍스트는 사용자 요청으로 전부 삭제됨). `UIManager.cs`도 `UpdateCrosshairSpread` 하나만 담당한다.
- `GameManager.cs`의 `score`/`isGameover`는 UI 호출 없이 상태만 유지한다(`isGameover`는 `PlayerInput`이 입력 게이팅에 사용하므로 삭제 금지).

## 과거에 크게 데였던 함정

### 에셋 폴더를 지울 때는 그 아래 모든 .meta의 GUID를 역참조 검사할 것
- 2026-09-20에 "Play Mode에 들어가면 캐릭터가 땅에 파묻힌다"는 회귀가 있었다. 원인은 내가 이전 세션에 지운 `Assets/Survivalist/StarterAssets/ThirdPersonController/Character/Animations/`의 FBX 7개였고, `SurvivalistTPS.controller`의 Base Layer 모션 8개가 전부 NULL이 되어 전신 포즈가 사라진 것이었다(팔만 덮는 오버라이드 레이어만 돌아서 힙이 아바타 기본 위치로 떨어짐).
- 삭제 전에 GUID 역참조를 검사하긴 했지만 `*.cs.meta`/`*.inputactions.meta`/`*.asset.meta`만 모았고 **애니메이션 FBX의 .meta를 빠뜨렸다.** 앞으로는 확장자를 한정하지 말고 폴더 아래 모든 `.meta`의 GUID를 모을 것.
- 복구는 Asset Store 캐시(`AppData/Roaming/Unity/Asset Store-5.x/Slayver/3D ModelsCharacters/Survivalist character.unitypackage`)를 `tar -xzf`로 풀어서 했다. unitypackage는 엔트리 폴더 이름이 곧 GUID라 깨진 GUID로 바로 찾을 수 있다.
- **`StarterAssets`의 `Editor` 폴더는 일부러 복구하지 않았다.** 복구하면 `StarterAssetsDeployMenu.cs`의 Cinemachine 컴파일 에러와 심볼을 되돌리는 `PackageChecker`가 같이 돌아온다. 현재 `Assets/Survivalist/StarterAssets` 아래에는 `ThirdPersonController/Character/Animations`만 있다.
- **런타임 재빌드 함정**: 외부에서 파일을 되돌리고 `AssetDatabase.Refresh`/`ImportAsset(ForceUpdate)`를 해도 `RuntimeAnimatorController.animationClips`가 갱신되지 않을 수 있다. 그 파일을 참조하는 컨트롤러도 `EditorUtility.SetDirty` + `SaveAssets` + 재임포트로 함께 더티 처리해야 한다.

## 검증 기준

- `recompile_status`가 `compilationFailed: false`, `errors: []`.
- `console_status`의 `groundTruth.consoleErrors == 0`. 남아 있는 경고 1건(루트 더미 Animator)은 알려진 것이다.
- Play Mode에서 `PlayerShooter.EquipWeapon(0/1/2)`를 각각 호출한 뒤(같은 eval 안에서 연달아 부르지 말고, **별도의 도구 호출로 나눠서** 최소 한 프레임 이상 시간을 준 뒤) 오른손 본과 `rightHandMount.position` 거리가 0.00000이어야 한다. (같은 프레임에서 바로 측정하면 `gunPivot`이 이전 무기 기준 값으로 남아있어 오차가 커 보이는 함정이 있음 - IK는 `OnAnimatorIK` 콜백에서만 갱신됨.)
- 총구 이펙트(`MuzzleFlashEffect`)와 `Fire Position`의 월드 거리는 항상 0.00000이어야 한다(로컬 좌표를 맞춰뒀으므로 자동으로 성립).
- 캐릭터 파묻힘 회귀 확인용: Play Mode에서 부츠 `min.y`가 약 -0.06(1.1 아래로 내려가면 회귀), `Animator.GetCurrentAnimatorClipInfo(0).Length > 0`.

## 남은 미검증/미해결 항목

- **사용자 수동 확인 필요**: 마우스 우클릭 ADS 전환과 이동·사격의 육안 확인. MCP 파이프라인은 마우스 입력을 합성할 수 없어 자동 검증이 불가능하다. `checklist.md`에 미완료로 남아 있다.
- `FindObjectOfType` 폐지 경고 정리. 기능 영향 없음, 폴리싱 단계로 미뤘다.
- 왼손 IK의 무기별 2~3cm 오차. 조준에 영향 없음, 폴리싱 단계로 미뤘다.
- 근접무기(멜리) 전투. `Crusader Weapon`/`Free medieval weapons` 에셋이 있으나 스윙/히트박스 기반의 다른 전투 방식이 필요해 M8 콘텐츠 사안으로 미뤘다. `IDamageable`은 재사용 가능하지만 애니메이션/입력/판정은 새로 설계해야 한다.

## 권장 다음 작업

**M3 루팅과 캐릭터 관리.** 완료 조건은 상자 열기/아이템 획득, 인벤토리 화면, 장비 화면에서 무기 장착/해제, 상태 화면이다.

착수 전에 정해야 할 것이 있다. `GAME_DESIGN.md` §21에 **인벤토리가 무게제인지 슬롯/그리드제인지가 미정으로 남아 있다.** 데이터 구조 자체가 갈리므로 코드를 쓰기 전에 사용자에게 확인할 것.

설계 시 지킬 경계(`CLAUDE.md` §11.6).
- 아이템은 정적 데이터(`ItemData` 같은 ScriptableObject)와 런타임 인스턴스 상태를 분리한다.
- 인벤토리/장비 시스템은 UI를 몰라야 하고, UI는 공개된 상태/이벤트만 사용한다.
- 인벤토리, 장비, 상태 화면은 서로 분리한다.

## 최근 커밋

- `85587e9 캐릭터 파묻힘 회귀 복구와 시체 공격 버그 수정`
- `1760112 M2 커밋/push 체크리스트 항목 완료 처리`
- `8999645 M2 전투 수직 슬라이스 구현 (Zombie 에셋 기반 좀비 전투 루프)`
- `52c831d M2 전투 수직 슬라이스 계획과 체크리스트 작성`
- `6a9af1e 카메라 시점을 3인칭 확정으로 문서 갱신`
