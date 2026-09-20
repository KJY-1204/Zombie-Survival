# Claude 인수인계 메모

다른 컴퓨터/세션에서 이어서 작업하기 위한 현재 상태 스냅샷입니다. 오래되면 다시 갱신하세요.

## 시작 상태

- 기준 커밋은 `7b78149 HUD를 좌하단 체력·우하단 무기·막대형 차량 상태로 재배치`이며 `main`과 `origin/main`은 동기화되어 있다. 원격은 `https://github.com/KJY-1204/Zombie-Survival.git`이다.
- 사용자 소유의 미커밋 변경(있다면 되돌리거나 커밋하지 않는다): `ProjectSettings/ProjectSettings.asset`, `ProjectSettings/ShaderGraphSettings.asset`, `.vsconfig`.
- **씬이 두 개다.**
  - `Assets/Scenes/Prototype.unity`(인덱스 2) - M2~M5를 검증한 씬. `Test Ground` 50x50에 레거시 `NavMeshBuilder` 베이크를 쓴다. **건드리지 않는다.**
  - `Assets/Scenes/World.unity`(인덱스 3) - M6의 시드 청크 월드. 런타임 NavMesh를 쓴다.
- 플레이어 프리팹은 `Assets/Prefabs/Player Character.prefab`, 좀비 프리팹은 `Assets/Prefabs/Zombie Character.prefab`이다.
- 교재 씬 `Assets/Scenes/Main.unity`와 교재 `Assets/Prefabs/Zombie.prefab`은 참조 관계가 남아 있어 그대로 두었다. 건드리지 말 것.

## 마일스톤 진행도

- M0 프로젝트 진단/에셋 검증 - 완료.
- M1 플레이어와 카메라 프로토타입 - 완료.
- M2 전투 수직 슬라이스 - 완료(2026-09-20).
- M3 루팅과 캐릭터 관리 - 완료(2026-09-20). 무게제 인벤토리, 6슬롯 장비, 방어 계산, 인벤토리/장비/상태 화면, 상자 루팅까지 구현했다.
- M4 파밍과 거점 MVP - 완료(2026-09-20). 자원 채집, 자유 배치 건설, 건설물 4종, 철거, 저장 DTO 표현까지 구현했다.
- M5 오토바이 - 완료(2026-09-20). 탑승/하차, 주행, 연료·내구도, 저장 DTO 표현, 주행 HUD까지 구현했다.
- M6 시드 월드와 스트리밍 - 완료(2026-09-20). 결정론적 생성, 연결성 검증, 청크 스트리밍, 런타임 NavMesh, School POI까지 구현했다.
- M7 저장 시스템 - 완료(2026-09-20). 청크 콘텐츠 배치, 오토바이·좀비 스폰, 저장 DTO와 원자적 파일 입출력, 수집/복원, 10슬롯 UI와 자동저장까지 구현했다.
- M8-UI 팰월드풍 UI 리뉴얼 - 완료(2026-09-20). 아이콘 생성, 캐릭터 창 탭 통합, 상세 패인, 전 화면 테마 통일, HUD 재배치까지 구현했다.

## M8-UI에서 만든 것 (2026-09-20)

- **UI 한 곳 규칙**: 모든 게임 내 UI는 `Assets/Prefabs/HUD Canvas.prefab` **안에서** 만든다. 씬의 HUD Canvas는 프리팹 인스턴스라 **씬에서는 자식을 옮길 수 없다**(이름만 바뀌고 자리는 그대로다). `PrefabUtility.LoadPrefabContents`로 프리팹을 열어 고치고 저장한다. `World.unity`와 `Prototype.unity`가 같은 UI를 받는다.
- **테마**는 `Assets/Scripts/UI/UiTheme.cs` 한 곳에 있다(어두운 반투명 패널 + 호박색 강조 + 막대 색 규칙). 새 UI는 여기 값을 쓴다.
- **아이콘**: `Assets/Editor/ItemIconGenerator.cs`가 프리팹 미리보기를 찍어 `Assets/Art/ItemIcons/*.png`로 저장하고 `ItemData.icon` / `BuildableData.icon`에 연결한다. 메뉴 `Tools/아이템 아이콘 생성`.
  - **미리보기는 첫 호출에 항상 null**이라 여러 번 나눠 불러야 한다. eval은 메인 스레드 5초 제한이 있어 `Generate(false, 4)`처럼 끊어 돌린다.
  - 배경(회색)은 테두리 채우기로 지우고 감마 0.62로 밝힌다. 모델이 없는 방어구 3종은 색 타일이다.
