// 총소리 같은 큰 소리를 월드에 알리는 통로. 소리를 내는 쪽과 듣는 쪽이 서로를 모르게 한다
using System;
using UnityEngine;

public static class NoiseEvent {
    // 소리가 난 위치와 들리는 반경. 듣는 쪽(좀비)이 구독한다
    public static event Action<Vector3, float> onNoise;

    public static void Emit(Vector3 position, float radius) {
        if (onNoise != null)
        {
            onNoise(position, radius);
        }
    }
}
