using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Iversen-Krampitz, Ian 
 * 4/26/2025
 * Controls the alligators eye animation + might control full animations later
 */

public class Eyes : MonoBehaviour
{
    public SkinnedMeshRenderer eyeRenderer;
    private int frameCount;
    public float sadTime;
    public float happyTime;
    public bool isHappy;
    public bool isSad;
    public bool isBlinking;
    public bool canBlink;
    public bool isBored; //should be called in another script
                         //if the player hasnt made a move in a while
   
    // Start is called before the first frame update
    void Start()
    {
        canBlink = true;
        isBored = false;
    }

    // Update is called once per frame
    void Update()
    {
        //if not blinking and can blink, start random blink timer
        if (canBlink)
        {
           //Debug.Log("blinking start");
            StartCoroutine(Blink(Random.Range(.01f, 2f)));
        }
        else if (isBored)
        {
            //do code here for sleeping/bored eyes  
        }
        //for testing the anims 
        if (isSad)
        {
            isSad = false;
            //Debug.Log("start sad");
            StopAllCoroutines();
            StartCoroutine(Sad());
        } 
        if (isHappy)
        {
            isHappy = false;
            StopAllCoroutines();
            StartCoroutine(Happy());
        }
    }

    //do blink animation
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
        canBlink = true;
    }

    private IEnumerator Happy()
    {
        canBlink = false;
        eyeRenderer.material.SetTextureOffset("_MainTex", new Vector2(.7777f, 0));
        yield return new WaitForSeconds(happyTime);
        canBlink = true;
    }
}
