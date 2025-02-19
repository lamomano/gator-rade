using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchHandler : MonoBehaviour
{
    private PlayerInputs playerInputs;

    private void Awake()
    {
      //playerInputs = PlayerInputs.Enable;
    }
    public void FingerTouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Touch has been recieved");

        }

    }
}
