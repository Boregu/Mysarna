using UnityEngine;

public class SelectionCircle : MonoBehaviour
{
    private LineRenderer lr;
    private float lastRadius, lastWidth;
    private int lastSegments;
    private Color lastColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Awake()
    {
        lr = gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = false;
        lr.loop = true;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        // Default values
        UpdateCircle(1.0f, 0.05f, 40, Color.yellow);
    }

    public void UpdateCircle(float radius, float width, int segments, Color color)
    {
        if (lr == null) return;
        lr.widthMultiplier = width;
        lr.positionCount = segments;
        lr.startColor = color;
        lr.endColor = color;
        lastRadius = radius;
        lastWidth = width;
        lastSegments = segments;
        lastColor = color;
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, 0, z));
        }
    }

    // Optionally, call this if you want to update in real time
    public void Refresh()
    {
        UpdateCircle(lastRadius, lastWidth, lastSegments, lastColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
