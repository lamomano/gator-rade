using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
/* Iversen-Krampitz, Ian 
 * 4/20/2025
 * Controls teleporting the liquid/gatorcam.
 */
public class LiquidTeleport : MonoBehaviour
{
    public GameObject Camera;
    public GameObject start;
    public GameObject end;
    public PlayerUI playerUI;
    private float waitTime;
    private bool isActive = false;

    private void Start()
    {
        waitTime = 2f;
        //StartCoroutine(StartDelay());
        playerUI = FindObjectOfType<PlayerUI>();
    }
    private void Update()
    {
        if (playerUI.winCanvas.enabled)
        {
            Camera.SetActive(true);
        }
        else
        {
            Camera.SetActive(isActive);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Gatorade"))
        {
            if (other.GetComponent<Liquid>().type == LiquidType.Gatorade)
            {
                //print("calling coroutines");
                StopAllCoroutines();
                StartCoroutine(BallLogic());
                other.transform.position = end.transform.position;
                other.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }
    public IEnumerator StartDelay()
    {
       //print("started delay");
        isActive = true;
        yield return new WaitForSeconds(waitTime);
        isActive = false;
    }

    /// <summary>
    /// fuck
    /// </summary>
    /// <returns></returns>
    public IEnumerator BallLogic()
    {
       //print("started ball logic");
        isActive = true;
        float startTime = Time.time; 
        while (startTime + waitTime >= Time.time)
        {
            yield return null;
        }
        //print("loop has ended");
        isActive = false;
    }
}
