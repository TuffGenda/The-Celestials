using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;
    float speed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float gravity;
    [SerializeField] float jumpVel;
    [SerializeField] int jumpMax;
    int jumpCount;

    Vector3 moveDirection;
    Vector3 playerVel;

    [SerializeField] float shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] float shootDist;
    float shootTimer;

    [SerializeField] public float MaxHP;
    public float HP;
    float HPShake;

    Vector3 origPosition;

    Transform voidBound;

    InputAction moveAction;
    InputAction sprintKey;
    InputAction jumpKey;
    InputAction shootKey;

    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Player/Move");
        sprintKey = InputSystem.actions.FindAction("Player/Sprint");
        jumpKey = InputSystem.actions.FindAction("Player/Jump");
        shootKey = InputSystem.actions.FindAction("Player/Attack");
        speed = walkSpeed;
        voidBound = GameObject.FindGameObjectWithTag("Void").GetComponent<Transform>();

        HP = MaxHP;
        origPosition = gameManager.instance.player.GetComponent<Transform>().position;
        updatePlayerUI();
    }

    public void OnMove(InputValue value)
    {
        moveDirection = value.Get<Vector2>();
    }

    public void OnJump()
    {
        if (jumpCount < jumpMax)
        {
            playerVel.y = jumpVel;
            jumpCount++;
        }
    }

    void sprint()
    {
        if (sprintKey.IsPressed())
        {
            if (speed != sprintSpeed)
            {
                speed = sprintSpeed;
            }
        }
        else
        {
            if (speed != walkSpeed)
            {
                speed = walkSpeed;
            }
        }
    }

    void movement()
    {
        if (controller.isGrounded)
        {
            playerVel.y = 0;
            jumpCount = 0;
        }

        sprint();

        Vector3 moveDir = (moveDirection.x * transform.right) + (moveDirection.y * transform.forward);

        controller.Move(moveDir * speed * Time.deltaTime);

        controller.Move(playerVel * Time.deltaTime);
        playerVel.y -= gravity * Time.deltaTime;
    }

    void shoot()
    {

        if (!(shootKey.triggered && shootTimer > shootRate)) return;

        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            if (hit.collider != null && hit.collider.GetComponent<IDamage>() != null)
                hit.collider.GetComponent<IDamage>().takeDamage(shootDamage);
        }
    }

    public void takeDamage(float amount)
    {
        HP -= amount;
        HPShake = 20;

        updatePlayerUI();

        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = HP / MaxHP;
    }

    void tick()
    {
        movement();

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.blue);
        shoot();
        shootTimer += Time.deltaTime;

        if (transform.position.y <= voidBound.position.y)
        {
            transform.position = origPosition;
            takeDamage(1);
        }

        HPShake -= Time.deltaTime * 20;
        HPShake = Mathf.Max(0, HPShake);
        gameManager.instance.playerHPFlash.GetComponent<Image>().color = new Color(1, 1, 1, HPShake / 80);
    }

    void Update()
    {
        if (Time.timeScale <= 0) return;
        tick();
    }
}