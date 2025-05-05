using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Iversen-Krampitz, Ian 
 * 4/26/2025
 * Controls the alligators animations
 */

public class Eyes : MonoBehaviour
{
    public SkinnedMeshRenderer eyeRenderer;
    public Animator animator;
    private float sadTime = 1.417f;
    private float happyTime = 1f;
    public bool isHappy;
    public bool isSad;
    public bool isBlinking;
    public bool canBlink;

   
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        canBlink = true;
    }

    // Update is called once per frame
    void Update()
    {
        //if not blinking and can blink, start random blink timer
        if (canBlink)
        {
            //Debug.Log("blinking start");
            animator.SetBool("Idle", true);
            StartCoroutine(Blink(Random.Range(.01f, 2f)));
        }
        if (isSad)
        {
            isSad = false;
            animator.SetBool("Sad", true);
            //Debug.Log("start sad");
            StopAllCoroutines();
            StartCoroutine(Sad());
        } 
        if (isHappy)
        {
            isHappy = false;
            animator.SetBool("Happy", true);
            StopAllCoroutines();
            StartCoroutine(Happy());
        }
    }

    //do blink animation (this code is disgusting)
    private IEnumerator Blink(float blinkTime)
    {
        canBlink = false;
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.4f, 0));
        yield return new WaitForSeconds(blinkTime);
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.22222f, 0));
        yield return new WaitForSeconds(.08f);
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.11111f, 0));
        yield return new WaitForSeconds(.08f);
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.995f, 0));
        yield return new WaitForSeconds(.08f);
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.9f, 0));
        yield return new WaitForSeconds(.08f);
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.995f, 0));
        yield return new WaitForSeconds(.08f);
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.11111f, 0));
        yield return new WaitForSeconds(.08f);
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.22222f, 0));
        yield return new WaitForSeconds(.08f);
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.4f, 0));
        yield return new WaitForSeconds(.08f);
        //Debug.Log("done waiting");
        canBlink = true;
    }

    private IEnumerator Sad()
    {
        canBlink = false;
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.3333f, 0));
        yield return new WaitForSeconds(sadTime);
        animator.SetBool("Sad", false);
        canBlink = true;
    }

    private IEnumerator Happy()
    {
        canBlink = false;
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.7777f, 0));
        yield return new WaitForSeconds(happyTime);
        animator.SetBool("Happy", false);
        canBlink = true;
    }
}
