using UnityEngine;

// 플레이어 캐릭터를 조작하기 위한 사용자 입력을 감지
// 감지된 입력값을 다른 컴포넌트들이 사용할 수 있도록 제공
public class ZombiePlayerInput : MonoBehaviour {
    public string moveAxisName = "Vertical"; // 앞뒤 움직임을 위한 입력축 이름
    public string rotateAxisName = "Horizontal"; // 좌우 회전을 위한 입력축 이름
    public string fireButtonName = "Fire1"; // 발사를 위한 입력 버튼 이름
    public string reloadButtonName = "Reload"; // 재장전을 위한 입력 버튼 이름
    public string jumpButtonName = "Jump"; // 점프를 위한 입력 버튼 이름

    // 값 할당은 내부에서만 가능
    public float move { get; private set; } // 감지된 움직임 입력값
    public float rotate { get; private set; } // 감지된 회전 입력값
    public bool fire { get; private set; } // 감지된 발사 입력값
    public bool reload { get; private set; } // 감지된 재장전 입력값
    public bool jump { get; private set; } // 감지된 점프 입력값

    // 숫자키 1~9로 선택한 무기 인덱스(0부터 시작), 이번 프레임에 선택 입력이 없으면 -1
    public int selectWeaponIndex { get; private set; }

    // 매프레임 사용자 입력을 감지
    private void Update() {
        // 게임오버 상태이거나 인벤토리 같은 전체 화면이 열려 있으면 사용자 입력을 감지하지 않는다
        if (UIManager.isScreenOpen
            || (GameManager.instance != null
                && GameManager.instance.isGameover))
        {
            move = 0;
            rotate = 0;
            fire = false;
            reload = false;
            jump = false;
            selectWeaponIndex = -1;
            return;
        }

        // move에 관한 입력 감지
        move = Input.GetAxis(moveAxisName);
        // rotate에 관한 입력 감지
        rotate = Input.GetAxis(rotateAxisName);
        // fire에 관한 입력 감지
        fire = Input.GetButton(fireButtonName);
        // reload에 관한 입력 감지
        reload = Input.GetButtonDown(reloadButtonName);
        // jump에 관한 입력 감지
        jump = Input.GetButtonDown(jumpButtonName);

        // 무기 선택(1~9) 입력 감지
        selectWeaponIndex = -1;
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectWeaponIndex = i;
                break;
            }
        }
    }
}