- **캐릭터 창**: `CharacterScreenUI`(창) + `TabView`(탭). `I`/`O`/`K`가 각각 인벤토리/장비/상태 탭으로 같은 창을 열고, 같은 키를 다시 누르면 닫힌다. `CLAUDE.md` §11.1과 `GAME_DESIGN.md` §10을 이 결정에 맞춰 갱신해 두었다.
- **줄 프리팹 하나를 다섯 곳에서 재사용한다**: `Assets/Prefabs/UI/Item Row.prefab`(아이콘 + 이름 + 부가설명 + 우측 글자). 인벤토리, 장비 슬롯/후보, 보관 상자 양쪽, 건설 메뉴가 전부 이걸 쓴다. 우측 글자로 역할을 표시한다(개수/해제/장착/넣기/꺼내기/건설).
- **동작은 버튼이 아니라 줄 클릭**이다. 인벤토리만 예외로 오른쪽 상세 패인에 사용/장착/버리기 버튼이 있다.
- **HUD**: 좌하단 체력바(플레이어 프리팹의 슬라이더를 그대로 쓰고 `HealthBarLabel`이 수치를 겹친다) / 그 위 차량 연료·내구 막대 / 우하단 무기·탄약(`WeaponHudUI`) / 중앙 하단 상호작용 배지 / 상단 중앙 건설 안내.
- **주의할 함정**
  - **오버레이 캔버스는 `capture_game_view`에 안 찍힌다.** 눈으로 보려면 캔버스를 잠시 `ScreenSpaceCamera`로 바꾼다(플레이 중 변경이라 종료하면 되돌아간다).
  - **`Mask` + 알파 0 이미지로 스크롤 뷰포트를 만들면 내용이 전부 사라진다.** `RectMask2D`를 쓸 것.
  - **레이아웃 값은 다음 프레임에 확정된다.** `Refresh()` 직후 같은 프레임에 `rect.size`를 재면 0이 나온다.
  - **UI 버튼/막대는 스프라이트 없이** 만든다(`Image.type = Filled`는 스프라이트가 필요하므로 `anchorMax.x`로 채운다).
  - 플레이 중에 고친 씬 값은 종료하면 사라진다. 레이아웃 수정은 에디터 상태에서 하고 저장할 것.

## M7에서 만든 것 (2026-09-20)

- **청크 콘텐츠**. `ChunkLibrary`에 자원 노드/상자 프리팹과 청크 타입별 개수(`ChunkContentRule`)를 두고, `WorldChunkBuilder`가 칸 시드로 결정론적으로 뿌린다. 배치 순번이 그대로 id(`11_10_res_0`)가 된다.
  - 지붕·바위 위에 얹히지 않도록, 후보 지점 바로 위에서 내려봤을 때 먼저 닿는 것이 바닥 타일일 때만 받아들이고 최대 8번 다시 뽑는다.
  - 시작 청크 중앙은 스폰 지점이므로 `WorldStreamer.startClearRadius`(4m)만큼 비운다.
