// 인벤토리/장비/상태처럼 화면 전체를 덮는 창의 공통 여닫기 동작 (한 번에 하나만 열린다)
using UnityEngine;

public abstract class ScreenPanel : MonoBehaviour {
    public GameObject panel; // 열고 닫을 화면 본체
    public KeyCode toggleKey = KeyCode.I; // 화면을 여닫는 키

    private static ScreenPanel openPanel; // 현재 열려 있는 화면 (없으면 null)

    protected virtual void Awake() {
        panel.SetActive(false);

        // 씬을 다시 불러와도 static 상태가 남아 있으므로 여기서 초기화한다
        openPanel = null;
        UIManager.SetScreenOpen(false);
    }

    protected virtual void Update() {
        // 게임오버 상태에서는 화면을 열지 않는다
        if (Input.GetKeyDown(toggleKey)
            && (GameManager.instance == null || !GameManager.instance.isGameover))
        {
            SetOpen(!panel.activeSelf);
        }
    }

    // 화면을 열거나 닫는다. 다른 화면이 열려 있었다면 그쪽을 먼저 닫는다
    public void SetOpen(bool open) {
        if (open && openPanel != null && openPanel != this)
        {
            openPanel.SetOpen(false);
        }

        panel.SetActive(open);

        if (open)
        {
            openPanel = this;
            Refresh();
        }
        else if (openPanel == this)
        {
            openPanel = null;
        }

        // 파괴된 화면이 남아 있어도 Unity의 null 비교가 걸러준다
        UIManager.SetScreenOpen(openPanel != null);
    }

    // 화면 내용을 현재 상태로 다시 그린다
    protected abstract void Refresh();

    // 목록에 붙어 있던 줄을 모두 지운다
    // Destroy는 프레임 끝에야 처리되므로, 같은 프레임에 두 번 다시 그리면
    // 파괴 예정인 줄이 그대로 남아 목록이 중복된다. 부모에서 먼저 떼어내야 한다
    protected void ClearRows(Transform content) {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Transform row = content.GetChild(i);
            row.SetParent(null);
            Destroy(row.gameObject);
        }
    }
}
