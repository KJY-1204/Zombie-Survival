// 한 창 안에서 탭으로 전환되는 화면 조각의 공통 뼈대
// 여닫기·커서·단축키는 창(CharacterScreenUI)이 맡고, 탭은 자기 내용만 책임진다
using UnityEngine;

public abstract class TabView : MonoBehaviour {
    public GameObject root; // 이 탭의 내용 루트
    public string tabName = "탭"; // 탭 버튼에 표시할 이름
    public KeyCode shortcutKey = KeyCode.None; // 이 탭으로 바로 여는 키

    public bool isVisible => root != null && root.activeSelf;

    // 창이 이 탭을 보여주거나 감출 때 부른다
    public void SetVisible(bool visible) {
        if (root == null)
        {
            return;
        }

        root.SetActive(visible);

        if (visible)
        {
            Refresh();
        }
    }

    // 탭 내용을 현재 상태로 다시 그린다 (시스템의 onChanged에서도 불린다)
    public abstract void Refresh();

    // 목록에 붙어 있던 줄을 모두 지운다
    // Destroy는 프레임 끝에야 처리되므로 부모에서 먼저 떼어내야 목록이 중복되지 않는다
    protected void ClearRows(Transform content) {
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Transform row = content.GetChild(i);
            row.SetParent(null);
            Destroy(row.gameObject);
        }
    }
}
