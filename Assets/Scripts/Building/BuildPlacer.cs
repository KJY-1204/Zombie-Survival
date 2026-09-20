// 건설물을 자유 배치로 설치한다. 프리뷰를 띄우고 회전/판정/재료 소모를 처리한다 (UI는 공개 상태만 읽는다)
using System;
using UnityEngine;

public class BuildPlacer : MonoBehaviour {
    public float maxPlaceDistance = 8f; // 설치할 수 있는 최대 거리
    public float rotationStep = 15f; // 휠 한 칸당 회전 각도

    public BuildableData selected { get; private set; } // 지금 고른 건설물 (없으면 null)
    public bool isBuilding => selected != null; // 건설 모드인지
    public bool canPlaceHere { get; private set; } // 지금 위치에 설치할 수 있는지

    public event Action onStateChanged; // 선택이나 설치 가능 여부가 바뀔 때 발동

    private Inventory inventory; // 재료를 확인하고 차감할 인벤토리
    private Camera aimCamera; // 설치 지점을 찾을 카메라

    private GameObject preview; // 반투명 미리보기 오브젝트
    private Material previewMaterial; // 미리보기 전용 런타임 머티리얼
    private Vector3 previewPosition; // 미리보기 위치
    private float previewRotationY; // 미리보기 회전

    private static readonly Color allowedColor = new Color(0.35f, 1f, 0.45f, 0.45f);
    private static readonly Color blockedColor = new Color(1f, 0.32f, 0.28f, 0.45f);

    private void Start() {
        inventory = GetComponent<Inventory>();
        aimCamera = Camera.main;
    }

    private void OnDestroy() {
        DestroyPreview();

        if (previewMaterial != null)
        {
            Destroy(previewMaterial);
        }
    }

    // 건설할 대상을 고르고 건설 모드로 들어간다
    public void Select(BuildableData data) {
        selected = data;
        previewRotationY = 0f;

        DestroyPreview();

        if (selected != null)
        {
            CreatePreview();
        }

        NotifyStateChanged();
    }

    // 건설 모드에서 빠져나온다
    public void Cancel() {
        Select(null);
    }

    private void Update() {
        if (!isBuilding)
        {
            return;
        }

        // 화면이 열려 있는 동안에는 조작하지 않는다 (프리뷰는 그대로 둔다)
        if (UIManager.isScreenOpen)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Fire2"))
        {
            Cancel();
            return;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            previewRotationY += Mathf.Sign(scroll) * rotationStep;
        }

        UpdatePreview();

        if (Input.GetButtonDown("Fire1"))
        {
            TryPlace();
        }
    }

    // 바라보는 바닥 지점으로 프리뷰를 옮기고 설치 가능 여부를 갱신한다
    private void UpdatePreview() {
        if (!TryFindGround(out RaycastHit hit))
        {
            // 바닥을 찾지 못하면 설치할 수 없다
            SetPreviewVisible(false);
            SetCanPlace(false);
            return;
        }

        previewPosition = hit.point;
        SetPreviewVisible(true);

        Quaternion rotation = Quaternion.Euler(0f, previewRotationY, 0f);
        preview.transform.SetPositionAndRotation(previewPosition, rotation);

        bool affordable = selected.CanAfford(inventory);
        bool blocked = IsBlocked(previewPosition, rotation);
        SetCanPlace(affordable && !blocked);

        previewMaterial.SetColor("_BaseColor", canPlaceHere ? allowedColor : blockedColor);
    }

    // 설치할 바닥 지점을 찾는다
    // 조준선이 사정거리 안에서 바닥에 닿으면 그 지점을 쓰고,
    // 거의 수평으로 보고 있어서 닿지 않으면 사정거리 끝 지점에서 아래로 내려 바닥을 찾는다
    private bool TryFindGround(out RaycastHit hit) {
        Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (TryRaycastIgnoringSelf(ray, maxPlaceDistance, out hit))
        {
            return true;
        }

        Vector3 far = ray.origin + ray.direction * maxPlaceDistance;
        var downRay = new Ray(far + Vector3.up * 5f, Vector3.down);
        return TryRaycastIgnoringSelf(downRay, 20f, out hit);
    }

