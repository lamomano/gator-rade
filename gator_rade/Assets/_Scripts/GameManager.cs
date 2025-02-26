using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class GameManager : MonoBehaviour
{

    // how much gatorade that needs to be in the goal in order to win
    public float PERCENTAGE_TO_WIN = 0.5f;

    public float spawnRate = 1f;
    public int gatoradeAmount = 10;
    public float delay = 3f;
    public float shootingPower = 1f;


    public GameGrid gameGrid;
    public PlayerData playerData;
    public GatoradeSpewer spewer;
    public PlayerUI playerUI;

    private List<GameObject> gatoradeOrbs = new List<GameObject>();
    private List<GameObject> successfulOrbs = new List<GameObject>();

    public int gatoradeCollected
    {
        get { return successfulOrbs.Count; }
    }




    private Coroutine gameLoopThread;


    // Start is called before the first frame update
    void Start()
    {
        TouchSimulation.Enable();

        gameGrid = gameObject.GetComponent<GameGrid>();
        playerData = gameObject.GetComponent<PlayerData>();
        spewer = (GatoradeSpewer)FindObjectOfType<GatoradeSpewer>();
        playerUI = (PlayerUI)FindObjectOfType<PlayerUI>();

        spewer.StartSpawning();
        playerUI.UpdateUI();

        bool success = true;
        if (gameGrid == null)
        {
            print("missing game grid gg");
            success = false;
        }
        if (playerData == null)
        {
            print("missing playerdata gg");
            success = false;
        }
        if (spewer == null)
        {
            print("missing spewer gg");
            success = false;
        }
        if (!success)
            return;

    }


    /// <summary>
    /// adds the balls to the list so that they can be registered when counting all the balls that need to be counted
    /// </summary>
    /// <param name="obj"></param>
    public void RegisterGatorade(GameObject obj)
    {
        if (!gatoradeOrbs.Contains(obj))
        {
            //print("registered orb");

            gatoradeOrbs.Add(obj);
        }
    }

    public void UnregisterGatorade(GameObject obj)
    {
        if (gatoradeOrbs.Contains(obj))
            gatoradeOrbs.Remove(obj);

    }



    /// <summary>
    /// called when gatorade is collected : )
    /// goes towards calculating for the win condition
    /// </summary>
    public void CollectGatorade(GameObject obj)
    {
        if (!successfulOrbs.Contains(obj) && gatoradeOrbs.Contains(obj))
        {
            print("registered new ball");
            successfulOrbs.Add(obj);
            playerUI.UpdateUI();
        }
        
    }



    /// <summary>
    /// main gameloop that periodically calls GatoradeStillActive()
    /// </summary>
    /// <returns></returns>
    private IEnumerator Init()
    {
        while (GatoradeStillActive())
        {
            yield return new WaitForSeconds(1);
        }
        gameLoopThread = null;
        EndRound();
    }


    public void NewRound()
    {
        foreach (GameObject obj in gatoradeOrbs)
        {
            Destroy(obj);
        }
        gatoradeOrbs.Clear();
        successfulOrbs.Clear();

        // now just start periodically checking for orb moving status

        if (gameLoopThread != null)
            StopCoroutine(gameLoopThread);

        gameLoopThread = StartCoroutine(Init());
        playerUI.UpdateUI();
    }




    /// <summary>
    /// calculates the total orbs collected and figures out if the player won or not
    /// </summary>
    public void EndRound()
    {
        float collectedPercentage = gatoradeCollected / gatoradeAmount;

        if (collectedPercentage > PERCENTAGE_TO_WIN)
        {

            print("You win! you had " + gatoradeCollected);
        }
        else
        {

            print("you lose! you had " + gatoradeCollected);
        }
    }


    /// <summary>
    /// checks to see if the gatorade is still actively moving and are still within the boundaries of the grid
    /// </summary>
    public bool GatoradeStillActive()
    {
        foreach (GameObject obj in gatoradeOrbs)
        {
            Rigidbody thisRb = obj.GetComponent<Rigidbody>();
            float currentSpeed = thisRb.velocity.magnitude;

            // check if within playing field still
            if (obj.transform.position.y >= -gameGrid.GetMaxPosY())
            {
                // is within bounds

                // now check if moving (or moving a lot)
                if (currentSpeed < 0.25f)
                {
                    // not moving, return true
                    return true;
                }
            }
        }
        return false;
    }




    /// <summary>
    /// these functions freeze / unfreeze the orbs whenever called, very cool batman
    /// </summary>
    public void FreezeAllLiquids()
    {
        foreach (GameObject obj in gatoradeOrbs)
        {
            Rigidbody thisRb = obj.GetComponent<Rigidbody>();
            thisRb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void UnfreezeLiquids()
    {
        foreach (GameObject obj in gatoradeOrbs)
        {
            Rigidbody thisRb = obj.GetComponent<Rigidbody>();
            thisRb.constraints = RigidbodyConstraints.None;
            thisRb.constraints = RigidbodyConstraints.FreezePositionZ;

            thisRb.constraints = RigidbodyConstraints.FreezeRotationX;
            thisRb.constraints = RigidbodyConstraints.FreezeRotationY;
        }
    }


    public void OnGUI()
    {
        if (GUILayout.Button("Check win"))
        {
            EndRound();
        }
    }
}
