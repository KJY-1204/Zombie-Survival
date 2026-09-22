using System.Collections;
using UnityEngine;
using UnityEngine.AI; // AI, 내비게이션 시스템 관련 코드를 가져오기

// 좀비 AI 구현
public class Zombie : LivingEntity {
    public LayerMask whatIsTarget; // 추적 대상 레이어

    [Header("시야")]
    public float sightRange = 15f; // 눈으로 발견할 수 있는 거리
    public float sightAngle = 110f; // 시야각(전체 각도)
    public float loseSightTime = 4f; // 대상을 놓친 뒤 추적을 포기하기까지의 시간

    [Header("청각")]
    public float investigateStopDistance = 2f; // 소리 난 지점에 이 거리까지 가면 도착으로 본다
    public float investigateGiveUpTime = 8f; // 소리를 쫓다가 포기하기까지의 시간

    private LivingEntity targetEntity; // 추적할 대상
    private NavMeshAgent navMeshAgent; // 경로계산 AI 에이전트

    private bool isInvestigating; // 소리가 난 지점으로 가는 중인지
    private Vector3 investigatePosition; // 소리가 난 지점
    private float investigateUntil; // 이 시점이 지나면 조사를 포기한다
    private float lastSeenTime = float.NegativeInfinity; // 대상을 마지막으로 본 시점
    private bool isPatrolling; // 유휴 상태에서 배회 지점으로 이동 중인지
    private float patrolWaitUntil; // 다음 배회 지점을 고를 수 있는 시점

    public ParticleSystem hitEffect; // 피격시 재생할 파티클 효과
    public AudioClip deathSound; // 사망시 재생할 소리
    public AudioClip hitSound; // 피격시 재생할 소리

    private Animator zombieAnimator; // 애니메이터 컴포넌트
    private AudioSource zombieAudioPlayer; // 오디오 소스 컴포넌트
    private Renderer zombieRenderer; // 렌더러 컴포넌트

    public float damage = 20f; // 공격력
    public float timeBetAttack = 0.5f; // 공격 간격
    private float lastAttackTime; // 마지막 공격 시점

    // 추적할 대상이 존재하는지 알려주는 프로퍼티
    private bool hasTarget
    {
        get
        {
            // 추적할 대상이 존재하고, 대상이 사망하지 않았다면 true
            if (targetEntity != null && !targetEntity.dead)
            {
                return true;
            }

            // 그렇지 않다면 false
            return false;
        }
    }

    private void Awake() {
        // 게임 오브젝트로부터 사용할 컴포넌트들을 가져오기
        navMeshAgent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        zombieAudioPlayer = GetComponent<AudioSource>();

        // 렌더러 컴포넌트는 자식 게임 오브젝트에게 있으므로
        // GetComponentInChildren() 메서드를 사용
        zombieRenderer = GetComponentInChildren<Renderer>();
    }

    // 좀비 AI의 초기 스펙을 결정하는 셋업 메서드
    public void Setup(ZombieData zombieData) {
        // 체력 설정
        startingHealth = zombieData.health;
        health = zombieData.health;
        // 공격력 설정
        damage = zombieData.damage;
        // 내비메시 에이전트의 이동 속도 설정
        navMeshAgent.speed = zombieData.speed;
        // 렌더러가 사용중인 머테리얼의 컬러를 변경, 외형 색이 변함
        zombieRenderer.material.color = zombieData.skinColor;
    }

    private void Start() {
        // 게임 오브젝트 활성화와 동시에 AI의 추적 루틴 시작
        StartCoroutine(UpdatePath());
    }

    protected override void OnEnable() {
        // LivingEntity의 상태 초기화(체력 복구, dead 해제)를 그대로 실행한다
        base.OnEnable();
        // 총소리 같은 큰 소리를 듣는다
        NoiseEvent.onNoise += OnHearNoise;
    }

    private void OnDisable() {
        NoiseEvent.onNoise -= OnHearNoise;
    }

    private void Update() {
        // 쫓아가는 대상, 소리 조사, 배회 중이면 걷는 애니메이션을 재생
        zombieAnimator.SetBool("HasTarget", hasTarget || isInvestigating || isPatrolling);
    }

