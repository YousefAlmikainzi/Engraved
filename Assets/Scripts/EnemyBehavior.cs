using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] int enemyHealth = 3;
    [SerializeField] int givenExp = 2;
    
    int currentHealth;
    bool isDead = false;

    void Start()
    {
        currentHealth = enemyHealth;
    }

    public void TakeDamage(int damageAmount, PlayerBehavior player)
    {
        if(isDead)
        {
            return;
        }
        currentHealth -= damageAmount;
        if (currentHealth <= 0)
        {
            Death(player);
        }
    }
    void Death(PlayerBehavior player)
    {
        isDead = true;

        player.GainExp(givenExp);

        var col = GetComponent<Collider>();
        col.enabled = false;

        var rend = GetComponentInChildren<Renderer>();
        rend.enabled = false;

        Destroy(gameObject);
    }
}
