// 1인칭 시점을 구성하는 컨트롤러 (탑다운/3인칭 시점 제거, 1인칭 전용)
using UnityEngine;

public class CameraRigController : MonoBehaviour {
    public GameObject firstPersonCamera; // 1인칭 카메라 가상 카메라 오브젝트
    public PlayerShooter playerShooter; // 총 배치 기준을 1인칭 마운트로 맞춰줄 슈터
    public PlayerMovement playerMovement; // 마우스 좌우 회전으로 전환할 이동 스크립트
    public FirstPersonLook firstPersonLook; // 카메라 상하 시점(피치) 조작 컴포넌트
    public GameObject bodyRoot; // 몸통(모든 파츠)의 루트. 1인칭 전용 팔/소매 파츠만 남기고 나머지를 숨긴다
    public GameObject firstPersonHands; // 1인칭 전용 팔/소매 뷰모델 루트 (FPS_HANDS). 몸 전체를 보여주는 방식에서는 사용하지 않음

    public GameObject weaponViewModelTarget; // 1인칭 전용 카메라로만 보이게 할 총 오브젝트 (Gun)
    public string firstPersonWeaponLayer = "FirstPersonWeapon"; // 1인칭 전용 무기 카메라가 그리는 레이어

    private void Awake() {
        if (firstPersonCamera != null)
        {
            firstPersonCamera.SetActive(true);
        }

        if (playerShooter != null)
        {
            // 몸 전체가 보이므로 카메라 전용 마운트 대신 캐릭터 팔꿈치 IK를 기준으로 총을 든다
            // (실제 조준 자세 애니메이션 + IK 손 정렬을 그대로 사용해 총을 "제대로 쥔" 모습이 되게 한다)
            playerShooter.useFirstPersonMount = false;
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
            // 몸통을 그대로 보여주는 방식으로 바꿨으므로 팔이 겹쳐 보이지 않도록
            // 전용 1인칭 팔/소매 뷰모델은 꺼둔다
            firstPersonHands.SetActive(false);
        }

        if (bodyRoot != null)
        {
            // 몸을 숨기지 않고 그대로 보여준다. 카메라가 몸통 메시와 겹쳐 화면을 가리는 문제는
            // 렌더러를 숨기는 대신 카메라 위치(FirstPerson Cam의 로컬 좌표)를 조정해서 해결한다
            var renderers = bodyRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var renderer in renderers)
            {
                renderer.enabled = true;
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
