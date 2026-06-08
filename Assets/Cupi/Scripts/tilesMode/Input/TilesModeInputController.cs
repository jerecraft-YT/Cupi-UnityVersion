using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TilesModeInputController : MonoBehaviour
{
    public static TilesModeInputController instance;

    [Header("Teclas")]
    [Tooltip("Teclas para el modo Tile")]
    public List<InputActionReference> padTile;

    public static event Action<CorrespondenciaTecla> NoteClick;
    public static event Action<CorrespondenciaTecla> NoteUnClick;

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
        padTile[0].action.actionMap.Enable();

        SuscribePad();
    }

    private void OnDisable()
    {
        padTile[0].action.actionMap.Disable();

        UnsuscribePad();
    }

    //se llama una vez se vincule con el inputController
    private void OnPad(InputAction.CallbackContext ctx)
    {
        CorrespondenciaTecla tecla = Enum.Parse<CorrespondenciaTecla>(ctx.action.name);

        if (ctx.performed)
        {
            NoteClick?.Invoke(tecla);
        }
        if (ctx.canceled)
        {
            NoteUnClick?.Invoke(tecla);

        }
    }

    private void SuscribePad()
    {
        foreach (InputActionReference InputAction in padTile)
        {
            InputAction.action.performed += OnPad;
            InputAction.action.canceled += OnPad;
        }
    }

    private void UnsuscribePad()
    {
        foreach (InputActionReference InputAction in padTile)
        {
            InputAction.action.performed -= OnPad;
            InputAction.action.canceled -= OnPad;
        }
    }
}
