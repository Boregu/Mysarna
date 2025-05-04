using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stoppingDistance = 0.1f;
    public float rotationSpeed = 10f;

    private NavMeshAgent agent;
    private bool isMoving = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
            agent.angularSpeed = rotationSpeed * 100f; // Convert to degrees per second
        }
    }

    void Update()
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            isMoving = agent.velocity.magnitude > 0.1f;
        }
    }

    public void MoveTo(Vector3 position)
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(position);
            isMoving = true;
        }
    }
} 