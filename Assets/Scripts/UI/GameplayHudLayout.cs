// 게임플레이 HUD를 생존 슈터용 가장자리 레이아웃으로 구성하고 기존 데이터 표시기를 연결한다.
using UnityEngine;

public class GameplayHudLayout : MonoBehaviour {
    private const float RefreshInterval = 0.1f;

    private readonly Color panelColor = new Color(0.025f, 0.04f, 0.055f, 0.78f);
    private readonly Color insetColor = new Color(0f, 0f, 0f, 0.52f);
    private readonly Color lineColor = new Color(1f, 0.64f, 0.16f, 0.88f);
    private readonly Color textColor = new Color(0.9f, 0.94f, 0.96f, 1f);
    private readonly Color mutedTextColor = new Color(0.58f, 0.68f, 0.73f, 1f);
    private readonly Color healthColor = new Color(0.17f, 0.82f, 0.48f, 1f);
    private readonly Color fuelColor = new Color(1f, 0.68f, 0.2f, 1f);

    private Font font;
    private UnityEngine.UI.Text headingText;
    private UnityEngine.UI.Text armorText;
    private UnityEngine.UI.Text weightText;
    private PlayerHealth player;
    private Inventory inventory;
    private Equipment equipment;
    private float nextRefreshTime;

    private void Awake() {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        HideLegacyGameplayElements();

        CreateTopNavigation();
        CreateControlGuide();
        CreateVitalsPanel();
        CreateVehiclePanel();
        CreateWeaponPanel();
        CreateInteractionPrompt();
        CreateBuildHint();
        CreateCrosshair();

        BindPlayer();
    }

    private void Start() {
        BindPlayer();
        RefreshReadouts();
    }

    private void Update() {
        if (Time.unscaledTime < nextRefreshTime) {
            return;
        }

        nextRefreshTime = Time.unscaledTime + RefreshInterval;
        RefreshReadouts();
    }

    private void HideLegacyGameplayElements() {
        string[] names = {
            "Crosshair", "Weapon Hud", "Health Bar", "Health Label", "Interaction Prompt",
            "Vehicle Status", "Build Hint"
        };

        foreach (string elementName in names) {
            Transform existing = FindDescendant(elementName);

            if (existing != null) {
                existing.gameObject.SetActive(false);
            }
        }
    }

