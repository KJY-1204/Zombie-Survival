// 인벤토리의 화살이나 볼트를 장전해 물리 투사체로 발사하는 장착 무기
using System.Collections;
using UnityEngine;

public class ProjectileWeapon : EquippedWeapon {
    public Transform fireTransform;
    public ProjectileWeaponData weaponData;

    public int magAmmo { get; private set; }
    public override float spreadRatio => 0f;

    private Transform ownerRoot;
    private Inventory ownerInventory;
    private Camera aimCamera;
    private float lastFireTime;
    private bool isReloading;
    private bool attackPending;

    private void Awake() {
        aimCamera = Camera.main;
    }

    public override void SetOwner(Transform owner) {
        ownerRoot = owner;
        ownerInventory = owner == null ? null : owner.GetComponent<Inventory>();
    }

    public override bool BeginAttack() {
        if (attackPending || !CanFire())
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
        if (!CanFire())
        {
            return false;
        }

        Camera camera = aimCamera != null ? aimCamera : Camera.main;

        if (camera == null)
        {
            return false;
        }

        Ray aimRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        GameObject projectileObject = Instantiate(
            weaponData.projectilePrefab, fireTransform.position, Quaternion.LookRotation(aimRay.direction));
        WeaponProjectile projectile = projectileObject.GetComponent<WeaponProjectile>();

        if (projectile == null)
        {
            Destroy(projectileObject);
            return false;
        }

        projectile.Launch(ownerRoot, aimRay.direction, weaponData.projectileSpeed, weaponData.damage);
        magAmmo--;
        lastFireTime = Time.time;
        NoiseEvent.Emit(fireTransform.position, weaponData.noiseRadius);
        return true;
    }

    private bool CanFire() {
        return weaponData != null && weaponData.projectilePrefab != null && fireTransform != null
            && !isReloading && magAmmo > 0 && Time.time >= lastFireTime + weaponData.fireInterval;
    }

    public override bool Reload() {
        if (weaponData == null || ownerInventory == null || weaponData.ammoItem == null
            || isReloading || magAmmo >= weaponData.magazineCapacity
            || ownerInventory.CountOf(weaponData.ammoItem) <= 0)
        {
            return false;
        }

        StartCoroutine(ReloadRoutine());
        return true;
    }

    public override string GetAmmoLabel() {
        if (weaponData == null || ownerInventory == null || weaponData.ammoItem == null)
        {
            return magAmmo.ToString();
        }

        return $"{magAmmo} / {ownerInventory.CountOf(weaponData.ammoItem)}";
    }

    private IEnumerator ReloadRoutine() {
        isReloading = true;
        yield return new WaitForSeconds(weaponData.reloadTime);

        int amount = Mathf.Min(
            weaponData.magazineCapacity - magAmmo,
            ownerInventory.CountOf(weaponData.ammoItem));

        if (amount > 0 && ownerInventory.Remove(weaponData.ammoItem, amount))
        {
            magAmmo += amount;
        }

        isReloading = false;
    }
}
