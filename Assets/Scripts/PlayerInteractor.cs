// 플레이어 주변의 상호작용 대상을 찾아 입력으로 사용하는 컴포넌트 (UI는 currentTarget만 읽는다)
using UnityEngine;

public class PlayerInteractor : MonoBehaviour {
    public float interactRange = 2.5f; // 상호작용할 수 있는 거리
    public KeyCode interactKey = KeyCode.E; // 상호작용 키

    public LootContainer currentTarget { get; private set; } // 지금 상호작용할 수 있는 대상

    private Inventory inventory; // 내용물을 받을 인벤토리

    private void Start() {
        inventory = GetComponent<Inventory>();
    }

    private void Update() {
        currentTarget = FindNearestContainer();

        // 전체 화면이 열려 있거나 게임오버면 상호작용하지 않는다
        if (currentTarget == null
            || UIManager.isScreenOpen
            || (GameManager.instance != null && GameManager.instance.isGameover))
        {
            return;
        }

        if (Input.GetKeyDown(interactKey))
        {
            currentTarget.Loot(inventory);
        }
    }

    // 사정거리 안에서 가장 가까운 상자를 찾는다 (빈 상자도 대상에 포함해 안내를 띄운다)
    private LootContainer FindNearestContainer() {
        LootContainer nearest = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider collider in
            Physics.OverlapSphere(transform.position, interactRange))
        {
            LootContainer container = collider.GetComponentInParent<LootContainer>();

            if (container == null)
            {
                continue;
            }

            float distance =
                Vector3.Distance(transform.position, container.transform.position);

            if (distance < nearestDistance)
            {
                nearest = container;
                nearestDistance = distance;
            }
        }

        return nearest;
    }
}
