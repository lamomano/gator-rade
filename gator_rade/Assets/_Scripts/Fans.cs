using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fans : MonoBehaviour
{

    public float power = 5f;

    public int distance = 8;
    public bool isEnabled = false;



    private void Awake()
    {
        // apply distance
        //BoxCollider thisCollider = gameObject.AddComponent<BoxCollider>();
        //thisCollider.isTrigger = true;
        BoxCollider thisCollider = GetComponent<BoxCollider>();
        // adding +1 so that it covers itself as well
        thisCollider.size = new Vector3(0.95f, distance+1, 1);
        thisCollider.center = new Vector3(0, (distance / 2) , 0);
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
