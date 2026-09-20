# TPS 카메라와 ADS 구현 계획

## 목표

`Prototype` 씬에서 플레이어 뒤쪽의 숄더뷰를 기본 카메라로 사용하고, 우클릭 중에는 오른쪽 어깨 너머의 근접 ADS 카메라로 전환한다.

## 완료 조건

- 마우스 X/Y로 플레이어와 카메라를 회전할 수 있다.
- 기본 시점은 플레이어 뒤·오른쪽 어깨에서 플레이어를 바라본다.
- 우클릭 중 ADS 위치와 FOV로 부드럽게 전환하고, 버튼을 놓으면 기본 시점으로 복귀한다.
- `Gun`의 화면 중앙 조준 레이가 전환 중인 실제 카메라를 사용한다.
- Unity Play Mode에서 이동, 카메라 전환, 사격이 오류 없이 동작한다.

## 구현 순서

1. 카메라 리그와 TPS/ADS 전환 컴포넌트를 추가한다.
2. `Prototype`에 플레이어와 최소 테스트 공간을 배치하고 연결한다.
3. Play Mode에서 조작과 콘솔 오류를 검증한다.

## TPS 이동과 플레이어 비주얼 보완

1. 수평 입력을 회전 전용이 아닌 스트레이프 이동에도 사용한다.
2. 기획서에서 3인칭 우선 후보로 지정한 `Survivalist/Prefab/PlayerArmature.prefab`을 플레이어 비주얼로 사용한다.
3. 기존 전투 루트와 총기 참조를 유지하고, 임시 여성 메시만 교체한다.
4. Play Mode에서 전후·좌우 이동과 Survivalist 비주얼 렌더링을 확인한다.

## Survivalist 이동 애니메이션과 무기 손 그립

1. 기존 전투 루트와 Survivalist 자식 Animator의 실제 컨트롤러·파라미터를 확인한다.
2. 이동 입력, 점프, 지면 상태를 Survivalist Animator에 전달한다.
3. PlayerShooter의 양손 IK를 Survivalist Animator에 적용한다.
4. Play Mode에서 이동 애니메이션, 손 위치, 콘솔 오류를 확인한다.

## Survivalist 총기 피벗 보정

1. Survivalist 오른손·총기 피벗·총구의 실제 월드 좌표를 비교한다.
2. 오른손 그립과 총기 손잡이가 일치하도록 피벗 위치와 회전을 매 프레임 계산한다.
3. 왼손만 보조 IK로 총기 전방 손잡이에 맞춘다.
4. Play Mode에서 총구가 캐릭터 배가 아닌 총기 모델에서 출발하는지 확인한다.

## IK Helper Tool 기반 왼손 그립

1. IK Helper Tool의 소총 예제와 문서에서 필요한 컴포넌트·효과기·스위치를 확인한다.
2. 기존 직접 왼손 IK를 `KevinIglesias.IKHelperTool`로 교체한다.
3. 왼손 효과기와 IK 스위치를 Survivalist 프리팹에 연결한다.
4. Play Mode에서 왼손 그립, 동적 총기 피벗, 콘솔 오류를 확인한다.

## M2 전투 수직 슬라이스 (2026-09-20)

### 목표

`Prototype` 씬에서 3인칭 플레이어가 좀비와 실제로 싸우고, 죽으면 다시 시작할 수 있게 한다. 좀비 비주얼은 구매 에셋 `Assets/Zombie`의 `Zombie3`를 사용한다.

### 사전에 확인한 사실

- `Prototype.unity`의 루트는 `Main Camera` / `Directional Light` / `Test Ground` / `Player Character` / `HUD Canvas` 5개뿐이다. 전투 요소가 하나도 배치돼 있지 않다.
- 코드와 에셋은 이미 다 있다. `Zombie.cs`, `ZombieSpawner.cs`, `ItemSpawner.cs`, `GameManager.cs`, `ZombieData`(Default/Fast/Heavy), `AmmoPack/HealthPack.prefab`, `Spawn Points.prefab`(4개, ±14 범위), `BloodSprayEffect.prefab`, `Zombie Damage/Die.wav`.
- `Test Ground`는 원점 기준 50x50이고 `Spawn Points`는 그 안에 들어간다.
- 발견한 블로커 3개.
  - `NavMesh.CalculateTriangulation().vertices.Length == 0` - 씬에 NavMesh가 없다. `Zombie.cs:42`가 `NavMeshAgent`를 쓰므로 추적이 아예 불가능하다.
  - `Test Ground`의 static flags가 0이다. 그대로 베이크하면 아무것도 안 나온다.
  - `Player Character`의 레이어가 0(Default)인데 `Zombie.whatIsTarget`은 512(레이어 9 `Player`)다. 즉 좀비가 플레이어를 **절대 탐지하지 못한다**.
