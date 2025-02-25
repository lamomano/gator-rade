using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bathtub : MonoBehaviour
{



    private GameManager gameManager;
    private void Awake()
    {
        gameManager = (GameManager)FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Gatorade")
        {
            gameManager.CollectGatorade(other.gameObject);
        }
    }
}
