# AGENTS.md

Behavioral guidelines for Codex and other coding agents. Merge with project-specific instructions as needed.

**Tradeoff:** These guidelines bias toward caution over speed. For trivial tasks, use judgment.

## Codex Notes

Codex reads `AGENTS.md` before doing work. Put this file at the project root for repository-wide guidance, or in a nested directory for narrower rules.

Keep this file short and concrete. Codex combines global and project instructions, and large instruction files can crowd out useful task context.

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
- "Add validation" -> "Write tests for invalid inputs, then make them pass"
- "Fix the bug" -> "Write a test that reproduces it, then make it pass"
- "Refactor X" -> "Ensure tests pass before and after"

For multi-step tasks, state a brief plan:
```
1. [Step] -> verify: [check]
2. [Step] -> verify: [check]
3. [Step] -> verify: [check]
```

Strong success criteria let you loop independently. Weak criteria ("make it work") require constant clarification.

## 5. Workspace Evidence Before Edits

**Inspect the actual files you will touch. Don't rely on memory or stale summaries.**

Before changing code:
- Use `rg` or project tools to find the relevant implementation.
- Read the exact files and nearby call sites before editing them.
- Treat open editor tabs, filenames, READMEs, and prior conversation summaries as hints, not proof.
- If local code disagrees with your assumption, trust the code and update the plan.

This is not a new "small change" rule. It exists to prevent confident edits based on imagined code.

## 6. Respect The Worktree

**Assume uncommitted changes belong to the user unless you made them.**

When the worktree is dirty:
- Do not revert, overwrite, or reformat unrelated changes.
- If user changes touch the same files, read them and adapt.
- If unrelated files are dirty, ignore them.
- Never run destructive git commands unless the user explicitly asked for them.

## 7. No Closing Colons (Korean Output)

**End Korean sentences with a period, not a colon.**

When the user writes in Korean, your output is also Korean:
- Don't end Korean sentences with `:` even if the next line is a list or example.
- LLMs trained on English docs leak the colon habit into Korean. Catch it.
- The test: every Korean sentence terminator should be `.`, `?`, or `!`, not `:`.
- Colons are fine inside code, key-value pairs, timestamps, or labels. Not as Korean sentence enders.

## 8. File Header Comments in Korean

**First line of every new source file: a one-line Korean comment stating its role.**

When creating a new source file:
- TypeScript/JavaScript: `// 사용자 인증 상태를 관리하는 Context Provider`
- Python: `# KIS API 호출을 비동기로 래핑하는 클라이언트`
- SQL: `-- 일별 집계 결과를 저장하는 머티리얼라이즈드 뷰`
- Place it directly under required directives (`'use client'`, `'use server'`, shebang).
- Skip config files (`*.config.ts`, `package.json`, lockfiles, generated files).

Why: agents read files selectively, not whole codebases. A one-line Korean header gives instant context so the next session can navigate without re-reading everything.

## 9. Plan + Checklist + Context Notes

**Before any non-trivial task, produce three artifacts. Don't start coding without them.**

- **Plan** - what we're building and why.
- **Checklist** (`checklist.md`) - concrete tasks as checkboxes. Tick as you go.
- **Context Notes** (`context-notes.md`) - decisions made during the work and the reasoning behind them. Append continuously.

If the user gives only a plan and asks you to start coding, stop and ask: "Should I create the checklist and context notes first?" The next session needs the notes to pick up without re-deriving every decision.

## 10. Run Tests Before Marking Complete

**If you touched code, run the relevant tests before saying "done".**

- `npm test`, `pytest`, `cargo test`, or whatever the project uses - run the smallest relevant check first, then broader checks when risk is high.
- If tests pass, report the exact command.
- If tests fail, read the actual error, fix it, and re-run.
- If no test setup exists, verify the project builds or typechecks.
- If you cannot run verification, say exactly why.

This is the step coding agents skip most often. Treat it as non-negotiable.

## 11. Verification Evidence In The Final Reply

**Report what you actually verified, not what you intended to verify.**

Final responses should include:
- The command or check that ran, such as `npm test` or `npx tsc --noEmit`.
- The result, such as "passed", "failed with X", or "not run because Y".
- Any remaining risk the user should know about.

Do not write "done", "fixed", or "works" unless that claim is backed by a concrete check.

## 12. Semantic Commits

**Commit when one logical change is complete. Don't wait for the user to ask.**

- The test: "Can I describe this commit in one sentence?" If yes, commit. If no, the changes are still mixed - split them.
- Good: "auth 미들웨어 추가". Bad: "auth 추가하고 UI도 고치고 버그도 수정" (split into 3).
- Don't accumulate unrelated edits and lose the ability to roll back individually.
- Don't commit just to commit - meaningful units only.
- If the environment or user workflow does not allow commits, keep changes uncommitted and clearly summarize them.

Note: For solo prototypes or throwaway scripts, group commits loosely if it slows you down. The point is reversibility, not ceremony.

## 13. Read Errors, Don't Guess

**Read the actual error/log line. Don't pattern-match from memory.**