    // 플레이어 자신의 콜라이더는 건너뛰고 가장 가까운 충돌을 찾는다
    // (카메라가 캐릭터 뒤쪽에 있어 조준 레이가 자기 몸을 먼저 통과할 수 있다)
    private bool TryRaycastIgnoringSelf(Ray ray, float distance, out RaycastHit result) {
        RaycastHit[] hits = Physics.RaycastAll(
            ray, distance, ~0, QueryTriggerInteraction.Ignore);

        bool found = false;
        result = default;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.root == transform.root)
            {
                continue;
            }

            if (!found || hit.distance < result.distance)
            {
                result = hit;
                found = true;
            }
        }

        return found;
    }

    // 설치 자리에 다른 물체가 겹치는지 확인한다
    // 판정 상자는 접촉 지점보다 살짝 위에서 시작하므로(BuildableData.checkCenter)
    // 딛고 선 바닥은 애초에 상자에 닿지 않는다. 바닥을 따로 예외 처리하면
    // 조준선이 바닥이 아니라 다른 건설물에 맞았을 때 그 건설물까지 예외가 되어버린다
    private bool IsBlocked(Vector3 position, Quaternion rotation) {
        Vector3 center = position + rotation * selected.checkCenter;

        Collider[] hits = Physics.OverlapBox(
            center, selected.checkSize * 0.5f, rotation, ~0, QueryTriggerInteraction.Ignore);

        foreach (Collider hit in hits)
        {
            // 자기 자신(플레이어)은 겹침으로 보지 않는다
            if (hit.transform.root == transform.root)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    // 지금 위치에 실제로 설치한다
    private bool TryPlace() {
        if (!canPlaceHere || BaseBuildState.instance == null)
        {
            return false;
        }

        // 재료를 먼저 차감하고, 차감에 실패하면 설치하지 않는다
        if (!selected.Pay(inventory))
        {
            return false;
        }

        BaseBuildState.instance.Place(selected, previewPosition, previewRotationY);

        // 재료가 남아 있으면 연속으로 더 지을 수 있게 건설 모드를 유지한다
        if (!selected.CanAfford(inventory))
        {
            Cancel();
        }

        return true;
    }

    // 프리뷰 오브젝트를 만든다 (콜라이더와 로직은 전부 제거하고 반투명 머티리얼만 남긴다)
    private void CreatePreview() {
        preview = Instantiate(selected.prefab);
        preview.name = selected.displayName + " Preview";

        // 콜라이더도 지우지 않고 끈다
        // Destroy는 프레임 끝에야 처리되므로, 그 프레임 동안 프리뷰가 자기 콜라이더에
        // 막혀 설치 불가로 보이거나 조준 레이를 가로챌 수 있다
        foreach (Collider collider in preview.GetComponentsInChildren<Collider>(true))
        {
            collider.enabled = false;
        }

        // 스크립트는 지우지 않고 끈다
        // RequireComponent로 묶인 컴포넌트(예: StorageContainer -> Inventory)는
        // 의존하는 쪽이 남아 있으면 제거가 거부되고 에러만 남는다
        foreach (MonoBehaviour behaviour in preview.GetComponentsInChildren<MonoBehaviour>(true))
        {
            behaviour.enabled = false;
        }

        if (previewMaterial == null)
        {
            previewMaterial = CreateTransparentMaterial();
        }

        foreach (Renderer renderer in preview.GetComponentsInChildren<Renderer>(true))
        {
            var materials = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterial;
            }
            renderer.sharedMaterials = materials;
        }
    }

    // URP Lit을 런타임에 반투명으로 설정한다 (인스펙터의 Surface Type = Transparent와 같은 설정)
    private static Material CreateTransparentMaterial() {
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.SetFloat("_Surface", 1f);
        material.SetFloat("_Blend", 0f);
        material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetFloat("_ZWrite", 0f);
        material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        material.DisableKeyword("_ALPHATEST_ON");
        material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        return material;
    }

    private void DestroyPreview() {
        if (preview != null)
        {
            Destroy(preview);
            preview = null;
        }
    }

    private void SetPreviewVisible(bool visible) {
        if (preview != null && preview.activeSelf != visible)
        {
            preview.SetActive(visible);
        }
    }

    private void SetCanPlace(bool value) {
        if (canPlaceHere == value)
        {
            return;
        }

        canPlaceHere = value;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() {
        if (onStateChanged != null)
        {
            onStateChanged();
        }
    }
}
