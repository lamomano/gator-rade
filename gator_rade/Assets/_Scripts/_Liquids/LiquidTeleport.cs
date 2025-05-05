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
    public Animator teleportAnim; //reference to animator for Alligator
    public Eyes eyes;
    public InputHandler inputHandler;
    private float waitTime;
    public float timeSinceTouch;
    public bool isActive = false;

    private void Start()
    {
        waitTime = 4f;
        StartCoroutine(StartDelay());
        playerUI = FindObjectOfType<PlayerUI>();
        inputHandler = FindObjectOfType<InputHandler>();
    }
    private void Update()
    {
        timeSinceTouch += Time.deltaTime;

        if (timeSinceTouch >= Time.deltaTime + 10f)
        {
            isActive = true;
        }

        if (playerUI.winCanvas.enabled)
        {
            isActive = true;
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
                if (!teleportAnim.GetBool("Happy"))
                {
                    eyes.isHappy = true;
                }
                    
            }
        }
    }
    public void StartBallLogic()
    {
        StopAllCoroutines();
        StartCoroutine(BallLogic());
    }

    public IEnumerator StartDelay()
    {
       //print("started delay");
        isActive = true;
        yield return new WaitForSeconds(waitTime);
        isActive = false;
    }

    public void ResetWaitDelay()
    {
        timeSinceTouch = 0;
        isActive = false;
    }

    /// <summary>
    /// fucks
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
