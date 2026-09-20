// 무기로 때리면 내구도가 깎이고 부서지면서 재료를 떨어뜨리는 필드 자원 (나무, 바위, 고철)
using System.Collections;
using UnityEngine;

public class ResourceNode : LivingEntity {
    public LootEntry[] drops; // 부서질 때 떨어뜨릴 재료

    public float dropRadius = 0.8f; // 재료가 흩어져 떨어질 반경
    public float dropHeight = 0.5f; // 재료가 떨어질 높이

    public float respawnTime = 60f; // 다시 자라날 때까지 걸리는 시간 (0 이하면 재생성하지 않는다)

    // 청크가 다시 올라와도 채집 상태를 이어받기 위한 id (청크 빌더가 채운다)
    [System.NonSerialized] public string runtimeId;

    public ParticleSystem hitEffect; // 타격할 때 재생할 효과
    public AudioClip hitSound; // 타격 소리
    public AudioClip breakSound; // 부서지는 소리

    private AudioSource audioPlayer; // 소리 재생기
    private Collider nodeCollider; // 부서진 동안 꺼둘 콜라이더
    private Renderer[] nodeRenderers; // 부서진 동안 숨길 렌더러

    private void Awake() {
        audioPlayer = GetComponent<AudioSource>();
        nodeCollider = GetComponent<Collider>();
        nodeRenderers = GetComponentsInChildren<Renderer>();
    }

    // 이미 채집된 자리에 다시 지어졌다면 채집된 모습으로 시작한다.
    // 상태는 OnEnable이 초기화하므로 그보다 늦은 Start에서 돌려놓는다
    private void Start() {
        if (string.IsNullOrEmpty(runtimeId) || WorldRuntimeState.instance == null
            || !WorldRuntimeState.instance.IsHarvested(runtimeId))
        {
            return;
        }

        dead = true;
        health = 0f;
        SetHarvested(true);

        float remaining = WorldRuntimeState.instance.RemainingRespawn(runtimeId);

        if (remaining > 0f)
        {
            StartCoroutine(RespawnAfterDelay(remaining));
        }
    }

    // Gun이 hit.collider.GetComponent<IDamageable>()로 찾으므로
    // 이 컴포넌트와 콜라이더는 반드시 같은 게임 오브젝트에 있어야 한다
    public override void OnDamage(float damage, Vector3 hitPoint,
        Vector3 hitNormal) {
        if (dead)
        {
            return;
        }

        if (hitEffect != null)
        {
            hitEffect.transform.position = hitPoint;
            hitEffect.transform.rotation = Quaternion.LookRotation(hitNormal);
            hitEffect.Play();
        }

        if (audioPlayer != null && hitSound != null)
        {
            audioPlayer.PlayOneShot(hitSound);
        }

        base.OnDamage(damage, hitPoint, hitNormal);
    }

    // 내구도가 다하면 재료를 떨어뜨리고 모습을 감춘다
    public override void Die() {
        base.Die();

        DropAll();

        if (audioPlayer != null && breakSound != null)
        {
            audioPlayer.PlayOneShot(breakSound);
        }

        SetHarvested(true);

        if (!string.IsNullOrEmpty(runtimeId) && WorldRuntimeState.instance != null)
        {
            WorldRuntimeState.instance.MarkHarvested(runtimeId, respawnTime);
        }

        if (respawnTime > 0f)
        {
            StartCoroutine(RespawnAfterDelay(respawnTime));
        }
    }

    // 떨어뜨릴 재료를 주변에 흩뿌린다
    private void DropAll() {
        if (drops == null)
        {
            return;
        }

        foreach (LootEntry entry in drops)
        {
            if (entry.data == null || entry.count <= 0
                || entry.data.worldPrefab == null)
            {
                continue;
            }

            Vector2 offset = Random.insideUnitCircle * dropRadius;
            Vector3 position = transform.position
                + new Vector3(offset.x, dropHeight, offset.y);

            GameObject spawned = Instantiate(
                entry.data.worldPrefab, position, Quaternion.identity);

            WorldItem worldItem = spawned.GetComponent<WorldItem>();
            if (worldItem != null)
            {
                worldItem.count = entry.count;
            }
        }
    }

    // 채집된 동안에는 보이지도 맞지도 않게 한다 (오브젝트 자체는 재생성을 위해 남겨둔다)
    private void SetHarvested(bool harvested) {
        if (nodeCollider != null)
        {
            nodeCollider.enabled = !harvested;
        }

        foreach (Renderer renderer in nodeRenderers)
        {
            renderer.enabled = !harvested;
        }
    }

    private IEnumerator RespawnAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);

        // OnEnable과 같은 상태 초기화 (dead 해제, 내구도 복구)
        dead = false;
        health = startingHealth;
        SetHarvested(false);
    }
}
