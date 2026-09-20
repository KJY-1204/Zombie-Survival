// 스트리밍 월드에서 플레이어 주변에 좀비 수를 일정하게 유지한다
// 교재의 ZombieSpawner는 고정 스폰 지점 + 무한 웨이브라 대형 월드에서는 쓸 수 없다
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldZombieSpawner : MonoBehaviour {
    public Zombie zombiePrefab; // 생성할 좀비 원본
    public ZombieData[] zombieDatas; // 사용할 좀비 셋업 데이터들
    public Transform viewer; // 기준이 될 대상 (보통 플레이어). 비우면 자동으로 찾는다

    [Header("유지 개수")]
    public int maxAlive = 8; // 주변에 동시에 살아 있을 좀비 수
    public float checkInterval = 1f; // 몇 초마다 주변을 다시 계산할지

    [Header("거리")]
    public float minSpawnDistance = 25f; // 이보다 가까이에는 생성하지 않는다 (눈앞에 튀어나오지 않게)
    public float maxSpawnDistance = 45f; // 이보다 멀리에는 생성하지 않는다
    public float despawnDistance = 90f; // 이보다 멀어진 좀비는 정리한다

    private readonly List<Zombie> zombies = new List<Zombie>();
    private float nextCheckTime;

    public int aliveCount => zombies.Count;

    private void Start() {
        if (viewer == null)
        {
            PlayerHealth player = FindFirstObjectByType<PlayerHealth>();

            if (player != null)
            {
                viewer = player.transform;
            }
        }
    }

    private void Update() {
        if (viewer == null || zombiePrefab == null || zombieDatas == null
            || zombieDatas.Length == 0 || Time.time < nextCheckTime)
        {
            return;
        }

        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        nextCheckTime = Time.time + checkInterval;

        RemoveDistantZombies();

        if (zombies.Count < maxAlive)
        {
            TrySpawnOne();
        }
    }

    // 플레이어가 멀리 떠난 좀비는 지운다. 청크가 내려가도 좀비는 남기 때문이다
    private void RemoveDistantZombies() {
        for (int i = zombies.Count - 1; i >= 0; i--)
        {
            Zombie zombie = zombies[i];

            if (zombie == null)
            {
                zombies.RemoveAt(i);
                continue;
            }

            if (Vector3.Distance(zombie.transform.position, viewer.position) > despawnDistance)
            {
                zombies.RemoveAt(i);
                Destroy(zombie.gameObject);
            }
        }
    }

    // 플레이어 주변 고리 안의 NavMesh 위에 한 마리 생성한다
    private void TrySpawnOne() {
        if (!TryFindSpawnPoint(out Vector3 position))
        {
            return;
        }

        Zombie zombie = Instantiate(
            zombiePrefab, position, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
        zombie.Setup(zombieDatas[Random.Range(0, zombieDatas.Length)]);

        zombies.Add(zombie);
        zombie.onDeath += () => zombies.Remove(zombie);
        zombie.onDeath += () => Destroy(zombie.gameObject, 10f);

        if (GameManager.instance != null)
        {
            zombie.onDeath += () => GameManager.instance.AddScore(100);
        }
    }

    // 고리 안에서 NavMesh 위의 지점을 찾는다. 스트리밍 경계 밖은 NavMesh가 없으므로 실패할 수 있다
    private bool TryFindSpawnPoint(out Vector3 position) {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 candidate = viewer.position
                + new Vector3(direction.x, 0f, direction.y) * distance;

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                position = hit.position;
                return true;
            }
        }

        position = Vector3.zero;
        return false;
    }

    // 검증용 요약
    public string Describe() {
        return $"살아있음 {zombies.Count}/{maxAlive}";
    }
}
