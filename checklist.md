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
