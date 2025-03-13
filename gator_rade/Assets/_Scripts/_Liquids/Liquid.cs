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

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

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
    }

    public void Powerade()
    {
        type = LiquidType.Powerade;
        gameObject.tag = "Powerade";
        meshRenderer.material = powerade;
    }

    public void Peak()
    {
        type = LiquidType.Peak;
        gameObject.tag = "Peak";
        meshRenderer.material = peak;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Gatorade")
        {
            if (type != LiquidType.Gatorade)
            {
                if (type == LiquidType.Peak)
                {
                    collision.gameObject.GetComponent<Liquid>().Peak();
                }
                if (type == LiquidType.Powerade)
                {
                    collision.gameObject.GetComponent<Liquid>().Powerade();
                }
            }
        }
    }
}