- **`WorldRuntimeState`**. 채집/루팅 여부를 청크 밖에서 id로 들고 있다. 청크가 내려갔다 올라와도 상태가 이어지고, 자원은 남은 시간이 지나면 다시 자란다.
- **저장 시스템** `Assets/Scripts/Save/`. `SaveRegistry`(id -> 에셋) / `SaveData` 계열 DTO / `SaveSystem`(경로·원자적 쓰기·목록·삭제) / `SaveManager`(수집·복원·자동저장).
  - 슬롯 10개, 슬롯마다 `slot{N}.json`(수동)과 `slot{N}.auto.json`(자동)으로 나눠 둔다. 경로는 `Application.persistentDataPath/Saves`.
  - 쓰기는 `.tmp`에 먼저 쓰고 교체한다. 실패하면 `.tmp`를 지우고 false를 돌려주며 기존 저장은 그대로 남는다(검증 완료).
  - **새 아이템/건설물을 만들면 `Assets/ScriptableData/Save Registry.asset`에 등록할 것.** 안 하면 불러오기에서 조용히 사라진다.
- **복원 순서가 중요하다.** 플레이어를 먼저 옮기고 → 시드가 다르면 `WorldStreamer.Regenerate` → 채집/루팅 상태 복원 → 건설물 → 오토바이 순이다. 장비는 인벤토리를 가리키므로 인벤토리를 먼저 채운다.
- **슬롯 UI** `SaveSlotUI`(HUD Canvas, `ESC`). 줄마다 수동 요약 + 자동 요약 + 저장/불러오기/자동 불러오기/삭제 버튼. 줄 프리팹은 `Assets/Prefabs/UI/Save Slot Row.prefab`.
- **자동저장**은 기본 300초마다 `currentSlot`의 **자동 칸에만** 쓴다. 수동 저장은 절대 덮지 않는다.
- **좀비 스폰 방식을 바꿨다.** World 씬은 교재의 `ZombieSpawner`(고정 지점 + 무한 웨이브) 대신 `WorldZombieSpawner`를 쓴다. 플레이어 주변 25~45m 고리의 NavMesh 위에 최대 8마리를 유지하고 90m 밖은 정리한다. 좀비는 저장 대상이 아니다.
- **`ItemSpawner`는 World 씬에 없다.** 상자 루팅과 자원 채집이 공급원이다(사용자 결정).
- **주의할 함정**
  - **오버레이 캔버스는 `capture_game_view`에 안 찍힌다.** UI 배치를 눈으로 보려면 캔버스를 잠시 `ScreenSpaceCamera`로 바꿀 것.
  - **플레이 중에 고친 씬 값은 종료 시 사라진다.** 패널 크기를 플레이 중에 고쳐두고 한 번 날렸다.
  - **UI 버튼은 중심 기준**이라 오른쪽 앵커에서 폭의 절반만큼 더 안쪽에 두어야 패널을 안 벗어난다.
  - 주기적 동작은 ‘다음 시각’이 아니라 ‘마지막 시각 + 주기’로 판정해야 런타임에 주기를 바꿔도 바로 반영된다.

## M6에서 만든 것 (2026-09-20)

- **`Assets/Scripts/World/`**. `WorldGenerator`(static, 순수 계산) / `WorldChunkData` / `WorldMap` / `WorldGenerationSettings` / `WorldConnectivityReport` / `ChunkLibrary`(프리팹 모음) / `WorldChunkBuilder` / `WorldStreamer` / `WorldNavMeshBaker`.
- **생성**: 시드 + `generatorVersion`으로 결정론적. 50m 청크 20x20 = 1km. 도로를 격자로 깔아 연결성을 구조적으로 보장하고 BFS로 따로 검사한다. **400청크 생성은 0.0ms** - 비용은 인스턴스화에 있다.
  - **`UnityEngine.Random`을 쓰지 않는다**(전역 상태라 결정론이 깨진다). 칸 시드는 좌표 해시로 뽑아 생성 순서와 무관하다.
  - `generatorVersion`은 현재 **2**(POI 주변 칸 비우기 규칙 추가). 규칙을 바꾸면 반드시 올릴 것.
