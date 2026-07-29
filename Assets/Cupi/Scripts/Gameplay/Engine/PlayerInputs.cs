using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class PlayerInputs : IInputDevice
{
    public event Action<CorrespondenciaTecla, double> OnButtonPressed;
    public event Action<CorrespondenciaTecla, double> OnButtonReleased;

    public bool ClickPressed(CorrespondenciaTecla tecla)
    {
        foreach (InputAction InputAction in actualActionMap.actions)
        {
            if (inputKeys[InputAction] == tecla)
            {
                //Debug.Log(InputAction.IsPressed() + "|" + inputKeys[InputAction] + "|" + tecla);
                return InputAction.IsPressed();
            }
        }
        return false;
    }

    private GameInputs gameInputs;

    private InputActionMap actualActionMap;

    private Dictionary<TileModePlayStyle, InputActionMap> InputActionMaps;

    private Dictionary<InputAction, CorrespondenciaTecla> inputKeys = new();

    public PlayerInputs(TileModePlayStyle style)
    {
        gameInputs = InputController.instance.gameInputs;
        SetActionMaps();

        ChangeActionMap(style);
    }

    private void SetActionMaps()
    {
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

    public void ChangeActionMap(TileModePlayStyle playStyle)
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
            OnButtonPressed?.Invoke(tecla,-1f);
        }
        if (ctx.canceled)
        {
            OnButtonReleased?.Invoke(tecla,-1f);
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

    public void Dispose()
    {
        UnsuscribePad();
        actualActionMap?.Disable();
    }
}
