using UnityEngine;
using UnityEngine.UI;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] int playerHealth = 20;
    [SerializeField] Slider healthSlider;

    [SerializeField] int expNeededToLevelUp = 30;
    [SerializeField] Slider expSlider;

    [SerializeField] int damageIncreasePerLevel = 1;

    [SerializeField] int damageTakenFromEnemies = 1;

    private int currentHealth;
    private int currentExp;
    private int playerLevel = 1;
    void Start()
    {
        currentHealth = playerHealth;
        healthSlider.maxValue = currentHealth;
        healthSlider.value = currentHealth;

        currentExp = 0;
        expSlider.maxValue = expNeededToLevelUp;
        expSlider.value = currentExp;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            currentHealth -= damageTakenFromEnemies;
            currentHealth = Mathf.Max(0, currentHealth);
            healthSlider.value = currentHealth;
        }
    }
    public void GainExp(int expAmount)
    {
        currentExp += expAmount;

        if (currentExp >= expNeededToLevelUp)
        {
            currentExp = 0;
            PlayerLevelUp();
        }
        expSlider.maxValue = expNeededToLevelUp;
        expSlider.value = currentExp;
    }
    void PlayerLevelUp()
    {
        playerLevel++;
        playerHealth += 2;
        currentHealth = playerHealth;
        expNeededToLevelUp += 4;
        UpdateHealthUI();

        var attack = GetComponent<PlayerAttack>();
        attack.IncreaseDamage(damageIncreasePerLevel);
    }
    void UpdateHealthUI()
    {
        healthSlider.maxValue = playerHealth;
        healthSlider.value = currentHealth;
    }
}