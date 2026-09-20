using UnityEngine;

// 필요한 UI에 즉시 접근하고 변경할 수 있도록 허용하는 UI 매니저
public class UIManager : MonoBehaviour {
    // 싱글톤 접근용 프로퍼티
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindFirstObjectByType<UIManager>();
            }

            return m_instance;
        }
    }

    private static UIManager m_instance; // 싱글톤이 할당될 변수

    public SpreadCrosshair crosshair; // 탄퍼짐에 따라 벌어지는 조준점

    // 인벤토리 같은 전체 화면이 열려 있는지 여부
    // 열려 있는 동안에는 플레이어 입력과 카메라 회전을 멈추고 마우스 커서를 보여준다
    public static bool isScreenOpen { get; private set; }

    // 전체 화면의 열림 상태를 바꾸고 마우스 커서를 함께 전환
    public static void SetScreenOpen(bool open) {
        isScreenOpen = open;
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;
    }

    // 조준점 탄퍼짐 갱신
    public void UpdateCrosshairSpread(float normalizedSpread) {
        crosshair.SetSpread(normalizedSpread);
    }
}
