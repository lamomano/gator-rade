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


    



    public MeshRenderer meshRenderer;
    public SphereCollider sphereCollider;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
        meshRenderer = GetComponent<MeshRenderer>();
        sphereCollider = GetComponent<SphereCollider>();

        if (type == LiquidType.Gatorade)
            Gatorade();
        if (type == LiquidType.Peak)
            Peak();
        if (type == LiquidType.Powerade)
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

        gameManager.RegisterGatorade(gameObject);
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
        EnablePhysics(true);
        
        gameManager.UnregisterGatorade(gameObject);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Gatorade")
        {
            if (type == LiquidType.Peak)
            {
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
}
