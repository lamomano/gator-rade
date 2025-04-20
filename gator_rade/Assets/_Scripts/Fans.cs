using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fans : MonoBehaviour
{

    public float power = 5f;

    private int maxDistance;
    public bool isEnabled = false;

    private BoxCollider thisCollider;
    private ParticleSystem ps;
    private GameGrid gameGrid;

    public LayerMask tileLayer;



    private void Awake()
    {
        // apply distance
        //BoxCollider thisCollider = gameObject.AddComponent<BoxCollider>();
        //thisCollider.isTrigger = true;
        gameGrid = (GameGrid)FindObjectOfType(typeof(GameGrid));
        ps = transform.Find("Particle System").gameObject.GetComponent<ParticleSystem>();

        thisCollider = GetComponent<BoxCollider>();
        maxDistance = gameGrid.gridSizeX > gameGrid.gridSizeY ? gameGrid.gridSizeX : gameGrid.gridSizeY;

        UpdateAoe();
    }


    /// <summary>
    /// shoots a raycast and changes the size of the boxcollider accordingly
    /// </summary>
    public void UpdateAoe()
    {
        
        Vector3 direction = transform.up;
        Vector3 origin = transform.position - (transform.up * 0.5f);



        RaycastHit hit;
        // this either equals the max distance or the distance returned by the raycast
        float newDistance = maxDistance;

        if (Physics.Raycast(origin, direction, out hit, maxDistance, tileLayer))
        {
            // calculate new distance based off of transform.position to hit.point
            newDistance = Vector3.Distance(transform.position, hit.point);
        }

        // adding +1 so that it covers its own tile space as well
        thisCollider.size = new Vector3(0.95f, newDistance + 1, 1);
        thisCollider.center = new Vector3(0, (newDistance / 2), 0);



        // --- particle system ---

        var main = ps.main;
        // only update if distance is significant
        if (Mathf.Abs(main.duration - newDistance) > 0.01f)
        {
            // fully stop and clear
            //ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            main.startLifetime = newDistance/2;

            ps.Play();
        }
        //Debug.DrawRay(origin, direction * newDistance, Color.red, 1.0f);
    }




    private void OnTriggerStay(Collider other)
    {
        if (isEnabled)
        {
            if (other.tag == "Gatorade" || other.tag == "Peak" || other.tag == "Powerade")
            {
                //print("movin these liquids");
                Rigidbody thisRb = other.GetComponent<Rigidbody>();
                thisRb.AddForce(transform.up.normalized * power, ForceMode.Force);

            }
        }
    }

}
