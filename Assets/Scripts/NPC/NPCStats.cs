using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCStats : MonoBehaviour
{
    [Header("Health Settings")]
    public string characterName = "NPC";
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public TextMeshProUGUI nameText;
    public Slider healthSlider;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (nameText != null)
            nameText.text = characterName;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    void Die()
    {
        gameObject.SetActive(false); 
    }
}