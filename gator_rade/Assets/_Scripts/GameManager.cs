using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
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



    public bool isPaused = false;


    public GameGrid gameGrid;
    public PlayerData playerData;
    public GatoradeSpewer spewer;
    public PlayerUI playerUI;
    public MoveTracker moveTracker;
    public Powerups powerups;


    private List<GameObject> gatoradeOrbs = new List<GameObject>();
    private List<GameObject> successfulOrbs = new List<GameObject>();
    private List<GameObject> outOfBoundsOrbs = new List<GameObject>();

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
    private Dictionary<GameObject, LiquidType> initialLiquidStates = new Dictionary<GameObject, LiquidType>();

    // Start is called before the first frame update
    void Start()
    {
        TouchSimulation.Enable();

        gameGrid = (GameGrid)FindObjectOfType(typeof(GameGrid));
        playerData = gameObject.GetComponent<PlayerData>();
        spewer = (GatoradeSpewer)FindObjectOfType<GatoradeSpewer>();
        playerUI = (PlayerUI)FindObjectOfType<PlayerUI>();
        moveTracker = (MoveTracker)FindObjectOfType<MoveTracker>();
        powerups = (Powerups)FindObjectOfType<Powerups>();

        //spewer.StartSpawning();
        //playerUI.UpdateUI();



        // get all balls in the scene and register them
        List<GameObject> liquidObjects = GetAllLiquidObjects();

        foreach (GameObject obj in liquidObjects)
        {
            Liquid liquid = obj.GetComponent<Liquid>();
            if (liquid.type == LiquidType.Gatorade)
            {
                RegisterGatorade(obj);
            }
            initialBallPositions[obj] = obj.transform.position;
            initialLiquidStates[obj] = liquid.type;
        }


        // hitbox for detecting when gatorade falls off the screen
        gameObject.transform.position = new Vector3(0, 0, 0);
        BoxCollider bottomBox = gameObject.AddComponent<BoxCollider>();
        bottomBox.size = new Vector3(10, 1, 50);
        bottomBox.center = new Vector3(0, -gameGrid.GetMaxPosY() - 5, 0);
        bottomBox.isTrigger = true;

        NewRound();



        /*
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
        if (!success)
            return;
        */
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
    /// returns a list of all the liquid gameobjects in the game
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetAllLiquidObjects()
    {
        Liquid[] liquidList = FindObjectsOfType<Liquid>();

        List<GameObject> gameObjectList = new List<GameObject>();

        foreach (Liquid liquid in liquidList)
        {
            gameObjectList.Add(liquid.gameObject);
        }
        return gameObjectList;
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
        foreach (GameObject obj in GetAllLiquidObjects())
        {
            if (!initialBallPositions.TryGetValue(obj, out Vector3 thisPos))
            {
                initialBallPositions.Remove(obj);
                gatoradeOrbs.Remove(obj);
                Destroy(obj);
            }
            else
            {
                obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                obj.transform.position = thisPos;
            }

            if (!initialLiquidStates.TryGetValue(obj, out LiquidType liquidType))
            {
                initialLiquidStates.Remove(obj);
                gatoradeOrbs.Remove(obj);
                Destroy(obj);
            }
            else
            {
                Liquid thisLiquid = obj.GetComponent<Liquid>();

                if (liquidType == LiquidType.Peak)
                {
                    thisLiquid.canConvert = false;
                }

                thisLiquid.UpdateState(liquidType);

                thisLiquid.EnablePhysics(true);
            }
        }
        gatoradeOrbs.Clear();

        gatoradeOrbs.AddRange(GameObject.FindGameObjectsWithTag("Gatorade"));

        successfulOrbs.Clear();
        outOfBoundsOrbs.Clear();


        // now just start periodically checking for orb moving status

        if (gameLoopThread != null)
            StopCoroutine(gameLoopThread);

        gameGrid.GenerateGrid();

        //MOVES_LEFT = amountOfMoves;

        //gameLoopThread = StartCoroutine(Init());
        playerUI.UpdateUI();
        moveTracker.ResetMoves();
        powerups.ResetPowerupUses();

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

        if (gatoradeCollected >= gatoradeNeeded)
        {
            print("You win! you had " + gatoradeCollected);
            //SceneManager.LoadScene("WinScene");
            playerUI.ShowWinScreen();
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
        foreach (GameObject obj in GetAllLiquidObjects())
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
        foreach (GameObject obj in GetAllLiquidObjects())
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

        objectVelocities.Clear();
        angularVelocities.Clear();
        Time.timeScale = 1;
    }


    public void Pause()
    {
        isPaused = true;
        FreezeAllLiquids();
    }

    public void Unpause()
    {
        isPaused = false;
        UnfreezeLiquids();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Gatorade")
        {
            if (!outOfBoundsOrbs.Contains(other.gameObject))
            {
                outOfBoundsOrbs.Add(other.gameObject);
            }

            print(totalGatorade - outOfBoundsOrbs.Count);
            if (totalGatorade - outOfBoundsOrbs.Count < gatoradeNeeded)
            {
                playerUI.ShowLoseScreen();
            }
        }
    }


    public void OnGUI()
    {
        /*
        if (GUILayout.Button("Check win"))
        {
            CheckForWin();
        }
        
        if (GUILayout.Button("New Game"))
        {
            NewRound();
        }
        
        if (GUILayout.Button("Generate Grid"))
        {
            gameGrid.GenerateGrid();
        }
        */
    }
}