- `Zombie3.prefab`은 SkinnedMeshRenderer가 1개라 `Zombie.cs:48`의 `GetComponentInChildren<Renderer>()`가 의도대로 전신에 `skinColor`를 적용한다. 구조도 교재 프리팹과 같아(루트 Animator + `Base HumanPelvis` 스켈레톤과 메시가 형제) 아바타 경로가 맞는다.
- 에셋 `Zombie.controller`에는 파라미터가 없다. `HasTarget`(Bool) / `Die`(Trigger) / `Attack`(Trigger)를 가진 컨트롤러를 새로 만들어야 한다.

### 완료 조건

- 같은 씬에서 좀비가 플레이어를 탐지하고 NavMesh로 추적한다.
- 총기 3종으로 좀비에게 데미지를 주고 죽일 수 있다. 피격 시 `BloodSprayEffect`와 피격음이 난다.
- 좀비가 접촉 시 공격 애니메이션을 재생하며 플레이어 체력을 깎는다.
- 좀비 사망 시 사망 애니메이션이 재생되고 10초 뒤 파괴된다.
- 탄약/회복 아이템이 플레이어 근처 NavMesh 위에 드랍된다.
- 플레이어가 죽으면 게임오버 상태가 되고, 키 입력으로 씬을 다시 시작할 수 있다.
- `compilationFailed: false`, `consoleErrors: 0`.

### 구현 순서

1. `Player Character`의 레이어를 `Player`(9)로 바꾼다 -> 검증: `Physics.OverlapSphere`로 좀비 탐지 레이어에 걸리는지 확인.
2. `Test Ground`에 Navigation Static을 설정하고 NavMesh를 베이크한다 -> 검증: `NavMesh.CalculateTriangulation().vertices.Length > 0`.
3. `Zombie3`를 루트로 하는 새 좀비 프리팹을 만들고 `NavMeshAgent` / `AudioSource` / 솔리드 콜라이더 / 트리거 콜라이더 / `Zombie.cs` / `BloodSprayEffect` 자식을 붙인다 -> 검증: 필드 참조 전부 non-null.
4. 에셋 클립으로 `HasTarget` / `Die` / `Attack` 컨트롤러를 만들어 연결한다. 이동 클립은 루트 모션이 `NavMeshAgent`와 충돌하지 않도록 `_InPlace` 변종을 쓴다 -> 검증: 파라미터 3개 존재, Play Mode에서 상태 전이 확인.
5. `Zombie.cs`의 `OnTriggerStay` 공격 성공 지점에 `SetTrigger("Attack")` 1줄을 추가한다 -> 검증: 접촉 시 공격 애니메이션 재생.
6. `GameManager` / `ZombieSpawner` / `ItemSpawner` / `Spawn Points`를 씬에 배치하고 참조를 연결한다 -> 검증: Play Mode에서 웨이브 스폰과 아이템 드랍 확인.
7. `GameManager`에 게임오버 상태에서의 씬 재시작 입력을 추가한다. 삭제된 게임오버 UI는 사용자가 의도적으로 없앤 것이므로 다시 만들지 않는다 -> 검증: 사망 후 재시작으로 초기 상태 복귀.
8. Play Mode 전체 루프 검증 후 커밋/push.

### 의도적으로 범위에서 제외

- 게임오버/웨이브/스코어 UI 재도입. 사용자가 명시적으로 삭제한 것이다.
- 근접무기 전투. M8 콘텐츠 확장 사안이다.
- `Zombie1`(14개 렌더러 절단용 변종)을 이용한 부위별 데미지.
- `FindObjectOfType` 폐지 경고 정리. 별도 작업으로 남긴다.

## M3 루팅과 캐릭터 관리 (2026-09-20)

### 목표

`Prototype` 씬에서 플레이어가 아이템을 주워 무게제 인벤토리에 담고, 인벤토리에서 사용하고, 장비 화면에서 무기/방어구를 장착하고, 상태 화면에서 수치를 확인하고, 필드의 상자를 열어 아이템을 얻을 수 있게 한다.

### 사용자가 확정한 설계 결정 (2026-09-20)

