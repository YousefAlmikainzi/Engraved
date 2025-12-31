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
    [SerializeField] float preLungeTime = 0.25f;
    [SerializeField] float shakeAmount = 0.08f;

    bool preLunge = false;
    bool lungeActive = false;

    Vector3 lungeStartPos;
    Vector3 lungeDirection;
    Vector3 shakeOrigin;

    float preLungeTimer = 0f;
    float nextLungeTime = 0f;
    float fixedY;

    const float EPS = 0.05f;

    void Start()
    {
        fixedY = transform.position.y;
    }

    void Update()
    {
        Vector3 toPlayer = player.position - transform.position;
        Vector3 toPlayerXZ = new Vector3(toPlayer.x, 0f, toPlayer.z);
        float distanceToPlayer = toPlayerXZ.magnitude;

        if (preLunge)
        {
            preLungeTimer -= Time.deltaTime;

            Vector3 shake = Random.insideUnitSphere * shakeAmount;
            shake.y = 0f;
            transform.position = shakeOrigin + shake;

            if (preLungeTimer <= 0f)
            {
                preLunge = false;
                lungeActive = true;
                lungeStartPos = transform.position;
            }
            return;
        }

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
            Vector3 dirToPlayer = toPlayerXZ.normalized;

            if (distanceToPlayer <= stayAwayDistance + EPS && Time.time >= nextLungeTime)
            {
                preLunge = true;
                preLungeTimer = preLungeTime;
                shakeOrigin = transform.position;
                lungeDirection = dirToPlayer;
                transform.forward = new Vector3(dirToPlayer.x, 0f, dirToPlayer.z);
                return;
            }

            if (Mathf.Abs(distanceToPlayer - stayAwayDistance) > EPS)
            {
                Vector3 target = player.position - dirToPlayer * stayAwayDistance;
                target.y = fixedY;
                transform.position = Vector3.MoveTowards(transform.position, target, enemySpeed * Time.deltaTime);

                Vector3 lookDirection = new Vector3(dirToPlayer.x, 0f, dirToPlayer.z);
                if (lookDirection.sqrMagnitude > 0.0001f)
                    transform.forward = lookDirection;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stayAwayDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lungeDistance);
    }
}
