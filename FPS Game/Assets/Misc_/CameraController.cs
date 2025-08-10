using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    InputAction lookAction;

    float rotX;

    private void Start()
    {
        lookAction = InputSystem.actions.FindAction("Player/Look");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        Vector2 mouse = lookAction.ReadValue<Vector2>() * sens * Time.deltaTime;

        if (invertY)
        {

            rotX += mouse.y;


        }
        else
        {
            rotX -= mouse.y;
        }

        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        transform.parent.Rotate(Vector3.up * mouse.x);
    }
}