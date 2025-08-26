using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class shopManager : MonoBehaviour
{
    public GameObject player;
    public GameObject shopPanel;
    public Button closeShopButton;
    public TextMeshProUGUI playerMoneyText;
    public IAllowPickup playerPickupInterface;

    public Transform weaponContainer;
    public GameObject weaponItemPrefab;
    public GameObject messagePanel;

    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponStatsText;
    public TextMeshProUGUI weaponPriceText;
    public TextMeshProUGUI messageText;
    public float messageDuration = 1.5f;
    public float fadeDuration = 0.5f;
    public Image weaponPreviewImage;
    public Button purchaseButton;
    public Button sellButton;


    public gunStats[] availableWeapons;

    public int playerLevel = 1;

    public gunStats selectedWeapon;
    public List<gunStats> ownedWeapons = new List<gunStats>();
    public List<GameObject> weaponUIItems = new List<GameObject>();

    CanvasGroup messageCanvasGroup;

    void Start()
    {
        closeShopButton.onClick.AddListener(closeShop);
        purchaseButton.onClick.AddListener(purchaseSelectedWeapon);
        sellButton.onClick.AddListener(sellSelectedWeapon);

        if (player != null)
        {
            playerPickupInterface = player.GetComponent<IAllowPickup>();
        }
        else
        {

            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerPickupInterface = player.GetComponent<IAllowPickup>();
        }

        shopPanel.SetActive(false);
        

        messageCanvasGroup = messagePanel.GetComponent<CanvasGroup>();
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        if (messageText != null)
            messageText.text = "";


        if (availableWeapons.Length > 0)
        {
            ownedWeapons.Add(availableWeapons[0]);

            if (playerPickupInterface != null)
            {
                gunStats startingWeapon = ScriptableObject.Instantiate(availableWeapons[0]);
                startingWeapon.ammoCur = startingWeapon.ammoMax;
                playerPickupInterface.GetGunStats(startingWeapon);
            }

        }
    }

    public void openShop()
    {
        gamemanager.instance.statePause();
        shopPanel.SetActive(true);
        PopulateWeaponList();
        ClearWeaponInfo();

    }

    public void closeShop()
    {
        gamemanager.instance.stateUnpause();
        shopPanel.SetActive(false);

        if (messageText != null)
            messageText.text = "";
    }

    void PopulateWeaponList()
    {

        foreach (GameObject item in weaponUIItems)
        {
            Destroy(item);
        }
        weaponUIItems.Clear();

        foreach (gunStats weapon in availableWeapons)
        {
            GameObject weaponItem = Instantiate(weaponItemPrefab, weaponContainer);
            weaponUIItems.Add(weaponItem);

            SetupWeaponItem(weaponItem, weapon);
        }
    }

    void SetupWeaponItem(GameObject weaponItem, gunStats weapon)
    {
        TextMeshProUGUI nameText = weaponItem.transform.Find("WeaponName").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI priceText = weaponItem.transform.Find("Price").GetComponent<TextMeshProUGUI>();
        Image rarityBorder = weaponItem.transform.Find("RarityBorder").GetComponent<Image>();
        Button selectButton = weaponItem.transform.Find("SelectButton").GetComponent<Button>();

        Image weaponIconImage = weaponItem.transform.Find("WeaponIcon")?.GetComponent<Image>();
        if (weaponIconImage != null && weapon.weaponIcon != null)
        {
            weaponIconImage.sprite = weapon.weaponIcon;
            weaponIconImage.gameObject.SetActive(true);
        }


        nameText.text = GetWeaponDisplayName(weapon);
        priceText.text = "$" + weapon.price.ToString();
        rarityBorder.color = weapon.GetRarityColor();

        bool isOwned = ownedWeapons.Contains(weapon);
        bool isUnlocked = playerLevel >= weapon.unlockLevel;

        if (isOwned)
        {
            priceText.text = "OWNED";
            priceText.color = Color.green;
        }
        else if (!isUnlocked)
        {
            priceText.text = "LOCKED (Lv." + weapon.unlockLevel + ")";
            priceText.color = Color.red;
            selectButton.interactable = false;
        }

        selectButton.onClick.RemoveAllListeners();

        selectButton.onClick.AddListener(() => SelectWeapon(weapon));
    }

    string GetWeaponDisplayName(gunStats weapon)
    {

        string rarityPrefix = "";
        switch (weapon.rarity)
        {
            case WeaponRarity.Rare: rarityPrefix = "Rare "; break;
            case WeaponRarity.Epic: rarityPrefix = "Epic "; break;
            case WeaponRarity.Legendary: rarityPrefix = "Legendary "; break;
        }

        return rarityPrefix + weapon.weaponType.ToString();
    }

    public void SelectWeapon(gunStats weapon)
    {
        selectedWeapon = weapon;
        DisplayWeaponInfo(weapon);
        UpdatePurchaseButton();
    }

    void DisplayWeaponInfo(gunStats weapon)
    {
        weaponNameText.text = GetWeaponDisplayName(weapon);
        weaponNameText.color = weapon.GetRarityColor();

        if (weaponPreviewImage != null && weapon.weaponImage != null)
        {
            weaponPreviewImage.sprite = weapon.weaponImage;
            weaponPreviewImage.gameObject.SetActive(true);
        }
        else if (weaponPreviewImage != null)
        {
            weaponPreviewImage.gameObject.SetActive(false);
        }

        weaponStatsText.text = $"Damage: {weapon.shootDamage}\n" +
                              $"Range: {weapon.shootDist}m\n" +
                              $"Fire Rate: {weapon.shootRate:F1}\n" +
                              $"Crit Chance: {weapon.critChance:F1}%\n" +
                              $"Ammo: {weapon.ammoMax}\n" +
                              $"Reload: {weapon.reloadTime:F1}s\n" +
                              $"Recoil: {weapon.recoil:F1}\n" +
                              $"Move Speed: {weapon.moveSpeed:F1}x";

        weaponPriceText.text = "Price: $" + weapon.price.ToString();
    }

    void UpdatePurchaseButton()
    {
        if (selectedWeapon == null) return;

        bool isOwned = ownedWeapons.Contains(selectedWeapon);
        bool canAfford = currencyManager.instance.GetMoney() >= selectedWeapon.price;
        bool isUnlocked = playerLevel >= selectedWeapon.unlockLevel;


        if (isOwned)
        {
            purchaseButton.gameObject.SetActive(false);
            sellButton.gameObject.SetActive(true);
        }
        else
        {
            purchaseButton.gameObject.SetActive(true);
            sellButton.gameObject.SetActive(false);
            purchaseButton.interactable = canAfford && isUnlocked;

            if (!isUnlocked)
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "LOCKED";
            else if (!canAfford)
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "CAN'T AFFORD";
            else
                purchaseButton.GetComponentInChildren<TextMeshProUGUI>().text = "PURCHASE";
        }
    }

    public void purchaseSelectedWeapon()
    {
        if (selectedWeapon == null) return;

        bool canAfford = currencyManager.instance.GetMoney() >= selectedWeapon.price;
        bool isUnlocked = playerLevel >= selectedWeapon.unlockLevel;
        bool isOwned = ownedWeapons.Contains(selectedWeapon);

        if (!isOwned && canAfford && isUnlocked)
        {

            currencyManager.instance.SpendMoney(selectedWeapon.price);
            ownedWeapons.Add(selectedWeapon);

            if (playerPickupInterface != null)
            {
                selectedWeapon.ammoCur = selectedWeapon.ammoMax;
                playerPickupInterface.GetGunStats(selectedWeapon);
                Debug.Log("Purchased and equipped: " + GetWeaponDisplayName(selectedWeapon));
            }
            else
            {
                Debug.LogWarning("Could not equip weapon - player pickup interface not found!");
            }


            
            PopulateWeaponList();
            UpdatePurchaseButton();

            Debug.Log("Purchased: " + GetWeaponDisplayName(selectedWeapon));
        }
    }

    public void sellSelectedWeapon()
    {
        if (selectedWeapon == null) return;

        bool isOwned = ownedWeapons.Contains(selectedWeapon);

        if (isOwned && ownedWeapons.Count > 1)
        {

            int sellValue = Mathf.RoundToInt(selectedWeapon.price * 0.6f);
            currencyManager.instance.AddMoney(sellValue);
            ownedWeapons.Remove(selectedWeapon);


            
            PopulateWeaponList();
            UpdatePurchaseButton();

            Debug.Log("Sold: " + GetWeaponDisplayName(selectedWeapon) + " for $" + sellValue);
        }
        else if (isOwned && ownedWeapons.Count <= 1)
        {
            ShowMessage("Cannot sell your only weapon!");
        }
    }


    void ShowMessage(string message)
    {
        if (messagePanel != null && messageText != null)
        {
            messageText.text = message;
            messagePanel.SetActive(true);

            if (messageCanvasGroup != null)
                messageCanvasGroup.alpha = 1f;

            StopAllCoroutines();
            StartCoroutine(FadeMessage());
        }
        Debug.Log("Showing message: " + message);
    }


    IEnumerator FadeMessage()
    {
        messageCanvasGroup.alpha = 1;

        yield return new WaitForSeconds(messageDuration);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            messageCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            yield return null;
        }

        messagePanel.SetActive(false);
    }



    void ClearWeaponInfo()
    {
        weaponNameText.text = "Select a weapon";
        weaponStatsText.text = "";
        weaponPriceText.text = "";
        purchaseButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
    }


    public List<gunStats> GetOwnedWeapons()
    {
        return ownedWeapons;
    }


    public void setPlayerLevel(int level)
    {
        playerLevel = level;
        if (shopPanel.activeInHierarchy)
        {
            PopulateWeaponList();
        }
    }



}