- **스트리밍**: `loadRadius`(기본 2) 안의 청크만 올린다. 월드 경계에서는 자동으로 줄어든다.
- **배치**: 에셋 피벗이 모서리 기준이고 축마다 달라서 **좌표를 하드코딩하지 않고 인스턴스화 후 bounds에서 계산**한다. 스케일도 `FitScale`로 긴 축을 맞춘다(균등 스케일만).
- **NavMesh**: `WorldNavMeshBaker`가 저수준 `NavMeshBuilder`로 플레이어를 따라다니며 비동기 갱신한다. 건설물에는 `NavMeshObstacle`(carve)이 붙어 있다.
- **POI**: `Assets/Prefabs/World/School POI.prefab`(School 씬에서 건물 본체만 추출, 렌더러 58 / 콜라이더 21 / 85x10x62m).
- **주의할 함정**
  - **`NavMeshSurface`를 청크마다 두면 안 된다.** 분리된 `NavMeshData`끼리는 이어지지 않아 좀비가 청크 경계를 못 넘는다. 그리고 `UpdateNavMesh`는 볼륨이 움직여도 재수집하지 않는다(정점 1160 -> 122를 확인했다). 그래서 `NavMeshBuilder`를 직접 쓴다.
  - **런타임 NavMesh 베이크는 메시 Read/Write가 필요하다.** 없으면 **에디터에서만 동작하고 빌드하면 실패**한다. `Assets/Apocalyptic_World`의 모델 132개에 켜뒀다. 새 에셋을 월드에 쓰면 똑같이 켤 것. 메모리 비용은 M8 사안이다.
  - **NavMesh는 청크가 다 올라온 뒤에 구워야 한다.** `Start()`에서 구우면 `WorldStreamer.Start`와 순서가 안 정해져 빈 NavMesh가 나온다. `WorldStreamer.onChunksChanged`를 구독할 것.
  - **`World.unity`는 원점에서 수백 m 떨어진 곳에서 시작한다.** `ThirdPersonCameraController`에 첫 프레임 스냅이 들어 있으니 지우지 말 것.
  - 지면 위에 무언가를 놓을 때는 **자기 콜라이더를 제외하고** 레이캐스트할 것. 안 그러면 자기 키만큼 공중에 뜬다.
  - **에디터가 포커스를 잃으면 Play가 멈춘다**(`Application.runInBackground=False`). 검증 중 시간이 안 흐르면 `Time.frameCount`를 먼저 의심할 것.

## M5에서 만든 것 (2026-09-20)

- **오토바이 프리팹** `Assets/Prefabs/Motorcycle.prefab`. 에셋 `P_RSG_Bike_B_Dirty_URP`(Rigidbody + `Gadd420.BicycleVehicle` + WheelCollider 2 + 콜라이더 17 + CenterOfMass)를 언팩해 쓰고 `Seat`/`Exit Point`/`Motorcycle`을 붙였다.
  - **서드파티 코드는 한 줄도 수정하지 않았다.** `Motorcycle.Awake`가 `BicycleVehicle`을 타입 이름으로 찾아 `enabled`만 토글한다. 프리팹의 `Input_Manager`는 꺼두고 `Input_Compat` 폴백(레거시 `Horizontal`/`Vertical` 축)을 쓰게 했다.
  - 오토바이는 런타임에 움직이므로 `NavigationStatic`을 주지 않는다. NavMesh 재베이크도 필요 없다.
- **`Motorcycle`**(차량) / **`RiderControl`**(플레이어)로 책임을 나눴다. 차량은 탑승 상태·연료·내구도의 소유자이고, 플레이어의 조작·물리·애니메이션·카메라 전환은 `RiderControl`이 한다.
  - 탑승 시: `PlayerMovement`/`PlayerShooter`/`BuildPlacer` 비활성, `Rigidbody.isKinematic = true`, 콜라이더 끄기, `Seat`에 부착, `Mounted` 포즈, **`Weapon Hold Arms` 레이어 가중치 0**, 카메라 `target`만 오토바이로 교체.
  - `PlayerInteractor`는 끄지 않는다(끄면 `E`로 못 내린다). 대신 타고 있는 동안 `currentTarget`을 차량으로 고정한다.
