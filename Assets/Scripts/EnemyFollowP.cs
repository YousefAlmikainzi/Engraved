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
                lungeActive = true;
                lungeStartPos = transform.position;
                lungeDirection = dirToPlayer;
                transform.forward = new Vector3(lungeDirection.x, 0f, lungeDirection.z);
                return;
            }

            if (Mathf.Abs(distanceToPlayer - stayAwayDistance) > EPS)
            {
                Vector3 target = player.position - dirToPlayer * stayAwayDistance;
                target.y = fixedY;
                transform.position = Vector3.MoveTowards(transform.position, target, enemySpeed * Time.deltaTime);

                Vector3 lookDirection = new Vector3(dirToPlayer.x, 0, dirToPlayer.z);
                if (lookDirection.sqrMagnitude > 0.0001f)
                    transform.forward = lookDirection;
            }
        }
    }
}
