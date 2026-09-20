// 인벤토리/장비/상태를 탭으로 묶은 캐릭터 창 (팰월드처럼 한 창에서 전환한다)
using UnityEngine;
using UnityEngine.UI;

public class CharacterScreenUI : ScreenPanel {
    public TabView[] tabs; // 창에 들어갈 탭들 (표시 순서)
    public Transform tabBar; // 탭 버튼이 붙을 부모
    public GameObject tabButtonPrefab; // 탭 버튼 프리팹 (Text 자식 하나)
    public Text hintText; // 하단 안내

    private int currentTab;

    protected override void Awake() {
        base.Awake();

        foreach (TabView tab in tabs)
        {
            tab.SetVisible(false);
        }
    }

    // 탭마다 단축키가 있다. 열려 있는 탭의 키를 다시 누르면 닫힌다
    protected override void Update() {
        if (GameManager.instance != null && GameManager.instance.isGameover)
        {
            return;
        }

        for (int i = 0; i < tabs.Length; i++)
        {
            if (!Input.GetKeyDown(tabs[i].shortcutKey))
            {
                continue;
            }

            if (panel.activeSelf && currentTab == i)
            {
                SetOpen(false);
                return;
            }

            currentTab = i;
            SetOpen(true);
            return;
        }
    }

    // 지정한 탭으로 창을 연다 (다른 화면에서 불러 쓸 수 있게 공개해 둔다)
    public void OpenTab(int index) {
        currentTab = Mathf.Clamp(index, 0, tabs.Length - 1);
        SetOpen(true);
    }

    protected override void Refresh() {
        if (!panel.activeSelf)
        {
            return;
        }

        BuildTabBar();

        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetVisible(i == currentTab);
        }

        if (hintText != null)
        {
            hintText.text = $"{tabs[currentTab].shortcutKey} 키로 닫기";
        }
    }

    private void BuildTabBar() {
        ClearRows(tabBar);

        for (int i = 0; i < tabs.Length; i++)
        {
            GameObject button = Instantiate(tabButtonPrefab, tabBar);
            bool selected = i == currentTab;

            var label = button.transform.Find("Text").GetComponent<Text>();
            label.text = $"{tabs[i].tabName}  ({tabs[i].shortcutKey})";
            label.color = selected ? UiTheme.TextAccent : UiTheme.TextSecondary;

            var image = button.GetComponent<Image>();
            image.color = selected ? UiTheme.HeaderBackground : Color.clear;

            int index = i;
            button.GetComponent<Button>().onClick.AddListener(() => {
                currentTab = index;
                Refresh();
            });
        }
    }
}
