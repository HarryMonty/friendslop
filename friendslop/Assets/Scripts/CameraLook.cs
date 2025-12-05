using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("Camera Settings")]
    public float mouseSensitivity = 200f;
    private Transform cameraTransform;
    float xRotation = 0f;
    float yRotation = 0f;
    void Start()
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Grab camera transform
        cameraTransform = gameObject.transform;
    }

    void Update()
    {
        // Check for camera transform
        if (!cameraTransform)
        {
            Debug.Log("No camera transform");
            return;
        }

        // Grab user inputs
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Move camera based on user inputs and sensitivity
        mouseX = mouseX * mouseSensitivity * Time.deltaTime;
        mouseY = mouseY * mouseSensitivity * Time.deltaTime;

        // Calculate horizontal & vertical movement
        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, -45f, 45f);
        yRotation = Mathf.Clamp(yRotation, -45f, 45f);

        // Change camera rotation
        cameraTransform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
