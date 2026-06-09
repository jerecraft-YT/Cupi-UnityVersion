using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TilesModeInputController : MonoBehaviour
{
    public static TilesModeInputController instance;

    public GameInputs TileModeInputs;

    private TilesModeMaster tilesModeMaster;

    private InputActionMap[] actionMaps;

    private InputActionMap actualActionMap;

    public static event Action<CorrespondenciaTecla> NoteClick;
    public static event Action<CorrespondenciaTecla> NoteUnClick;

    private Dictionary<string, CorrespondenciaTecla> inputKeys = new();

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        tilesModeMaster = GetComponent<TilesModeMaster>();

        TileModeInputs = new GameInputs();

        actionMaps = new InputActionMap[]{
            TileModeInputs.TileModeOneKey,

            TileModeInputs.TileModeTwoKeys,

            TileModeInputs.TileModeThreeKeys,

            TileModeInputs.TileModeFourKeys,

            TileModeInputs.TileModeFiveKeys,

            TileModeInputs.TileModeSixKeys,

            TileModeInputs.TileModeSevenKeys,

            TileModeInputs.TileModeEightKeys,

            TileModeInputs.TileModeNineKeys,

            TileModeInputs.TileModeTenKeys
        };

        actualActionMap = actionMaps[(int)tilesModeMaster.PlayStyle];
    }

    private void OnEnable()
    {
        CreateKeys();
        SuscribePad();

        actualActionMap.Enable();
    }

    private void OnDisable()
    {
        actualActionMap.Disable();

        UnsuscribePad();
    }

    private void SetActionMap(TileModePlayStyle playStyle)
    {
        actualActionMap?.Disable();
        UnsuscribePad();

        actualActionMap = actionMaps[(int)playStyle];

        actualActionMap.Enable();
        SuscribePad();

        CreateKeys();
    }

    private void CreateKeys()
    {
        inputKeys.Clear();

        foreach (InputAction InputAction in actualActionMap.actions)
        {
            inputKeys.Add(InputAction.name, Enum.Parse<CorrespondenciaTecla>(InputAction.name));
        }
    }

    private void OnPad(InputAction.CallbackContext ctx)
    {
        CorrespondenciaTecla tecla = inputKeys[ctx.action.name];

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
        foreach (InputAction InputAction in actualActionMap.actions)
        {
            InputAction.performed += OnPad;
            InputAction.canceled += OnPad;
        }
    }

    private void UnsuscribePad()
    {
        foreach (InputAction InputAction in actualActionMap.actions)
        {
            InputAction.performed -= OnPad;
            InputAction.canceled -= OnPad;
        }
    }
}
