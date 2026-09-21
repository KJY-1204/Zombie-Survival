// 활과 석궁의 발사 속도, 피해량, 탄약을 정의하는 정적 데이터
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/Combat/Projectile Weapon", fileName = "Projectile Weapon Data")]
public class ProjectileWeaponData : ScriptableObject {
    public ItemData ammoItem;
    public GameObject projectilePrefab;
    public float damage = 40f;
    public float projectileSpeed = 45f;
    public float fireInterval = 0.8f;
    public float reloadTime = 0.8f;
    public int magazineCapacity = 1;
    public float noiseRadius = 12f;
}
