using System.Collections.Generic;
using UnityEngine;

public class SelectSystem : MonoBehaviour
{
    [Header("Selection Settings")]
    public List<string> selectableTags = new List<string> { "Minion" };
    public string groundTag = "Ground";

    [Header("Selection Circle Settings")]
    public float selectionCircleRadius = 1.0f;
    public float selectionCircleWidth = 0.05f;
    public int selectionCircleSegments = 40;
    public Color selectionCircleColor = Color.yellow;
    public float selectionCircleYOffset = 0.0f;

    private List<GameObject> selectedObjects = new List<GameObject>();
    private Dictionary<GameObject, GameObject> selectionCircles = new Dictionary<GameObject, GameObject>();
    private Vector2 dragStartPos;
    private bool isDragging = false;
    private Rect selectionRect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Start drag or click
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = Input.mousePosition;
            isDragging = true;
        }

        // Dragging
        if (Input.GetMouseButton(0) && isDragging)
        {
            selectionRect = GetScreenRect(dragStartPos, Input.mousePosition);
        }

        // End drag or single click
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            if (Vector2.Distance(dragStartPos, Input.mousePosition) < 10f)
            {
                // Single click
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject clicked = hit.collider.gameObject;
                    string tag = clicked.tag;
                    Debug.Log($"Clicked object tag: {tag}");

                    if (selectableTags.Contains(tag))
                    {
                        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
                            ClearSelection();
                        SelectObject(clicked);
                    }
                    else if (tag == groundTag || !selectableTags.Contains(tag))
                    {
                        ClearSelection();
                    }
                }
                else
                {
                    ClearSelection();
                }
            }
            else
            {
                // Drag select
                SelectObjectsInRect(selectionRect);
            }
        }

        // Optional: live update selection circles if settings change in inspector
        #if UNITY_EDITOR
        if (Application.isEditor)
        {
            if (UnityEditor.EditorApplication.isPlaying)
            {
                ApplySelectionCircleSettingsToAll();
            }
        }
        #endif
    }

    void OnGUI()
    {
        if (isDragging)
        {
            DrawScreenRect(selectionRect, new Color(1, 1, 0, 0.25f));
            DrawScreenRectBorder(selectionRect, 2, Color.yellow);
        }
    }

    void SelectObject(GameObject obj)
    {
        if (!selectedObjects.Contains(obj))
        {
            selectedObjects.Add(obj);
            AddSelectionCircle(obj);
        }
    }

    void ClearSelection()
    {
        foreach (var obj in selectedObjects)
        {
            RemoveSelectionCircle(obj);
        }
        selectedObjects.Clear();
    }

    void SelectObjectsInRect(Rect rect)
    {
        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            ClearSelection();
        foreach (var obj in FindObjectsOfType<GameObject>())
        {
            if (!obj.activeInHierarchy) continue;
            if (!selectableTags.Contains(obj.tag)) continue;
            var renderer = obj.GetComponentInChildren<Renderer>();
            if (renderer == null) continue;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(obj.transform.position);
            screenPos.y = Screen.height - screenPos.y; // invert y for GUI
            if (rect.Contains(screenPos, true))
            {
                SelectObject(obj);
                Debug.Log($"Drag selected object tag: {obj.tag}");
            }
        }
    }

    void AddSelectionCircle(GameObject obj)
    {
        if (selectionCircles.ContainsKey(obj)) return;
        GameObject circleObj = new GameObject("SelectionCircle");
        circleObj.transform.SetParent(obj.transform);
        circleObj.transform.localPosition = new Vector3(0, selectionCircleYOffset, 0);
        circleObj.transform.localRotation = Quaternion.identity; // No rotation, always flat
        circleObj.layer = LayerMask.NameToLayer("Ignore Raycast"); // Prevent blocking clicks
        var sc = circleObj.AddComponent<SelectionCircle>();
        sc.UpdateCircle(selectionCircleRadius, selectionCircleWidth, selectionCircleSegments, selectionCircleColor);
        selectionCircles[obj] = circleObj;
    }

    void RemoveSelectionCircle(GameObject obj)
    {
        if (selectionCircles.ContainsKey(obj))
        {
            Destroy(selectionCircles[obj]);
            selectionCircles.Remove(obj);
        }
    }

    public void ApplySelectionCircleSettingsToAll()
    {
        foreach (var kvp in selectionCircles)
        {
            var sc = kvp.Value.GetComponent<SelectionCircle>();
            if (sc != null)
            {
                sc.UpdateCircle(selectionCircleRadius, selectionCircleWidth, selectionCircleSegments, selectionCircleColor);
                kvp.Value.transform.localPosition = new Vector3(0, selectionCircleYOffset, 0);
            }
        }
    }

    // --- Utility for drawing selection rectangle ---
    Rect GetScreenRect(Vector2 screenPosition1, Vector2 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector2.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector2.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    void DrawScreenRect(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        GUI.color = Color.white;
    }

    void DrawScreenRectBorder(Rect rect, float thickness, Color color)
    {
        // Top
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        DrawScreenRect(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        DrawScreenRect(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        DrawScreenRect(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public List<GameObject> GetSelectedObjects()
    {
        return selectedObjects;
    }
}
