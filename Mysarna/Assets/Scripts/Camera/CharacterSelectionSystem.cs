using UnityEngine;

public class CharacterSelectionSystem : MonoBehaviour
{
    [Header("Selection Settings")]
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 1.5f;

    private GameObject selectedCharacter;
    private Camera mainCamera;
    private Material originalMaterial;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Left click to select character
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if we clicked on a character (has ClickableObject script)
                ClickableObject clickable = hit.collider.GetComponent<ClickableObject>();
                if (clickable != null)
                {
                    // Deselect previous character if any
                    if (selectedCharacter != null)
                    {
                        RemoveHighlight(selectedCharacter);
                    }

                    // Select new character
                    selectedCharacter = hit.collider.gameObject;
                    Debug.Log($"Selected character: {selectedCharacter.name}");
                    
                    // Add highlight to new selection
                    AddHighlight(selectedCharacter);
                }
            }
        }

        // Right click to move selected character
        if (Input.GetMouseButtonDown(1) && selectedCharacter != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Check if we clicked on the ground (has a collider)
                if (hit.collider.gameObject.CompareTag("Ground"))
                {
                    // Get the character's movement component
                    CharacterMovement movement = selectedCharacter.GetComponent<CharacterMovement>();
                    if (movement != null)
                    {
                        movement.MoveTo(hit.point);
                        Debug.Log($"Moving {selectedCharacter.name} to {hit.point}");
                    }
                }
            }
        }
    }

    private void AddHighlight(GameObject character)
    {
        Renderer renderer = character.GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material;
            
            // Create highlight material
            Material highlightMaterial = new Material(originalMaterial);
            highlightMaterial.color = highlightColor;
            highlightMaterial.SetColor("_EmissionColor", highlightColor * highlightIntensity);
            renderer.material = highlightMaterial;
        }
    }

    private void RemoveHighlight(GameObject character)
    {
        Renderer renderer = character.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = originalMaterial;
        }
    }
} 