using UnityEngine;
using UnityEngine.InputSystem;

public class CameraClicker : MonoBehaviour
{
    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Camera.Click.performed += ctx => TryClick();
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void TryClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clickedObject = hit.collider.gameObject;
            clickedObject.SendMessage("OnClicked", SendMessageOptions.DontRequireReceiver);
        }
    }
}
