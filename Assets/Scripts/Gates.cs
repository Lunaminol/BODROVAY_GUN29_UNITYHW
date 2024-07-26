using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour
{
    private int score = 0;

    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();

        if (ball != null)
        {
            score++;

            Debug.Log("Current Score: " + score);

            Destroy(other.gameObject);
        }
    }
}
