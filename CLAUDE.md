# CLAUDE.md

Behavioral guidelines to reduce common LLM coding mistakes. Merge with project-specific instructions as needed.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## 1. Think Before Coding

**Don't assume. Don't hide confusion. Surface tradeoffs.**

Before implementing:
- State your assumptions explicitly. If uncertain, ask.
- If multiple interpretations exist, present them - don't pick silently.
- If a simpler approach exists, say so. Push back when warranted.
- If something is unclear, stop. Name what's confusing. Ask.

## 2. Simplicity First

**Minimum code that solves the problem. Nothing speculative.**

- No features beyond what was asked.
- No abstractions for single-use code.
- No "flexibility" or "configurability" that wasn't requested.
- No error handling for impossible scenarios.
- If you write 200 lines and it could be 50, rewrite it.

Ask yourself: "Would a senior engineer say this is overcomplicated?" If yes, simplify.

## 3. Surgical Changes

**Touch only what you must. Clean up only your own mess.**

When editing existing code:
- Don't "improve" adjacent code, comments, or formatting.
- Don't refactor things that aren't broken.
- Match existing style, even if you'd do it differently.
- If you notice unrelated dead code, mention it - don't delete it.

When your changes create orphans:
- Remove imports/variables/functions that YOUR changes made unused.
- Don't remove pre-existing dead code unless asked.

The test: Every changed line should trace directly to the user's request.

## 4. Goal-Driven Execution

**Define success criteria. Loop until verified.**

Transform tasks into verifiable goals:
- "Add validation" → "Write tests for invalid inputs, then make them pass"
- "Fix the bug" → "Write a test that reproduces it, then make it pass"
- "Refactor X" → "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] → verify: [check]
2. [Step] → verify: [check]
3. [Step] → verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

## 5. No Closing Colons (Korean Output)

**End Korean sentences with a period, not a colon.**

When the user writes in Korean, your output is also Korean:
- Don't end sentences with `:` even if the next line is a list or example.
- LLMs trained on English docs leak the colon habit into Korean. Catch it.
- The test: every Korean sentence terminator should be `.`, `?`, or `!` — not `:`.
- Colons are fine inside code, key-value pairs, or labels. Not as sentence enders.

## 6. File Header Comments in Korean

**First line of every new source file: a one-line Korean comment stating its role.**

When creating a new file:
- TypeScript/JavaScript: `// 사용자 인증 상태를 관리하는 Context Provider`
- Python: `# KIS API 호출을 비동기로 래핑하는 클라이언트`
- SQL: `-- 일별 집계 결과를 저장하는 머티리얼라이즈드 뷰`
- Place it directly under required directives (`'use client'`, `'use server'`, shebang).
- Skip config files (`*.config.ts`, `package.json`, etc.).

Why: agents read files selectively, not whole codebases. A one-line Korean header gives instant context so the next session (human or agent) can navigate without re-reading the entire file.

## 7. Plan + Checklist + Context Notes

**Before any non-trivial task, produce three artifacts. Don't start coding without them.**

- **Plan** — what we're building and why.
- **Checklist** (`checklist.md`) — concrete tasks as checkboxes. Tick as you go.
- **Context Notes** (`context-notes.md`) — decisions made during the work and the reasoning behind them. Append continuously.

If the user gives only a plan and asks you to start coding, stop and ask: "Should I create the checklist and context notes first?" The next session — yours or someone else's — needs the notes to pick up where you left off without re-deriving every decision.

## 8. Run Tests Before Marking Complete

**If you touched code, run the tests before saying "done".**

- `npm test`, `pytest`, `cargo test`, whatever the project uses — run it.
- If tests pass, report results. If they fail, fix and re-run.
- No test setup? At minimum, verify the project builds/compiles.
- Run tests proactively, before the user signals "끝", "완료", "다 됐어" — not after.

This is the step LLMs skip most often. Treat it as non-negotiable.

## 9. Semantic Commits

**Commit when one logical change is complete. Don't wait for the user to ask.**

- The test: "Can I describe this commit in one sentence?" If yes, commit. If no, the changes are still mixed — split them.
- Good: "auth 미들웨어 추가". Bad: "auth 추가하고 UI도 고치고 버그도 수정" (split into 3).
- Don't accumulate 20 unrelated edits and lose the ability to roll back individually.
- Don't commit just to commit — meaningful units only.

