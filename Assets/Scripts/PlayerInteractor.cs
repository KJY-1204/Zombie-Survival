// 플레이어 주변의 상호작용 대상을 찾아 입력으로 사용하는 컴포넌트 (UI는 currentTarget만 읽는다)
using UnityEngine;

public class PlayerInteractor : MonoBehaviour {
    public float interactRange = 2.5f; // 상호작용할 수 있는 거리
    public KeyCode interactKey = KeyCode.E; // 상호작용 키
    public KeyCode refuelKey = KeyCode.R; // 오토바이 연료 보충 키
    public KeyCode repairKey = KeyCode.F; // 오토바이 수리 키

    public IInteractable currentTarget { get; private set; } // 지금 상호작용할 수 있는 대상

    // 상호작용 외에 추가로 할 수 있는 조작 안내 (없으면 빈 문자열). UI가 이것만 읽는다
    public string serviceHint { get; private set; } = string.Empty;

    private RiderControl rider; // 차량에 타고 있는지 알려주는 컴포넌트
    private Inventory inventory; // 연료·수리 재료를 꺼낼 인벤토리

    private void Start() {
        rider = GetComponent<RiderControl>();
        inventory = GetComponent<Inventory>();
    }

    private void Update() {
        // 차량에 타고 있는 동안에는 내리는 것만 할 수 있다
        currentTarget = rider != null && rider.isRiding
            ? rider.vehicle
            : FindNearestInteractable();

        // 전체 화면이 열려 있거나 게임오버면 상호작용하지 않는다
        if (currentTarget == null
            || UIManager.isScreenOpen
            || (GameManager.instance != null && GameManager.instance.isGameover))
        {
            serviceHint = string.Empty;
            return;
        }

        if (Input.GetKeyDown(interactKey) && currentTarget.CanInteract(gameObject))
        {
            currentTarget.Interact(gameObject);
        }

        UpdateMotorcycleService();
    }

    // 오토바이 옆에 서 있을 때만 연료 보충과 수리를 받는다
    private void UpdateMotorcycleService() {
        var motorcycle = currentTarget as Motorcycle;

        if (motorcycle == null || motorcycle.isRidden)
        {
            serviceHint = string.Empty;
            return;
        }

        serviceHint =
            $"연료 {motorcycle.fuel:F0}/{motorcycle.maxFuel:F0}"
            + $"  ·  내구 {motorcycle.durability:F0}/{motorcycle.maxDurability:F0}"
            + $"  ·  [{refuelKey}] 주유  [{repairKey}] 수리";

        if (Input.GetKeyDown(refuelKey))
        {
            motorcycle.Refuel(inventory);
        }
        else if (Input.GetKeyDown(repairKey))
        {
            motorcycle.Repair(inventory);
        }
    }

    // 사정거리 안에서 가장 가까운 상호작용 대상을 찾는다
    // 지금 다룰 수 없는 대상(빈 상자 등)도 포함해 안내를 띄운다
    private IInteractable FindNearestInteractable() {
        IInteractable nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider collider in
            Physics.OverlapSphere(transform.position, interactRange))
        {
            var interactable = collider.GetComponentInParent<IInteractable>();

            if (interactable == null)
            {
                continue;
            }

            var behaviour = interactable as MonoBehaviour;
            float distance = Vector3.Distance(
                transform.position, behaviour.transform.position);

            if (distance < nearestDistance)
            {
                nearest = interactable;
                nearestDistance = distance;
            }
        }

        return nearest;
    }
}
