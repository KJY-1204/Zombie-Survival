// 플레이어가 아이템을 넣고 꺼낼 수 있는 보관 상자 (내용물은 자기 Inventory 컴포넌트가 들고 있다)
using System;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class StorageContainer : MonoBehaviour, IInteractable {
    public string displayName = "보관 상자"; // 상호작용 안내에 표시할 이름

    public Inventory storage { get; private set; } // 상자 안의 내용물

    // 상자를 열어달라는 요청. UI가 구독한다 (보관 상자는 UI를 직접 알지 않는다)
    public static event Action<StorageContainer> onOpenRequested;

    private void Awake() {
        storage = GetComponent<Inventory>();
    }

    public bool CanInteract(GameObject interactor) {
        return true;
    }

    public string GetInteractionLabel() {
        return $"{displayName} 열기 ({storage.items.Count}칸)";
    }

    public bool Interact(GameObject interactor) {
        if (onOpenRequested == null)
        {
            return false;
        }

        onOpenRequested(this);
        return true;
    }
}
