using UnityEngine;

public class cameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] float zoomFOV = 30f;
    [SerializeField] float zoomSpeed = 10f;
    [SerializeField] Camera gunCam;

    float rotX;
    float originalFOV;
    float originalGunFOV;
    Camera cam;
    bool isZoomed = false;
    public bool invertY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        originalFOV = cam.fieldOfView;
        originalGunFOV = gunCam.fieldOfView;
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
        float targetGunFOV = isZoomed ? zoomFOV : originalGunFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * zoomSpeed);
        gunCam.fieldOfView = Mathf.Lerp(gunCam.fieldOfView, targetGunFOV, Time.deltaTime * zoomSpeed);
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
