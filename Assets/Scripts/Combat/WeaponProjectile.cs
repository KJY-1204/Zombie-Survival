// 활과 석궁의 탄환이 충돌 대상에게 피해를 주고 사라지게 하는 투사체
using UnityEngine;

public class WeaponProjectile : MonoBehaviour {
    private Transform ownerRoot;
    private float damage;
    private float speed;
    private float remainingLifetime;

    public void Launch(Transform owner, Vector3 direction, float newSpeed, float newDamage) {
        ownerRoot = owner;
        speed = newSpeed;
        damage = newDamage;
        remainingLifetime = 5f;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void Update() {
        float distance = speed * Time.deltaTime;
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, distance, ~0, QueryTriggerInteraction.Ignore);
        RaycastHit? closest = FindClosestValidHit(hits);

        if (closest != null)
        {
            RaycastHit hit = closest.Value;
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            if (target != null)
            {
                target.OnDamage(damage, hit.point, hit.normal);
            }

            Destroy(gameObject);
            return;
        }

        transform.position += transform.forward * distance;
        remainingLifetime -= Time.deltaTime;

        if (remainingLifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private RaycastHit? FindClosestValidHit(RaycastHit[] hits) {
        RaycastHit? closest = null;

        foreach (RaycastHit hit in hits)
        {
            if (ownerRoot != null && hit.transform.IsChildOf(ownerRoot))
            {
                continue;
            }

            if (closest == null || hit.distance < closest.Value.distance)
            {
                closest = hit;
            }
        }

        return closest;
    }
}
