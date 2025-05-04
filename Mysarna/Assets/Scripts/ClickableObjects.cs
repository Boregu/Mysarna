using UnityEngine;

public class ClickableObject : MonoBehaviour
{
    public void OnClicked()
    {
        Debug.Log($"{name} was clicked!");
    }
}
