using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitateLiquid : MonoBehaviour
{
    private Rigidbody liquidRb;
    public GameObject target;
    public float moveSpeed;
    public float radius;
    public float deadZone;
    public float power;


    private void Update()
    {
        if (target != null)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
            transform.position = newPosition;
        }
        MoveBack();
    }
    /// <summary>
    /// for debug so you can see sphere radius
    /// </summary>
   /* void OnDrawGizmos()
    {
        Gizmos.color = Color.green; // Set the color of the sphere
        Gizmos.DrawWireSphere(transform.position, radius); // Draw the wireframe sphere
        Gizmos.DrawWireSphere(transform.position, deadZone); // Draw the wireframe sphere
    }
   */
   
    void MoveBack()
    {
        //sets target back to nothing if theres nothing in range 
        target = null;
        //checks if theres objects in the radius 
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in hitColliders)
        {
            //checks if the distance is smaller than the deadzone, sets the target to other object if not and if it has the liquid tag 
           if ((Vector3.Distance(collider.transform.position, transform.position) >= deadZone) && collider.CompareTag("Gatorade"))
           {
                target = collider.gameObject;
                break;
           }
        }
    }
}
