// 10개 저장 슬롯을 보여주고 저장/불러오기/삭제를 실행하는 화면
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : ScreenPanel {
    public Transform content; // 슬롯 줄이 붙을 부모
    public GameObject rowPrefab; // 줄 하나의 프리팹
    public Text statusText; // 마지막 동작과 자동저장 안내

    private string status = "";

    protected override void Refresh() {
        if (!panel.activeSelf)
        {
            return;
        }

        ClearRows(content);

        foreach (SaveSlotInfo info in SaveSystem.ListSlots())
        {
            CreateRow(info);
        }

        UpdateStatus();
    }

    private void CreateRow(SaveSlotInfo info) {
        GameObject row = Instantiate(rowPrefab, content);
        int slot = info.slot;

        row.transform.Find("Label").GetComponent<Text>().text =
            $"슬롯 {slot + 1}   "
            + (info.manual == null ? "(비어 있음)" : info.manual.Summarize());

        row.transform.Find("Auto Label").GetComponent<Text>().text =
            info.auto == null ? "자동저장 없음" : $"자동저장   {info.auto.Summarize()}";

        BindButton(row, "Save Button", "저장", true, () => {
            status = SaveManager.instance.Save(slot, false)
                ? $"슬롯 {slot + 1}에 저장했다."
                : $"슬롯 {slot + 1} 저장에 실패했다.";
            Refresh();
        });

        BindButton(row, "Load Button", "불러오기", info.manual != null, () => {
            LoadAndClose(slot, false);
        });

        BindButton(row, "Auto Load Button", "자동 불러오기", info.auto != null, () => {
            LoadAndClose(slot, true);
        });

        BindButton(row, "Delete Button", "삭제", !info.isEmpty, () => {
            SaveSystem.Delete(slot, false);
            SaveSystem.Delete(slot, true);
            status = $"슬롯 {slot + 1}을 지웠다.";
            Refresh();
        });
    }

    private void BindButton(
        GameObject row, string name, string caption, bool interactable,
        UnityEngine.Events.UnityAction action) {
        Button button = row.transform.Find(name).GetComponent<Button>();
        button.transform.Find("Text").GetComponent<Text>().text = caption;
        button.interactable = interactable;
        button.onClick.AddListener(action);
    }

    private void LoadAndClose(int slot, bool auto) {
        if (SaveManager.instance.Load(slot, auto))
        {
            SetOpen(false);
            return;
        }

        status = $"슬롯 {slot + 1} 불러오기에 실패했다.";
        Refresh();
    }

    private void UpdateStatus() {
        SaveManager manager = SaveManager.instance;
        float remaining = manager.timeUntilAutoSave;

        statusText.text = status
            + (remaining >= 0f
                ? $"   다음 자동저장까지 {remaining:F0}초 (슬롯 {manager.currentSlot + 1})"
                : "   자동저장 꺼짐");
    }
}
