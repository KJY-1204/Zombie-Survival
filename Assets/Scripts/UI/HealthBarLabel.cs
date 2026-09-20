// 체력 슬라이더 위에 현재/최대 수치를 글자로 겹쳐 보여준다
// 슬라이더 값만 읽으므로 PlayerHealth를 몰라도 된다
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HealthBarLabel : MonoBehaviour {
    public Slider slider; // 값을 읽어올 슬라이더

    private Text label;

    private void Awake() {
        label = GetComponent<Text>();

        if (slider == null)
        {
            slider = GetComponentInParent<Slider>();
        }
    }

    private void Update() {
        if (slider == null)
        {
            return;
        }

        label.text = $"{slider.value:F0} / {slider.maxValue:F0}";
    }
}
