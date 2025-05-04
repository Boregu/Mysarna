using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stoppingDistance = 0.1f;
    public float rotationSpeed = 10f;

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Update()
    {
        if (isMoving)
        {
            // Calculate direction to target
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; // Keep movement on the XZ plane

            // Check if we've reached the target
            if (direction.magnitude <= stoppingDistance)
            {
                isMoving = false;
                return;
            }

            // Rotate towards target
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Move towards target
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    public void MoveTo(Vector3 position)
    {
        targetPosition = position;
        targetPosition.y = transform.position.y; // Keep the same Y position
        isMoving = true;
    }
} 