# 작업 컨텍스트

## 2026-09-18 - Prototype 작업 씬 준비

- 사용자 요청에 따라 이전 `checklist.md`와 `context-notes.md`의 내용을 새 작업 기준으로 초기화했다.
- 새 작업 씬은 `Assets/Scenes/Prototype.unity`로 만들었다.
- 씬에는 Unity 기본 3D 템플릿의 `Main Camera`와 `Directional Light`만 있다.
- 기존 `Assets/Scenes/Main.unity`와 `Assets/Scenes/FirstPersonTest.unity`는 수정하지 않았다.
- 기능 요구사항은 아직 정해지지 않았으므로 게임플레이 오브젝트나 스크립트는 추가하지 않았다.
- 작업 시작 시 사용자 소유 변경으로 `ProjectSettings/ProjectSettings.asset`, `ProjectSettings/ShaderGraphSettings.asset`, `.vsconfig`가 이미 변경 상태였으며 건드리지 않았다.

## 2026-09-18 - TPS 백뷰와 ADS 결정

- 사용자가 최종 카메라 방향을 TPS 백뷰/숄더뷰로 결정했다.
- 기본 카메라는 플레이어의 오른쪽 어깨 뒤에 배치하고, 우클릭 중에는 더 가까운 ADS 위치와 좁은 FOV로 전환한다.
- 기존 `Gun`은 `Camera.main`의 화면 중앙 레이를 사용하므로, 전환 중에도 활성 메인 카메라를 하나만 유지해 사격 조준 기준을 보존한다.
- 이번 수직 슬라이스는 카메라와 최소 플레이어 조작에 한정하며, 인벤토리·좀비·루팅은 포함하지 않는다.
- `ThirdPersonCameraController`는 `PlayerMovement`를 자동으로 찾아 기존 마우스 X 회전을 유지하고, 카메라의 마우스 Y 피치와 `Fire2` 입력 기반 ADS 위치·FOV 전환만 담당한다.
- `Prototype`에는 기존 `Player Character` 프리팹과 50x50 단위의 `Test Ground`를 배치했고, `Main Camera`에 TPS 카메라 컨트롤러를 연결했다.
- Unity Play Mode에서 기본 숄더뷰 렌더링과 신규 콘솔 오류 0건을 확인했다. Pipeline에서는 마우스 입력을 합성하지 못하므로 우클릭 ADS, 이동, 사격은 Editor에서 수동 확인이 남아 있다.
- TPS 카메라, Prototype 씬 구성, 빌드 등록과 작업 문서를 하나의 로컬 커밋으로 기록했다. 원격 push는 외부 공유 브랜치 변경에 대한 별도 사용자 승인이 필요해 수행하지 못했다.

## 2026-09-18 - TPS 스트레이프와 Survivalist 플레이어 선택

- `PlayerMovement`는 `Vertical` 입력만 전후 이동에 사용하고 `Horizontal` 입력은 회전에만 사용하므로 TPS 좌우 이동이 동작하지 않는다.
- 기획서의 `Survivalist character`는 3인칭 프로토타입 우선 플레이어 후보이며, 실제 `Assets/Survivalist/Prefab/PlayerArmature.prefab`은 Humanoid Avatar와 `StarterAssetsThirdPerson` Animator Controller를 보유한다.
- 기존 `Player Character` 루트는 체력·입력·이동·사격·총기 참조를 이미 가진다. 이를 유지하고 임시 `Woman` 메시를 Survivalist 비주얼로 교체해 전투 시스템의 재연결 범위를 최소화한다.
- `PlayerMovement`는 전후와 좌우 입력을 로컬 `forward`와 `right` 방향으로 합성하고, `Vector3.ClampMagnitude`로 대각선 가속을 막도록 변경했다. Play Mode에서 수평 입력 `1`로 `x` 위치가 `0.00`에서 `0.03`으로 이동하는 것을 확인했다.
- 기존 전투 루트를 새 `Assets/Prefabs/TPS Player.prefab`으로 저장하고, 해당 루트의 `Survivalist Visual` 자식에는 모델용 `Animator`만 남겼다. Survivalist 원본의 `CharacterController`와 Starter Assets 입력·이동 컴포넌트는 중복 제어를 막기 위해 포함하지 않았다.
- Unity Play Mode에서 Survivalist 후방 숄더뷰 렌더링과 콘솔 오류·경고 0건을 확인했다. 모델 전용 `StarterAssetsThirdPerson` Animator Controller와 기존 `PlayerMovement`의 Animator 파라미터·총기 IK는 아직 연결하지 않았으므로, 이동 애니메이션과 손 그립 보정은 별도 작업으로 남긴다.