- **인벤토리는 무게제다.** 칸 개수 제한이 없고 총 무게로만 제한한다. 무게 초과 시 이동속도가 감소한다.
- **기존 즉시효과 아이템(AmmoPack/HealthPack/Coin)은 인벤토리 아이템으로 전환한다.** 밟으면 인벤토리에 들어가고, 인벤토리에서 사용해야 효과가 난다.
- **장비 슬롯은 6개다.** 주무기 / 보조무기 / 근접 / 머리 / 상체 / 하체.
- **방어구는 수치만 있고 비주얼은 고정이다.** 캐릭터 외형은 장착 여부에 따라 바뀌지 않는다.
- **방어 계산은 고정 수치 차감이다.** `final = Max(1, damage - totalArmor)`.

### 사전에 확인한 사실

- `Assets/Scripts`에 인벤토리/장비/상자 계열 스크립트가 하나도 없다. 전부 신규다.
- 기존 아이템 3종은 `IItem.Use(GameObject)` 하나만 구현하고 **자신을 즉시 `Destroy`한다**. `PlayerHealth.OnTriggerEnter`가 `IItem`을 찾아 바로 `Use`를 호출하는 구조라, 인벤토리 경유로 바꾸려면 이 두 지점을 같이 고쳐야 한다.
- `PlayerShooter.weaponPrefabs`는 하드코딩된 3종 고정 배열이고 `Start()`에서 `EquipWeapon(0)`을 부른다. 장비 시스템이 이 배열을 대체해야 한다.
- `Gun`은 탄약(`ammoRemain`)을 자기 안에 들고 있다. `AmmoPack.Use`가 `playerShooter.gun.ammoRemain`을 직접 더한다.
- `PlayerMovement.Move()`가 `moveSpeed` 필드를 직접 쓴다. 무게 페널티는 여기 한 곳만 손대면 된다.
- 방어구 비주얼은 불가능하다고 판단한 근거: Survivalist는 파츠 14개로 분리돼 있지만(`Cap1`/`Vest1_4`/`Shirt3`/`Pants2`/`Boots3` 등), **하체는 `Pants2` 하나뿐이고 맨다리 메시가 없다.** 끄면 다리가 사라진다. 사용자가 이 제약을 보고 "전부 수치만"을 선택했다.
- 상자 에셋은 `Assets/Ditag Design/Mesh Pack/Chest 01/Model/SM_Chest01~16.fbx`다(기획서 §15의 `Realistic Crate & Chest Bundle`). 추가로 `Assets/Apocalyptic_World/.../SM_Props_Box_01/02.prefab`도 쓸 수 있다.

### 전제로 깔고 가는 기본값 (사용자가 다르게 원하면 바꾼다)

- 최대 무게 기본값 40kg. 인스펙터에 노출해 튜닝 가능하게 한다.
- 무게를 초과해도 획득은 막지 않는다. 대신 초과 상태에서 이동속도에 고정 배율(기본 0.5)을 곱한다. 무게 제한을 "획득 차단"이 아니라 "이동 페널티"로 만든 건 사용자가 고른 안의 설명을 그대로 따른 것이다.
- 화면 단축키는 `I` 인벤토리 / `O` 장비 / `K` 상태 / `E` 상호작용(상자)으로 시작한다.
- 탄약은 종류를 나누지 않고 단일 탄약으로 둔다. 탄종 구분은 M8 콘텐츠 사안이다.
- `Coin`은 소지 개념이 어색하지만 사용자가 3종 모두 인벤토리행을 택했으므로 "사용하면 점수가 오르는 귀중품"으로 둔다. 어색하면 즉시효과로 되돌리기 쉽다.

### 데이터 소유권과 경계 (`CLAUDE.md` §11.6)

- 정적 데이터는 ScriptableObject, 런타임 상태는 별도 클래스로 분리한다.
  - `ItemData`(base) - id, 표시 이름, 아이콘, 개당 무게, 최대 스택, 설명.
  - `ConsumableItemData : ItemData` - 효과 종류(Heal/Ammo/Score)와 수치.
  - `WeaponItemData : ItemData` - 장착 슬롯, 무기 프리팹 참조.
  - `ArmorItemData : ItemData` - 장착 슬롯, 방어력 수치.
  - `ItemStack` - 런타임 인스턴스 상태(ItemData 참조 + 개수). ScriptableObject가 아니다.
