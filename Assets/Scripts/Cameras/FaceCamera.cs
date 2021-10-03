using UnityEngine;

public class FaceCamera : MonoBehaviour {
    private Transform mainCameraTransform = null;

    private void Start() {
        mainCameraTransform = Camera.main.transform;
    }

    private void LateUpdate() {
        var mainCameraRotation = mainCameraTransform.rotation;
        transform.LookAt(
            transform.position + mainCameraRotation * Vector3.forward,
            mainCameraRotation * Vector3.up);
    }
}