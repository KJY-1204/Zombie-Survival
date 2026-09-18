// 1인칭 시점을 구성하는 컨트롤러 (탑다운/3인칭 시점 제거, 1인칭 전용)
using UnityEngine;

public class CameraRigController : MonoBehaviour {
    public GameObject firstPersonCamera; // 1인칭 카메라 가상 카메라 오브젝트
    public PlayerShooter playerShooter; // 총 배치 기준을 1인칭 마운트로 맞춰줄 슈터
    public PlayerMovement playerMovement; // 마우스 좌우 회전으로 전환할 이동 스크립트
    public FirstPersonLook firstPersonLook; // 카메라 상하 시점(피치) 조작 컴포넌트
    public GameObject bodyRoot; // 몸통(모든 파츠)의 루트. 1인칭 전용 팔/소매 파츠만 남기고 나머지를 숨긴다
    public GameObject firstPersonHands; // 1인칭 전용 팔/소매 뷰모델 루트 (FPS_HANDS). 무기와 같은 전용 레이어로 옮겨 근평면 클리핑을 피한다
    public string[] firstPersonVisiblePartNames = { "SK_Miliary_FPS_Arms_Gloves1", "SK_Miliary_Military_FPS_Shirt3" }; // 1인칭에서도 보여줄 파츠 이름 (FPS_HANDS 하위의 전용 뷰모델 메시)

    public GameObject weaponViewModelTarget; // 1인칭 전용 카메라로만 보이게 할 총 오브젝트 (Gun)
    public string firstPersonWeaponLayer = "FirstPersonWeapon"; // 1인칭 전용 무기 카메라가 그리는 레이어

    private void Awake() {
        if (firstPersonCamera != null)
        {
            firstPersonCamera.SetActive(true);
        }

        if (playerShooter != null)
        {
            playerShooter.useFirstPersonMount = true;
        }

        if (playerMovement != null)
        {
            playerMovement.useMouseLook = true;
        }

        if (firstPersonLook != null)
        {
            firstPersonLook.enabled = true;
        }

        if (firstPersonHands != null)
        {
            // 전용 1인칭 팔/소매 뷰모델을 활성화하고 무기와 같은 레이어로 옮긴다.
            // 몸통 스켈레톤 기준 팔 위치는 메인 카메라 근평면(nearClipPlane)보다 가까워 잘려 보이지 않으므로,
            // 무기와 동일하게 근평면이 훨씬 얇은 1인칭 전용 카메라에서만 그리게 한다
            firstPersonHands.SetActive(true);
            SetLayerRecursively(firstPersonHands, LayerMask.NameToLayer(firstPersonWeaponLayer));
        }

        if (bodyRoot != null)
        {
            // 1인칭 카메라가 몸통 메시 안쪽에서 시작해 화면을 가리는 것을 막기 위해 대부분을 숨기되,
            // 총을 쥔 전용 1인칭 팔/소매 파츠만 남겨서 총이 허공에 떠 있지 않고 자연스럽게 보이게 한다
            var renderers = bodyRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var renderer in renderers)
            {
                renderer.enabled = System.Array.IndexOf(firstPersonVisiblePartNames, renderer.gameObject.name) >= 0;
            }
        }

        if (weaponViewModelTarget != null)
        {
            // 총을 전용 무기 카메라만 그리는 레이어로 옮겨서
            // 메인 카메라에서는 안 보이고(포스트 프로세싱/근접 왜곡 없이) 무기 카메라에서만 보이게 한다
            SetLayerRecursively(weaponViewModelTarget, LayerMask.NameToLayer(firstPersonWeaponLayer));
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // target과 모든 자식 오브젝트의 레이어를 재귀적으로 변경
    private static void SetLayerRecursively(GameObject target, int layer) {
        if (layer < 0)
        {
            return;
        }

        target.layer = layer;
        foreach (Transform child in target.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
