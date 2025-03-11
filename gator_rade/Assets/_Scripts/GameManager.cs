using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // how much gatorade that needs to be in the goal in order to win
    public float PERCENTAGE_TO_WIN = 0.5f;

    public float spawnRate = 1f;
    public int gatoradeAmount = 10;
    public int amountOfMoves = 10;
    public float delay = 3f;
    public float shootingPower = 1f;


    


    public GameGrid gameGrid;
    public PlayerData playerData;
    public GatoradeSpewer spewer;
    public PlayerUI playerUI;
    public MoveTracker moveTracker;


    private List<GameObject> gatoradeOrbs = new List<GameObject>();
    private List<GameObject> successfulOrbs = new List<GameObject>();

    public float gatoradeCollected
    {
        get { return successfulOrbs.Count; }
    }

    public float totalGatorade
    {
        get { return gatoradeOrbs.Count; }
    }

    public int gatoradeNeeded
    {
        get { return (int)Mathf.RoundToInt(PERCENTAGE_TO_WIN * totalGatorade); }
    }



    private Coroutine gameLoopThread;


    private Dictionary<GameObject, Vector3> initialBallPositions = new Dictionary<GameObject, Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        TouchSimulation.Enable();

        gameGrid = (GameGrid)FindObjectOfType(typeof(GameGrid));
        playerData = gameObject.GetComponent<PlayerData>();
        spewer = (GatoradeSpewer)FindObjectOfType<GatoradeSpewer>();
        playerUI = (PlayerUI)FindObjectOfType<PlayerUI>();
        moveTracker = (MoveTracker)FindObjectOfType<MoveTracker>();

        //spewer.StartSpawning();
        //playerUI.UpdateUI();

        // get all balls in the scene and register them
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag("Gatorade");
        foreach (GameObject obj in objectsWithTag)
        {
            RegisterGatorade(obj);
            initialBallPositions[obj] = obj.transform.position;
        }

        NewRound();


        

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
            playerUI.UpdateUI();
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
            //print("registered new ball");
            successfulOrbs.Add(obj);
            playerUI.UpdateUI();
        }
        CheckForWin();

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
        CheckForWin();
    }


    public void NewRound()
    {
        foreach (GameObject obj in gatoradeOrbs)
        {
            if (!initialBallPositions.TryGetValue(obj, out Vector3 thisPos))
            {
                gatoradeOrbs.Remove(obj);
                Destroy(obj);
            }
            else
            {
                obj.transform.position = thisPos;
            }
                
        }
        //gatoradeOrbs.Clear();
        successfulOrbs.Clear();
        

        // now just start periodically checking for orb moving status

        if (gameLoopThread != null)
            StopCoroutine(gameLoopThread);

        gameGrid.GenerateGrid();

        //MOVES_LEFT = amountOfMoves;

        //gameLoopThread = StartCoroutine(Init());
        playerUI.UpdateUI();
        moveTracker.ResetMoves();

        if (spewer != null)
        {
            //spewer.CancelInvoke();
            //spewer.StartSpawning();
        }
    }




    /// <summary>
    /// calculates the total orbs collected and figures out if the player won or not
    /// </summary>
    public void CheckForWin()
    {
        
        if (gatoradeCollected > gatoradeNeeded)
        {
            print("You win! you had " + gatoradeCollected);
            SceneManager.LoadScene(3);
        }
        else
        {
            //print("you lose! you had " + gatoradeCollected);
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




    // for maintaining velocity when unfrozen
    // i love roblox dictionaries god bless them
    private Dictionary<GameObject, Vector3> objectVelocities = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Vector3> angularVelocities = new Dictionary<GameObject, Vector3>();

    /// <summary>
    /// these functions freeze / unfreeze the orbs whenever called, very cool batman
    /// </summary>
    public void FreezeAllLiquids()
    {
        foreach (GameObject obj in gatoradeOrbs)
        {
            Rigidbody thisRb = obj.GetComponent<Rigidbody>();

            objectVelocities[obj] = thisRb.velocity;
            angularVelocities[obj] = thisRb.angularVelocity;
            thisRb.constraints = RigidbodyConstraints.FreezeAll;
            thisRb.velocity = Vector3.zero;
        }
        Time.timeScale = 0;
    }

    public void UnfreezeLiquids()
    {
        foreach (GameObject obj in gatoradeOrbs)
        {
            Rigidbody thisRb = obj.GetComponent<Rigidbody>();

            thisRb.constraints = RigidbodyConstraints.FreezePositionZ;


            // new dictionary tech, wow!
            if (objectVelocities.TryGetValue(obj, out Vector3 regularVelocity))
            {
                thisRb.velocity = regularVelocity;
            }
            if (angularVelocities.TryGetValue(obj, out Vector3 angularVelocity))
            {
                thisRb.angularVelocity = angularVelocity;
            }
        }
        Time.timeScale = 1;
    }


    public void OnGUI()
    {
        if (GUILayout.Button("Check win"))
        {
            CheckForWin();
        }
        /*
        if (GUILayout.Button("New Game"))
        {
            NewRound();
        }
        */
        if (GUILayout.Button("Generate Grid"))
        {
            gameGrid.GenerateGrid();
        }
    }
}
