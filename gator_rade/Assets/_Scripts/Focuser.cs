using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Focuser : MonoBehaviour
{
    private Vector2 screenResolution;
    public float height1;
    public float height2;
    // Start is called before the first frame update
    void Start()
    {
        screenResolution = new Vector2(Screen.width, Screen.height);
        MatchCamtoLvl();
    }

    // Update is called once per frame
    void Update()
    {
        if (screenResolution.x != Screen.width || screenResolution.y != Screen.height)
        {
            MatchCamtoLvl();
            screenResolution.x = Screen.width;
            screenResolution.y = Screen.height;
        }
    }
    private void MatchCamtoLvl()
    {
        float LvlHeightScale = height1 * Camera.main.orthographicSize / height2;
        float LvlWidthScale = LvlHeightScale * Camera.main.aspect;
        gameObject.transform.localScale = new Vector3(LvlWidthScale, LvlHeightScale, 1);
    }
}
