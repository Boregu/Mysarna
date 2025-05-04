using UnityEngine;

public class CameraOrbitWithMovement : MonoBehaviour
{
    public Vector3 pivot = Vector3.zero;
    public float moveSpeed = 10f;
    public float verticalSpeed = 5f;
    public float rotationSpeed = 5f;
    public float scrollSpeed = 5f;
    public float minDistance = 2f;
    public float maxDistance = 50f;

    [Header("Keybinds")]
    public KeyCode moveUpKey = KeyCode.Space;
    public KeyCode moveDownKey = KeyCode.LeftShift;
    public KeyCode rotateKey = KeyCode.Mouse1; // Right mouse button

    private float yaw = 0f;
    private float pitch = 20f;
    private float distance = 10f;

    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
        UpdateCameraPosition();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S

        // Flattened movement directions
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 flatRight = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        Vector3 move = (flatRight * h + flatForward * v) * moveSpeed;

        float vertical = 0f;
        if (Input.GetKey(moveUpKey)) vertical += 1f;
        if (Input.GetKey(moveDownKey)) vertical -= 1f;

        pivot += (move + Vector3.up * vertical * verticalSpeed) * Time.deltaTime;
    }

    void HandleRotation()
    {
        if (Input.GetKey(rotateKey))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            pitch = Mathf.Clamp(pitch, -89f, 89f);
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * scrollSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    void UpdateCameraPosition()
    {
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        transform.position = pivot + offset;
        transform.LookAt(pivot);
    }
}
