using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TilesModeInputController : MonoBehaviour
{
    public static TilesModeInputController instance;

    public GameInputs TileModeInputs;

    private TilesModeMaster tilesModeMaster;

    private Dictionary<TileModePlayStyle, InputActionMap> InputActionMaps;

    private InputActionMap actualActionMap;

    public static event Action<CorrespondenciaTecla> NoteClick;
    public static event Action<CorrespondenciaTecla> NoteUnClick;

    private Dictionary<InputAction, CorrespondenciaTecla> inputKeys = new();

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

        InputActionMaps = new()
        {
            {TileModePlayStyle.OneKey,TileModeInputs.TileModeOneKey },
            {TileModePlayStyle.TwoKeys,TileModeInputs.TileModeTwoKeys },
            {TileModePlayStyle.ThreeKeys,TileModeInputs.TileModeThreeKeys },
            {TileModePlayStyle.FourKeys,TileModeInputs.TileModeFourKeys },
            {TileModePlayStyle.FiveKeys,TileModeInputs.TileModeFiveKeys },
            {TileModePlayStyle.SixKeys,TileModeInputs.TileModeSixKeys },
            {TileModePlayStyle.SevenKeys,TileModeInputs.TileModeSevenKeys  },
            {TileModePlayStyle.EightKeys,TileModeInputs.TileModeEightKeys  },
            {TileModePlayStyle.NineKeys,TileModeInputs.TileModeNineKeys  },
            {TileModePlayStyle.TenKeys,TileModeInputs.TileModeTenKeys },
        };

        actualActionMap = InputActionMaps[tilesModeMaster.PlayStyle];
    }

    private void OnEnable()
    {
        actualActionMap.Enable();

        CreateKeys();
        SuscribePad();
    }

    private void OnDisable()
    {
        actualActionMap.Disable();

        UnsuscribePad();
    }

    public void SetActionMap(TileModePlayStyle playStyle)
    {
        actualActionMap?.Disable();
        UnsuscribePad();

        actualActionMap = InputActionMaps[playStyle];

        actualActionMap.Enable();
        SuscribePad();

        CreateKeys();
    }

    private void CreateKeys()
    {
        inputKeys.Clear();

        foreach (InputAction InputAction in actualActionMap.actions)
        {
            inputKeys.Add(InputAction, Enum.Parse<CorrespondenciaTecla>(InputAction.name));
        }
    }

    private void OnPad(InputAction.CallbackContext ctx)
    {
        CorrespondenciaTecla tecla = inputKeys[ctx.action];


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