- **연료/내구도**. 연료는 **입력이 아니라 실제 이동 거리**로 소모한다(`fuelPerMeter`). 둘 중 하나라도 0이면 `canDrive`가 거짓이 되어 `BicycleVehicle`이 꺼진다. 타는 것 자체는 막지 않고 안내가 `(연료 없음)`/`(고장)`으로 바뀐다.
  - 주유 `R`(휘발유통 1개당 40), 수리 `F`(고철 5개당 30). 입력은 `PlayerInteractor`에 있고 `Motorcycle`은 `Refuel`/`Repair`만 공개한다.
- **`MotorcycleSaveData`**(위치/회전Y/연료/내구/탑승 여부)와 `ToSaveData`/`LoadFromSaveData`/`ToJson`. M4의 `PlacedBuilding`과 같은 형태라 M7에서 한 파일로 묶기 쉽다. 복원은 `Mathf.Clamp`로 범위를 가둔다.
- **`VehicleHudUI`** - 탑승 중 연료/내구도 표시, 고갈 시 경고.
- **주의할 함정**
  - **복합 콜라이더 물체의 충돌 콜백은 여러 번 들어온다.** 오토바이는 콜라이더가 17개라 한 번 부딪혀도 `OnCollisionEnter`가 여러 번 호출돼 내구도가 계산값의 두 배(45.6 vs 25)로 깎였다. `crashCooldown = 0.5초`로 막았다.
  - **캐릭터 치수는 본이 아니라 메시 렌더러 bounds로 재라.** 이 프로젝트의 플레이어에는 Survivalist 본(`Hips`/`Left_Foot`)과 마네킹 본(`pelvis`/`foot_l`) 두 벌이 섞여 있고, 아바타가 `Hips`를 루트 근처(y≈0)에 매핑해서 "머리-엉덩이 거리" 같은 지표가 전혀 맞지 않는다. 부츠(`SK_Military_Boots3`)와 모자(`SK_Military_Cap1`)의 bounds를 쓸 것(키 약 1.89m).
  - **물리로 계속 움직이는 대상의 왕복 테스트는 한 eval 안에서 끝내라.** 프레임이 지나면 위치도 연료도 실제로 변해서 스냅샷과 어긋난 것처럼 보인다.
  - 라이더의 **다리가 약간 길게 내려온다.** Survivalist와 원본 마네킹의 비율 차이에서 오는 리타게팅 아티팩트다. 손·발 IK는 폴리싱 단계로 미뤘다.

## M4에서 만든 것 (2026-09-20)

- **자원 채집** `ResourceNode : LivingEntity`. 나무/바위/고철을 **총으로 쏴서** 내구도를 깎고 부수면 재료 픽업이 드랍된다. 부서지면 오브젝트를 파괴하지 않고 콜라이더·렌더러만 꺼두었다가 `respawnTime`(기본 60초) 뒤 되살린다. 씬의 `Resource Nodes` 아래 7개 배치.
  - **`Gun.cs:114`가 `hit.collider.GetComponent<IDamageable>()`를 쓴다(`GetComponentInParent`가 아니다).** 총에 맞아야 하는 오브젝트는 **로직과 콜라이더가 같은 GameObject에** 있어야 한다. 에셋 프리팹의 자식 콜라이더는 전부 제거하고 루트에만 콜라이더를 뒀다.
  - 반대로 **상호작용(`IInteractable`)은 `GetComponentInParent`로 찾으므로 자식 콜라이더여도 된다.** 두 규칙이 반대라는 걸 헷갈리지 말 것.
