using TMPro;
using UnityEngine;

public class PlayerEconomy : MonoBehaviour
{
    [Header("Settings")]
    public int currentMoney = 100;
    public int maxMoney = 9999;

    [Header("UI")]
    public TextMeshProUGUI moneyText;

    void Start()
    {
        UpdateMoneyUI();    
    }

    public void AddMoney(int amount)
    {
        currentMoney += amount;
        if(currentMoney > maxMoney) currentMoney = maxMoney;
        UpdateMoneyUI();
    }

    public bool TrySpendMoney(int amount)
    {
        if(currentMoney >= amount)
        {
            currentMoney -= amount;
            UpdateMoneyUI();
            return true;
        }
        else
        {
            Debug.Log("Nie masz wystarczaj¹co pieniêdzy");
            return false;
        }
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = "$" + currentMoney;
        }
    }
}
