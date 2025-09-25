using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;

public class playerController : MonoBehaviour, IAllowDamage, IAllowPickup
{


    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] CharacterController controller;
    [SerializeField] GameObject playerCamera;
    [Header("--- Health ---")]
    [SerializeField] int HP; //The current health of the player
    [SerializeField] AudioClip healSound; //The sound played when the player heals
    [Header("--- Movement ---")]
    [SerializeField] float speed; //The base speed of the player
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
    [SerializeField] float reloadTime; //The time it takes to reload the player's weapon
    [Header("--- Guns ---")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    [SerializeField] Transform gunModelPos;
    [SerializeField] bool reloadTimes;
    [SerializeField] AudioSource reloadSound;
    [SerializeField] AudioSource gunStereo;
    [SerializeField] bool melee;
    [SerializeField] float windup;
    [SerializeField] Transform raiseWindupPos;
    [SerializeField] Transform lowerWindupPos;
    [Header("--- Ally ---")]
    [SerializeField] GameObject waypointObj;
    [SerializeField] GameObject ally;
    [Header("--- Sound ---")]
    [SerializeField] public AudioSource plrSoundSource;
    [SerializeField] AudioClip footstepSound;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;
    [SerializeField] AudioClip painSound;
    [SerializeField] AudioClip meleeSound;
    [SerializeField] AudioClip gunPickupSound;
    [SerializeField] float stepRate;




    Vector3 moveDirection;
    Vector3 playerVelocity;
    Vector3 shootPosOrig;
    Quaternion shootRotOrig;

    float shootTimer;
    float exactStamina;
    bool inTransition = false;
    int jumpcount;
    int HPOriginal;
    int staminaOriginal;
    bool isSprinting = false;
    float speedOriginal;
    int gunListPos;
    bool reloadUI = false;
    bool notWinding = true;
    float stepTimer;
    float stepRateOrig;

    GameObject curGun;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speedOriginal = speed;
        stepRateOrig = stepRate;
        HPOriginal = HP;
        exactStamina = stamina;
        staminaOriginal = stamina;
        shootPosOrig = gunModelPos.localPosition;
        shootRotOrig = gunModelPos.localRotation;
        melee = true; //Default to melee until a gun is picked up
        gameData data = saveManager.instance.LoadGame();
        if (data != null && SceneManager.GetActiveScene().buildIndex != 0)
        {
            loadPlayerData(data);
        }
        spawnPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (controller.enabled)
        {
            movement();
            sprint();
        }

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
        if (reloadUI)
        {
            float curr = Mathf.MoveTowards(gamemanager.instance.reloadBar.fillAmount, 0, Time.deltaTime / reloadTime);
            gamemanager.instance.reloadBar.fillAmount = curr;
        }
        if (inTransition)
        {
            Color c = gamemanager.instance.fadeScreen.GetComponent<Image>().color;
            c.a = Mathf.MoveTowards(c.a, 1, Time.deltaTime / gamemanager.instance.fadeSpeed);

            gamemanager.instance.fadeScreen.GetComponent<Image>().color = c;
        }

        if (notWinding && gunList.Count > 0 && melee)
        {
            gunModelPos.localPosition = Vector3.MoveTowards(gunModelPos.localPosition, lowerWindupPos.localPosition, Time.deltaTime * 10);
            gunModelPos.localRotation = Quaternion.RotateTowards(gunModelPos.localRotation, lowerWindupPos.localRotation, Time.deltaTime * 400);
        } 
        else if (!notWinding && gunList.Count > 0 && melee)
        {
            gunModelPos.localPosition = Vector3.MoveTowards(gunModelPos.localPosition, raiseWindupPos.localPosition, Time.deltaTime / gunList[gunListPos].windup);
            gunModelPos.localRotation = Quaternion.RotateTowards(gunModelPos.localRotation, raiseWindupPos.localRotation, Time.deltaTime * 400);
        }
        

    }

    void movement()
    {
        shootTimer += Time.deltaTime;
        stepTimer += Time.deltaTime;
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
        if (Input.GetButton("Fire1") && (melee || (gunList.Count > 0 && gunList[gunListPos].ammoCur > 0)) && shootTimer >= shootRate)
        {
            shoot();
        }
        if (controller.isGrounded && moveDirection != Vector3.zero && stepTimer >= stepRate && (settingsManager.instance.GetAxis("Horizontal") != 0 || settingsManager.instance.GetAxis("Vertical") != 0))
        {
            stepTimer = 0;
            plrSoundSource.PlayOneShot(footstepSound);
        }
        if (Input.GetButtonDown("Zoom"))
        {
            playerCamera.GetComponent<cameraController>().ZoomIn();
        }
        else if (Input.GetButtonUp("Zoom"))
        {
            playerCamera.GetComponent<cameraController>().ZoomOut();
        }
        selectGun();
        reload();
        placeWaypoint();
    }

    void jump()
    {
        if (settingsManager.instance.GetKeyDown("Jump") && jumpcount < jumpMax)
        {
            jumpcount++;
            plrSoundSource.PlayOneShot(jumpSound);
            playerVelocity.y = jumpSpeed;
        }
    }
    void sprint()
    {
        if (settingsManager.instance.GetKeyDown("Sprint"))
        {
            if (stamina >= minStamina || stamina == -1)
            {
                stepRate /= sprintMod;
                speed *= sprintMod;
                isSprinting = true;
            }
        }
        else if (settingsManager.instance.GetKeyUp("Sprint"))
        {
            speed = speedOriginal; //Changed the division here into a variable to decrease room for bugs!
            stepRate = stepRateOrig;
            if (gunList.Count > 0)
            {
                speed = speedOriginal * gunList[gunListPos].moveSpeed;
            }
            isSprinting = false;
        }
        if (stamina == 0)
        {
            speed = speedOriginal; //Changed the division here into a variable to decrease room for bugs!
            stepRate = stepRateOrig;
            if (gunList.Count > 0)
            {
                speed = speedOriginal * gunList[gunListPos].moveSpeed;
            }
            isSprinting = false;
        }
    }

    void shoot()
    {
        if ((melee || (gunList.Count > 0)) && !reloadUI && gamemanager.instance.menuActive == null)
        {
            shootTimer = 0;
            if (!melee)
            {
                gunList[gunListPos].ammoCur--;
                updateAmmoUI();
                gunStereo.clip = gunList[gunListPos].shootSound[0];
                gunStereo.Play();
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
                {
                    //Debug.Log(hit.collider.name);
                    //Instantiate(gunList[gunListPos].hitEffect, hit.point, Quaternion.identity);

                    //Play Windup for melee

                    IAllowDamage dmg = hit.collider.GetComponent<IAllowDamage>();

                    if (dmg != null)
                    {
                        dmg.TakeDamage(shootDamage);
                    }

                }
            }
            else if (notWinding)
            {
                StartCoroutine(windupDebounce());
            }
        }
    }

    public void startTransition()
    {
        inTransition = true;
        gamemanager.instance.fadeScreen.SetActive(true);
        Color c = gamemanager.instance.fadeScreen.GetComponent<Image>().color;
        c.a = 1;
        controller.enabled = false;
    }

    void placeWaypoint()
    {
        if (Input.GetButtonDown("Waypoint"))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100, ~ignoreLayer))
            {
                if (gamemanager.instance.currentWaypoint != null)
                {
                    Destroy(gamemanager.instance.currentWaypoint);
                }
                gamemanager.instance.currentWaypoint = Instantiate(waypointObj, hit.point, transform.rotation);
            }
        }
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload") && gunList.Count > 0 && !reloadUI && !melee && gamemanager.instance.menuActive == null && gunList[gunListPos].ammoCur != gunList[gunListPos].ammoMax)
        {
            reloadSound.Play();
            if (reloadTimes)
            {
                gamemanager.instance.reloadBar.fillAmount = 1;
                reloadUI = true;
                StartCoroutine(reloadDebounce());

            }
            else
            {
                gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
                updateAmmoUI();
            }

        }
    }

    public void spawnPlayer()
    {
        gamemanager.instance.gameActionText.text = "";
        HP = HPOriginal;
        stamina = staminaOriginal;
        controller.enabled = false;
        if (gamemanager.instance.playerSpawnPOS != null)
        {
            transform.position = gamemanager.instance.playerSpawnPOS.transform.position;
        }
        controller.enabled = true;
        
        updateHealthUI();
        updateStaminaUI();
    }

    public void TakeDamage(int amount)
    {
        if (!inTransition)
        {
            HP -= amount;
            updateHealthUI();
            StartCoroutine(flashDamageScreen());
            plrSoundSource.PlayOneShot(painSound);
            if (HP <= 0)
            {
                if (gamemanager.instance != null)
                {
                    gamemanager.instance.youLose();
                }
            }
        }
    }
    public void loadPlayerData(gameData data)
    {
        if (data == null)
        {
            SceneManager.LoadScene(levelManager.instance.GetCurrentLevel());
            return;
        }
        //We dont have more levels, So I can't really do the level stuff yet.

        gunList = data.gunList;
        gunListPos = 0;
        currencyManager.instance.SetMoney(data.money);
        gamemanager.instance.readCollectibleData(data.collectibles);
        updateAmmoUI();
        updateHealthUI();
        updateStaminaUI();
        changeGun();

    }
    public gameData givePlayerData()
    {
        gameData data = new gameData();

        // I changed this since I edited it to add level. All I really did was change the name, sorry. - Tuff Genda
        data.level = levelManager.instance.GetCurrentLevel();
        data.collectibles = gamemanager.instance.sendCollectibleData();
        data.health = HP;
        data.gunList = gunList;
        data.money = currencyManager.instance.GetMoney();

        return data;
    }

    /*
     * public class gameData
{
    public int playerLevel; // as in unlocked levels (1-8 for what levels are unlocked. 1 is nothing, 8 is everything)
    public int health;
    public int stamina;
    public List<gunStats> gunList;
    public int money;
}
     */
    public void sendActionText(string text)
    {
        StartCoroutine(feedback(text));

    }

    IEnumerator feedback(string text)
    {
        gamemanager.instance.gameActionText.text = text;
        yield return new WaitForSeconds(2f);
        gamemanager.instance.gameActionText.text = "";
    }

    public void HealDamage(int amount, bool onCooldown)
    {
        if (onCooldown == false && HP < HPOriginal)
        {
            plrSoundSource.PlayOneShot(healSound);
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


    public void updateAmmoUI()
    {
        if (gamemanager.instance != null && gunList.Count > 0)
        {
            gamemanager.instance.ammoCountUI.enabled = !melee;
            gamemanager.instance.ammoCountUI.text = gunList[gunListPos].ammoCur + " / " + gunList[gunListPos].ammoMax;
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

    IEnumerator reloadDebounce()
    {


        yield return new WaitForSeconds(reloadTime);
        gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
        updateAmmoUI();
        reloadUI = false;
        gamemanager.instance.reloadBar.fillAmount = 0;
    }
    IEnumerator windupDebounce()
    {

        notWinding = false;
        //Play windup animation/sound here

        yield return new WaitForSeconds(windup);
        notWinding = true;
        plrSoundSource.PlayOneShot(meleeSound, 0.2f);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            //Play Windup for melee
            //Play punch animation/sound here
            

            IAllowDamage dmg = hit.collider.GetComponent<IAllowDamage>();
            if (gunList.Count > 0)
            {
                gunStereo.clip = gunList[gunListPos].missSound[0];
                gunStereo.Play();
            }

            if (dmg != null)
            {
                if (gunList.Count > 0)
                {
                    gunStereo.clip = gunList[gunListPos].hitSound[0];
                    gunStereo.Play();
                }

                dmg.TakeDamage(shootDamage);
            }


        }
    }

    public void GetGunStats(gunStats gun)
    {
        if (!gunList.Contains(gun))
        {
            gunList.Add(gun);
            gunListPos = gunList.Count - 1;
            gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
            plrSoundSource.PlayOneShot(gunPickupSound);
            changeGun();
        }
    }

    void changeGun()
    {
        // I added this if statement to make sure that loading works. - Tuff Genda
        if (gunList.Count > 0)
        {
            shootDamage = gunList[gunListPos].shootDamage;
            shootDist = gunList[gunListPos].shootDist;
            shootRate = gunList[gunListPos].shootRate;
            reloadTime = gunList[gunListPos].reloadTime;
            melee = gunList[gunListPos].Melee;
            windup = gunList[gunListPos].windup;
            if (melee)
            {
                gunModelPos.localPosition = lowerWindupPos.localPosition;
                gunModelPos.localRotation = lowerWindupPos.localRotation;
            }
            else
            {
                gunModelPos.localPosition = shootPosOrig;
                gunModelPos.localRotation = shootRotOrig;
            }
            speed = speedOriginal * gunList[gunListPos].moveSpeed;

            updateAmmoUI();
            gunStereo.volume = gunList[gunListPos].shootVol;
            if (curGun != null)
            {
                Destroy(curGun);
            }
            curGun = Instantiate(gunList[gunListPos].model, gunModelPos.position, gunModelPos.rotation, gunModelPos);
        }
        else
        {
            shootDamage = 1;
            shootDist = 5;
            shootRate = 1;
            reloadTime = 0;
            speed = 5;

            updateAmmoUI();
        }
    }



    public List<gunStats> getGunList()
    {
        return gunList;
    }
    public void removeGun(gunStats gun)
    {
        if (gunList.Count > 0)
        {
            gunListPos--;
            if (gunListPos < 0)
            {
                gunListPos = 0;
            }
            gunList.Remove(gun);
        }
    }


    void selectGun()
    {

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1 && !reloadUI && notWinding)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0 && !reloadUI && notWinding)
        {
            gunListPos--;
            changeGun();
        }
    }

    public void GetAllyStats(SurvivorStats survivorStats)
    {
    }
}
