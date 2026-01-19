using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI")]
    public TextMeshProUGUI healthText;
    public GameObject gameOverPanel;

    [Header("Scripts")]
    public PlayerMovement playerMovement;
    public Weapon weaponScript;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Heal(10);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthUI();
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "¯ycie: " + (int)currentHealth;

            if (currentHealth <= 20)
            {
                healthText.color = Color.red;
            }
            else
            {
                healthText.color = Color.white;
            }
        }
    }

    private void Die()
    {
        print("Gracz nie ¿yje!");

        gameOverPanel.SetActive(true);
        if (playerMovement != null) playerMovement.enabled = false;
        if (weaponScript != null) weaponScript.enabled = false;
    }
}