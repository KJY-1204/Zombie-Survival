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
