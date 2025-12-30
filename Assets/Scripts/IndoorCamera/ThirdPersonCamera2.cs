using UnityEngine;

public class ThirdPersonCamera2 : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 cameraOffset = new Vector3(0, 2, -4);
    [SerializeField] float cameraSensitivity = 3.0f;
    [SerializeField] float collisionRadius = 0.25f;
    [SerializeField] LayerMask collisionMask;

    float yaw;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * cameraSensitivity;

        Quaternion rotation = Quaternion.Euler(0, yaw, 0);

        Vector3 lookOrigin = player.position + Vector3.up * 1.6f;

        Vector3 desiredPosition = lookOrigin + rotation * cameraOffset;

        Vector3 direction = desiredPosition - lookOrigin;
        float distance = direction.magnitude;

        if (Physics.SphereCast(
            lookOrigin,
            collisionRadius,
            direction.normalized,
            out RaycastHit hit,
            distance,
            collisionMask))
        {
            transform.position = hit.point + hit.normal * collisionRadius;
        }
        else
        {
            transform.position = desiredPosition;
        }

        transform.LookAt(lookOrigin);
    }
}