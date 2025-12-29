using UnityEngine;

public class EnemyFollowP : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float detectionRange = 5f;
    [SerializeField] float stayAwayDistance = 2f;
    [SerializeField] float enemySpeed = 6f;
    [SerializeField] float lungeSpeed = 10f;
    [SerializeField] float lungeDistance = 5f;
    [SerializeField] float lungeCooldown = 2f;

    private bool lungeActive = false;
    private Vector3 lungeStartPos;
    private Vector3 lungeDirection;
    private float nextLungeTime = 0f;
    private float fixedY;

    void Start()
    {
        fixedY = transform.position.y;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (lungeActive)
        {
            Vector3 newPos = transform.position + lungeDirection * lungeSpeed * Time.deltaTime;
            newPos.y = fixedY;
            transform.position = newPos;

            if (Vector3.Distance(lungeStartPos, transform.position) >= lungeDistance)
            {
                lungeActive = false;
                nextLungeTime = Time.time + lungeCooldown;
            }
            return;
        }

        if (distanceToPlayer <= detectionRange)
        {
            Vector3 direction = (transform.position - player.position).normalized;

            if (distanceToPlayer <= stayAwayDistance && Time.time >= nextLungeTime)
            {
                lungeActive = true;
                lungeStartPos = transform.position;
                lungeDirection = -direction;
                transform.forward = lungeDirection;
                return;
            }

            if (distanceToPlayer != stayAwayDistance)
            {
                Vector3 target = player.position + direction * stayAwayDistance;
                target.y = fixedY;
                transform.position = Vector3.MoveTowards(transform.position, target, enemySpeed * Time.deltaTime);

                Vector3 lookDirection = (player.position - transform.position).normalized;
                lookDirection.y = 0;
                transform.forward = lookDirection;
            }
        }
    }
}
