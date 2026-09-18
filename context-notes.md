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
