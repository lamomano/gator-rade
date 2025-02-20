
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchHandler : MonoBehaviour
{
    private PlayerInputs playerInputs;

    private void Awake()
    {
        playerInputs = new PlayerInputs();

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
        playerInputs.Controls.TouchPress.started += ctx => StartTouch(ctx);
        playerInputs.Controls.TouchPress.canceled -= ctx => EndTouch(ctx);

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
