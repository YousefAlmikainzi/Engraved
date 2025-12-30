using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] int enemyHealth = 3;
    [SerializeField] int givenExp = 2;
    [SerializeField] int Score = 1;

    int currentHealth;
    bool isDead = false;

    void Start()
    {
        currentHealth = enemyHealth;
    }

    public void TakeDamage(int damageAmount, PlayerBehavior player)
    {
        if (isDead) return;

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

        ScoreManager.Instance.AddScore(Score);

        GetComponent<Collider>().enabled = false;
        GetComponentInChildren<Renderer>().enabled = false;

        Destroy(gameObject);
    }
}
