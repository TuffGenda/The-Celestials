using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class shopManager : MonoBehaviour
{
    public GameObject shopPanel;
    public Button closeShopButton;
    public TextMeshProUGUI playerMoneyText;

    public Transform weaponContainer;
    public GameObject weaponItemPrefab;

    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI weaponStatsText;
    public TextMeshProUGUI weaponPriceText;
    public Button purchaseButton;
    public Button sellButton;
    public Button selectButton;

    public gunStats[] availableWeapons;

    public int playerMoney = 5000;
    public int playerLevel = 1;

    public gunStats selectedWeapon;
    public List<gunStats> ownedWeapons = new List<gunStats>();
    public List<GameObject> weaponUIItems = new List<GameObject>();

    void Start()
    {
        closeShopButton.onClick.AddListener(closeShop);
        purchaseButton.onClick.AddListener(purchaseSelectedWeapon);
        sellButton.onClick.AddListener(sellSelectedWeapon);

        shopPanel.SetActive(false);
        UpdateMoneyDisplay();


        if (availableWeapons.Length > 0)
        {
            ownedWeapons.Add(availableWeapons[0]); 
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
        bool canAfford = playerMoney >= selectedWeapon.price;
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

        bool canAfford = playerMoney >= selectedWeapon.price;
        bool isUnlocked = playerLevel >= selectedWeapon.unlockLevel;
        bool isOwned = ownedWeapons.Contains(selectedWeapon);

        if (!isOwned && canAfford && isUnlocked)
        {
            
            playerMoney -= selectedWeapon.price;
            ownedWeapons.Add(selectedWeapon);

            
            UpdateMoneyDisplay();
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
            playerMoney += sellValue;
            ownedWeapons.Remove(selectedWeapon);

            
            UpdateMoneyDisplay();
            PopulateWeaponList();
            UpdatePurchaseButton();

            Debug.Log("Sold: " + GetWeaponDisplayName(selectedWeapon) + " for $" + sellValue);
        }
    }

    void UpdateMoneyDisplay()
    {
        playerMoneyText.text = "Money: $" + playerMoney.ToString();
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

    public void addMoney(int amount)
    {
        playerMoney += amount;
        UpdateMoneyDisplay();
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
