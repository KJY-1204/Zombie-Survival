// 플레이어가 상호작용 키로 다룰 수 있는 대상이 공통으로 가져야 하는 인터페이스
using UnityEngine;

public interface IInteractable {
    // 지금 상호작용할 수 있는 상태인지 (false면 안내만 띄우고 키 입력은 무시한다)
    bool CanInteract(GameObject interactor);

    // 화면에 띄울 안내 문구 (상호작용 키는 UI가 앞에 붙인다)
    string GetInteractionLabel();

    // 실제 상호작용. 무언가 실제로 일어났을 때만 true를 반환한다
    bool Interact(GameObject interactor);
}