- **재료 아이템** 목재/돌/고철. 이를 위해 `ItemData`의 abstract를 해제했다(그대로 쓰면 효과 없는 재료, 상속하면 소비/무기/방어구).
- **건설 시스템** `Assets/Scripts/Building/`.
  - `BuildableData`(정적 정의: 프리팹, 재료, 겹침 판정 상자) / `PlacedBuilding`(순수 데이터: id, 위치, 회전, 열림) / `BaseBuildState`(설치 목록 소유자) / `PlacedBuildingLink`(씬 오브젝트 <-> 기록 연결).
  - `BuildPlacer`가 반투명 프리뷰, 휠 회전(15도), 겹침 판정, 재료 차감, `X` 철거(재료 전액 환급)를 담당한다. `BuildMenuUI`(`B` 키)가 목록을 보여준다.
  - 건설물 4종: 벽(목재10+돌5) / 바리케이드(목재6+고철4) / 문(목재8+고철6) / 보관 상자(목재15).
  - **보관 상자는 자기 `Inventory` 컴포넌트를 내용물로 쓴다**(`[RequireComponent]`). `StorageUI`가 `StorageContainer.onOpenRequested` static 이벤트를 구독해 열린다(상자는 UI를 모른다).
  - **문은 `Hinge` 자식에 콜라이더를 붙여** 열리면 콜라이더가 함께 비켜난다. 열림 상태는 `PlacedBuildingLink.record.isOpen`에 기록되고 `Start`에서 복원한다.
  - `BaseBuildState.ToJson()`이 설치 상태를 씬 참조 없이 직렬화한다. **이 출력이 곧 M7 저장 DTO의 형태다.**
- **주의할 함정**
  - **3인칭에서 사정거리를 카메라 기준으로 재면 안 된다.** 카메라가 플레이어 뒤 3.4m에 있어서 `demolishDistance=4`가 실제로는 앞 0.6m밖에 안 됐다. 레이 길이는 `카메라-플레이어 거리 + 사정거리`로 잡고 판정은 플레이어 기준으로 한다. 건설 프리뷰도 같은 이유로 "사정거리 끝에서 아래로 재투영"하는 보정이 들어가 있다.
  - **`Destroy`는 프레임 끝에야 처리된다.** 이 세션에서만 세 번 물렸다(UI 줄 중복, 프리뷰 자기 콜라이더, 목록 재생성). 런타임에 즉시 없애야 하면 `enabled = false`를 쓴다.
  - 프리뷰는 컴포넌트를 **지우지 않고 끈다.** `RequireComponent`로 묶인 컴포넌트는 제거가 거부되고 콘솔 에러만 남는다(`StorageContainer -> Inventory`).
  - **씬에 정적 장애물을 놓으면 NavMesh 재베이크 후 스폰 지점과 필수 경로를 샘플링해 확인할 것.** 나무를 `(-14,0,2)`에 놓았다가 `Spawn Point 1`이 NavMesh에서 파여나간 적이 있다. 정점 수만 보면 놓친다.
  - **건설물은 NavMesh에 반영되지 않는다.** 런타임 생성물이라 베이크로 못 막는다. 물리 콜라이더로만 막으므로 좀비는 경로를 벽 너머로 계산하고 실제로는 밀린다. 정식 처리(`NavMeshObstacle` carving)는 M6 사안이다.
  - **문틀이 없다.** 문짝만 있어서 옆으로 돌아갈 수 있다. 벽과 조합해 쓰는 전제다.

## M3에서 만든 것 (2026-09-20)

