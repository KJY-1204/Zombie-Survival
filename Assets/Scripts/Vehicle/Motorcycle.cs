// 플레이어가 타고 내릴 수 있는 오토바이. 탑승 상태의 단일 소유자이고 UI를 알지 못한다
using System;
using UnityEngine;

public class Motorcycle : MonoBehaviour, IInteractable {
    public string displayName = "오토바이"; // 상호작용 안내에 표시할 이름

    public Transform seat; // 라이더를 붙일 좌석 위치
    public Transform exitPoint; // 하차했을 때 내려설 지점

    [Header("연료")]
    public float maxFuel = 100f; // 연료 탱크 용량
    public float fuel = 100f; // 현재 연료
    public float fuelPerMeter = 0.06f; // 1m 주행당 소모하는 연료
    public ItemData fuelItem; // 보충에 쓰는 아이템 (휘발유통)
    public float fuelPerCan = 40f; // 한 통이 채우는 연료량

    [Header("내구도")]
    public float maxDurability = 100f; // 최대 내구도
    public float durability = 100f; // 현재 내구도
    public float crashSpeedThreshold = 4f; // 이 속도 이상으로 부딪히면 내구도가 깎인다
    public float crashDamagePerSpeed = 2.5f; // 충돌 속도 1당 깎이는 내구도
    public float crashCooldown = 0.5f; // 한 번의 충돌로 여러 번 깎이지 않도록 하는 간격
    public ItemData repairItem; // 수리에 쓰는 아이템 (고철)
    public int repairItemCount = 5; // 한 번 수리에 쓰는 개수
    public float repairAmount = 30f; // 한 번 수리로 회복하는 내구도

    public bool isRidden { get; private set; } // 지금 누가 타고 있는지
    public GameObject rider { get; private set; } // 타고 있는 대상

    // 연료나 내구도가 바닥나면 주행할 수 없다
    public bool canDrive => fuel > 0f && durability > 0f;

    public event Action onStateChanged; // 탑승 상태나 연료/내구도가 바뀔 때 발동

    private MonoBehaviour driveController; // 에셋의 주행 컴포넌트 (탑승 중에만 켠다)
    private Rigidbody body; // 주행 거리와 충돌 속도를 재기 위한 리지드바디
    private Vector3 lastPosition; // 지난 프레임 위치 (주행 거리 계산용)
    private float lastCrashTime = -999f; // 마지막으로 내구도가 깎인 시점

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

        body = GetComponent<Rigidbody>();
        lastPosition = transform.position;

        SetDriveEnabled(false);
    }

    private void Update() {
        if (!isRidden)
        {
            lastPosition = transform.position;
            return;
        }

        ConsumeFuelByDistance();

        // 주행 중에 바닥나면 즉시 멈춘다
        SetDriveEnabled(canDrive);
    }

    // 실제로 움직인 거리만큼 연료를 소모한다 (입력이 아니라 이동량 기준)
    private void ConsumeFuelByDistance() {
        float distance = Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (distance <= 0f || fuel <= 0f)
        {
            return;
        }

        float before = fuel;
        fuel = Mathf.Max(0f, fuel - distance * fuelPerMeter);

        if (!Mathf.Approximately(before, fuel))
        {
            NotifyStateChanged();
        }
    }

    // 빠르게 부딪히면 내구도가 깎인다
    // 오토바이는 콜라이더가 여러 개라 한 번 부딪혀도 이 콜백이 여러 번 온다.
    // 쿨다운을 두지 않으면 한 번의 충돌로 내구도가 몇 배로 깎인다
    private void OnCollisionEnter(Collision collision) {
        float impact = collision.relativeVelocity.magnitude;

        if (impact < crashSpeedThreshold || durability <= 0f
            || Time.time < lastCrashTime + crashCooldown)
        {
            return;
        }

        lastCrashTime = Time.time;

        durability = Mathf.Max(
            0f, durability - (impact - crashSpeedThreshold) * crashDamagePerSpeed);

        if (!canDrive)
        {
            SetDriveEnabled(false);
        }

        NotifyStateChanged();
    }

    // 연료를 보충한다. 인벤토리에 연료통이 있어야 한다
    public bool Refuel(Inventory inventory) {
        if (inventory == null || fuelItem == null
            || fuel >= maxFuel || inventory.CountOf(fuelItem) <= 0)
        {
            return false;
        }

        if (!inventory.Remove(fuelItem, 1))
        {
            return false;
        }

        fuel = Mathf.Min(maxFuel, fuel + fuelPerCan);
        SetDriveEnabled(isRidden && canDrive);
        NotifyStateChanged();
        return true;
    }

    // 수리 재료를 소비해 내구도를 회복한다
    public bool Repair(Inventory inventory) {
        if (inventory == null || repairItem == null
            || durability >= maxDurability
            || inventory.CountOf(repairItem) < repairItemCount)
        {
            return false;
        }

        if (!inventory.Remove(repairItem, repairItemCount))
        {
            return false;
        }

        durability = Mathf.Min(maxDurability, durability + repairAmount);
        SetDriveEnabled(isRidden && canDrive);
        NotifyStateChanged();
        return true;
    }

    public bool CanInteract(GameObject interactor) {
        // 이미 타고 있으면 내리는 것도 상호작용이다
        return !isRidden || rider == interactor;
    }

    public string GetInteractionLabel() {
        if (isRidden)
        {
            return $"{displayName}에서 내리기";
        }

        // 탈 수 없는 상태면 왜 그런지 알려준다
        if (fuel <= 0f)
        {
            return $"{displayName} 타기 (연료 없음)";
        }

        if (durability <= 0f)
        {
            return $"{displayName} 타기 (고장)";
        }

        return $"{displayName} 타기";
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
        lastPosition = transform.position;

        var control = rider.GetComponent<RiderControl>();
        if (control != null)
        {
            control.EnterVehicle(this);
        }

        // 연료나 내구도가 바닥나 있으면 타기는 하되 주행은 되지 않는다
        SetDriveEnabled(canDrive);
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
