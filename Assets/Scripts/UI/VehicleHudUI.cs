// 오토바이에 타고 있는 동안 연료와 내구도를 화면에 표시한다 (차량의 공개 상태만 읽는다)
using UnityEngine;
using UnityEngine.UI;

public class VehicleHudUI : MonoBehaviour {
    public Text statusText; // 연료/내구도를 표시할 텍스트

    private RiderControl rider; // 지금 무엇을 타고 있는지 알려주는 컴포넌트

    private void Start() {
        PlayerHealth player = FindObjectOfType<PlayerHealth>();

        if (player != null)
        {
            rider = player.GetComponent<RiderControl>();
        }

        statusText.text = string.Empty;
    }

    private void Update() {
        // 타고 있지 않거나 화면이 열려 있으면 숨긴다
        if (rider == null || !rider.isRiding || UIManager.isScreenOpen)
        {
            statusText.text = string.Empty;
            return;
        }

        Motorcycle vehicle = rider.vehicle;

        string warning = vehicle.fuel <= 0f
            ? "   연료 없음"
            : (vehicle.durability <= 0f ? "   고장" : string.Empty);

        statusText.text =
            $"연료 {vehicle.fuel:F0} / {vehicle.maxFuel:F0}"
            + $"     내구 {vehicle.durability:F0} / {vehicle.maxDurability:F0}"
            + warning;
    }
}
