// 상호작용으로 여닫는 문. 문짝이 경첩을 기준으로 회전하며 열림 상태를 PlacedBuilding 기록에 반영한다
using UnityEngine;

public class BuildableDoor : MonoBehaviour, IInteractable {
    public string displayName = "문"; // 상호작용 안내에 표시할 이름
    public Transform hinge; // 회전할 문짝의 부모 (콜라이더도 여기 붙어 함께 움직인다)
    public float openAngle = 90f; // 열렸을 때의 회전 각도

    public bool isOpen { get; private set; } // 현재 열려 있는지

    private PlacedBuildingLink link; // 열림 상태를 기록할 대상

    private void Awake() {
        link = GetComponent<PlacedBuildingLink>();
    }

    private void Start() {
        // 저장에서 불러온 경우 기록된 열림 상태를 그대로 복원한다
        if (link != null && link.record != null)
        {
            SetOpen(link.record.isOpen);
        }
        else
        {
            SetOpen(false);
        }
    }

    public bool CanInteract(GameObject interactor) {
        return true;
    }

    public string GetInteractionLabel() {
        return isOpen ? $"{displayName} 닫기" : $"{displayName} 열기";
    }

    public bool Interact(GameObject interactor) {
        SetOpen(!isOpen);
        return true;
    }

    // 문짝을 실제로 돌리고 열림 상태를 기록에 반영한다
    public void SetOpen(bool open) {
        isOpen = open;

        if (hinge != null)
        {
            hinge.localRotation = Quaternion.Euler(0f, open ? openAngle : 0f, 0f);
        }

        if (link != null && link.record != null)
        {
            link.record.isOpen = open;
        }
    }
}