Note: For solo prototypes or throwaway scripts, group commits loosely if it slows you down. The point is reversibility, not ceremony.

## 10. Read Errors, Don't Guess

**Read the actual error/log line. Don't pattern-match from memory.**

When something fails:
- Read the full error message and stack trace.
- Check the actual log output, not what you assume it should say.
- Don't apply a "common fix" before confirming the cause.
- If unclear, add a print/log to verify state — then fix.

This is the step LLMs skip most often after "run tests". They guess from error keywords and apply the most-recent-pattern fix. That's how a one-line bug becomes a three-file refactor.

---

**These guidelines are working if:** fewer unnecessary changes in diffs, fewer rewrites due to overcomplication, and clarifying questions come before implementation rather than after mistakes.

## 11. 좀비 생존 슈터 프로젝트 전용 하네스

이 프로젝트는 Unity 6.3 LTS 기반 싱글 플레이 좀비 생존 슈터입니다. `GAME_DESIGN.md`를 게임 범위의 단일 기준으로 삼고, 이 문서는 Claude가 설계/구현/리뷰를 이어받을 때 필요한 실행 규칙만 유지합니다.

### 11.1 프로젝트 핵심 방향

- 아포칼립스 지상 대형 월드를 탐색하고 좀비와 싸우며 재료를 파밍해 거점을 확장합니다.
- 최종 시점은 3인칭으로 확정되었습니다(어깨너머 숄더뷰 + 우클릭 ADS). 카메라와 게임플레이 코어는 계속 분리해서 유지합니다.
- 탈것은 우선 오토바이 1종만 구현합니다.
- 인벤토리, 장비, 상태는 한 창(캐릭터 창) 안에서 탭으로 전환합니다(2026-09-20 사용자 결정, 팰월드풍 UI). 단, 각 탭의 데이터 경계는 그대로 분리해 둡니다. 탭은 자기 내용만 그리고 여닫기는 창이 맡습니다.
- 자동저장과 수동저장을 모두 지원하며 10개의 플레이어 저장 슬롯을 제공합니다.
- 월드는 시드에 의해 결정론적으로 구성되며, 완전 절차 지형보다 준비된 도시/도로/산/초원/POI 청크 조합을 우선합니다.
- 멀티플레이는 현재 범위 밖입니다.

### 11.2 교재 기반과 확장 원칙

교재 PART 06의 레벨 아트/플레이어, 총기 슈터, 생명체/좀비 AI, HUD/UI 흐름을 출발점으로 사용합니다. 탑다운 전용 구현은 그대로 복사하지 말고 카메라 독립 코어, 대형 월드, 파밍, 거점, 저장, 오토바이, 시드 생성 요구에 맞게 확장합니다.

### 11.3 보유 에셋 매핑

실제 Unity 프로젝트에서 에셋의 폴더, 프리팹, 컴포넌트, 라이선스 메모를 먼저 확인합니다. 에셋 이름만 보고 API를 추정하지 않습니다.

- 플레이어 후보: `Survivalist character`, `Stylized Character Female`.
- 좀비: `Zombie`.
- 애니메이션: `MC Sample - Believable 3D Animations by MoCap Central`, `Human Soldier Animations FREE`.
- IK: `IK Helper Tool`.
- 무기: `Weapons Pack - Realistic LowPoly`, `Crusader Weapon`, `Free pack of medieval weapons`.
- 투척/VFX: `Grenade System — Free Edition`, `Grenade M18 Smoke`, `War FX`.
- 조준 UI: `Too Many Crosshairs`.
- 오토바이: `Motorcycle Interaction Animset FREE`, `Post Apocalyptic Motorcycle 3D Model Rigged Off Road`.
- 루팅: `Realistic Crate & Chest Bundle - 4K PBR Props`.
- POI/월드: `School Scene`, `Post Apocalyptic World Pack`.

### 11.4 Claude 작업 하네스

1. 비사소한 작업 전 `GAME_DESIGN.md`, `checklist.md`, `context-notes.md`와 관련 코드를 읽습니다.
2. 사용자 요구를 기능 단위 완료 조건으로 바꿉니다.
3. 설계가 필요한 경우 인터페이스와 데이터 소유권부터 정하고 구현합니다.
4. 한 번에 하나의 수직 슬라이스만 구현합니다.
5. Unity 컴파일과 관련 테스트/수동 재현 절차로 검증합니다.
6. 변경 이유와 중요한 선택을 `context-notes.md`에 짧게 누적합니다.
7. 검증된 논리 단위마다 커밋하고, 원격/인증이 준비되어 있으면 즉시 push 합니다.

