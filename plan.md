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
