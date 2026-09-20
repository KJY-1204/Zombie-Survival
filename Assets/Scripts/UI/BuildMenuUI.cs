// 건설물 목록과 필요 재료를 보여주고 고르면 건설 모드로 들어가는 화면 (건설 시스템의 공개 상태만 읽는다)
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuUI : ScreenPanel {
    public Transform content; // 건설물 줄이 붙을 부모
    public GameObject rowPrefab; // 아이템 줄 프리팹 (인벤토리와 같은 것을 쓴다)
    public Text hintText; // 건설 모드 안내 (화면 밖 HUD에 표시된다)

    private BuildPlacer placer; // 건설 모드로 들어갈 컴포넌트
    private Inventory inventory; // 재료 보유량을 표시할 인벤토리
    private BaseBuildState buildState; // 건설물 목록을 가진 상태

    private void Start() {
        placer = FindFirstObjectByType<BuildPlacer>();
        buildState = FindFirstObjectByType<BaseBuildState>();

        if (placer != null)
        {
            inventory = placer.GetComponent<Inventory>();
            inventory.onChanged += Refresh;
        }
    }

    private void OnDestroy() {
        if (inventory != null)
        {
            inventory.onChanged -= Refresh;
        }
    }

    protected override void Update() {
        base.Update();

        // 건설 모드이거나 철거할 수 있는 건설물을 보고 있을 때만 안내를 띄운다
        if (hintText == null || placer == null)
        {
            return;
        }

        if (placer.isBuilding)
        {
            hintText.text =
                $"건설 모드 - {placer.selected.displayName}  |  휠 회전 · 좌클릭 설치 · 우클릭/ESC 취소";
        }
        else if (placer.demolishTarget != null)
        {
            BuildableData target = buildState.Find(placer.demolishTarget.buildableId);
            string name = target != null ? target.displayName : placer.demolishTarget.buildableId;
            hintText.text = $"[{placer.demolishKey}] {name} 철거";
        }
        else
        {
            hintText.text = string.Empty;
        }
    }

    protected override void Refresh() {
        if (buildState == null || !panel.activeSelf)
        {
            return;
        }

        ClearRows(content);

        foreach (BuildableData data in buildState.catalog)
        {
            if (data != null)
            {
                CreateRow(data);
            }
        }
    }

    // 건설물 한 줄: 아이콘, 이름, 필요 재료. 재료가 모자라면 붉게 표시하고 고를 수 없다
    private void CreateRow(BuildableData data) {
        GameObject row = Instantiate(rowPrefab, content);
        bool affordable = data.CanAfford(inventory);

        row.GetComponent<Image>().color = UiTheme.RowBackground;
        row.transform.Find("Icon Slot/Icon").GetComponent<Image>().sprite = data.icon;

        Text name = row.transform.Find("Name").GetComponent<Text>();
        name.text = data.displayName;
        name.color = affordable ? UiTheme.TextPrimary : UiTheme.TextDanger;

        Text detail = row.transform.Find("Detail").GetComponent<Text>();
        detail.text = data.DescribeCosts();
        detail.color = affordable ? UiTheme.TextSecondary : UiTheme.TextDanger;

        Text action = row.transform.Find("Count").GetComponent<Text>();
        action.text = affordable ? "건설" : "재료 부족";
        action.color = affordable ? UiTheme.TextAccent : UiTheme.TextDanger;

        Button button = row.GetComponent<Button>();
        button.interactable = affordable;
        button.onClick.AddListener(() => {
            placer.Select(data);
            // 배치하려면 화면을 닫아야 조준과 좌클릭이 먹는다
            SetOpen(false);
        });
    }
}