### 11.5 Codex와의 협업 규칙

- Claude와 Codex는 같은 작업을 동시에 중복 구현하지 않습니다.
- 작업을 넘길 때 `context-notes.md`에 현재 상태, 변경 파일, 남은 문제, 검증 방법을 기록합니다.
- 다른 에이전트가 남긴 미커밋 변경을 자신의 스타일로 재작성하지 않습니다.
- 상충하는 구현이 발견되면 실제 코드와 최근 검증 결과를 우선하고, 문서가 오래되었으면 문서를 갱신합니다.
- 대규모 리팩터링을 인계 해결책으로 사용하지 않습니다. 먼저 가장 작은 호환 수정안을 찾습니다.

### 11.6 시스템 경계

- 카메라 모드가 바뀌어도 인벤토리, 데미지, AI, 저장 로직은 바뀌지 않아야 합니다.
- `LivingEntity/Damageable` 계층은 플레이어와 좀비가 공통으로 사용할 수 있게 하되 과도한 상속 트리를 만들지 않습니다.
- 아이템과 무기 정의는 정적 데이터와 런타임 인스턴스 상태를 분리합니다.
- 인벤토리/장비 시스템은 UI를 몰라야 하고 UI는 공개된 상태/이벤트만 사용합니다.
- 월드 생성은 초기 배치를 담당하고, 플레이 중 파괴/루팅/건설 같은 변경 상태는 별도 런타임 월드 상태가 담당합니다.
- 저장 시스템은 각 시스템 내부 객체를 직접 직렬화하지 않고 버전이 있는 저장 DTO를 사용합니다.

### 11.7 시드 월드 생성 기준

- 동일 시드 + 동일 생성기 버전 = 동일 초기 월드입니다.
- 도시/도로/산/초원/POI 청크를 연결 규칙과 가중치로 배치합니다.
- 시작 지점과 필수 경로가 막히지 않도록 연결성 검증을 우선합니다.
- `WorldSeed`, `GeneratorVersion`, 런타임 변경 상태를 저장에서 분리합니다.
- 대형 맵은 한 번에 모두 활성화하지 않고 섹터/청크 단위 스트리밍을 전제로 설계합니다.

### 11.8 저장 기준

- 플레이어가 볼 수 있는 슬롯은 10개입니다.
- 각 슬롯에서 수동 저장과 자동저장 상태를 구분합니다.
- 저장 대상은 플레이어 위치/상태, 인벤토리/장비, 월드 시드, 루팅 상태, 건설 상태, 오토바이 상태, 진행 상태입니다.
- `SaveVersion`을 반드시 포함합니다.
- 저장 실패가 기존 정상 저장을 손상시키지 않도록 임시 파일 작성 후 교체 같은 원자적 저장 방식을 우선 검토합니다.

### 11.9 GitHub 동기화 규칙

- 작업 시작 시 `git status --short`, 현재 브랜치, 원격 저장소를 확인합니다.
- 사용자 변경을 revert/reset/clean으로 지우지 않습니다.
- 테스트/컴파일이 통과한 하나의 의미 있는 변경마다 커밋합니다.
- 원격과 인증이 준비되어 있으면 커밋 직후 `git push` 합니다.
- push 실패 시 오류를 그대로 읽고 원인을 해결합니다. 강제 push는 사용자 명시 요청 없이는 금지합니다.
- 저장소/remote/인증이 없으면 임의로 구성하지 말고 필요한 설정을 명확히 보고합니다.

### 11.10 개발 우선순위

1. 플레이어 이동과 카메라 프로토타입.
2. 좀비 1종 + 총기 1종 전투 루프.
3. 루팅과 기본 아이템.
4. 인벤토리/장비/상태 UI.
5. 채집과 거점 건설.
6. 오토바이.
7. 시드 월드 생성/스트리밍.
8. 10슬롯 저장/자동저장.
9. 콘텐츠와 최적화.

### 11.11 완료 보고

완료 응답에는 변경 파일, 실제 검증 결과, 커밋 해시, push 결과, 남은 위험을 적습니다. 검증되지 않은 상태를 완료라고 표현하지 않습니다.