When something fails:
- Read the full error message and stack trace.
- Check the actual log output, not what you assume it should say.
- Don't apply a "common fix" before confirming the cause.
- If unclear, add a print/log to verify state - then fix.

This is the step coding agents skip most often after "run tests". They guess from error keywords and apply the most recent pattern. That's how a one-line bug becomes a three-file refactor.

---

**These guidelines are working if:** fewer unnecessary changes in diffs, fewer rewrites due to overcomplication, verification is reported with exact checks, and clarifying questions come before implementation rather than after mistakes.

## 14. 좀비 생존 슈터 프로젝트 전용 하네스

이 저장소의 목표는 Unity 6.3 LTS 기반의 싱글 플레이 좀비 생존 슈터를 단계적으로 완성하는 것입니다. 상세 게임 규칙과 범위의 단일 기준 문서는 `GAME_DESIGN.md`입니다. 이 섹션은 Codex가 구현 작업을 안전하게 이어받기 위한 최소 실행 규칙만 정의합니다.

### 14.1 현재 고정된 프로젝트 방향

- 배경은 아포칼립스 이후의 대형 지상 월드입니다.
- 핵심 루프는 탐색 -> 전투 -> 파밍 -> 귀환/거점 확장 -> 장비 정비 -> 더 먼 지역 탐색입니다.
- 카메라는 1인칭과 3인칭 중 최종 결정 전입니다. 코어 게임 로직을 카메라 구현과 분리하고, 초기 프로토타입에서는 두 시점을 교체 시험할 수 있게 유지합니다.
- 맵은 완전한 노이즈 지형 생성보다 준비된 환경 청크와 POI를 시드로 결정론적으로 조합하는 방식을 우선합니다.
- 탈것은 현재 오토바이 1종만 범위에 포함합니다.
- 인벤토리, 장비, 상태 UI는 서로 분리된 화면으로 구성합니다.
- 저장은 자동저장과 수동저장을 모두 지원하고, 플레이어에게 10개의 저장 슬롯을 제공합니다.
- 멀티플레이는 현재 범위 밖입니다.

### 14.2 교재 PART 06을 베이스라인으로 사용

교재의 탑다운 슈터/좀비 서바이버 구조를 그대로 복제하지 말고 다음 기능을 재사용 가능한 코어로 해석해 확장합니다.

- 레벨 아트와 플레이어 준비 -> 대형 월드의 환경 청크, 조명, 애니메이션 레이어, IK, 카메라 구조로 확장합니다.
- 총기 슈터 -> 조준, 발사, 데미지 인터페이스, 탄약, 재장전, 투척물, 이펙트 시스템으로 확장합니다.
- 생명과 좀비 AI -> 플레이어/좀비 공통 생명체 베이스, 상태, 네비게이션, 탐지/추적/공격 AI로 확장합니다.
- UI와 포스트 프로세싱 -> HUD와 별도의 인벤토리/장비/상태 화면, 설정, 저장 UI로 확장합니다.

### 14.3 현재 보유 에셋의 우선 용도

에셋은 먼저 존재 여부와 실제 폴더 구조를 확인한 뒤 사용합니다. 이름만 보고 API나 프리팹 구조를 추측하지 않습니다.

- `Survivalist character`, `Stylized Character Female` -> 플레이어 캐릭터 후보/테스트 캐릭터.
- `Zombie` -> 기본 좀비 원형.
- `MC Sample - Believable 3D Animations by MoCap Central`, `Human Soldier Animations FREE` -> 이동/전투 애니메이션 후보.
- `IK Helper Tool` -> 무기 손 위치 보정 및 양손 그립.
- `Weapons Pack - Realistic LowPoly`, `Crusader Weapon`, `Free pack of medieval weapons` -> 총기/근접무기 풀.
- `Grenade System — Free Edition`, `Grenade M18 Smoke`, `War FX` -> 수류탄/연막/전투 VFX.
- `Too Many Crosshairs` -> 조준점 UI.
- `Motorcycle Interaction Animset FREE`, `Post Apocalyptic Motorcycle 3D Model Rigged Off Road` -> 오토바이 탑승/하차/주행 프로토타입.
- `Realistic Crate & Chest Bundle - 4K PBR Props` -> 루팅 컨테이너.
- `School Scene` -> 주요 POI 후보.
- `Post Apocalyptic World Pack` -> 월드 환경, 도로, 폐허, 배경 오브젝트의 주 재료.

### 14.4 하네스 실행 순서

비사소한 작업은 다음 순서로 진행합니다.

1. `GAME_DESIGN.md`, `checklist.md`, `context-notes.md`와 실제 관련 코드를 읽습니다.
2. 이번 작업에서 바꿀 시스템과 완료 조건을 3~7줄로 적습니다.
3. 가장 작은 수직 슬라이스로 구현합니다. 한 번에 전투, UI, 저장, 월드 생성 같은 여러 대형 시스템을 섞지 않습니다.
4. Unity 컴파일 오류를 먼저 제거하고, 가능하면 관련 EditMode/PlayMode 테스트 또는 재현 가능한 수동 플레이 절차로 검증합니다.
5. `checklist.md`를 갱신하고 결정 사항은 `context-notes.md`에 append 합니다.
6. 검증된 한 논리 단위만 Git 커밋합니다.
7. 원격 저장소와 인증이 준비되어 있으면 해당 커밋을 즉시 `git push` 합니다.

