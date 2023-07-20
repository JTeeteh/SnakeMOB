using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private enum SnakeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    private SnakeDirection _direction = SnakeDirection.Right;
    private List<Transform> _segments = new List<Transform>();
    public Transform segmentPrefab;
    public Transform tailPrefab;
    public Transform snakeHead;

    public int initialSize = 4;
    public int foodScore = 10;
    public ScoreManager scoreManager;
    public GameObject foodPrefab; // New field for the food prefab
    public GameObject spawnArea; // Game object with the BoxCollider2D for spawning area
    public GameOverScreen gameOverScreen; // Reference to the GameOverScreen script
    private bool isGameOver = false; // Add a flag to track game over state

    // Swipe control variables
    private Vector2 _touchStartPos;
    public float _minSwipeDistance = 50f;

    private void Start()
    {
        gameOverScreen.gameObject.SetActive(false); // Make sure Game Over screen is initially inactive
        ResetState();
    }

    private void Update()
    {
        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if the touch phase is Began
            if (touch.phase == TouchPhase.Began)
            {
                // Store the initial touch position
                _touchStartPos = touch.position;
            }
            // Check if the touch phase is Ended
            else if (touch.phase == TouchPhase.Ended)
            {
                // Determine the swipe direction based on the initial and end touch positions
                Vector2 swipeDirection = touch.position - _touchStartPos;

                // Check if the swipe distance exceeds the minimum swipe distance
                if (swipeDirection.magnitude >= _minSwipeDistance)
                {
                    // Normalize the swipe direction to prevent the snake from moving too fast
                    swipeDirection.Normalize();

                    // Check the direction of the swipe and update the snake direction accordingly
                    if (Mathf.Abs(swipeDirection.x) > Mathf.Abs(swipeDirection.y))
                    {
                        // Swipe is horizontal
                        if (swipeDirection.x > 0 && _direction != SnakeDirection.Left)
                        {
                            _direction = SnakeDirection.Right;
                        }
                        else if (swipeDirection.x < 0 && _direction != SnakeDirection.Right)
                        {
                            _direction = SnakeDirection.Left;
                        }
                    }
                    else
                    {
                        // Swipe is vertical
                        if (swipeDirection.y > 0 && _direction != SnakeDirection.Down)
                        {
                            _direction = SnakeDirection.Up;
                        }
                        else if (swipeDirection.y < 0 && _direction != SnakeDirection.Up)
                        {
                            _direction = SnakeDirection.Down;
                        }
                    }
                }
            }
        }

        // For debugging on PC, keep the WASD controls
        if (Input.GetKeyDown(KeyCode.W) && _direction != SnakeDirection.Down)
        {
            _direction = SnakeDirection.Up;
        }
        else if (Input.GetKeyDown(KeyCode.S) && _direction != SnakeDirection.Up)
        {
            _direction = SnakeDirection.Down;
        }
        else if (Input.GetKeyDown(KeyCode.A) && _direction != SnakeDirection.Right)
        {
            _direction = SnakeDirection.Left;
        }
        else if (Input.GetKeyDown(KeyCode.D) && _direction != SnakeDirection.Left)
        {
            _direction = SnakeDirection.Right;
        }

        UpdateHeadRotation();
    }

    private void FixedUpdate()
    {
        if (isGameOver) return; // Stop updating the snake if the game is already over

        for (int i = _segments.Count - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
            _segments[i].rotation = _segments[i - 1].rotation;
        }

        snakeHead.position = new Vector3(
            Mathf.Round(snakeHead.position.x) + GetDirectionVector().x,
            Mathf.Round(snakeHead.position.y) + GetDirectionVector().y,
            0.0f
        );

        snakeHead.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(GetDirectionVector().y, GetDirectionVector().x) * Mathf.Rad2Deg);
    }

    private void Grow()
    {
        Transform segment;

        Vector3 tailPosition = _segments[_segments.Count - 1].position;
        Quaternion tailRotation = _segments[_segments.Count - 1].rotation;

        Destroy(_segments[_segments.Count - 1].gameObject);
        _segments.RemoveAt(_segments.Count - 1);

        segment = Instantiate(segmentPrefab, tailPosition, tailRotation);

        _segments.Add(segment);

        Transform newTail = Instantiate(tailPrefab, tailPosition, tailRotation);

        _segments.Add(newTail);

        // Spawn a new food using the food prefab and spawn area
        SpawnFood();
    }

    public void ResetState()
    {
        for (int i = 1; i < _segments.Count; i++)
        {
            Destroy(_segments[i].gameObject);
        }

        _segments.Clear();
        _segments.Add(snakeHead);

        for (int i = 1; i < initialSize; i++)
        {
            _segments.Add(Instantiate(segmentPrefab));
        }

        Transform tail = Instantiate(tailPrefab);
        tail.position = _segments[0].position;
        tail.rotation = _segments[0].rotation;

        _segments.Add(tail);

        snakeHead.position = Vector3.zero;
        snakeHead.rotation = Quaternion.identity;

        isGameOver = false; // Reset the game over flag
        enabled = true; // Enable the snake movement
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isGameOver) return; // Ignore collisions if the game is already over

        if (other.CompareTag("Food"))
        {
            Destroy(other.gameObject);
            Grow();
            scoreManager.IncrementScore(foodScore);
        }
        else if (other.CompareTag("Obstacle"))
        {
            GameOver();
        }
        else if (other.CompareTag("Bullet"))
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        // Trigger game over event or show the game over screen
        int finalScore = scoreManager.GetScore();
        gameOverScreen.Setup(finalScore); // Show the Game Over screen and pass the final score
        isGameOver = true; // Set the game over flag to true

        // Stop the snake from moving by disabling the FixedUpdate method
        enabled = false;
    }

    private void UpdateHeadRotation()
    {
        Vector2 direction = GetDirectionVector();

        if (direction == Vector2.up)
        {
            snakeHead.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector2.down)
        {
            snakeHead.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (direction == Vector2.left)
        {
            snakeHead.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == Vector2.right)
        {
            snakeHead.rotation = Quaternion.Euler(0, 0, -90);
        }
    }

    private Vector2 GetDirectionVector()
    {
        switch (_direction)
        {
            case SnakeDirection.Up:
                return Vector2.up;
            case SnakeDirection.Down:
                return Vector2.down;
            case SnakeDirection.Left:
                return Vector2.left;
            case SnakeDirection.Right:
                return Vector2.right;
            default:
                return Vector2.right;
        }
    }

    private void SpawnFood()
    {
        if (spawnArea != null)
        {
            BoxCollider2D spawnCollider = spawnArea.GetComponent<BoxCollider2D>();

            float minX = spawnCollider.bounds.min.x;
            float maxX = spawnCollider.bounds.max.x;
            float minY = spawnCollider.bounds.min.y;
            float maxY = spawnCollider.bounds.max.y;

            // Define a maximum number of attempts to prevent an infinite loop
            int maxAttempts = 100;
            int attemptCount = 0;

            Vector2 randomPosition;

            // Keep generating a random position until a suitable position is found
            do
            {
                randomPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
                attemptCount++;

                // Check if the random position is inside any obstacles
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(randomPosition, 0.1f); // Adjust the radius as needed

                bool isPositionValid = true;

                foreach (Collider2D collider in hitColliders)
                {
                    if (collider.CompareTag("Obstacle"))
                    {
                        // The random position is inside an obstacle, mark it as invalid
                        isPositionValid = false;
                        break;
                    }
                }

                // If the position is valid or the number of attempts exceeds the maximum, break the loop
                if (isPositionValid || attemptCount >= maxAttempts)
                {
                    break;
                }

            } while (true);

            // Instantiate the food at the final random position
            Instantiate(foodPrefab, randomPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Spawn area not assigned. Cannot spawn food.");
        }
    }
}
