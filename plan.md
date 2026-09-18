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
