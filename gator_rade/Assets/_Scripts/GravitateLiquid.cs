using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitateLiquid : MonoBehaviour
{
    private Rigidbody liquidRb;
    public GameObject target;
    public float moveSpeed;
    public float radius;
    public float power;


    private void Update()
    {
        if (target != null)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
            transform.position = newPosition;
        }
    }

    void MoveBack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in hitColliders)
        {
            if (liquidRb != null)
                liquidRb.AddExplosionForce(power, transform.position, radius);
        }
    }
}
