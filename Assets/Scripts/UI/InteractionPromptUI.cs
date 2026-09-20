// 상호작용할 수 있는 대상이 있을 때 화면에 안내를 띄운다 (PlayerInteractor의 공개 상태만 읽는다)
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour {
    public Text promptText; // 안내를 표시할 텍스트
    public GameObject background; // 안내가 있을 때만 보일 배경 배지

    private PlayerInteractor interactor; // 대상을 알려주는 컴포넌트

    private void Start() {
        interactor = FindFirstObjectByType<PlayerInteractor>();
        SetPrompt(string.Empty);
    }

    private void Update() {
        // 전체 화면이 열려 있는 동안에는 안내를 숨긴다
        IInteractable target = interactor != null && !UIManager.isScreenOpen
            ? interactor.currentTarget
            : null;

        if (target == null)
        {
            SetPrompt(string.Empty);
            return;
        }

        // 지금 다룰 수 있는 대상에만 키 안내를 붙인다
        string label = target.CanInteract(interactor.gameObject)
            ? $"[{interactor.interactKey}] {target.GetInteractionLabel()}"
            : target.GetInteractionLabel();

        // 추가 조작이 있으면 아랫줄에 함께 보여준다
        SetPrompt(string.IsNullOrEmpty(interactor.serviceHint)
            ? label
            : label + "\n" + interactor.serviceHint);
    }

    // 문구가 비어 있으면 배경까지 감춘다
    private void SetPrompt(string value) {
        promptText.text = value;

        if (background != null)
        {
            background.SetActive(!string.IsNullOrEmpty(value));
        }
    }
}