    // 소리가 들리면 그 지점으로 가본다 (이미 대상을 쫓는 중이면 무시)
    private void OnHearNoise(Vector3 position, float radius) {
        if (dead || hasTarget)
        {
            return;
        }

        if (Vector3.Distance(transform.position, position) > radius)
        {
            return;
        }

        StartInvestigating(position);
    }

    // 소리가 난 지점(또는 마지막으로 본 자리)으로 가도록 설정한다
    // 그 지점이 NavMesh 밖이면 가장 가까운 갈 수 있는 자리로 바꿔 잡는다
    private void StartInvestigating(Vector3 position) {
        isPatrolling = false;

        if (NavMesh.SamplePosition(position, out NavMeshHit hit, 8f, NavMesh.AllAreas))
        {
            investigatePosition = hit.position;
        }
        else
        {
            // 갈 수 없는 자리면 조사하지 않는다
            isInvestigating = false;
            return;
        }

        isInvestigating = true;
        investigateUntil = Time.time + investigateGiveUpTime;
    }

    // 눈으로 볼 수 있는 대상을 찾는다
    // 거리 + 시야각 + 시야 차단을 모두 확인하므로 벽 너머의 플레이어는 보지 못한다
    private LivingEntity FindVisibleTarget() {
        Collider[] colliders =
            Physics.OverlapSphere(transform.position, sightRange, whatIsTarget);

        foreach (Collider collider in colliders)
        {
            LivingEntity candidate = collider.GetComponent<LivingEntity>();

            if (candidate == null || candidate.dead)
            {
                continue;
            }

            if (IsVisible(candidate))
            {
                return candidate;
            }
        }

        return null;
    }

    // 대상이 시야각 안에 있고 사이에 막히는 것이 없는지
    private bool IsVisible(LivingEntity candidate) {
        // 발끝이 아니라 가슴 높이를 기준으로 본다
        Vector3 eye = transform.position + Vector3.up * 1.5f;
        Vector3 targetPoint = candidate.transform.position + Vector3.up * 1.0f;
        Vector3 toTarget = targetPoint - eye;

        if (toTarget.magnitude > sightRange)
        {
            return false;
        }

        // 시야각 밖이면 보지 못한다
        float angle = Vector3.Angle(transform.forward, new Vector3(toTarget.x, 0f, toTarget.z));

        if (angle > sightAngle * 0.5f)
        {
            return false;
        }

        // 사이에 벽 같은 것이 있으면 보지 못한다
        if (Physics.Raycast(eye, toTarget.normalized, out RaycastHit hit,
            toTarget.magnitude, ~0, QueryTriggerInteraction.Ignore))
        {
            // 맞은 것이 대상 자신이 아니면 가려진 것이다
            if (!hit.transform.IsChildOf(candidate.transform)
                && hit.transform != candidate.transform)
            {
                return false;
            }
        }

        return true;
    }

    // 주기적으로 추적할 대상의 위치를 찾아 경로를 갱신
    private IEnumerator UpdatePath() {
        // 살아있는 동안 무한 루프
        while (!dead)
        {
            // 눈에 보이는 대상이 있으면 그것을 쫓는다. 소리를 쫓던 중이어도 눈이 우선이다
            LivingEntity visible = FindVisibleTarget();

            if (visible != null)
            {
                targetEntity = visible;
                lastSeenTime = Time.time;
                isInvestigating = false;
                isPatrolling = false;
            }
            else if (hasTarget && Time.time > lastSeenTime + loseSightTime)
            {
                // 한동안 못 보면 놓친 것으로 보고, 마지막으로 본 자리를 조사한다
                StartInvestigating(targetEntity.transform.position);
                targetEntity = null;
            }

            if (hasTarget)
            {
                // 쫓는 대상이 있으면 계속 따라간다 (잠깐 시야에서 사라져도 loseSightTime 동안은 쫓는다)
                navMeshAgent.isStopped = false;
                navMeshAgent.SetDestination(targetEntity.transform.position);
            }
            else if (isInvestigating)
            {
                // 소리가 난 지점으로 간다
                bool arrived = Vector3.Distance(transform.position, investigatePosition)
                    <= investigateStopDistance;

                if (arrived || Time.time > investigateUntil)
                {
                    // 도착했거나 너무 오래 걸렸으면 포기하고 제자리에 선다
                    isInvestigating = false;
                    navMeshAgent.isStopped = true;
                }
                else
                {
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(investigatePosition);
                }
            }
            else
            {
                UpdatePatrol();
            }

            // 0.25초 주기로 처리 반복
            yield return new WaitForSeconds(0.25f);
        }
    }

