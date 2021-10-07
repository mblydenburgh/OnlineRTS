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
        if (Mouse.current.leftButton.wasPressedThisFrame && previewMode == true) {
            Debug.Log("click while preview mode is true");
            if (buildingPreviewInstance != null) {
                Debug.Log("Placing building");
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("Click");
        if (eventData.button == PointerEventData.InputButton.Right) {
            if (buildingPreviewInstance != null) {
                Debug.Log("Clicked right mouse button, destroying preview");    
            }
            Debug.Log("Clicked right mouse button, no preview to destroy");
        }

        if (eventData.button == PointerEventData.InputButton.Left) {
            if (previewMode == true) {
                Debug.Log("Clicked when preview mode is true");
                // Do raycast to build
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) {
                    Debug.Log("Clicked at a valid point, placing building");
                    // Place building and delete    
                } else {
                    Debug.Log("invalid raycast, cant place");
                }
                Debug.Log("Destroying building instance");
                Destroy(buildingPreviewInstance);
            }

            Debug.Log("Clicked when preview mode is false");
            // instantiate the instance, assign the renderer, set preview mode, and default the preview to invis
            buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
            buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();
            Debug.Log("Setting preview mode to true");
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
            Debug.Log("Raycast hit valid point, turning on preview");
            buildingPreviewInstance.SetActive(true);
        }
    }
}