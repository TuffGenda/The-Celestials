using UnityEngine;

public class Shop : MonoBehaviour
{
    public shopManager shopManager;
    bool inTrigger;

    void Start()
    {
        
        if (shopManager == null)
        {
            shopManager = Object.FindFirstObjectByType<shopManager>();
            if (shopManager == null)
            {
                Debug.LogWarning("ShopManager not found in scene. Assign it in the Inspector.");
            }
        }
    }

    void Update()
    {
        if (inTrigger && Input.GetButtonDown("Interact"))
        {
            if (shopManager != null)
            {
                shopManager.openShopWithoutPause();

                
                if (gamemanager.instance?.buttonInteract != null)
                    gamemanager.instance.buttonInteract.SetActive(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            inTrigger = true;

            
            if (gamemanager.instance?.buttonInteract != null)
                gamemanager.instance.buttonInteract.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            inTrigger = false;

           
            if (gamemanager.instance?.buttonInteract != null)
                gamemanager.instance.buttonInteract.SetActive(false);
        }
    }
}