    // 유휴 좀비가 NavMesh 위의 가까운 지점을 천천히 오간다
    private void UpdatePatrol() {
        if (isPatrolling)
        {
            if (navMeshAgent.pathPending
                || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
            {
                return;
            }

            isPatrolling = false;
            navMeshAgent.isStopped = true;
            patrolWaitUntil = Time.time + Random.Range(2f, 4f);
            return;
        }

        if (Time.time < patrolWaitUntil)
        {
            return;
        }

        Vector2 direction = Random.insideUnitCircle.normalized;
        Vector3 candidate = transform.position + new Vector3(direction.x, 0f, direction.y)
            * Random.Range(3f, 8f);

        if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 4f, NavMesh.AllAreas))
        {
            isPatrolling = true;
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(hit.position);
        }
        else
        {
            patrolWaitUntil = Time.time + 2f;
        }
    }

    // 데미지를 입었을때 실행할 처리
    public override void OnDamage(float damage,
        Vector3 hitPoint, Vector3 hitNormal) {
        // 아직 사망하지 않은 경우에만 피격 효과 재생
        if (!dead)
        {
            // 공격 받은 지점과 방향으로 파티클 효과를 재생
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation
                = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();

            // 피격 효과음 재생
            zombieAudioPlayer.PlayOneShot(hitSound);
        }

        // LivingEntity의 OnDamage()를 실행하여 데미지 적용
        base.OnDamage(damage, hitPoint, hitNormal);
    }

    // 사망 처리
    public override void Die() {
        // LivingEntity의 Die()를 실행하여 기본 사망 처리 실행
        base.Die();

        // 다른 AI들을 방해하지 않도록 자신의 모든 콜라이더들을 비활성화
        Collider[] zombieColliders = GetComponents<Collider>();
        for (int i = 0; i < zombieColliders.Length; i++)
        {
            zombieColliders[i].enabled = false;
        }

        // AI 추적을 중지하고 내비메쉬 컴포넌트를 비활성화
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;

        // 사망 애니메이션 재생
        zombieAnimator.SetTrigger("Die");
        // 사망 효과음 재생
        zombieAudioPlayer.PlayOneShot(deathSound);
    }

    private void OnTriggerStay(Collider other) {
        // 자신이 사망하지 않았으며,
        // 최근 공격 시점에서 timeBetAttack 이상 시간이 지났다면 공격 가능
        if (!dead && Time.time >= lastAttackTime + timeBetAttack)
        {
            // 상대방으로부터 LivingEntity 타입을 가져오기 시도
            LivingEntity attackTarget
                = other.GetComponent<LivingEntity>();

            // 상대방의 LivingEntity가 살아있는 자신의 추적 대상이라면 공격 실행
            // 이미 사망한 대상은 때리지 않는다
            if (attackTarget != null && !attackTarget.dead
                && attackTarget == targetEntity)
            {
                // 최근 공격 시간을 갱신
                lastAttackTime = Time.time;

                // 공격 애니메이션 재생
                zombieAnimator.SetTrigger("Attack");

                // 상대방의 피격 위치와 피격 방향을 근삿값으로 계산
                Vector3 hitPoint
                    = other.ClosestPoint(transform.position);
                Vector3 hitNormal
                    = transform.position - other.transform.position;

                // 공격 실행
                attackTarget.OnDamage(damage, hitPoint, hitNormal);
            }
        }
    }
}
