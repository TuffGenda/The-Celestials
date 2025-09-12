using UnityEngine;
using TMPro;

public class currencyManager : MonoBehaviour
{
    public static currencyManager instance;

    [Header("Currency Settings")]
    [SerializeField] int startingMoney = 5000;
    [SerializeField] TextMeshProUGUI moneyText;

    int currentMoney;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        currentMoney = startingMoney;
        UpdateMoneyDisplay();
    }

    public int GetMoney()
    {
        return currentMoney;
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        UpdateMoneyDisplay();
    }

    public void SetMoney(int amount)
    {
        currentMoney = amount;
        UpdateMoneyDisplay();
    }

    public bool SpendMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyDisplay();
            return true;
        }
        return false;
    }

    void UpdateMoneyDisplay()
    {
        if (moneyText != null)
            moneyText.text = "Money: $" + currentMoney;
    }


}
