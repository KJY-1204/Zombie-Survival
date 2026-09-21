// 플레이어가 손에 든 모든 무기가 공유하는 공격과 재장전 계약
using UnityEngine;

public abstract class EquippedWeapon : MonoBehaviour {
    public abstract float spreadRatio { get; }
    public abstract bool Fire();

    public virtual bool Reload() {
        return false;
    }

    public virtual void SetOwner(Transform owner) {
    }

    public virtual string GetAmmoLabel() {
        return string.Empty;
    }
}
