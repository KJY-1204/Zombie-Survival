// 화면 우하단에 지금 든 무기와 남은 탄약을 보여준다 (PlayerShooter의 공개 상태만 읽는다)
using UnityEngine;
using UnityEngine.UI;

public class WeaponHudUI : MonoBehaviour {
    public GameObject root; // 무기가 없을 때 통째로 숨길 영역
    public Image weaponIcon; // 장착한 무기 아이콘
    public Text weaponName; // 무기 이름
    public Text ammoText; // 탄창 / 남은 탄약

    private PlayerShooter shooter; // 손에 든 총을 알려주는 컴포넌트
    private Equipment equipment; // 아이콘과 이름을 가져올 장비

    private void Start() {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();

        if (player != null)
        {
            shooter = player.GetComponent<PlayerShooter>();
            equipment = player.GetComponent<Equipment>();
        }
    }

    private void Update() {
        EquippedWeapon equippedWeapon = shooter != null ? shooter.equippedWeapon : null;

        // 전체 화면이 열려 있는 동안에는 숨긴다
        if (equippedWeapon == null || UIManager.isScreenOpen)
        {
            root.SetActive(false);
            return;
        }

        root.SetActive(true);
        ammoText.text = equippedWeapon.GetAmmoLabel();
        ammoText.color = UiTheme.TextPrimary;

        ItemData weapon = shooter.currentWeaponItem;
        weaponName.text = weapon != null ? weapon.displayName : equippedWeapon.name;
        weaponIcon.sprite = weapon != null ? weapon.icon : null;
        weaponIcon.enabled = weaponIcon.sprite != null;
    }

}
