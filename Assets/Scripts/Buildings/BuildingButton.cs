using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerClickHandler {
    [SerializeField] private Building building = null;
    [SerializeField] private Image image = null;
    [SerializeField] private TMP_Text priceText = null;
    [SerializeField] private LayerMask floorMask;

    private Camera mainCamera;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;
    private bool previewMode = false;


    private void Start() {
        mainCamera = Camera.main;
        image.sprite = building.GetIcon();
        priceText.text = building.GetPrice().ToString();
    }

    private void Update() {
        if (player == null) {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (buildingPreviewInstance == null) return;

        UpdateBuildingPreviewPosition();
        PlaceBuilding();
    }

    public void OnPointerClick(PointerEventData eventData) {

        if (eventData.button == PointerEventData.InputButton.Left) {
            // instantiate the preview instance, assign the renderer, set preview mode, and default to invis
            buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
            buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();
            previewMode = true;
            buildingPreviewInstance.SetActive(false);    
        }
        
    }

    private void UpdateBuildingPreviewPosition() {
        // do a raycast, move preview to mouse point.
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) return;
        buildingPreviewInstance.transform.position = hit.point;
        
        // if the update point is valid, set the preview active
        if (buildingPreviewInstance.activeSelf == false) {
            buildingPreviewInstance.SetActive(true);
        }
    }

    private void PlaceBuilding() {
        if (Mouse.current.leftButton.wasPressedThisFrame && previewMode == true) {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) {
                player.CmdTryPlaceBuilding(building.GetId(), hit.point);    
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame && previewMode == true) {
            if (buildingPreviewInstance != null) {
                buildingPreviewInstance.SetActive(false);
                Destroy(buildingPreviewInstance);
                previewMode = false;
            }
        }
    }
}