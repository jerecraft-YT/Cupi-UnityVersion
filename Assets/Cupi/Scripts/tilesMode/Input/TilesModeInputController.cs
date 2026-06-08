using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TilesModeInputController : MonoBehaviour
{
    public static TilesModeInputController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnEnable()
    {
        TilesModeMaster.instance.padTile[0].action.actionMap.Enable();
    }

    private void OnDisable()
    {
        TilesModeMaster.instance.padTile[0].action.actionMap.Disable();
    }

    public void SuscribePad(Action<InputAction.CallbackContext> function)
    {
        foreach (InputActionReference InputAction in TilesModeMaster.instance.padTile)
        {
            InputAction.action.performed += function;
            InputAction.action.canceled += function;
        }
    }

    public void UnsuscribePad(Action<InputAction.CallbackContext> function)
    {
        foreach (InputActionReference InputAction in TilesModeMaster.instance.padTile)
        {
            InputAction.action.performed -= function;
            InputAction.action.canceled -= function;
        }
    }
}
