using UnityEngine;
using System.Collections.Generic;

public class MoveIndicator : MonoBehaviour
{
    public Transform unit;
    public Vector3 destination;
    public float circleRadius = 0.5f;
    public float circleWidth = 0.05f;
    public int circleSegments = 32;
    public Color color = Color.cyan;
    public float stopDistance = 0.2f;

    private LineRenderer line;
    private GameObject circleObj;

    // Track one indicator per unit
    private static Dictionary<Transform, MoveIndicator> activeIndicators = new Dictionary<Transform, MoveIndicator>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Remove any previous indicator for this unit
        if (unit != null && activeIndicators.ContainsKey(unit))
        {
            Destroy(activeIndicators[unit].gameObject);
            activeIndicators.Remove(unit);
        }
        if (unit != null)
            activeIndicators[unit] = this;

        // Line
        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = color;
        line.endColor = color;
        line.widthMultiplier = 0.05f;

        // Circle
        circleObj = new GameObject("MoveCircle");
        circleObj.transform.position = destination + Vector3.up * 0.05f;
        circleObj.transform.SetParent(transform);
        var circle = circleObj.AddComponent<LineRenderer>();
        circle.useWorldSpace = false;
        circle.loop = true;
        circle.positionCount = circleSegments;
        circle.material = new Material(Shader.Find("Sprites/Default"));
        circle.startColor = color;
        circle.endColor = color;
        circle.widthMultiplier = circleWidth;
        for (int i = 0; i < circleSegments; i++)
        {
            float angle = 2 * Mathf.PI * i / circleSegments;
            float x = Mathf.Cos(angle) * circleRadius;
            float z = Mathf.Sin(angle) * circleRadius;
            circle.SetPosition(i, new Vector3(x, 0, z));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Update line
        if (unit != null)
        {
            var renderer = unit.GetComponentInChildren<Renderer>();
            float baseY = renderer ? renderer.bounds.min.y : unit.position.y;
            Vector3 basePos = new Vector3(unit.position.x, baseY, unit.position.z);
            line.SetPosition(0, basePos);
            line.SetPosition(1, destination + Vector3.up * 0.1f);
        }

        // Rotate circle
        if (circleObj != null)
        {
            circleObj.transform.Rotate(Vector3.up, 180 * Time.deltaTime, Space.World);
        }

        bool arrived = false;
        if (unit != null)
        {
            var agent = unit.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null && agent.enabled)
            {
                if (!agent.pathPending && agent.remainingDistance <= stopDistance)
                    arrived = true;
            }
            else
            {
                if (Vector3.Distance(unit.position, destination) < stopDistance)
                    arrived = true;
            }
        }
        else
        {
            arrived = true;
        }

        if (arrived)
        {
            if (unit != null && activeIndicators.ContainsKey(unit) && activeIndicators[unit] == this)
                activeIndicators.Remove(unit);
            Destroy(gameObject);
        }
    }
}