- `Inventory`/`Equipment`는 UI를 모른다. 상태와 `OnChanged` 이벤트만 공개한다. UI는 이 이벤트만 구독한다.
- 인벤토리 / 장비 / 상태 화면은 서로 다른 컴포넌트로 분리한다.
- 상속 트리는 `ItemData` 아래 3개로 끝낸다. 더 깊게 만들지 않는다.

### 완료 조건 (`GAME_DESIGN.md` §20 M3)

- 필드 아이템을 밟으면 인벤토리에 들어가고, 인벤토리에서 사용하면 효과가 난다.
- 인벤토리 화면에서 보유 아이템과 현재 무게/최대 무게를 확인하고 아이템을 사용/버릴 수 있다.
- 장비 화면에서 6개 슬롯에 무기/방어구를 장착·해제할 수 있고, 주무기/보조무기 전환이 실제 총 교체로 이어진다.
- 상태 화면에서 체력, 총 방어력, 총 무게, 장착 무기 공격력을 확인할 수 있다.
- 필드의 상자에 다가가 상호작용하면 내용물을 인벤토리로 옮길 수 있다.
- 방어구를 장착하면 좀비 피해가 `Max(1, damage - armor)`로 줄어든다.
- 무게를 초과하면 이동속도가 느려진다.
- `compilationFailed: false`, `consoleErrors: 0`.

### 구현 순서 (수직 슬라이스 5단계, 각 단계마다 커밋)

1. **아이템 데이터 + 무게제 인벤토리 코어 (UI 없음)**
   `ItemData`/`ConsumableItemData`/`WeaponItemData`/`ArmorItemData`/`ItemStack`/`Inventory`를 만들고 `PlayerMovement`에 무게 페널티를 연결한다.
   검증: Play Mode `eval`로 아이템 추가·스택·제거·무게 합산·초과 시 이동속도 배율을 직접 확인.

2. **기존 아이템 3종을 인벤토리 경유로 전환 + 인벤토리 UI**
   `AmmoPack`/`HealthPack`/`Coin`을 `WorldItem`(ItemData + 개수를 들고 있는 픽업) 하나로 통합하고 `PlayerHealth.OnTriggerEnter`를 인벤토리 추가로 바꾼다. 인벤토리 화면을 만든다.
   검증: Play Mode에서 `ItemSpawner`가 뿌린 아이템을 밟아 인벤토리에 쌓이고, 화면에서 사용하면 체력/탄약/점수가 실제로 변하는 것을 확인.

3. **장비 시스템 6슬롯 + 장비 UI + 방어 계산**
   `EquipmentSlot` enum과 `Equipment`를 만들고, `PlayerShooter.weaponPrefabs` 하드코딩을 장비 슬롯 참조로 대체한다. `PlayerHealth.OnDamage`에 `Max(1, damage - armor)`를 적용한다.
   검증: 장비 화면에서 주/보조 무기를 바꾸면 손에 든 총이 바뀌고(오른손 그립 거리 0.00000 유지), 방어구 장착 전후로 좀비 피해량이 달라지는 것을 확인.

4. **상태 화면**
   체력/방어력/무게/공격력을 읽기 전용으로 표시한다.
   검증: 장비·인벤토리를 바꿀 때 수치가 따라 갱신되는 것을 확인.

5. **상자 루팅**
   `LootContainer`와 상호작용 입력(`E`)을 만들고 `Prototype` 씬에 상자를 배치한다.
   검증: 상자에 다가가 열면 내용물이 인벤토리로 들어가고, 빈 상자는 다시 열리지 않는 것을 확인.

### 의도적으로 범위에서 제외

- **방어구 비주얼 교체.** 하체 맨다리 메시가 없어 불가능하고, 사용자가 "전부 수치만"을 택했다.
- **1인칭 잔여 메시 3종 정리**(`SK_Miliary_FPS_Arms`, `FPS_Arms_Gloves1`, `FPS_Shirt3`, 합계 9,125 verts). 3인칭 확정이라 쓰이지 않는데 켜진 채 남아 있다. 요청 범위 밖이라 보고만 하고 건드리지 않는다.
- **근접무기 실제 전투.** 근접 슬롯은 만들지만 스윙/히트박스 구현은 M8 사안이다.
- **탄종 구분, 아이템 내구도, 아이템 드래그&드롭.** M3 완료 조건에 없다.
- **저장/불러오기.** M7 사안이다. 다만 `ItemStack`은 나중에 저장 DTO로 옮기기 쉬운 순수 데이터 형태로 만든다.

## M4 파밍과 거점 MVP (2026-09-20)

