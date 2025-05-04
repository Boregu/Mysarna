using UnityEngine;

public class CameraClicker : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject clickedObject = hit.collider.gameObject;
                Debug.Log("Clicked on: " + clickedObject.name);

                // Call a method on the clicked object (if it has one)
                clickedObject.SendMessage("OnClicked", SendMessageOptions.DontRequireReceiver);
            }
        }
    }
}
