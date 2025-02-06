using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // positions
    public int x;
    public int y;


    /*
     * red = 1
     * blue = 2
     * yellow = 3
     * green = 4
     * 
     */


    public int type = 1;


    private GameObject tileObject;


    public void Start()
    {
        if (tileObject == null)
        {
            tileObject = gameObject;
        }

        UpdateAppearance();
    }


    private void ChangeColor(Color color)
    {
        gameObject.GetComponent<Renderer>().material.color = color;
    }


    public void UpdateAppearance()
    {
        switch (type)
        {
            case 1:
                ChangeColor(Color.red);
                break;
            case 2:
                ChangeColor(Color.blue);
                break;
            case 3:
                ChangeColor(Color.yellow);
                break;
            case 4:
                ChangeColor(Color.green);
                break;
            default:
                print("not a valid type");
                break;
        }
    }
}
