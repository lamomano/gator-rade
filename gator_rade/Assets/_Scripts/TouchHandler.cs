using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class TouchHandler : MonoBehaviour
{
    private PlayerInput playerInputs;

    
    private InputAction touchPositionAction;
    private InputAction touchPressAction;


    private void Awake()
    {
        playerInputs = GetComponent<PlayerInput>();
        touchPressAction = playerInputs.actions.FindAction("TouchPress");
        touchPositionAction = playerInputs.actions.FindAction("TouchPosition");
    }

    private void OnEnable()
    {
        touchPressAction.performed += TouchPressed;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= TouchPressed;

    }

    private void TouchPressed(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();
        Debug.Log(value);
    }
}
