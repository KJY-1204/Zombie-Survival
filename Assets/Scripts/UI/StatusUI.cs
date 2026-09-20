// 체력/방어력/무게/장착 무기의 핵심 수치를 읽기 전용으로 보여주는 탭
using UnityEngine;
using UnityEngine.UI;

public class StatusUI : TabView {
    [Header("체력")]
    public RectTransform healthFill; // 체력 막대의 채움 부분
    public Text healthText; // 현재 체력 / 최대 체력

    [Header("수치")]
    public Text statsText; // 방어력, 무게, 손에 든 무기
    public Text equipText; // 슬롯별 장착 상태

    private PlayerHealth playerHealth; // 체력을 읽을 대상
    private Inventory inventory; // 무게를 읽을 대상
    private Equipment equipment; // 방어력과 장착 무기를 읽을 대상
    private PlayerShooter playerShooter; // 지금 손에 든 총을 읽을 대상

    private void Start() {
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            inventory = playerHealth.GetComponent<Inventory>();
            equipment = playerHealth.GetComponent<Equipment>();
            playerShooter = playerHealth.GetComponent<PlayerShooter>();
        }
    }

    // 체력은 이벤트가 없어서 보이는 동안 매 프레임 갱신한다
    private void Update() {
        if (isVisible)
        {
            Refresh();
        }
    }

    public override void Refresh() {
        if (playerHealth == null || !isVisible)
        {
            return;
        }

        float ratio = playerHealth.startingHealth > 0f
            ? Mathf.Clamp01(playerHealth.health / playerHealth.startingHealth)
            : 0f;

        healthFill.anchorMax = new Vector2(ratio, 1f);
        healthFill.GetComponent<Image>().color = UiTheme.BarColorForRemaining(ratio);
        healthText.text = $"{playerHealth.health:F0} / {playerHealth.startingHealth:F0}";

        statsText.text = string.Join("\n", new[] {
            $"총 방어력      {equipment.totalArmor}",
            $"무게          {inventory.totalWeight:F2} / {inventory.maxWeight:F1} kg"
                + (inventory.isOverweight ? "   (과적 - 이동속도 감소)" : ""),
            $"손에 든 무기   {DescribeHeldWeapon()}",
        });

        equipText.text = string.Join("\n", new[] {
            $"주무기        {DescribeSlot(EquipmentSlot.PrimaryWeapon)}",
            $"보조무기      {DescribeSlot(EquipmentSlot.SecondaryWeapon)}",
            $"근접          {DescribeSlot(EquipmentSlot.Melee)}",
            $"머리          {DescribeSlot(EquipmentSlot.Head)}",
            $"상체          {DescribeSlot(EquipmentSlot.Chest)}",
            $"하체          {DescribeSlot(EquipmentSlot.Legs)}",
        });
    }

    // 지금 손에 든 총의 공격력과 탄약
    private string DescribeHeldWeapon() {
        Gun gun = playerShooter.gun;

        if (gun == null)
        {
            return "맨손";
        }

        return $"공격력 {gun.gunData.damage:F0}   탄약 {gun.magAmmo} / {gun.ammoRemain}";
    }

    // 해당 슬롯에 장착한 아이템 이름과 수치
    private string DescribeSlot(EquipmentSlot slot) {
        ItemData item = equipment.Get(slot);

        if (item == null)
        {
            return "-";
        }

        if (item is ArmorItemData armor)
        {
            return $"{armor.displayName}   방어 +{armor.armor}";
        }

        return item.displayName;
    }
}
