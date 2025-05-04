using UnityEngine;
using System.Collections.Generic;

public class CharacterSelectionSystem : MonoBehaviour
{
    [Header("Selection Settings")]
    public Color highlightColor = Color.yellow;
    public float highlightIntensity = 1.5f;

    [Header("Tag Settings")]
    public List<string> highlightableTags = new List<string>();
    public List<string> clickableTags = new List<string>();

    private List<GameObject> selectedObjects = new List<GameObject>();
    private Camera mainCamera;
    private Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();
    private Material highlightMaterial;

    void Start()
    {
        mainCamera = Camera.main;
        // Create a single highlight material instance
        highlightMaterial = new Material(Shader.Find("Standard"));
        highlightMaterial.color = highlightColor;
        highlightMaterial.EnableKeyword("_EMISSION");
        highlightMaterial.SetColor("_EmissionColor", highlightColor * highlightIntensity);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                string tag = hit.collider.gameObject.tag;
                bool isHighlightable = highlightableTags.Contains(tag);
                bool isClickable = clickableTags.Contains(tag);
                bool shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
                GameObject clickedObject = hit.collider.gameObject;

                if (!isHighlightable && !isClickable)
                {
                    DeselectAll();
                    return;
                }

                if (!shiftHeld)
                {
                    DeselectAll();
                }

                if (!selectedObjects.Contains(clickedObject))
                {
                    selectedObjects.Add(clickedObject);
                    if (isHighlightable)
                    {
                        AddHighlight(clickedObject);
                    }
                    // If just clickable, do nothing extra for now
                }
            }
        }
    }

    private void AddHighlight(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (!originalMaterials.ContainsKey(obj) && renderer.material != highlightMaterial)
            {
                originalMaterials[obj] = renderer.material;
            }
            renderer.material = highlightMaterial;
        }
    }

    private void RemoveHighlight(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null && originalMaterials.ContainsKey(obj))
        {
            renderer.material = originalMaterials[obj];
            originalMaterials.Remove(obj);
        }
    }

    private void DeselectAll()
    {
        foreach (var obj in new List<GameObject>(selectedObjects))
        {
            if (highlightableTags.Contains(obj.tag))
            {
                RemoveHighlight(obj);
            }
        }
        selectedObjects.Clear();
    }
} 