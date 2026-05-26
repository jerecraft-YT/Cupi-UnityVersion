using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TilesModeInputController : MonoBehaviour
{
    private GameInputs Input;
    public static TilesModeInputController instance;

    private InputAction leftPad;
    private InputAction rightPad;
    private InputAction middlePad;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Input = new GameInputs();

        leftPad = Input.Player.leftPad;
        rightPad = Input.Player.rigthPad;
        middlePad = Input.Player.middlePad;
    }

    private void OnEnable()
    {
        Input.Player.Enable();
    }

    private void OnDisable()
    {
        Input.Player.Disable();
    }

    public void SuscribePad(Action<InputAction.CallbackContext> function)
    {

        leftPad.performed += function;
        rightPad.performed += function;
        middlePad.performed += function;
    }

    public void UnsuscribePad(Action<InputAction.CallbackContext> function)
    {
        leftPad.performed -= function;
        rightPad.performed -= function;
        middlePad.performed -= function;
    }

    public InputAction LeftPad => leftPad;
    public InputAction RightPad => rightPad;
    public InputAction MiddlePad => middlePad;
}
