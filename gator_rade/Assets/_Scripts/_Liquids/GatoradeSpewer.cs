using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/*
 * Author: [Nguyen, Kanyon]
 * Last Updated: [02/21/2025]
 * [spawns gatorade orbs]
 */
public class GatoradeSpewer : MonoBehaviour
{
    public GameObject gatoradePrefab;

    private GameManager gameManager;

    public int currentAmount;



    private void Awake()
    {
        gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
        currentAmount = gameManager.gatoradeAmount;
    }

    





    /// <summary>
    /// called by another script in order to start spawning the gator orbs
    /// </summary>
    public void StartSpawning()
    {
        InvokeRepeating("SpawnGatorade", gameManager.delay, gameManager.spawnRate);
    }

    private void SpawnGatorade()
    {
        if (currentAmount > 0)
        {
            GameObject thisGatorade = Instantiate(gatoradePrefab);
            thisGatorade.transform.position = gameObject.transform.position;
            
            gameManager.RegisterGatorade(thisGatorade);

            Rigidbody thisRb = thisGatorade.GetComponent<Rigidbody>();
            thisRb.AddForce(gameObject.transform.up * gameManager.shootingPower, ForceMode.Impulse);


            currentAmount--;
        }
        else
        { 
            CancelInvoke();
        }
            
    }


}
