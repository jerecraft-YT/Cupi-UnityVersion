using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cupi.Input;

public class TilesModeInputController : MonoBehaviour
{
    public static TilesModeInputController instance;

    public GameInputs gameInputs;

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

        gameInputs = InputController.instance.gameInputs;

        InputActionMaps = new()
        {
            {TileModePlayStyle.OneKey       ,   gameInputs.TileModeOneKey },
            {TileModePlayStyle.TwoKeys      ,   gameInputs.TileModeTwoKeys },
            {TileModePlayStyle.ThreeKeys    ,   gameInputs.TileModeThreeKeys },
            {TileModePlayStyle.FourKeys     ,   gameInputs.TileModeFourKeys },
            {TileModePlayStyle.FiveKeys     ,   gameInputs.TileModeFiveKeys },
            {TileModePlayStyle.SixKeys      ,   gameInputs.TileModeSixKeys },
            {TileModePlayStyle.SevenKeys    ,   gameInputs.TileModeSevenKeys  },
            {TileModePlayStyle.EightKeys    ,   gameInputs.TileModeEightKeys  },
            {TileModePlayStyle.NineKeys     ,   gameInputs.TileModeNineKeys  },
            {TileModePlayStyle.TenKeys      ,   gameInputs.TileModeTenKeys },
        };
    }

    private void Start()
    {
        SetActionMap(tilesModeMaster.PlayStyle);
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
        if (actualActionMap == null) return;

        foreach (InputAction InputAction in actualActionMap.actions)
        {
            InputAction.performed -= OnPad;
            InputAction.canceled -= OnPad;
        }
    }
}
