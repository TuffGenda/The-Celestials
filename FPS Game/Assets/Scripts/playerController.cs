using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour, IAllowDamage
{


    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] CharacterController controller;
    [SerializeField] GameObject playerCamera;

    [SerializeField] int HP;
    [SerializeField] int speed;

    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int stamina; //The amount of stamina the player has
    [SerializeField] int minStamina; //The lowest amount of stamina the player can have to sprint
    [SerializeField] float staminaGainMult; //The speed at which stamina is gained
    [SerializeField] float staminaLossMult; //The speed at which stamina is lost


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
        }
        else if (isSprinting && stamina != -1)
        {
            //If the player is sprinting, lose stamina
            exactStamina -= Time.deltaTime * staminaLossMult;
            stamina = (int)exactStamina;
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
        moveDirection = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(moveDirection * speed * Time.deltaTime);
        jump();
        controller.Move(playerVelocity * Time.deltaTime);
        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }
        if (Input.GetButtonDown("Zoom")) { 
            //Zoom in function
        } else if (Input.GetButtonUp("Zoom")) {
            //Zoom out function
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpcount < jumpMax)
        {
            jumpcount++;
            playerVelocity.y = jumpSpeed;
        }
    }
    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            if (stamina >= minStamina || stamina == -1) {
                speed *= sprintMod;
                isSprinting = true;
            }
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed = speedOriginal; //Changed the division here into a variable to decrease room for bugs!
            isSprinting = false;
        }
        if (stamina == 0) {
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
        /*
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());
        */
        if (HP <= 0)
        {
            gamemanager.instance.youLose();
        }
    }

    public void HealDamage(int amount, bool onCooldown)
    {
        if (onCooldown == false) {
            if (HP < HPOriginal) {
                HP += amount;
            }
        }
    }
}