- **아이템 데이터** `Assets/Scripts/Items/`. `ItemData`(abstract) 아래 `ConsumableItemData`(Heal/Ammo/Score enum + 수치), `WeaponItemData`(슬롯 + 무기 프리팹), `ArmorItemData`(슬롯 + 방어력) 3종. 런타임 상태는 `ItemStack`(ScriptableObject 아님). 에셋은 `Assets/ScriptableData/Items/`에 9개.
- **`Inventory`** - 칸 제한 없이 총 무게(`maxWeight` 기본 40kg)로만 제한한다. **무게를 넘어도 획득은 막지 않고 이동속도만 깎는다**(`PlayerMovement.overweightSpeedMultiplier` 기본 0.5). `Add`/`Remove`/`Use`/`DropAt`/`CountOf`와 `onChanged` 이벤트를 공개하고 UI를 모른다.
- **`Equipment`** - 6슬롯(주무기/보조무기/근접/머리/상체/하체). **아이템을 인벤토리에서 빼가지 않고 "무엇을 장착 중인지"만 가리킨다.** 그래야 무게가 두 번 세어지지 않는다. `inventory.onChanged`를 구독해 버리거나 소진된 장착품을 자동 해제한다.
- **방어 계산** - `PlayerHealth.OnDamage`에서 `Mathf.Max(1f, damage - equipment.totalArmor)`. 사용자가 고른 고정 수치 차감 방식이다.
- **`PlayerShooter`의 `weaponPrefabs` 하드코딩 배열은 제거됐다.** 이제 `equipment.Get(activeSlot)`의 `WeaponItemData.weaponPrefab`을 쓴다. 숫자키 1/2가 주무기/보조무기다(`PlayerInput`은 그대로 두고 `PlayerShooter`가 해석). 시작 장비는 `Equipment.startingItems`(돌격소총 + 권총).
- **UI** - `ScreenPanel`(abstract) 아래 `InventoryUI`(`I`), `EquipmentUI`(`O`), `StatusUI`(`K`). **한 번에 하나만 열린다.** 열려 있는 동안 `UIManager.isScreenOpen`이 참이 되어 `PlayerInput`/`PlayerMovement.Rotate`/`ThirdPersonCameraController.LateUpdate`가 입력을 무시하고 커서가 풀린다. 별도로 `InteractionPromptUI`가 상자 안내를 띄운다.
- **상자** - `LootContainer` + `PlayerInteractor`(`E`, 반경 2.5m) + `Loot Chest.prefab`. `Prototype` 씬에 `Loot Chests` 아래 3개 배치.
- **주의할 함정 몇 가지**
  - 맨손 상태(무기 미장착)에서 서드파티 `IKHelperTool`이 파괴된 이펙터를 참조해 매 프레임 예외를 던진다. `PlayerShooter`가 맨손일 때 `ikHelperTool.enabled = false`로 꺼서 막는다. **이 컴포넌트를 마음대로 켜면 안 된다.**
  - UI 목록을 다시 그릴 때는 `ScreenPanel.ClearRows`를 쓴다. `Destroy`만 하면 같은 프레임에 두 번 그릴 때 줄이 중복된다.
  - 상자 에셋의 URP 프리팹은 텍스처 없는 FBX 내장 머티리얼을 참조한다. 새 상자를 쓸 때는 `Material/URP/M_Chest NN.mat`으로 직접 교체해야 한다.
  - **씬에 정적 장애물을 놓으면 `NavigationStatic`을 주고 NavMesh를 다시 베이크할 것.** 상자 3개를 놓고 재베이크해서 정점이 16 -> 108이 됐다.

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
- **사용자 수동 확인 필요**: 키 바인딩 전체. 같은 이유로 키/마우스 입력을 합성할 수 없다. 코드 경로와 UI 버튼 클릭은 전부 검증했고 남은 것은 바인딩 자체뿐이다.
  - `I` 인벤토리 / `O` 장비 / `K` 상태 / `B` 건설 메뉴 / `T` 보관 상자 닫기
  - `E` 상호작용(루팅 상자·보관 상자·문·오토바이 타기/내리기) / `X` 철거 / `1`·`2` 주무기·보조무기
  - `R` 오토바이 주유 / `F` 오토바이 수리 / `WASD` 주행
  - 건설 모드: 휠 회전 / 좌클릭 설치 / 우클릭·ESC 취소
