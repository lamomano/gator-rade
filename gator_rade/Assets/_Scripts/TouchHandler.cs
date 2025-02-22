using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TouchHandler : MonoBehaviour
{
    private PlayerInputs playerInputs;
    public bool isTouching;

    private InputAction TouchPosition;
    private InputAction touchPressAction; 


    private void Awake()
    {
        //playerInputs = new PlayerInputs();
        playerInputs = GetComponent<PlayerInputs>();
        touchPressAction = playerInputs.FindAction("TouchPress");
        TouchPosition = playerInputs.FindAction("TouchPosition");

    }

    private void OnEnable()
    {
        TouchPosition.performed += TouchPress;
    }

    private void OnDisable()
    {
        TouchPosition.performed -= TouchPress;

    }

    private void TouchPress(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        Debug.Log(value);

    }

    private void Update()
    { 
        if (!isTouching) { 
        
        }
    }


}