### 목표

`Prototype` 씬에서 플레이어가 필드 자원을 타격해 재료를 얻고, 그 재료를 소비해 거점 건설물 4종을 자유 배치로 설치/철거할 수 있게 한다. 설치된 건설물은 M7 저장으로 바로 옮길 수 있는 순수 데이터로 표현한다.

### 사용자가 확정한 설계 결정 (2026-09-20)

- **채집은 무기로 타격해서 한다.** 자원 오브젝트가 내구도를 갖고 여러 번 맞으면 부서지며 재료를 드랍한다. 상호작용(E) 즉시 채집을 추천했지만 사용자가 타격 방식을 택했다.
  - 내가 단점으로 적어 보낸 내용을 그대로 옮겨둔다 - **총으로 벌목하게 되고 탄약을 소모한다.** 근접무기는 M8로 미뤄져 있어 지금은 총밖에 없다. 플레이해보고 어색하면 E 채집으로 바꾸는 비용은 작다(`ResourceNode`의 파괴 진입점만 교체).
- **건설물은 자유 배치다.** 그리드 스냅을 쓰지 않는다. 바라보는 바닥 지점에 반투명 프리뷰를 띄우고 휠로 회전, 좌클릭으로 설치한다. 기획서 §11.3의 미정 항목이 이것으로 확정됐다.
- **건설물은 4종이다.** 벽 / 바리케이드 / 보관 상자 / 문. 문은 열림 상태를 갖고 저장 데이터에도 포함한다.

### 사전에 확인한 사실

- **사격 데미지는 `hit.collider.GetComponent<IDamageable>()`로 전달된다**(`Gun.cs:114`). 즉 `IDamageable` 구현체가 **콜라이더와 같은 GameObject에** 있어야 한다. 자원 노드 프리팹을 만들 때 로직을 루트에 두고 콜라이더도 루트에 둬야 한다.
- `LivingEntity`가 이 프로젝트의 사실상 유일한 피해 처리 기반이고 `Zombie`/`PlayerHealth`가 이미 상속한다. 자원 노드가 세 번째 사용자가 된다(`CLAUDE.md` §11.6의 "과도한 상속 트리를 만들지 않는다" 범위 안).
- `ItemData`가 **abstract**라 효과 없는 순수 재료 아이템을 만들 수 없다. M3에서 소비/무기/방어구 3종만 필요해 abstract로 뒀던 것이므로 이번에 해제한다.
- 쓸 수 있는 에셋(전부 `Apocalyptic_World`, URP 프리팹 확인 필요).
  - 자원: `Veg/SM_Veg_Tree_01`, `SM_Veg_Tree_Fir_01~03`(나무), `Rocks/SM_Rock_01/02`(바위), `Props/SM_Props_Dumpster_01`·`SM_Props_Can_01~03`·`Barrel`(고철).
  - 건설물: `Structures/SM_Structure_Wall_01~03`(벽), `Props/SM_Props_Sandbag_01~04`·`SM_Props_BarbWire_01`·`SM_Props_Barrier_01`(바리케이드), `Props/SM_Props_Gate_Door_01`(문). 보관 상자는 M3에서 만든 `Loot Chest.prefab` 계열을 확장한다.
  - **주의**: M3에서 겪은 대로 에셋 프리팹이 텍스처 없는 FBX 내장 머티리얼을 참조할 수 있다. 프리팹을 만들 때마다 `_BaseMap`을 확인할 것.
- 상자 3개를 놓고 NavMesh를 재베이크했더니 정점이 16 -> 108이 됐다. 자원 노드와 건설물도 정적 장애물이므로 같은 절차가 필요하다. 다만 **건설물은 런타임에 생기므로 베이크로는 못 막는다** - 이건 아래 "제외 범위"에 적는다.

### 데이터 소유권과 경계 (`CLAUDE.md` §11.6)

- `BuildableData`(ScriptableObject) - 건설물의 정적 정의(id, 이름, 프리팹, 필요 재료 목록, 설치 판정용 크기).
- `PlacedBuilding`(순수 데이터 클래스) - 런타임 상태(어떤 `BuildableData`인지, 위치, 회전, 문 열림 여부). **MonoBehaviour를 직접 직렬화하지 않는다.** M7 저장 DTO로 그대로 옮길 수 있는 형태로 만든다.
- `BaseBuildState`(MonoBehaviour) - 설치된 건설물 목록의 단일 소유자. 월드 생성(초기 배치)과 분리된 "런타임 변경 상태"다.
- 건설 시스템은 UI를 모른다. `BuildPlacer`는 상태와 이벤트만 공개하고 `BuildMenuUI`가 그것만 읽는다.
- 자원 노드는 월드 초기 배치 대상이고, 채집으로 사라진 상태는 런타임 상태다. M4에서는 재생성(respawn)으로 처리하고 저장은 M7로 미룬다.

