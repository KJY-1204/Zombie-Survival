// 사용하면 즉시 효과가 발동하고 1개 소모되는 아이템의 정의
using UnityEngine;

// 소비 아이템이 발동시킬 효과의 종류
public enum ConsumableEffect {
    Heal, // 체력 회복
    Ammo, // 장착한 총의 남은 탄약 충전
    Score // 점수 증가
}

[CreateAssetMenu(menuName = "Scriptable/Item/Consumable", fileName = "Consumable Item")]
public class ConsumableItemData : ItemData {
    public ConsumableEffect effect = ConsumableEffect.Heal; // 발동할 효과
    public float amount = 50f; // 효과의 크기(회복량, 탄약 수, 점수)

    // target에게 효과를 적용한다. 효과가 실제로 적용됐을 때만 true를 반환해
    // 아무 일도 일어나지 않은 경우에는 아이템이 소모되지 않게 한다
    public bool Apply(GameObject target) {
        switch (effect)
        {
            case ConsumableEffect.Heal:
                LivingEntity life = target.GetComponent<LivingEntity>();
                if (life == null || life.dead)
                {
                    return false;
                }
                life.RestoreHealth(amount);
                return true;

            case ConsumableEffect.Ammo:
                PlayerShooter shooter = target.GetComponent<PlayerShooter>();
                if (shooter == null || shooter.gun == null)
                {
                    return false;
                }
                shooter.gun.ammoRemain += (int)amount;
                return true;

            case ConsumableEffect.Score:
                // 게임 오버 상태에서는 AddScore가 조용히 무시되므로 아이템도 소모하지 않는다
                if (GameManager.instance == null || GameManager.instance.isGameover)
                {
                    return false;
                }
                GameManager.instance.AddScore((int)amount);
                return true;
        }

        return false;
    }
}
