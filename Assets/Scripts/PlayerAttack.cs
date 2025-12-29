using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] Vector3 attackRange;
    [SerializeField] float attackRadius = 0.5f;
    [SerializeField] int attackDamage = 1;
    [SerializeField] float attackCoolDown = 1.0f;
    [SerializeField] LayerMask enemyLayer;
    [SerializeField] Animator animator;

    float attackTimer = 0f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= attackTimer)
        {
            animator.SetTrigger("Punch");

            Vector3 attackPoint = transform.position + transform.TransformDirection(attackRange);
            Collider[] hits = Physics.OverlapSphere(attackPoint, attackRadius, enemyLayer);

            foreach (Collider c in hits)
            {
                if (c.CompareTag("Enemy"))
                {
                    EnemyBehavior enemy = c.GetComponent<EnemyBehavior>();
                    enemy.TakeDamage(attackDamage, GetComponent<PlayerBehavior>());
                }
            }

            attackTimer = Time.time + attackCoolDown;
        }
    }

    public void IncreaseDamage(int amount)
    {
        attackDamage += amount;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 attackPoint = transform.position + transform.TransformDirection(attackRange);
        Gizmos.DrawWireSphere(attackPoint, attackRadius);
    }
}
