# checklist.md

작업 체크리스트. GAME_DESIGN.md 20장 마일스톤 기준으로 구성. 완료 항목은 [x]로 표시.

## M0. 프로젝트 진단과 에셋 검증

- [x] Unity 버전 확인 (6000.3.23f1, Unity 6.3 LTS)
- [x] 렌더 파이프라인 확인 (URP 커스텀 파이프라인 적용됨)
- [x] 입력 시스템 상태 확인 (activeInputHandler: 2 = 레거시+신규 Input System 병행, 코드는 현재 레거시 Input.GetAxis만 사용)
- [x] 애니메이션 리그/코드 기준 확인 (교재 PART 06 탑다운 좀비 서바이버 원본 그대로)
- [x] 주요 에셋 폴더 존재 확인 (11.3절 목록, 전부 로컬 존재/`.gitignore`로 git 미추적)
- [x] 최소 플레이 씬 확인 (`Assets/Scenes/Main.unity`)
- [x] Unity 에디터에서 실제 컴파일 오류 0건 확인
- [x] Main 씬 Play 모드 진입 시 콘솔 에러 0건 확인

## M1. 플레이어와 카메라 프로토타입

- [x] 기존 탑다운 이동(`PlayerMovement`/`PlayerInput`)이 정상 동작하는지 재확인 (Play 모드 진입, 콘솔 에러 0건)
- [ ] 총기 1종 조준/발사/재장전 동작 재확인 (`Gun`/`PlayerShooter`) - 씬 배치/컴파일만 확인, 실제 발사 입력 테스트는 미실시
- [ ] 좀비 1종 탐지/추적/공격/사망 동작 재확인 (`Zombie`/`LivingEntity`) - 씬에 좀비가 스폰/배치되는 것만 확인, 추적/공격 시퀀스 테스트는 미실시
- [x] `Zombie.Setup()` 체력 대입 버그 수정 (`health = zombieData.damage` -> `health = zombieData.health`)
- [x] 카메라 모드를 `FirstPerson`/`ThirdPerson` 전략으로 분리하는 구조 설계 및 최소 구현 (`CameraRigController`)
- [ ] 공용 조준 입력(`AimContext`) 초안 설계 (이동 방식이 카메라 시점에 따라 바뀌기 전까지는 보류)
- [x] 1인칭/3인칭 카메라를 교체해서 시험할 수 있는 최소 구현 (`C` 키로 전환, Play 모드에서 두 시점 모두 캡처 확인)
- [x] 1인칭 무기 구성 재검토 - 별도 마운트/스위치 인프라 추가, 근접 프레이밍 문제 확인됨 (자세한 내용은 context-notes.md)
- [x] 1인칭 전용 무기 뷰모델(URP Base+Overlay 카메라 스택, 전용 레이어) 설계 및 구현
- [ ] **사람 확인 필요**: Unity 에디터에서 직접 Play하여 1인칭 무기 카메라가 실제로 화면에 렌더링되는지 육안 확인 (자동화 도구로는 검증 불가 - context-notes.md 참고)
- [x] 플레이어 캐릭터를 Survivalist로 교체 (Humanoid 리타겟팅, CapsuleCollider/1인칭 눈높이 재조정)
- [x] Human Soldier Animations 2.0 FREE로 이동/조준/재장전/사망 클립 교체 (Animator 파라미터/구조 유지)
- [x] 장착 무기를 WeaponsPack (LowPoly)의 American Light AssaultRifle로 교체
- [x] Pistol Gun / Sniper Gun 프리팹 + 전용 GunData 준비 (아직 미장착, 인벤토리/장비 시스템 대기)
- [ ] **사람 확인 필요**: 이동(Walk/Run) 블렌드 자세가 실제로 자연스러운지, 재장전 애니메이션과 타이밍이 맞는지 육안 확인 (Idle/Aim/Die만 스크린샷으로 검증함)
- [ ] IK Helper Tool(Kevin Iglesias) 도입 검토 - 이번엔 기존 PlayerShooter 수동 IK를 유지 (자세한 이유는 context-notes.md)
- [x] 1인칭 테스트 씬 생성 (`Assets/Scenes/FirstPersonTest.unity`) - 거리 마커/키 비교 기둥/사격 타겟 포함, 좀비 없음
- [x] 1인칭 무기 카메라 스택이 실제로 화면에 나오는지 재검증 완료 - **정상 작동 확인** (이전 "검증 불가" 결론 정정, context-notes.md 참고)
- [x] 1인칭 시점을 실제 FPS 방식(마우스 X=몸통 Yaw, 마우스 Y=카메라 Pitch, 커서 잠금)으로 개선, FOV 40->75
- [ ] **사람 확인 필요**: 실제 마우스로 시점을 움직여봤을 때 감도/반응이 자연스러운지 확인 (자동화로는 실제 마우스 입력을 시뮬레이션할 수 없어 컴포넌트 연결/커서 잠금 상태만 코드로 검증함)
- [x] 탑다운/3인칭 추적 카메라(Follow Cam) 제거, 1인칭을 유일한 시점으로 고정 (GAME_DESIGN.md 5.1/21장 갱신)
- [x] 1인칭 무기 마운트 거리 조정 (총이 너무 앞으로 나와 보이는 문제, Z 0.4 -> 0.2)
- [x] 장착 무기(AssaultRifle)에 맞는 조준/재장전 모션으로 교체 (Rifle 카테고리 -> AssaultRifle 카테고리)
- [x] "1인칭에서 캐릭터가 사라진다" 버그 진단/수정 - 실제 원인은 (1) 그래스톤 프리팹 1개가 3배 스케일로 스폰 코앞에 배치됨 (2) 1인칭 팔이 전용 FPS_HANDS 뷰모델 대신 3인칭용 팔이 배선되어 카메라 근평면에 클리핑됨 (3) FirstPersonWeaponMount Z가 음수라 총도 근평면에 클리핑됨. `CameraRigController`가 FPS_HANDS를 무기와 동일한 전용 레이어로 옮기도록 수정 (자세한 내용은 context-notes.md)
- [x] `capture_game_view` 캡처 파라미터 정정 - `source: "camera"`(기본값)는 카메라 스택 합성을 반영하지 않음, 반드시 `source: "screen"` + Play 모드로 검증 (context-notes.md 참고)
- [x] `Gun.Shot()`이 총구 방향 대신 화면 중앙(조준점) 기준으로 레이캐스트하도록 수정 - 1인칭에서 총구 위치가 카메라 중심에서 벗어나 있어 총구 기준 발사 시 조준점과 실제 탄착점이 어긋나던 문제 해결
- [x] 화면 중앙 크로스헤어 UI 추가 (`Too Many Crosshairs` 에셋의 `Cross128` 스프라이트, `HUD Canvas/Crosshair`) - `Gun.Shot()`이 실제로 조준하는 지점과 시각적으로 일치
- [x] 점프 모션/기능 추가 - `ShooterAnimator.controller`에 `Jump`(Trigger)/`IsGrounded`(Bool) 파라미터와 `JumpStart`->`InAir`->`Movement` 상태/전환 추가 (Survivalist StarterAssets의 `Jump--Jump`/`Jump--InAir` 클립 재사용), `PlayerMovement`에 바닥 검사 + 점프 물리 구현. 기존 무기(Aim/Reload)/이동(Walk/Run) 모션은 이전 세션에서 이미 구현되어 있었음을 재확인
- [ ] **사람 확인 필요**: 실제 스페이스바 입력으로 점프 감도/타이밍이 자연스러운지, 착지 시 애니메이션 전환이 어색하지 않은지 육안 확인 (자동화로는 리플렉션으로 `Jump()`를 직접 호출해 물리/애니메이터 상태 전환만 검증함)
- [x] 몸이 안 보이는(전신 숨김) 1인칭 방식을 폐기하고 몸 전체가 보이는 근접 3인칭(어깨너머) 시점으로 전환 - `CameraRigController`가 더 이상 몸통 렌더러를 숨기지 않음, `PlayerShooter.useFirstPersonMount=false`로 전환해 팔꿈치 IK 기반 자연스러운 파지 자세 사용, `FirstPerson Cam` 위치를 캐릭터 뒤 위쪽으로 이동해 카메라가 몸 메시 안에 끼는 문제 해결
- [x] **버그 수정**: 카메라가 캐릭터 몸 뒤로 이동하면서 조준 레이(화면 중앙 기준)가 자기 자신의 콜라이더를 먼저 맞춰 총 쏘면 플레이어가 죽던 문제 - `Gun`에 `SetOwner()`로 소유자 루트를 등록하고 레이캐스트에서 자기 자신의 콜라이더를 제외하도록 수정