- **사용자 수동 확인 필요**: `World.unity` 플레이. **에디터가 포커스를 잃으면 게임이 멈춘다**(Run In Background 꺼짐). 사용자 소유 설정이라 켜지 않았다.
- **사용자 판단 필요**: 도로 폭이 21m라 50m 칸의 나머지가 풀밭이다. 도로 칸에 서 있어도 풀 위일 수 있다.
- **사용자 판단 필요**: 총으로 자원을 부수는 채집 방식의 조작감. 사용자가 이 방식을 택했지만 실제로 해보고 어색하면 `E` 즉시 채집으로 바꾸는 비용은 작다(`ResourceNode`의 진입점만 교체).
- **사용자 수동 확인 필요**: `ESC` 저장 화면의 실제 키 조작과 버튼 클릭 감각. 코드 경로와 버튼 `onClick`은 전부 검증했고 남은 것은 입력 바인딩뿐이다.
- `FindObjectOfType` 폐지 경고 정리. 기능 영향 없음, 폴리싱 단계로 미뤘다.
- 왼손 IK의 무기별 2~3cm 오차. 조준에 영향 없음, 폴리싱 단계로 미뤘다.
- 근접무기(멜리) 전투. `Crusader Weapon`/`Free medieval weapons` 에셋이 있으나 스윙/히트박스 기반의 다른 전투 방식이 필요해 M8 콘텐츠 사안으로 미뤘다. `IDamageable`은 재사용 가능하지만 애니메이션/입력/판정은 새로 설계해야 한다.

## 권장 다음 작업

**M8 콘텐츠와 최적화.** M1~M7로 수직 슬라이스가 닫혔다(이동·전투·루팅·인벤토리·건설·오토바이·시드 월드·저장). 우선순위 후보는 다음과 같다.

- **사용자 수동 플레이 피드백이 먼저다.** 키 바인딩·ADS 조작감·오토바이 주행감·채집 방식은 자동 검증이 불가능해 `checklist.md`에 미완료로 남아 있다. 이것부터 확인받고 우선순위를 정하는 편이 좋다.
- **저장 시스템 후속**: 시작 시 자동 불러오기(타이틀/이어하기 흐름)이 아직 없다. 지금은 슬롯 화면에서만 불러온다.
- **루팅 테이블**: 모든 상자가 같은 내용물(Ammo Box 1 / Bandage 1 / Material Scrap 2)을 준다. 칸 타입별로 다른 전리품을 넣으려면 루팅 테이블이 필요하다.
- **POI 실내 루팅**: 현재 자원/상자는 바닥 타일 위에만 놓인다. 학교 실내에 전리품을 놓으려면 실내 바닥을 받아들이는 규칙이 따로 필요하다.
- **최적화**: 런타임 NavMesh 베이크가 596ms로 가장 비싸다. 메시 Read/Write를 켜둔 모델 132개의 메모리 비용도 이 단계 사안이다.
- **콘텐츠**: 근접무기, 좀비 종류 확장, POI 추가.

## 최근 커밋

- `7b78149 HUD를 좌하단 체력·우하단 무기·막대형 차량 상태로 재배치`
- `6484d19 장비·상태·보관·건설·저장 화면을 같은 테마로 통일`
- `27c9583 인벤토리 탭을 아이콘 목록과 상세 패인으로 재구성`
- `a6694db 인벤토리·장비·상태를 캐릭터 창 탭으로 통합`
- `64c44ff UI 테마와 아이템 아이콘 13종 생성`
- `f54cf78 M8-UI 팰월드풍 UI 리뉴얼 계획과 체크리스트 작성`
- `bc9c746 인수인계 메모를 M7 완료 시점 기준으로 갱신`
