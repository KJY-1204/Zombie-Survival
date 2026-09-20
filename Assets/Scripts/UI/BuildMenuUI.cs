// 건설물 목록과 필요 재료를 보여주고 고르면 건설 모드로 들어가는 화면 (건설 시스템의 공개 상태만 읽는다)
using UnityEngine;
using UnityEngine.UI;

public class BuildMenuUI : ScreenPanel {
    public Transform content; // 건설물 줄이 붙을 부모
    public GameObject rowPrefab; // 줄 하나의 프리팹 (Label + Action Button)
    public Text hintText; // 건설 모드 안내

    private BuildPlacer placer; // 건설 모드로 들어갈 컴포넌트
    private Inventory inventory; // 재료 보유량을 표시할 인벤토리
    private BaseBuildState buildState; // 건설물 목록을 가진 상태

    private void Start() {
        placer = FindObjectOfType<BuildPlacer>();
        buildState = FindObjectOfType<BaseBuildState>();

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

        // 건설 모드일 때만 조작 안내를 띄운다
        if (hintText != null)
        {
            hintText.text = placer != null && placer.isBuilding
                ? $"건설 모드 - {placer.selected.displayName}  |  휠 회전 · 좌클릭 설치 · 우클릭/ESC 취소"
                : string.Empty;
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

    // 건설물 한 줄: 이름과 필요 재료, 그리고 고르기 버튼
    private void CreateRow(BuildableData data) {
        GameObject row = Instantiate(rowPrefab, content);

        bool affordable = data.CanAfford(inventory);

        Text label = row.transform.Find("Label").GetComponent<Text>();
        label.text = $"{data.displayName}    {data.DescribeCosts()}";
        label.color = affordable ? Color.white : new Color(1f, 0.55f, 0.5f, 1f);

        Button action = row.transform.Find("Action Button").GetComponent<Button>();
        action.transform.Find("Text").GetComponent<Text>().text = "건설";
        action.interactable = affordable;

        action.onClick.AddListener(() => {
            placer.Select(data);
            // 배치하려면 화면을 닫아야 조준과 좌클릭이 먹는다
            SetOpen(false);
        });
    }
}