    private void CreateTopNavigation() {
        GameObject panel = CreatePanel("Top Navigation", transform, new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -22f),
            new Vector2(680f, 56f), panelColor);
        CreateAccentLine("Accent", panel.transform, new Vector2(0.5f, 1f),
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), Vector2.zero, new Vector2(680f, 3f));

        CreateText("West", panel.transform, "W", 17, mutedTextColor, TextAnchor.MiddleCenter,
            new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
            new Vector2(86f, -3f), new Vector2(36f, 28f));
        CreateText("North", panel.transform, "N", 19, lineColor, TextAnchor.MiddleCenter,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
            new Vector2(0f, -3f), new Vector2(42f, 30f));
        CreateText("East", panel.transform, "E", 17, mutedTextColor, TextAnchor.MiddleCenter,
            new Vector2(1f, 0.5f), new Vector2(1f, 0.5f), new Vector2(1f, 0.5f),
            new Vector2(-86f, -3f), new Vector2(36f, 28f));
        headingText = CreateText("Heading", panel.transform, "방향 확인 중", 14, textColor,
            TextAnchor.MiddleCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f), new Vector2(0f, 11f), new Vector2(160f, 22f));
        CreateText("Zone", panel.transform, "생존 구역", 14, mutedTextColor, TextAnchor.MiddleLeft,
            new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
            new Vector2(20f, -3f), new Vector2(150f, 26f));
    }

    private void CreateControlGuide() {
        GameObject panel = CreatePanel("Survival Guide", transform, new Vector2(1f, 1f),
            new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-34f, -34f),
            new Vector2(356f, 184f), panelColor);
        CreateAccentLine("Accent", panel.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(0f, 1f), new Vector2(0f, -1f), new Vector2(5f, 182f));
        CreateText("Title", panel.transform, "생존 안내", 18, lineColor, TextAnchor.MiddleLeft,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
            new Vector2(22f, -24f), new Vector2(300f, 30f));
        CreateText("Line", panel.transform, "[I] 인벤토리    [O] 장비    [K] 상태", 14, textColor,
            TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(0f, 1f), new Vector2(22f, -64f), new Vector2(312f, 24f));
        CreateText("Line", panel.transform, "[1] 주무기    [2] 보조무기    [3] 근접", 14, textColor,
            TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(0f, 1f), new Vector2(22f, -94f), new Vector2(312f, 24f));
        CreateText("Line", panel.transform, "[E] 상호작용    [B] 건설    [ESC] 저장", 14,
            mutedTextColor, TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(0f, 1f), new Vector2(22f, -126f), new Vector2(312f, 24f));
        CreateText("Footer", panel.transform, "표시된 단축키는 현재 사용 가능한 기능입니다.", 12,
            mutedTextColor, TextAnchor.MiddleLeft, new Vector2(0f, 0f), new Vector2(1f, 0f),
            new Vector2(0f, 0f), new Vector2(22f, 18f), new Vector2(312f, 20f));
    }

    private void CreateVitalsPanel() {
        GameObject panel = CreatePanel("Player Vitals", transform, Vector2.zero, Vector2.zero,
            Vector2.zero, new Vector2(34f, 34f), new Vector2(360f, 142f), panelColor);
        CreateAccentLine("Accent", panel.transform, new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(5f, 142f));
        CreateText("Title", panel.transform, "생존자", 18, textColor, TextAnchor.MiddleLeft,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
            new Vector2(22f, -22f), new Vector2(150f, 28f));
        CreateText("Health Caption", panel.transform, "체력", 13, mutedTextColor, TextAnchor.MiddleLeft,
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(22f, -49f), new Vector2(46f, 20f));

        UnityEngine.UI.Slider healthSlider = CreateBar("Health Bar", panel.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
            new Vector2(72f, -52f), new Vector2(-22f, -52f), healthColor);
        UnityEngine.UI.Text healthLabel = CreateText("Health Label", healthSlider.transform,
            "100 / 100", 13, textColor, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one,
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        HealthBarLabel label = healthLabel.gameObject.AddComponent<HealthBarLabel>();
        label.slider = healthSlider;

        armorText = CreateText("Armor", panel.transform, "방어  0", 14, textColor,
            TextAnchor.MiddleLeft, new Vector2(0f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0f, 0f), new Vector2(22f, 34f), new Vector2(150f, 24f));
        weightText = CreateText("Weight", panel.transform, "무게  0.0 / 0.0 kg", 14, textColor,
            TextAnchor.MiddleLeft, new Vector2(0.5f, 0f), new Vector2(1f, 0f),
            new Vector2(1f, 0f), new Vector2(-22f, 34f), new Vector2(190f, 24f));

        PlayerHealth foundPlayer = FindFirstObjectByType<PlayerHealth>();
        if (foundPlayer != null) {
            foundPlayer.healthSlider = healthSlider;
        }
    }

    private void CreateVehiclePanel() {
        GameObject root = CreatePanel("Vehicle Status", transform, Vector2.zero, Vector2.zero,
            Vector2.zero, new Vector2(34f, 194f), new Vector2(360f, 112f), panelColor);
        CreateText("Title", root.transform, "오토바이 상태", 16, lineColor, TextAnchor.MiddleLeft,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
            new Vector2(22f, -20f), new Vector2(200f, 24f));
        CreateText("Fuel Caption", root.transform, "연료", 13, mutedTextColor, TextAnchor.MiddleLeft,
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(22f, -48f), new Vector2(42f, 20f));
        UnityEngine.UI.Slider fuel = CreateBar("Fuel Bar", root.transform, new Vector2(0f, 1f),
            new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(70f, -51f),
            new Vector2(-22f, -51f), fuelColor);
        UnityEngine.UI.Text fuelText = CreateText("Fuel Text", fuel.transform, "0 / 0", 12, textColor,
            TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, Vector2.zero);
        CreateText("Durability Caption", root.transform, "내구", 13, mutedTextColor,
            TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(0f, 1f), new Vector2(22f, -77f), new Vector2(42f, 20f));
        UnityEngine.UI.Slider durability = CreateBar("Durability Bar", root.transform,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
            new Vector2(70f, -80f), new Vector2(-22f, -80f), healthColor);
        UnityEngine.UI.Text durabilityText = CreateText("Durability Text", durability.transform,
            "0 / 0", 12, textColor, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one,
            new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        UnityEngine.UI.Text warning = CreateText("Warning", root.transform, string.Empty, 13,
            UiTheme.TextDanger, TextAnchor.MiddleRight, new Vector2(0.5f, 1f), new Vector2(1f, 1f),
            new Vector2(1f, 1f), new Vector2(-22f, -20f), new Vector2(110f, 24f));

        VehicleHudUI vehicleHud = GetComponent<VehicleHudUI>();
        vehicleHud.root = root;
        vehicleHud.fuelFill = GetFill(fuel);
        vehicleHud.fuelText = fuelText;
        vehicleHud.durabilityFill = GetFill(durability);
        vehicleHud.durabilityText = durabilityText;
        vehicleHud.warningText = warning;
    }

    private void CreateWeaponPanel() {
        GameObject root = CreatePanel("Weapon Hud", transform, new Vector2(1f, 0f),
            new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-34f, 34f),
            new Vector2(354f, 150f), panelColor);
        CreateAccentLine("Accent", root.transform, new Vector2(1f, 0f), new Vector2(1f, 1f),
            new Vector2(1f, 0.5f), new Vector2(0f, 0f), new Vector2(5f, 150f));
        GameObject iconFrame = CreatePanel("Icon Frame", root.transform, new Vector2(0f, 0.5f),
            new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(18f, 0f),
            new Vector2(104f, 104f), insetColor);
        UnityEngine.UI.Image icon = CreateImage("Weapon Icon", iconFrame.transform, Vector2.zero,
            Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero, Color.white);
        icon.preserveAspect = true;
        CreateText("Equipped", root.transform, "장착 무기", 13, mutedTextColor, TextAnchor.MiddleLeft,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
            new Vector2(140f, -30f), new Vector2(174f, 22f));
        UnityEngine.UI.Text name = CreateText("Weapon Name", root.transform, "무기 없음", 20,
            textColor, TextAnchor.MiddleLeft, new Vector2(0f, 1f), new Vector2(1f, 1f),
            new Vector2(0f, 1f), new Vector2(140f, -58f), new Vector2(180f, 30f));
        UnityEngine.UI.Text ammo = CreateText("Ammo", root.transform, "-", 28, lineColor,
            TextAnchor.MiddleRight, new Vector2(0.5f, 0f), new Vector2(1f, 0f),
            new Vector2(1f, 0f), new Vector2(-22f, 25f), new Vector2(175f, 42f));

        WeaponHudUI weaponHud = GetComponent<WeaponHudUI>();
        weaponHud.root = root;
        weaponHud.weaponIcon = icon;
        weaponHud.weaponName = name;
        weaponHud.ammoText = ammo;
    }

    private void CreateInteractionPrompt() {
        GameObject root = CreatePanel("Interaction Prompt", transform, new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 34f),
            new Vector2(440f, 54f), insetColor);
        CreateAccentLine("Accent", root.transform, new Vector2(0f, 0f), new Vector2(0f, 1f),
            new Vector2(0f, 0.5f), Vector2.zero, new Vector2(4f, 54f));
        UnityEngine.UI.Text prompt = CreateText("Prompt", root.transform, string.Empty, 16, textColor,
            TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(-24f, 0f));

        InteractionPromptUI promptUi = GetComponent<InteractionPromptUI>();
        promptUi.promptText = prompt;
        promptUi.background = root;
    }

    private void CreateBuildHint() {
        UnityEngine.UI.Text hint = CreateText("Build Hint", transform, string.Empty, 15, textColor,
            TextAnchor.MiddleCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
            new Vector2(0.5f, 0f), new Vector2(0f, 100f), new Vector2(660f, 32f));
        hint.horizontalOverflow = HorizontalWrapMode.Overflow;
        GetComponent<BuildMenuUI>().hintText = hint;
    }

    private void CreateCrosshair() {
        GameObject root = CreateObject("Crosshair", transform);
        SetRect(root.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero,
            new Vector2(2f, 2f));
        UnityEngine.UI.Image top = CreateImage("Top", root.transform, new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0f), Vector2.zero,
            new Vector2(2f, 10f), textColor);
        UnityEngine.UI.Image bottom = CreateImage("Bottom", root.transform, new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 1f), Vector2.zero,
            new Vector2(2f, 10f), textColor);
        UnityEngine.UI.Image left = CreateImage("Left", root.transform, new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), Vector2.zero,
            new Vector2(10f, 2f), textColor);
        UnityEngine.UI.Image right = CreateImage("Right", root.transform, new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, 0.5f), new Vector2(0f, 0.5f), Vector2.zero,
            new Vector2(10f, 2f), textColor);
        SpreadCrosshair crosshair = root.AddComponent<SpreadCrosshair>();
        crosshair.top = top.rectTransform;
        crosshair.bottom = bottom.rectTransform;
        crosshair.left = left.rectTransform;
        crosshair.right = right.rectTransform;
        GetComponent<UIManager>().crosshair = crosshair;
    }

    private void BindPlayer() {
        player = FindFirstObjectByType<PlayerHealth>();

        if (player == null) {
            return;
        }

        inventory = player.GetComponent<Inventory>();
        equipment = player.GetComponent<Equipment>();
        UnityEngine.UI.Slider healthBar = FindDescendant("Health Bar (Styled)")
            ?.GetComponent<UnityEngine.UI.Slider>();

        if (healthBar != null) {
            player.healthSlider = healthBar;
            healthBar.maxValue = player.startingHealth;
            healthBar.value = player.health;
        }
    }

    private void RefreshReadouts() {
        if (player == null) {
            BindPlayer();
        }

        if (player == null) {
            return;
        }

        if (headingText != null) {
            float angle = Mathf.Repeat(player.transform.eulerAngles.y, 360f);
            headingText.text = $"{HeadingFor(angle)} · {angle:000}°";
        }

        if (armorText != null && equipment != null) {
            armorText.text = $"방어  {equipment.totalArmor:F0}";
        }

        if (weightText != null && inventory != null) {
            weightText.text = $"무게  {inventory.totalWeight:F1} / {inventory.maxWeight:F1} kg";
        }
    }

    private string HeadingFor(float angle) {
        string[] directions = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
        return directions[Mathf.RoundToInt(angle / 45f) % directions.Length];
    }

    private GameObject CreatePanel(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 pivot, Vector2 position, Vector2 size, Color color) {
        GameObject panel = CreateObject(name, parent, typeof(UnityEngine.UI.Image));
        SetRect(panel.GetComponent<RectTransform>(), anchorMin, anchorMax, pivot, position, size);
        UnityEngine.UI.Image image = panel.GetComponent<UnityEngine.UI.Image>();
        image.color = color;
        image.raycastTarget = false;
        return panel;
    }

    private void CreateAccentLine(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax,
        Vector2 pivot, Vector2 position, Vector2 size) {
        CreateImage(name, parent, anchorMin, anchorMax, pivot, position, size, lineColor);
    }

    private UnityEngine.UI.Slider CreateBar(string name, Transform parent, Vector2 anchorMin,
        Vector2 anchorMax, Vector2 pivot, Vector2 offsetMin, Vector2 offsetMax, Color fillColor) {
        GameObject root = CreateObject(name + " (Styled)", parent, typeof(UnityEngine.UI.Image),
            typeof(UnityEngine.UI.Slider));
        RectTransform rootRect = root.GetComponent<RectTransform>();
        rootRect.anchorMin = anchorMin;
        rootRect.anchorMax = anchorMax;
        rootRect.pivot = pivot;
        rootRect.offsetMin = offsetMin;
        rootRect.offsetMax = offsetMax;
        UnityEngine.UI.Image background = root.GetComponent<UnityEngine.UI.Image>();
        background.color = insetColor;
        background.raycastTarget = false;

        GameObject fillArea = CreateObject("Fill Area", root.transform);
        SetRect(fillArea.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f),
            Vector2.zero, new Vector2(-4f, -4f));
        GameObject fill = CreateObject("Fill", fillArea.transform, typeof(UnityEngine.UI.Image));
        SetRect(fill.GetComponent<RectTransform>(), Vector2.zero, Vector2.one, new Vector2(0f, 0.5f),
            new Vector2(2f, 2f), new Vector2(-2f, -2f));
        UnityEngine.UI.Image fillImage = fill.GetComponent<UnityEngine.UI.Image>();
        fillImage.color = fillColor;
        fillImage.raycastTarget = false;

        UnityEngine.UI.Slider slider = root.GetComponent<UnityEngine.UI.Slider>();
        slider.fillRect = fill.GetComponent<RectTransform>();
        slider.direction = UnityEngine.UI.Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 100f;
        slider.transition = UnityEngine.UI.Selectable.Transition.None;
        return slider;
    }

    private UnityEngine.UI.Text CreateText(string name, Transform parent, string value, int size,
        Color color, TextAnchor alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 position, Vector2 rectSize) {
        GameObject textObject = CreateObject(name, parent, typeof(UnityEngine.UI.Text));
        SetRect(textObject.GetComponent<RectTransform>(), anchorMin, anchorMax, pivot, position, rectSize);
        UnityEngine.UI.Text text = textObject.GetComponent<UnityEngine.UI.Text>();
        text.font = font;
        text.fontSize = size;
        text.color = color;
        text.alignment = alignment;
        text.text = value;
        text.raycastTarget = false;
        return text;
    }

    private UnityEngine.UI.Image CreateImage(string name, Transform parent, Vector2 anchorMin,
        Vector2 anchorMax, Vector2 pivot, Vector2 position, Vector2 size, Color color) {
        GameObject imageObject = CreateObject(name, parent, typeof(UnityEngine.UI.Image));
        SetRect(imageObject.GetComponent<RectTransform>(), anchorMin, anchorMax, pivot, position, size);
        UnityEngine.UI.Image image = imageObject.GetComponent<UnityEngine.UI.Image>();
        image.color = color;
        image.raycastTarget = false;
        return image;
    }

    private GameObject CreateObject(string name, Transform parent, params System.Type[] components) {
        GameObject created = new GameObject(name, typeof(RectTransform));

        foreach (System.Type component in components) {
            if (component != typeof(RectTransform)) {
                created.AddComponent(component);
            }
        }

        created.transform.SetParent(parent, false);
        return created;
    }

    private void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 position, Vector2 size) {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    private RectTransform GetFill(UnityEngine.UI.Slider slider) {
        return slider.fillRect;
    }

    private Transform FindDescendant(string targetName) {
        foreach (Transform child in GetComponentsInChildren<Transform>(true)) {
            if (child.name == targetName) {
                return child;
            }
        }

        return null;
    }
}
