// 오토바이에 타고 있는 동안 연료와 내구도를 막대로 표시한다 (차량의 공개 상태만 읽는다)
using UnityEngine;
using UnityEngine.UI;

public class VehicleHudUI : MonoBehaviour {
    public GameObject root; // 타고 있지 않을 때 통째로 숨길 영역
    public RectTransform fuelFill;
    public Text fuelText;
    public RectTransform durabilityFill;
    public Text durabilityText;
    public Text warningText; // 연료 없음 / 고장 안내

    private RiderControl rider; // 지금 무엇을 타고 있는지 알려주는 컴포넌트

    private void Start() {
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();

        if (player != null)
        {
            rider = player.GetComponent<RiderControl>();
        }
    }

    private void Update() {
        // 타고 있지 않거나 화면이 열려 있으면 숨긴다
        if (rider == null || !rider.isRiding || UIManager.isScreenOpen)
        {
            root.SetActive(false);
            return;
        }

        root.SetActive(true);
        Motorcycle vehicle = rider.vehicle;

        SetBar(fuelFill, fuelText, vehicle.fuel, vehicle.maxFuel);
        SetBar(durabilityFill, durabilityText, vehicle.durability, vehicle.maxDurability);

        warningText.text = vehicle.fuel <= 0f
            ? "연료 없음"
            : (vehicle.durability <= 0f ? "고장" : string.Empty);
    }

    private static void SetBar(RectTransform fill, Text label, float value, float max) {
        float ratio = max > 0f ? Mathf.Clamp01(value / max) : 0f;

        fill.anchorMax = new Vector2(ratio, 1f);
        fill.GetComponent<Image>().color = UiTheme.BarColorForRemaining(ratio);
        label.text = $"{value:F0} / {max:F0}";
    }
}
