using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform cameraObj;
    [SerializeField] float movementSpeed = 1.0f;
    [SerializeField] float playerRotationSpeed = 3.0f;
    [SerializeField] Animator animator;

    Rigidbody rb;
    Vector2 playerInputs;

    public float Speed { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Application.targetFrameRate = 30;
        Time.fixedDeltaTime = 1.0f / 30.0f;
    }

    void Update()
    {
        playerInputs.x = Input.GetAxisRaw("Horizontal");
        playerInputs.y = Input.GetAxisRaw("Vertical");
        playerInputs = Vector2.ClampMagnitude(playerInputs, 1);
    }

    void FixedUpdate()
    {
        Vector3 camForward = cameraObj.forward;
        Vector3 camRight = cameraObj.right;
        camForward.y = 0;
        camRight.y = 0;

        Vector3 move = camRight * playerInputs.x + camForward * playerInputs.y;

        Vector3 v = rb.linearVelocity;
        v = new Vector3(move.x * movementSpeed, v.y, move.z * movementSpeed);
        rb.linearVelocity = v;

        Speed = move.magnitude;
        animator.SetFloat("Speed", Speed);

        if (move.sqrMagnitude > 0.001f)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, playerRotationSpeed * Time.fixedDeltaTime);
        }
    }
}
