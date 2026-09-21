// 전방 부채꼴 안의 가장 가까운 피해 대상을 한 번 타격하는 장착 무기
using UnityEngine;

public class MeleeWeapon : EquippedWeapon {
    public MeleeWeaponData weaponData;
    public override float spreadRatio => 0f;
    public override bool usesLeftHandGrip => false;

    private Transform ownerRoot;
    private float lastAttackTime;
    private bool attackPending;

    public override void SetOwner(Transform owner) {
        ownerRoot = owner;
    }

    public override bool BeginAttack() {
        if (attackPending || !CanAttack())
        {
            return false;
        }

        attackPending = true;
        return true;
    }

    public override bool ResolveAnimationHit() {
        if (!attackPending)
        {
            return false;
        }

        attackPending = false;
        return Fire();
    }

    public override bool Fire() {
        if (!CanAttack())
        {
            return false;
        }

        lastAttackTime = Time.time;
        LivingEntity target = FindTarget();

        if (target != null)
        {
            Vector3 hitPoint = target.transform.position + Vector3.up;
            Vector3 hitNormal = (hitPoint - ownerRoot.position).normalized;
            target.OnDamage(weaponData.damage, hitPoint, hitNormal);
        }

        NoiseEvent.Emit(ownerRoot.position, weaponData.noiseRadius);
        return true;
    }

    private bool CanAttack() {
        return weaponData != null && ownerRoot != null
            && Time.time >= lastAttackTime + weaponData.attackInterval;
    }

    public override string GetAmmoLabel() {
        return "근접";
    }

    private LivingEntity FindTarget() {
        Collider[] colliders = Physics.OverlapSphere(ownerRoot.position, weaponData.range);
        LivingEntity closest = null;
        float closestDistance = float.MaxValue;

        foreach (Collider collider in colliders)
        {
            LivingEntity candidate = collider.GetComponentInParent<LivingEntity>();

            if (candidate == null || candidate.dead || candidate.transform == ownerRoot
                || candidate.transform.IsChildOf(ownerRoot) || ownerRoot.IsChildOf(candidate.transform))
            {
                continue;
            }

            Vector3 flatOffset = Vector3.ProjectOnPlane(
                candidate.transform.position - ownerRoot.position, Vector3.up);

            if (flatOffset == Vector3.zero
                || Vector3.Angle(ownerRoot.forward, flatOffset) > weaponData.arc * 0.5f)
            {
                continue;
            }

            float distance = flatOffset.sqrMagnitude;

            if (distance < closestDistance)
            {
                closest = candidate;
                closestDistance = distance;
            }
        }

        return closest;
    }
}
