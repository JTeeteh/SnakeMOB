using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public GameObject foodPrefab;
    public GameObject spawnArea;
    public int maxFoodCount = 2;
    public float spawnInterval = 5;

    private List<GameObject> _currentFood = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnFoodRoutine());
    }

    private IEnumerator SpawnFoodRoutine()
    {
        while (true)
        {
            if (_currentFood.Count < maxFoodCount)
            {
                Vector2 randomPosition = GetRandomPosition();
                GameObject food = Instantiate(foodPrefab, randomPosition, Quaternion.identity);
                _currentFood.Add(food);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector2 GetRandomPosition()
    {
        if (spawnArea != null)
        {
            BoxCollider2D spawnCollider = spawnArea.GetComponent<BoxCollider2D>();

            float minX = spawnCollider.bounds.min.x;
            float maxX = spawnCollider.bounds.max.x;
            float minY = spawnCollider.bounds.min.y;
            float maxY = spawnCollider.bounds.max.y;

            int maxAttempts = 100;
            int currentAttempts = 0;

            while (currentAttempts < maxAttempts)
            {
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
        }

        float defaultX = Mathf.Round(Random.Range(-8, 8));
        float defaultY = Mathf.Round(Random.Range(-4, 4));

        return new Vector2(defaultX, defaultY);
    }

    public void SpawnFood()
    {
        if (_currentFood.Count < maxFoodCount)
        {
            Vector2 randomPosition = GetRandomPosition();
            GameObject food = Instantiate(foodPrefab, randomPosition, Quaternion.identity);
            _currentFood.Add(food);
        }
    }
}
