using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Snake snake = other.GetComponent<Snake>();
            if (snake != null)
            {
                snake.ResetState();
            }
        }
        if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }

    }
}
