using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;
    [SerializeField] float zoomFOV = 30f;
    [SerializeField] float zoomSpeed = 10f;

    float rotX;
    float originalFOV;
    Camera cam;
    bool isZoomed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        cam = GetComponent<Camera>();
        originalFOV = cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        // get input
        float mouseX = Input.GetAxisRaw("Mouse X") * sens * Time.deltaTime;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sens * Time.deltaTime;


        // use invertY to give option of look up/down
        if (invertY)
            rotX += mouseY;
        else
            rotX -= mouseY;

        // clamp the camera on the x-axis
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        // rotate the camera to look up and down
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        // rotate the player to look left and right
        transform.parent.Rotate(Vector3.up * mouseX);

        // smooth the zooming in
        float targetFOV = isZoomed ? zoomFOV : originalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
    }

    public void ZoomIn()
    {
        isZoomed = true;
    }

    public void ZoomOut()
    {
        isZoomed = false;
    }

}
