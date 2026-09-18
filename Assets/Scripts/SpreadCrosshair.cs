// 탄퍼짐 정도에 따라 4방향 조준선이 벌어지는 조준점 UI
using UnityEngine;

public class SpreadCrosshair : MonoBehaviour {
    public RectTransform top;
    public RectTransform bottom;
    public RectTransform left;
    public RectTransform right;

    public float baseGap = 8f; // 탄퍼짐이 없을 때 중심에서 떨어진 거리
    public float maxGap = 24f; // 탄퍼짐이 최대일 때 추가로 벌어지는 거리

    // normalizedSpread: 0(탄퍼짐 없음) ~ 1(최대 탄퍼짐)
    public void SetSpread(float normalizedSpread) {
        float gap = baseGap + maxGap * Mathf.Clamp01(normalizedSpread);

        top.anchoredPosition = new Vector2(0f, gap);
        bottom.anchoredPosition = new Vector2(0f, -gap);
        left.anchoredPosition = new Vector2(-gap, 0f);
        right.anchoredPosition = new Vector2(gap, 0f);
    }
}
