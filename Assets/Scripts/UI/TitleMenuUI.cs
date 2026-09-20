// 게임 시작 시 새 게임 또는 기존 저장을 선택하는 타이틀 화면
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class StartupLoadRequest {
    private static int slot = -1;
    private static bool auto;

    public static void Set(int requestedSlot, bool requestedAuto) {
        slot = requestedSlot;
        auto = requestedAuto;
    }

    public static bool TryConsume(out int requestedSlot, out bool requestedAuto) {
        requestedSlot = slot;
        requestedAuto = auto;
        slot = -1;
        return requestedSlot >= 0;
    }
}

public class TitleMenuUI : MonoBehaviour {
    private const string WorldSceneName = "World";

    private readonly List<GameObject> loadRows = new List<GameObject>();

    private GameObject mainPanel;
    private GameObject loadPanel;
    private Transform loadContent;
    private Button continueButton;
    private Font font;

    private void Start() {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        BuildUi();
        ShowMain();
    }

    private void BuildUi() {
        var canvasObject = new GameObject("Title Canvas", typeof(Canvas), typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        if (FindFirstObjectByType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        CreateBackground(canvasObject.transform);
        mainPanel = CreatePanel("Main Panel", canvasObject.transform, new Vector2(620f, 520f));
        loadPanel = CreatePanel("Load Panel", canvasObject.transform, new Vector2(1000f, 800f));

        BuildMainPanel();
        BuildLoadPanel();
    }

    private void CreateBackground(Transform parent) {
        var background = CreateObject("Background", parent, typeof(Image));
        var rect = background.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        background.GetComponent<Image>().color = UiTheme.PanelBackground;
    }

    private void BuildMainPanel() {
        CreateText("Title", mainPanel.transform, "ZOMBIE SURVIVAL", UiTheme.TitleFontSize + 20,
            UiTheme.TextAccent, new Vector2(0f, 170f), new Vector2(520f, 70f));
        CreateText("Subtitle", mainPanel.transform, "생존을 이어가거나 새 월드를 시작합니다.",
            UiTheme.BodyFontSize, UiTheme.TextSecondary, new Vector2(0f, 110f), new Vector2(520f, 36f));

        continueButton = CreateButton("Continue Button", mainPanel.transform, "이어하기",
            new Vector2(0f, 25f), ShowLoad);
        CreateButton("New Game Button", mainPanel.transform, "새 게임",
            new Vector2(0f, -55f), StartNewGame);
        CreateButton("Quit Button", mainPanel.transform, "종료",
            new Vector2(0f, -135f), Quit);
    }

    private void BuildLoadPanel() {
        CreateText("Load Title", loadPanel.transform, "이어하기", UiTheme.TitleFontSize,
            UiTheme.TextAccent, new Vector2(0f, 335f), new Vector2(820f, 48f));
        CreateText("Load Hint", loadPanel.transform, "불러올 수동 또는 자동저장을 선택하세요.",
            UiTheme.BodyFontSize, UiTheme.TextSecondary, new Vector2(0f, 292f), new Vector2(820f, 32f));

        var scroll = CreateObject("Slot Scroll", loadPanel.transform, typeof(Image), typeof(ScrollRect));
        var scrollRect = scroll.GetComponent<RectTransform>();
        scrollRect.anchoredPosition = new Vector2(0f, -5f);
        scrollRect.sizeDelta = new Vector2(850f, 555f);
        scroll.GetComponent<Image>().color = UiTheme.SlotBackground;

        var viewport = CreateObject("Viewport", scroll.transform, typeof(RectMask2D));
        var viewportRect = viewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = Vector2.zero;
        viewportRect.anchorMax = Vector2.one;
        viewportRect.offsetMin = new Vector2(12f, 12f);
        viewportRect.offsetMax = new Vector2(-12f, -12f);

        var content = CreateObject("Content", viewport.transform, typeof(VerticalLayoutGroup),
            typeof(ContentSizeFitter));
        var contentRect = content.GetComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 1f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.pivot = new Vector2(0.5f, 1f);
        contentRect.anchoredPosition = Vector2.zero;
        contentRect.sizeDelta = new Vector2(0f, 0f);

        var layout = content.GetComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(6, 6, 6, 6);
        layout.spacing = 8f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        var fitter = content.GetComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        var scrollComponent = scroll.GetComponent<ScrollRect>();
        scrollComponent.viewport = viewportRect;
        scrollComponent.content = contentRect;
        scrollComponent.horizontal = false;
        scrollComponent.vertical = true;
        scrollComponent.movementType = ScrollRect.MovementType.Clamped;
        loadContent = content.transform;

        CreateButton("Back Button", loadPanel.transform, "뒤로", new Vector2(0f, -350f), ShowMain);
    }

    private void ShowMain() {
        mainPanel.SetActive(true);
        loadPanel.SetActive(false);
        continueButton.interactable = HasAnySave();
        UIManager.SetScreenOpen(true);
    }

    private void ShowLoad() {
        mainPanel.SetActive(false);
        loadPanel.SetActive(true);
        ClearLoadRows();

        foreach (SaveSlotInfo info in SaveSystem.ListSlots())
        {
            CreateLoadRow(info);
        }
    }

    private bool HasAnySave() {
        foreach (SaveSlotInfo info in SaveSystem.ListSlots())
        {
            if (!info.isEmpty)
            {
                return true;
            }
        }

        return false;
    }

    private void CreateLoadRow(SaveSlotInfo info) {
        var row = CreateObject($"Slot {info.slot + 1}", loadContent, typeof(Image),
            typeof(HorizontalLayoutGroup), typeof(LayoutElement));
        row.GetComponent<Image>().color = UiTheme.RowBackground;

        var layout = row.GetComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(16, 16, 8, 8);
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = false;

        row.GetComponent<LayoutElement>().preferredHeight = 70f;
        CreateRowLabel(row.transform, info);

        CreateRowButton(row.transform, "수동 불러오기", info.manual != null,
            () => StartSavedGame(info.slot, false));
        CreateRowButton(row.transform, "자동 불러오기", info.auto != null,
            () => StartSavedGame(info.slot, true));
        loadRows.Add(row);
    }

    private void CreateRowLabel(Transform parent, SaveSlotInfo info) {
        string manual = info.manual == null ? "수동 저장 없음" : info.manual.Summarize();
        string automatic = info.auto == null ? "자동저장 없음" : info.auto.Summarize();
        var label = CreateText("Summary", parent,
            $"슬롯 {info.slot + 1}\n수동  {manual}\n자동  {automatic}",
            UiTheme.SmallFontSize, UiTheme.TextPrimary, Vector2.zero, Vector2.zero);
        var element = label.gameObject.AddComponent<LayoutElement>();
        element.preferredWidth = 500f;
        element.flexibleWidth = 1f;
        label.alignment = TextAnchor.MiddleLeft;
    }

    private void CreateRowButton(Transform parent, string label, bool interactable,
        UnityEngine.Events.UnityAction action) {
        Button button = CreateButton(label, parent, label, Vector2.zero, action);
        var rect = button.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = new Vector2(135f, 44f);
        button.interactable = interactable;
        button.gameObject.AddComponent<LayoutElement>().preferredWidth = 135f;
    }

    private void StartNewGame() {
        SceneManager.LoadScene(WorldSceneName);
    }

    private void StartSavedGame(int slot, bool auto) {
        StartupLoadRequest.Set(slot, auto);
        SceneManager.LoadScene(WorldSceneName);
    }

    private void Quit() {
        Application.Quit();
    }

    private void ClearLoadRows() {
        foreach (GameObject row in loadRows)
        {
            Destroy(row);
        }

        loadRows.Clear();
    }

    private GameObject CreatePanel(string name, Transform parent, Vector2 size) {
        var panel = CreateObject(name, parent, typeof(Image));
        var rect = panel.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        panel.GetComponent<Image>().color = UiTheme.HeaderBackground;
        return panel;
    }

    private Text CreateText(string name, Transform parent, string value, int fontSize,
        Color color, Vector2 position, Vector2 size) {
        var textObject = CreateObject(name, parent, typeof(Text));
        var rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        var text = textObject.GetComponent<Text>();
        text.font = font;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = TextAnchor.MiddleCenter;
        text.text = value;
        return text;
    }

    private Button CreateButton(string name, Transform parent, string label, Vector2 position,
        UnityEngine.Events.UnityAction action) {
        var buttonObject = CreateObject(name, parent, typeof(Image), typeof(Button));
        var rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(360f, 58f);
        buttonObject.GetComponent<Image>().color = UiTheme.ButtonNeutral;

        var button = buttonObject.GetComponent<Button>();
        button.targetGraphic = buttonObject.GetComponent<Image>();
        button.onClick.AddListener(action);
        CreateText("Text", buttonObject.transform, label, UiTheme.BodyFontSize,
            UiTheme.TextPrimary, Vector2.zero, new Vector2(330f, 42f));
        return button;
    }

    private GameObject CreateObject(string name, Transform parent, params System.Type[] components) {
        var created = new GameObject(name, components);
        created.transform.SetParent(parent, false);
        return created;
    }
}
