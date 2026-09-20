// 플레이어가 타고 내릴 수 있는 오토바이. 탑승 상태의 단일 소유자이고 UI를 알지 못한다
using System;
using UnityEngine;

public class Motorcycle : MonoBehaviour, IInteractable {
    public string displayName = "오토바이"; // 상호작용 안내에 표시할 이름

    public Transform seat; // 라이더를 붙일 좌석 위치
    public Transform exitPoint; // 하차했을 때 내려설 지점

    public bool isRidden { get; private set; } // 지금 누가 타고 있는지
    public GameObject rider { get; private set; } // 타고 있는 대상

    public event Action onStateChanged; // 탑승 상태가 바뀔 때 발동

    private MonoBehaviour driveController; // 에셋의 주행 컴포넌트 (탑승 중에만 켠다)

    private void Awake() {
        // 서드파티 주행 컴포넌트를 이름으로 찾는다 (에셋 코드를 수정하지 않는다)
        foreach (MonoBehaviour behaviour in GetComponents<MonoBehaviour>())
        {
            if (behaviour.GetType().Name == "BicycleVehicle")
            {
                driveController = behaviour;
                break;
            }
        }

        SetDriveEnabled(false);
    }

    public bool CanInteract(GameObject interactor) {
        // 이미 타고 있으면 내리는 것도 상호작용이다
        return !isRidden || rider == interactor;
    }

    public string GetInteractionLabel() {
        return isRidden ? $"{displayName}에서 내리기" : $"{displayName} 타기";
    }

    public bool Interact(GameObject interactor) {
        return isRidden ? Dismount() : Mount(interactor);
    }

    // 플레이어를 좌석에 앉히고 주행을 켠다
    public bool Mount(GameObject newRider) {
        if (isRidden || newRider == null)
        {
            return false;
        }

        rider = newRider;
        isRidden = true;

        var control = rider.GetComponent<RiderControl>();
        if (control != null)
        {
            control.EnterVehicle(this);
        }

        SetDriveEnabled(true);
        NotifyStateChanged();
        return true;
    }

    // 플레이어를 오토바이 옆 지면에 내려놓고 주행을 끈다
    public bool Dismount() {
        if (!isRidden)
        {
            return false;
        }

        var control = rider.GetComponent<RiderControl>();
        if (control != null)
        {
            control.ExitVehicle(GetExitPosition());
        }

        rider = null;
        isRidden = false;

        SetDriveEnabled(false);
        NotifyStateChanged();
        return true;
    }

    // 하차 지점을 지면 위로 보정해서 돌려준다
    public Vector3 GetExitPosition() {
        Vector3 target = exitPoint != null
            ? exitPoint.position
            : transform.position + transform.right * -1.2f;

        // 지면을 찾아 그 위에 내려놓는다 (오토바이 자신의 콜라이더는 건너뛴다)
        RaycastHit[] hits = Physics.RaycastAll(
            target + Vector3.up * 3f, Vector3.down, 6f, ~0, QueryTriggerInteraction.Ignore);

        bool found = false;
        RaycastHit ground = default;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.root == transform.root)
            {
                continue;
            }

            if (!found || hit.distance < ground.distance)
            {
                ground = hit;
                found = true;
            }
        }

        return found ? ground.point : target;
    }

    // 주행 컴포넌트를 켜고 끈다 (에셋 스크립트는 수정하지 않고 활성만 다룬다)
    private void SetDriveEnabled(bool enabled) {
        if (driveController != null)
        {
            driveController.enabled = enabled;
        }
    }

    private void NotifyStateChanged() {
        if (onStateChanged != null)
        {
            onStateChanged();
        }
    }
}