### 14.5 GitHub 커밋 및 푸시 규칙

- 기능 구현 전 `git status --short`와 현재 브랜치를 확인합니다.
- 사용자의 미커밋 변경은 절대 되돌리거나 덮어쓰지 않습니다.
- 하나의 커밋은 하나의 의미 있는 변경만 담습니다.
- 커밋 메시지는 가능하면 한국어 명령형으로 짧게 작성합니다. 예: `플레이어 체력 시스템 추가`, `시드 기반 섹터 배치 추가`.
- 테스트 또는 Unity 컴파일이 실패한 상태에서는 원칙적으로 커밋/푸시하지 않습니다. 실패 원인을 명확히 기록해야 하는 특별한 경우에만 예외로 합니다.
- 커밋 후 원격 저장소가 설정되어 있고 인증이 가능하면 `git push` 합니다.
- push 실패 시 재시도 전에 실제 오류를 읽고, 강제 푸시(`--force`, `--force-with-lease`)는 사용자가 명시적으로 요청하지 않는 한 금지합니다.
- 저장소가 없거나 원격/인증이 준비되지 않았다면 임의로 저장소를 만들지 말고, 수행하지 못한 이유와 필요한 다음 단계만 보고합니다.

### 14.6 아키텍처 경계

다음 시스템은 서로 직접 결합하지 말고 명확한 데이터/API 경계를 둡니다.

- `Player/Camera`와 `Combat`.
- `Combat`와 `Damage/LivingEntity`.
- `Inventory`와 `Equipment`.
- `Inventory/Equipment`와 `UI`.
- `WorldGeneration`과 `RuntimeWorldState`.
- `SaveSystem`과 각 도메인 시스템.
- `Motorcycle`과 일반 플레이어 이동.

ScriptableObject는 아이템/무기/드랍 테이블/월드 청크 메타데이터처럼 정적 정의 데이터에 우선 사용합니다. 런타임 상태를 ScriptableObject 에셋에 직접 저장하지 않습니다.

### 14.7 시드 기반 월드 생성 규칙

- 같은 `WorldSeed`와 같은 생성기 버전은 같은 초기 월드 구성을 만들어야 합니다.
- 월드는 도시/도로/산/초원/주요 POI 같은 저작된 청크를 조합합니다.
- 도로 연결, 플레이어 시작 지점, 필수 POI 접근성 같은 제약을 먼저 만족시킨 뒤 가중치 랜덤을 적용합니다.
- 저장 데이터에는 최소한 `WorldSeed`, 생성기 버전, 플레이어가 바꾼 월드 상태를 분리해 기록합니다.
- 월드 생성 결과 전체를 매 저장마다 중복 직렬화하기 전에, 시드로 재현 가능한 부분과 런타임 변경분을 분리합니다.

### 14.8 저장 시스템 규칙

- 플레이어에게 10개의 저장 슬롯을 제공합니다.
- 각 슬롯은 수동 저장 스냅샷과 자동저장 스냅샷을 구분할 수 있어야 합니다.
- 저장 대상은 플레이어 상태, 인벤토리/장비, 위치, 월드 시드, 루팅/파괴/건설 상태, 오토바이 상태, 게임 진행 상태를 포함합니다.
- 저장 파일에는 `SaveVersion`을 둡니다. 구조 변경 시 마이그레이션 또는 명시적인 비호환 처리를 합니다.
- 저장 중 게임 상태를 직접 참조해 부분 저장하지 말고, 먼저 일관된 저장 DTO/스냅샷을 만든 뒤 디스크에 기록합니다.
- 자동저장은 전투 중 매 프레임 같은 방식으로 실행하지 않습니다. 안전한 이벤트 지점과 간격을 사용합니다.

### 14.9 구현 우선순위

1. 플레이어 이동과 1인칭/3인칭 카메라 시험 구조.
2. 기본 좀비 1종과 총기 1종의 전투 수직 슬라이스.
3. 루팅 컨테이너와 아이템/탄약/회복 드랍.
4. 인벤토리, 장비, 상태 UI.
5. 자원 채집과 최소 거점 건설.
6. 오토바이 탑승/하차/주행.
7. 시드 기반 월드 청크 생성과 대형 월드 스트리밍.
8. 10슬롯 저장/불러오기와 자동저장.
9. 콘텐츠 확장, 난이도, VFX/SFX, 최적화.

### 14.10 완료 보고 형식

최종 응답에는 반드시 다음을 포함합니다.

- 변경한 파일과 핵심 변경 내용.
- 실제로 실행한 Unity/테스트/검증 명령과 결과.
- 생성한 Git 커밋 해시와 커밋 메시지.
- `git push` 결과 또는 수행하지 못한 구체적인 이유.
- 다음 작업에 영향을 주는 미해결 위험이나 결정 사항.