### 완료 조건 (`GAME_DESIGN.md` §20 M4)

- 필드의 나무/바위/고철을 사격으로 부숴 목재/돌/고철을 얻고 인벤토리에 담을 수 있다.
- 건설 메뉴에서 건설물을 고르면 반투명 프리뷰가 바라보는 바닥에 뜨고, 휠로 회전하고, 좌클릭으로 설치된다.
- 재료가 모자라면 설치되지 않고, 설치되면 재료가 실제로 소모된다.
- 설치 불가능한 자리(다른 물체와 겹침)에서는 프리뷰가 빨갛게 되고 설치가 거부된다.
- 벽 / 바리케이드 / 보관 상자 / 문 4종이 설치된다.
- 보관 상자에 아이템을 넣고 꺼낼 수 있다.
- 문을 `E`로 여닫을 수 있고 닫혀 있으면 좀비가 통과하지 못한다.
- 설치된 건설물을 철거하면 사라지고 목록에서 빠진다.
- 설치된 건설물 전체가 `PlacedBuilding` 목록이라는 순수 데이터로 표현된다.
- `compilationFailed: false`, `consoleErrors: 0`.

### 구현 순서 (수직 슬라이스 4단계, 각 단계마다 커밋)

1. **재료 아이템과 자원 채집**
   `ItemData` abstract 해제, 목재/돌/고철 아이템과 픽업 프리팹, `ResourceNode : LivingEntity`, 나무/바위/고철 자원 노드 프리팹과 씬 배치, NavMesh 재베이크.
   검증: 사격으로 내구도가 깎이고 0에서 부서지며 픽업이 드랍되고, 주우면 인벤토리에 재료가 쌓인다.

2. **건설 배치 시스템 + 벽 1종**
   `BuildableData`, `PlacedBuilding`, `BaseBuildState`, `BuildPlacer`(프리뷰/휠 회전/겹침 판정/재료 소모/설치), `BuildMenuUI`(`B` 키).
   검증: 재료 부족 시 거부, 충분하면 설치되고 재료가 정확히 빠지며 `BaseBuildState`에 `PlacedBuilding`이 쌓인다.

3. **나머지 건설물 3종**
   바리케이드(정적), 보관 상자(`StorageContainer` + 넣기/꺼내기 화면), 문(`BuildableDoor` 열림/닫힘 + `E` + 닫힘 시 좀비 차단).
   검증: 각 건설물의 고유 동작을 Play Mode에서 확인.

4. **철거와 저장 데이터 표현**
   철거 입력과 `BaseBuildState`에서의 제거, 설치 상태 전체를 순수 데이터로 덤프해 확인.
   검증: 철거 후 목록에서 빠지고, 4종이 모두 담긴 `PlacedBuilding` 목록을 출력해 M7로 넘길 수 있는 형태임을 보인다.

### 의도적으로 범위에서 제외

- **건설물의 NavMesh 반영.** 런타임에 생기는 건설물은 베이크로 막을 수 없다. `NavMeshObstacle`(carve)이 정답이지만 AI Navigation 패키지 설정이 얽히므로 M4에서는 **물리 콜라이더로만 막는다**(좀비는 경로상 밀려 막힌다). 문 차단도 같은 방식이다. 정식 처리는 M6 시드 월드/스트리밍에서 같이 본다.
- **제작(크래프팅)과 작업대.** 기획서 §11.2의 "작업대 또는 기능성 오브젝트"는 제작 시스템이 붙어야 의미가 있는데 M4 완료 조건에 제작이 없다. 건설물 4종을 사용자가 벽/바리케이드/보관 상자/문으로 확정했으므로 작업대는 제외한다.
- **그리드 스냅.** 사용자가 자유 배치를 택했다.
- **자원 노드의 저장.** 채집 상태 복원은 M7이다. M4에서는 일정 시간 뒤 재생성으로 둔다.
- **건설물 내구도/파괴.** M4 완료 조건에 없다.
- **저장/불러오기 실제 구현.** M7이다. M4는 "저장 대상 데이터로 표현"까지만 한다.
