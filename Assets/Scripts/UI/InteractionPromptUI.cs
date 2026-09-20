// 상호작용할 수 있는 대상이 있을 때 화면에 안내를 띄운다 (PlayerInteractor의 공개 상태만 읽는다)
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour {
    public Text promptText; // 안내를 표시할 텍스트

    private PlayerInteractor interactor; // 대상을 알려주는 컴포넌트

    private void Start() {
        interactor = FindObjectOfType<PlayerInteractor>();
        promptText.text = string.Empty;
    }

    private void Update() {
        // 전체 화면이 열려 있는 동안에는 안내를 숨긴다
        LootContainer target = interactor != null && !UIManager.isScreenOpen
            ? interactor.currentTarget
            : null;

        if (target == null)
        {
            promptText.text = string.Empty;
            return;
        }

        promptText.text = target.isEmpty
            ? $"{target.displayName} (비어 있음)"
            : $"[{interactor.interactKey}] {target.displayName} 열기";
    }
}
