
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class TouchHandler : MonoBehaviour
{
    private PlayerInputs playerInputs;

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        EnhancedTouchSupport.Enable();

       // PlayerInput.SwitchCurrentControlScheme(InputSystem.devices.First(Mouse => Mouse.button == Touchscreen.current));
    }
    private void OnEnable()
    {
        playerInputs.Enable();

        

    }
    private void OnDisable()
    {
        playerInputs.Disable();
    }
    private void Start()
    {
       
        playerInputs.Controls.TouchPress.canceled -= ctx => EndTouch(ctx);

    }

    private void Update()
    {
        playerInputs.Controls.TouchPress.started += ctx => StartTouch(ctx);
    }
    private void StartTouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Touch started: " + playerInputs.Controls.TouchPress.ReadValue<Vector2>());

        }

    }

    private void EndTouch(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            Debug.Log("Touch has stopped");

        }

    }
}
