// 오토바이 하나의 저장 상태 (MonoBehaviour가 아니라 저장 DTO로 그대로 옮길 수 있는 순수 데이터)
using System;
using UnityEngine;

[Serializable]
public class MotorcycleSaveData {
    public Vector3 position; // 세워둔 위치
    public float rotationY; // 방향(Y축만)
    public float fuel; // 남은 연료
    public float durability; // 남은 내구도
    public bool isRidden; // 저장 시점에 플레이어가 타고 있었는지
}
