using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour
{


    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] CharacterController controller;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    Vector3 moveDirection;
    Vector3 playerVelocity;

    float shootTimer;

    int jumpcount;
    int HPOriginal;
    bool isSprinting = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOriginal = HP;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        movement();
        sprint();
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
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }
}
