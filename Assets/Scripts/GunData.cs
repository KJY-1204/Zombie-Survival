using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/GunData", fileName = "Gun Data")]
public class GunData : ScriptableObject
{
    public AudioClip shotClip; // 발사 소리
    public AudioClip reloadClip; // 재장전 소리

    public float damage = 25; // 공격력

    public int startAmmoRemain = 100; // 처음에 주어질 전체 탄약
    public int magCapacity = 25; // 탄창 용량

    public float timeBetFire = 0.12f; // 총알 발사 간격
    public float reloadTime = 1.8f; // 재장전 소요 시간

    public float minSpread = 1f; // 정지 상태의 최소 탄퍼짐 각도(도)
    public float maxSpread = 6f; // 최대 탄퍼짐 각도(도)
    public float spreadIncrement = 1.2f; // 발사 1회당 늘어나는 탄퍼짐 각도
    public float spreadRecoverSpeed = 4f; // 초당 회복되는 탄퍼짐 각도
}