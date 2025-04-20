using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidTeleport : MonoBehaviour
{
    public GameObject start;
    public GameObject end;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gatorade"))
        {
            if (other.GetComponent<Liquid>().type == LiquidType.Gatorade)
            {
                other.transform.position = end.transform.position;
            }
        }
    }
}
