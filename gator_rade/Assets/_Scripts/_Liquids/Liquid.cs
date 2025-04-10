using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public enum LiquidType
{
    Gatorade,
    Peak,
    Powerade
}


public class Liquid : MonoBehaviour
{
    public LiquidType type = LiquidType.Gatorade;

    public Material gatorade;
    public Material peak;
    public Material powerade;


    public bool canConvert = false; // so peak doens't convert as soon as it spawns and touche something



    private MeshRenderer meshRenderer;
    private SphereCollider sphereCollider;
    private GameManager gameManager;

    

    private void Awake()
    {
        
        meshRenderer = GetComponent<MeshRenderer>();
        sphereCollider = GetComponent<SphereCollider>();

        // have to wait for gamemanager to load for some reason XDDDDDDDDD?
        StartCoroutine(DelayStart());
    }
    private IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(0.02f);
        gameManager = (GameManager)FindObjectOfType<GameManager>();

        UpdateState(type);
    }

    public void UpdateState(LiquidType targetState)
    {
        if (targetState == LiquidType.Gatorade)
            Gatorade();
        if (targetState == LiquidType.Peak)
            Peak();
        if (targetState == LiquidType.Powerade)
            Powerade();
    }


    /*
    private IEnumerator testLoop()
    {
        while (true)
        {

            int rng = Random.Range(1, 4);

            if (rng == 1)
                Gatorade();
            if (rng == 2)
                Powerade();
            if (rng == 3)
                Peak();

            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Start()
    {
        StartCoroutine(testLoop());
    }
    */

    public void Gatorade()
    {
        type = LiquidType.Gatorade;
        gameObject.tag = "Gatorade";
        meshRenderer.material = gatorade;

        var thisType =  gameManager.GetType();
        if (gameManager != null && thisType.GetMethod("RegisterGatorade") != null)
            gameManager.RegisterGatorade(gameObject);
        else
        {
            gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
            gameManager.RegisterGatorade(gameObject);
        }
           
    }

    public void Powerade()
    {
        type = LiquidType.Powerade;
        gameObject.tag = "Powerade";
        meshRenderer.material = powerade;

        gameManager.UnregisterGatorade(gameObject);
    }

    public void Peak()
    {
        type = LiquidType.Peak;
        gameObject.tag = "Peak";
        meshRenderer.material = peak;

        gameManager.UnregisterGatorade(gameObject);
        canConvert = false;
        StartCoroutine(DelayConversion());
    }


    /// <summary>
    /// called when peak is made,
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayConversion()
    {
        yield return new WaitForSeconds(1);
        canConvert = true;
    }


    /// <summary>
    /// this functions disable / enables interactivity of the liquid ball
    /// </summary>
    public void EnablePhysics(bool boolean)
    {
        meshRenderer.enabled = boolean;
        sphereCollider.enabled = boolean;
    }

    /// <summary>
    /// called when powerade touches it, very funny
    /// </summary>
    public void DeleteSelf()
    {
        EnablePhysics(false);
        
        gameManager.UnregisterGatorade(gameObject);
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Gatorade")
        {
            if (type == LiquidType.Peak)
            {
                if (canConvert)
                    collision.gameObject.GetComponent<Liquid>().Peak();
            }

            // powerade should delete both itself and the other liquid
            if (type == LiquidType.Powerade)
            {
                //collision.gameObject.GetComponent<Liquid>().Powerade();
                EnablePhysics(false);
                collision.gameObject.GetComponent<Liquid>().EnablePhysics(false);
            }
        }
    }
    */
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Gatorade")
        {
            if (type == LiquidType.Peak)
            {
                if (canConvert)
                    collision.gameObject.GetComponent<Liquid>().Peak();
            }

            // powerade should delete both itself and the other liquid
            if (type == LiquidType.Powerade)
            {
                //collision.gameObject.GetComponent<Liquid>().Powerade();
                DeleteSelf();
                collision.gameObject.GetComponent<Liquid>().DeleteSelf();
            }
        }
    }
}
