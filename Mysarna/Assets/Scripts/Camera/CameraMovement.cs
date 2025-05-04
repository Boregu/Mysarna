using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbitWithMovement : MonoBehaviour
{
    public Vector3 pivot = Vector3.zero;
    public float moveSpeed = 10f;
    public float verticalSpeed = 5f;
    public float rotationSpeed = 5f;
    public float scrollSpeed = 5f;
    public float minDistance = 2f;
    public float maxDistance = 50f;

    private float yaw = 0f;
    private float pitch = 20f;
    private float distance = 10f;

    private PlayerControls controls;
    private Vector2 moveInput;
    private float verticalInput;
    private Vector2 lookInput;
    private float zoomInput;

    private bool rotating = false;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Camera.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Camera.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Camera.VerticalMove.performed += ctx => verticalInput = ctx.ReadValue<float>();
        controls.Camera.VerticalMove.canceled += _ => verticalInput = 0f;

        controls.Camera.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        controls.Camera.Look.canceled += _ => lookInput = Vector2.zero;

        controls.Camera.Zoom.performed += ctx => zoomInput = ctx.ReadValue<float>();
        controls.Camera.Zoom.canceled += _ => zoomInput = 0f;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    void Update()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            rotating = true;
            HandleRotation();
        }
        else
        {
            rotating = false;
        }

        HandleMovement();
        HandleZoom();
        UpdateCameraPosition();
    }

    void HandleMovement()
    {
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 flatRight = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        Vector3 move = (flatRight * moveInput.x + flatForward * moveInput.y) * moveSpeed;
        Vector3 vertical = Vector3.up * verticalInput * verticalSpeed;

        pivot += (move + vertical) * Time.deltaTime;
    }

    void HandleRotation()
    {
        yaw += lookInput.x * rotationSpeed * Time.deltaTime;
        pitch -= lookInput.y * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -89f, 89f);
    }

    void HandleZoom()
    {
        distance -= zoomInput * scrollSpeed;
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
