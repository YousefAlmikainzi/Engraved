using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] int playerHealth = 20;
    [SerializeField] Slider healthSlider;

    [SerializeField] int expNeededToLevelUp = 30;
    [SerializeField] Slider expSlider;

    [SerializeField] int damageIncreasePerLevel = 1;
    [SerializeField] int damageTakenFromEnemies = 1;

    [SerializeField] int nextHPIncrease = 2;
    [SerializeField] int nextEXPIncrease = 2;

    [SerializeField] Renderer[] skinRenderers;
    [SerializeField] float hitBlinkDuration = 0.08f;
    [SerializeField] Color hitColor = Color.red;

    int currentHealth;
    int currentExp;
    int playerLevel = 1;

    MaterialPropertyBlock mpb;
    Color[] originalColors;
    Coroutine blinkCoroutine;

    void Start()
    {
        currentHealth = playerHealth;
        healthSlider.maxValue = currentHealth;
        healthSlider.value = currentHealth;

        currentExp = 0;
        expSlider.maxValue = expNeededToLevelUp;
        expSlider.value = currentExp;

        mpb = new MaterialPropertyBlock();
        if (skinRenderers != null && skinRenderers.Length > 0)
        {
            originalColors = new Color[skinRenderers.Length];
            for (int i = 0; i < skinRenderers.Length; i++)
            {
                var r = skinRenderers[i];
                if (r != null && r.sharedMaterial != null && r.sharedMaterial.HasProperty("_Color"))
                    originalColors[i] = r.sharedMaterial.GetColor("_Color");
                else
                    originalColors[i] = Color.white;
                r.GetPropertyBlock(mpb);
                mpb.SetColor("_Color", originalColors[i]);
                r.SetPropertyBlock(mpb);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            TakeDamage(damageTakenFromEnemies);
        }
    }

    void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        healthSlider.value = currentHealth;

        if (skinRenderers != null && skinRenderers.Length > 0)
        {
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkAll());
        }

        if (currentHealth <= 0) Die();
    }

    IEnumerator BlinkAll()
    {
        for (int i = 0; i < skinRenderers.Length; i++)
        {
            var r = skinRenderers[i];
            if (r == null) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", hitColor);
            r.SetPropertyBlock(mpb);
        }

        yield return new WaitForSeconds(hitBlinkDuration);

        for (int i = 0; i < skinRenderers.Length; i++)
        {
            var r = skinRenderers[i];
            if (r == null) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", originalColors != null && i < originalColors.Length ? originalColors[i] : Color.white);
            r.SetPropertyBlock(mpb);
        }

        blinkCoroutine = null;
    }

    void Die()
    {
        SceneManager.LoadScene(3);
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
        playerHealth += nextHPIncrease;
        currentHealth = playerHealth;
        expNeededToLevelUp += nextEXPIncrease;
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
