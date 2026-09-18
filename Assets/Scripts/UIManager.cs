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
                m_instance = FindObjectOfType<UIManager>();
            }

            return m_instance;
        }
    }

    private static UIManager m_instance; // 싱글톤이 할당될 변수

    public SpreadCrosshair crosshair; // 탄퍼짐에 따라 벌어지는 조준점

    // 조준점 탄퍼짐 갱신
    public void UpdateCrosshairSpread(float normalizedSpread) {
        crosshair.SetSpread(normalizedSpread);
    }
}
