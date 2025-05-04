using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    //Place this script on any object that you want to be able to click and it will work. The click script is on the CameraRig and this is the receiver

    public void OnClicked()
    {
        Debug.Log($"{name} was clicked! Doing something...");
        // Place your logic here (e.g. open UI, activate effect, move, etc.)
    }
}
