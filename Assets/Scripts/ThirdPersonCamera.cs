using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 cameraOffset = new Vector3(0, 2, -4);
    [SerializeField] float cameraSensitivity = 3.0f;

    float yaw;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;    
    }
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * cameraSensitivity;

        Quaternion rotation = Quaternion.Euler(0, yaw, 0);
        transform.position = player.position + rotation * cameraOffset;
        transform.LookAt(player.position);
    }
}
