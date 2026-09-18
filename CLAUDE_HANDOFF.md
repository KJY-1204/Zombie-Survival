# Claude 인수인계 메모

## 시작 상태

- 기준 커밋은 `702e1a3 IK Helper Tool로 왼손 그립 보정`이며 `main`과 `origin/main`은 동기화되어 있다.
- 사용자 소유의 미커밋 변경은 `ProjectSettings/ProjectSettings.asset`, `ProjectSettings/ShaderGraphSettings.asset`, `.vsconfig`다. 되돌리거나 커밋하지 않는다.
- 테스트 씬은 `Assets/Scenes/Prototype.unity`이고 플레이어 프리팹은 `Assets/Prefabs/TPS Player.prefab`이다.

## 현재 구현

- 카메라는 `Assets/Scripts/Camera/ThirdPersonCameraController.cs`가 담당한다. 기본은 오른쪽 숄더뷰이고 `Fire2` 입력으로 ADS 거리와 FOV를 전환한다.
- `Assets/Scripts/PlayerMovement.cs`는 `Vertical`과 `Horizontal` 입력을 로컬 forward와 right로 합성해 TPS 스트레이프 이동을 처리한다.
- 플레이어 루트는 기존 체력·입력·이동·사격·총기 참조를 유지한다. 기존 루트 Animator는 비활성화되어 있다.
- 실제 보이는 모델은 `Player Character/Survivalist Visual/Geometry/SK_Military_Survivalist`다. 이 하위 Animator만 활성화되어 있고 `Assets/Animations/SurvivalistTPS.controller`를 사용한다.
- `SurvivalistTPS.controller`는 Survivalist 원본 컨트롤러 복사본이며 Base Layer IK Pass만 활성화했다. 파라미터는 `Speed`, `Jump`, `Grounded`, `FreeFall`, `MotionSpeed`다.
- `SurvivalistWeaponIK.cs`는 총기 피벗을 오른손 본과 오른손 그립의 초기 상대 변환으로 매 IK 프레임 계산한다. 따라서 총은 오른손 애니메이션을 따른다.
- 왼손은 직접 `Animator.SetIK*` 호출을 사용하지 않는다. `KevinIglesias.IKHelperTool`이 보정한다.

## IK Helper Tool 연결

- 사용 에셋은 `Assets/Kevin Iglesias/IKHelperTool`다.
- `KevinIglesias.IKHelperTool` 컴포넌트는 실제 메시 Animator가 있는 `SK_Military_Survivalist` 오브젝트에 붙어 있다.
- `IK Switch`는 그 오브젝트의 자식이며 로컬 위치 `(0, 1, 0)`이다. 에셋은 switch의 로컬 Y값으로 왼손 IK 가중치를 계산하므로 현재 가중치는 1이다.
- `IK Left Hand Effector`는 `Player Character/Gun Pivot/Gun/Left Handle`의 자식이며 로컬 변환은 모두 0이다. 총기와 함께 움직인다.
- 에셋 문서는 물체를 제어하는 손은 애니메이션으로 유지하고 반대 손만 IK로 맞추는 사용법을 권장한다. 현재 오른손이 제어 손이고 왼손이 보정 손이다.

## 확인된 문제

- 현재 소총 발사 시 캐릭터는 허리춤 힙 파이어 자세다.
- 원인은 IK가 아니라 Animator에 소총 조준·사격 자세가 없기 때문이다. 현재 컨트롤러는 `Idle Walk Run Blend`, `InAir`, `JumpLand`, `JumpStart`만 가진다.
- Play Mode에서 상체 중심은 Y=1.39, 오른손은 Y=0.96, 총구는 Y=1.19였다. 총구가 상체보다 약 19cm 낮아 힙 파이어 위치인 것을 확인했다.
- IK Helper Tool은 왼손을 총기 손잡이에 맞출 뿐 팔을 들어 조준시키지 않는다.

## 권장 다음 작업

1. `Assets/Kevin Iglesias/IKHelperTool/Animations/IK@RifleIdle.anim`과 `IK@RifleRun.anim`을 후보로 실제 Survivalist Avatar에서 확인한다.
2. 원본 에셋을 수정하지 말고 `SurvivalistTPS.controller`의 복사본 또는 전용 무기 컨트롤러를 만든다.
3. 힙 파이어와 ADS를 구분하는 무기 자세 상태를 만든다. ADS일 때는 Rifle Idle 또는 Rifle Run을 사용해 오른손이 어깨 높이에서 총을 제어하게 한다.
4. IK Helper Tool의 왼손 효과기는 그대로 유지한다. 오른손 그립과 왼손 효과기 거리를 다시 측정한다.
5. 우클릭 ADS, 이동, 사격을 Unity Editor에서 실제로 수동 확인한다. Pipeline은 마우스 입력을 안정적으로 합성하지 못한다.

## 검증 기준

- ADS에서 총구가 상체 중심보다 낮지 않고, 총기 모델의 `Fire Position`에서 탄도가 출발한다.
- 오른손과 오른손 그립의 거리는 거의 0이고 왼손과 효과기의 거리는 작은 오차 범위여야 한다.
- `unity command recompile_status --project-path 'D:\work\Zombie-Survival' --format json`이 성공해야 한다.
- `unity command console_status --project-path 'D:\work\Zombie-Survival' --format json`에서 신규 오류가 없어야 한다.

## 최근 커밋

- `f68b949 TPS 숄더뷰와 ADS 카메라 추가`.
- `abd74bc TPS 스트레이프와 Survivalist 플레이어 추가`.
- `18d8ab3 Survivalist 애니메이션과 무기 IK 연결`.
- `5e0dd5d 총기 피벗을 오른손 애니메이션에 연결`.
- `702e1a3 IK Helper Tool로 왼손 그립 보정`.
