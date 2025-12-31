using System.Collections;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] int enemyHealth = 3;
    [SerializeField] int givenExp = 2;
    [SerializeField] int Score = 1;
    [SerializeField] float hitBlinkDuration = 0.08f;
    [SerializeField] Renderer enemyRenderer;

    int currentHealth;
    bool isDead = false;
    MaterialPropertyBlock mpb;
    Color originalColor = Color.white;
    Coroutine blinkCoroutine;

    void Start()
    {
        currentHealth = enemyHealth;
        if (enemyRenderer == null)
            enemyRenderer = GetComponentInChildren<Renderer>();

        if (enemyRenderer != null)
        {
            mpb = new MaterialPropertyBlock();
            if (enemyRenderer.sharedMaterial != null)
            {
                if (enemyRenderer.sharedMaterial.HasProperty("_Color"))
                    originalColor = enemyRenderer.sharedMaterial.GetColor("_Color");
                else if (enemyRenderer.sharedMaterial.HasProperty("_BaseColor"))
                    originalColor = enemyRenderer.sharedMaterial.GetColor("_BaseColor");
            }
        }
    }

    public void TakeDamage(int damageAmount, PlayerBehavior player)
    {
        if (isDead) return;

        currentHealth -= damageAmount;

        if (enemyRenderer != null)
        {
            if (blinkCoroutine != null) StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(HitBlink());
        }

        if (currentHealth <= 0) Death(player);
    }

    IEnumerator HitBlink()
    {
        enemyRenderer.GetPropertyBlock(mpb);

        if (enemyRenderer.sharedMaterial != null && enemyRenderer.sharedMaterial.HasProperty("_Color"))
            mpb.SetColor("_Color", Color.red);
        else if (enemyRenderer.sharedMaterial != null && enemyRenderer.sharedMaterial.HasProperty("_BaseColor"))
            mpb.SetColor("_BaseColor", Color.red);
        else
            mpb.SetColor("_Color", Color.red);

        enemyRenderer.SetPropertyBlock(mpb);

        yield return new WaitForSeconds(hitBlinkDuration);

        enemyRenderer.GetPropertyBlock(mpb);

        if (enemyRenderer.sharedMaterial != null && enemyRenderer.sharedMaterial.HasProperty("_Color"))
            mpb.SetColor("_Color", originalColor);
        else if (enemyRenderer.sharedMaterial != null && enemyRenderer.sharedMaterial.HasProperty("_BaseColor"))
            mpb.SetColor("_BaseColor", originalColor);
        else
            mpb.SetColor("_Color", originalColor);

        enemyRenderer.SetPropertyBlock(mpb);
        blinkCoroutine = null;
    }

    void Death(PlayerBehavior player)
    {
        isDead = true;
        player.GainExp(givenExp);
        ScoreManager.Instance.AddScore(Score);

        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (enemyRenderer != null) enemyRenderer.enabled = false;

        Destroy(gameObject);
    }
}
