using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour, IAllowDamage
{


    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] CharacterController controller;
    [SerializeField] GameObject playerCamera;
    [Header("--- Health ---")]
    [SerializeField] int HP; //The current health of the player
    [Header("--- Movement ---")]
    [SerializeField] int speed; //The base speed of the player
    [SerializeField] int sprintMod; //The amount the speed is multiplied by when sprinting
    [SerializeField] int jumpSpeed; //The speed at which the player jumps
    [SerializeField] int jumpMax; //The maximum amount of jumps the player can do before touching the ground again
    [SerializeField] int gravity; //The gravity affecting the player
    [Header("--- Stamina ---")]
    [SerializeField] int stamina; //The amount of stamina the player has
    [SerializeField] int minStamina; //The lowest amount of stamina the player can have to sprint
    [SerializeField] float staminaGainMult; //The speed at which stamina is gained
    [SerializeField] float staminaLossMult; //The speed at which stamina is lost
    [Header("--- Shooting ---")]
    [SerializeField] int shootDamage; //The amount of damage the player's weapon does
    [SerializeField] float shootRate; //The rate of fire of the player's weapon
    [SerializeField] int shootDist; //The maximum distance the player's weapon can shoot



    Vector3 moveDirection;
    Vector3 playerVelocity;

    float shootTimer;
    float exactStamina;

    int jumpcount;
    int HPOriginal;
    int staminaOriginal;
    bool isSprinting = false;
    int speedOriginal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speedOriginal = speed;
        HPOriginal = HP;
        exactStamina = stamina;
        staminaOriginal = stamina;

        updateStaminaUI();
        updateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        movement();
        sprint();
        if (!isSprinting && stamina != -1 && stamina < staminaOriginal)
        {
            //If the player is not sprinting and has less stamina than original, gain stamina
            exactStamina += Time.deltaTime * staminaGainMult;
            stamina = (int)exactStamina;
            updateStaminaUI();
        }
        else if (isSprinting && stamina != -1 && (moveDirection.x != 0 || moveDirection.z != 0))
        {
            //If the player is sprinting, lose stamina
            exactStamina -= Time.deltaTime * staminaLossMult;
            stamina = (int)exactStamina;
            updateStaminaUI();
        }
        else if (isSprinting && stamina != -1 && (moveDirection.x == 0 && moveDirection.z == 0) && stamina < staminaOriginal)
        {
            //If the player is not sprinting and has less stamina than original, gain stamina
            exactStamina += Time.deltaTime * staminaGainMult;
            stamina = (int)exactStamina;
            updateStaminaUI();
        }
        //I know it looks weird, but this is the best way to prevent errors when using float math with deltaTime while also keeping the stamina as an int

    }

    void movement()
    {
        shootTimer += Time.deltaTime;
        if (controller.isGrounded)
        {
            jumpcount = 0;
            playerVelocity = Vector3.zero;
        }
        else
        {
            playerVelocity.y -= gravity * Time.deltaTime;
        }
        moveDirection = (settingsManager.instance.GetAxis("Horizontal") * transform.right) + (settingsManager.instance.GetAxis("Vertical") * transform.forward);

        controller.Move(moveDirection * speed * Time.deltaTime);
        jump();
        controller.Move(playerVelocity * Time.deltaTime);
        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }
        if (Input.GetButtonDown("Zoom"))
        {
            playerCamera.GetComponent<cameraController>().ZoomIn();
        }
        else if (Input.GetButtonUp("Zoom"))
        {
            playerCamera.GetComponent<cameraController>().ZoomOut();
        }
    }

    void jump()
    {
        if (settingsManager.instance.GetKeyDown("Jump") && jumpcount < jumpMax)
        {
            jumpcount++;
            playerVelocity.y = jumpSpeed;
        }
    }
    void sprint()
    {
        if (settingsManager.instance.GetKeyDown("Sprint"))
        {
            if (stamina >= minStamina || stamina == -1)
            {
                speed *= sprintMod;
                isSprinting = true;
            }
        }
        else if (settingsManager.instance.GetKeyUp("Sprint"))
        {
            speed = speedOriginal; //Changed the division here into a variable to decrease room for bugs!
            isSprinting = false;
        }
        if (stamina == 0)
        {
            speed = speedOriginal; //Changed the division here into a variable to decrease room for bugs!
            isSprinting = false;
        }
    }

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name); //Delete if you dont need :)

            IAllowDamage dmg = hit.collider.GetComponent<IAllowDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }
        }
    }

    public void spawnPlayer()
    {
        HP = HPOriginal;
        //tp player to spawn point
        //give them basic weaponary
    }

    public void TakeDamage(int amount)
    {
        HP -= amount;
        updateHealthUI();
        StartCoroutine(flashDamageScreen());
        if (HP <= 0)
        {
            if (gamemanager.instance != null)
            {
                gamemanager.instance.youLose();
            }
        }
    }

    public void HealDamage(int amount, bool onCooldown)
    {
        if (onCooldown == false && HP < HPOriginal)
        {

            HP += amount;
            updateHealthUI();
            StartCoroutine(flashHealingScreen());
            //This should flash green upon healing, that would be really cool :)
            if (HP > HPOriginal)
            {
                HP = HPOriginal; //Prevent healing over max health
            }

        }
    }

    public void updateHealthUI()
    {
        if (gamemanager.instance != null)
        {
            gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOriginal;
        }
    }

    public void updateStaminaUI()
    {
        if (gamemanager.instance != null)
        {
            gamemanager.instance.playerStaminaBar.fillAmount = (float)stamina / staminaOriginal;
        }
    }

    IEnumerator flashDamageScreen()
    {
        if (gamemanager.instance != null)
        {

            gamemanager.instance.playerDamageScreen.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            gamemanager.instance.playerDamageScreen.SetActive(false);
        }
    }


    IEnumerator flashHealingScreen()
    {
        if (gamemanager.instance != null)
        {
            gamemanager.instance.playerHealScreen.SetActive(true);
            yield return new WaitForSeconds(0.1f);
            gamemanager.instance.playerHealScreen.SetActive(false);
        }
    }
}
