using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float moveRange = 5f;
    public float moveSpeed = 2f;
    public float moveInterval = 2f;

    public GameObject bulletPrefab;
    public float bulletSpeed = 5f;
    public float shootInterval = 2f;

    public GameObject spawnArea; // Reference to the spawn area game object

    private float moveTimer;
    private float moveDirection;
    private float shootTimer;
    private Vector3 spawnPosition;

    private void Start()
    {
        moveTimer = moveInterval;
        moveDirection = Random.Range(-1f, 1f);
        shootTimer = shootInterval;
        spawnPosition = transform.position; // Store the initial spawn position
    }

    private void Update()
    {
        MoveSideways();
        ShootBullet();
    }

    private void MoveSideways()
    {
        moveTimer -= Time.deltaTime;

        if (moveTimer <= 0f)
        {
            moveDirection = -moveDirection;
            moveTimer = moveInterval;

            // Rotate the game object when the food moves left or right
            if (moveDirection > 0)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f); // Rotate to the right (original orientation)
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f); // Rotate to the left (180 degrees)
            }
        }

        Vector3 newPosition = transform.position + new Vector3(moveDirection * moveSpeed * Time.deltaTime, 0f, 0f);
        newPosition.x = Mathf.Clamp(newPosition.x, spawnPosition.x - moveRange, spawnPosition.x + moveRange);
        transform.position = newPosition;
    }

    private void ShootBullet()
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            // Set the bullet's movement direction (up, down, left, right) randomly
            Vector2 direction = GetRandomDirection();
            bulletRb.velocity = direction * bulletSpeed;

            // Update the bullet's rotation to match its movement direction
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            shootTimer = shootInterval;
        }
    }

    private Vector2 GetRandomDirection()
    {
        int randomDirection = Random.Range(0, 4);
        switch (randomDirection)
        {
            case 0: // Up
                return Vector2.up;
            case 1: // Down
                return Vector2.down;
            case 2: // Left
                return Vector2.left;
            case 3: // Right
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
            SpawnNewFood();
        }
    }

    private void SpawnNewFood()
    {
        GameObject food = Instantiate(gameObject, GetRandomSpawnPosition(), Quaternion.identity);
        // Optionally, you can apply any additional customization or behavior to the spawned food object.
    }

    private Vector3 GetRandomSpawnPosition()
    {
        BoxCollider2D spawnCollider = spawnArea.GetComponent<BoxCollider2D>();

        // Get the bounds of the spawn area's box collider
        float minX = spawnCollider.bounds.min.x;
        float maxX = spawnCollider.bounds.max.x;
        float minY = spawnCollider.bounds.min.y;
        float maxY = spawnCollider.bounds.max.y;

        int maxAttempts = 100; // Maximum attempts to find a valid position
        int currentAttempts = 0;

        while (currentAttempts < maxAttempts)
        {
            // Generate random coordinates within the spawn area's bounds
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);

            Vector2 randomPosition = new Vector2(x, y);

            Collider2D[] obstacleColliders = Physics2D.OverlapPointAll(randomPosition);
            bool intersectsObstacle = false;

            foreach (Collider2D collider in obstacleColliders)
            {
                if (collider.CompareTag("Obstacle"))
                {
                    intersectsObstacle = true;
                    break;
                }
            }

            if (!intersectsObstacle)
            {
                return randomPosition;
            }

            currentAttempts++;
        }

        Debug.LogWarning("Unable to find a valid spawn position after maximum attempts.");

        // Return the default position as a fallback
        return new Vector3(minX, minY, 0f);
    }
}
