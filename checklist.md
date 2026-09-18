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
- [ ] 원격 `main` 브랜치로 push한다.

## 2026-09-18 - TPS 스트레이프와 Survivalist 플레이어

- [x] 기획서와 실제 에셋을 대조해 `PlayerArmature.prefab`을 3인칭 플레이어 비주얼로 선택한다.
- [x] 수평 입력 기반 스트레이프 이동을 구현한다.
- [x] 임시 여성 메시를 Survivalist 비주얼로 교체한다.
- [x] Unity Play Mode에서 전후·좌우 이동과 비주얼을 검증한다.
- [x] 검증된 변경을 로컬 커밋한다.