## M2. 전투 수직 슬라이스

- [ ] 탄약/회복 아이템 드랍 확률 시스템 검토 (현재는 `ItemSpawner`가 주기적 스폰만 함)
- [ ] 플레이어 피격/사망/재시작 흐름 재확인

## M3. 루팅과 캐릭터 관리

- [ ] 아이템 정적 정의/런타임 인스턴스 데이터 구조 설계 (현재 `IItem`은 즉시 사용형만 지원)
- [ ] 인벤토리 화면
- [ ] 장비 화면
- [ ] 상태 화면

## M4. 파밍과 거점 MVP

- [ ] 필드 자원 채집
- [ ] 최소 건설물 2~4종
- [ ] 건설물 저장 데이터 표현

## M5. 오토바이

- [ ] 탑승/하차/주행
- [ ] 카메라 전환 안정화
- [ ] 저장 후 위치/상태 복원

## M6. 시드 월드와 스트리밍

- [ ] 시드 기반 청크 배치
- [ ] 도로 연결성 검증
- [ ] 청크 활성/비활성 스트리밍

## M7. 저장 시스템 완성

- [ ] 10개 저장 슬롯 UI
- [ ] 수동/자동 저장
- [ ] `SaveVersion` 포함 저장 DTO

## M8. 콘텐츠와 폴리시

- [ ] 좀비/무기/루팅 변형
- [ ] VFX/SFX/포스트 프로세싱 정리
- [ ] 성능 프로파일링
- [ ] 저장 안정성 검증
