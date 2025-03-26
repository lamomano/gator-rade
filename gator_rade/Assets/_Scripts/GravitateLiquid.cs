using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitateLiquid : MonoBehaviour
{
    public GameObject target;
    public float moveSpeed;

    private void Update()
    {
        if (target != null)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
            transform.position = newPosition;
        }
    }
}
