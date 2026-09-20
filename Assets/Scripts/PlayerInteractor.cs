// 플레이어 주변의 상호작용 대상을 찾아 입력으로 사용하는 컴포넌트 (UI는 currentTarget만 읽는다)
using UnityEngine;

public class PlayerInteractor : MonoBehaviour {
    public float interactRange = 2.5f; // 상호작용할 수 있는 거리
    public KeyCode interactKey = KeyCode.E; // 상호작용 키

    public IInteractable currentTarget { get; private set; } // 지금 상호작용할 수 있는 대상

    private void Update() {
        currentTarget = FindNearestInteractable();

        // 전체 화면이 열려 있거나 게임오버면 상호작용하지 않는다
        if (currentTarget == null
            || UIManager.isScreenOpen
            || (GameManager.instance != null && GameManager.instance.isGameover))
        {
            return;
        }

        if (Input.GetKeyDown(interactKey) && currentTarget.CanInteract(gameObject))
        {
            currentTarget.Interact(gameObject);
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
