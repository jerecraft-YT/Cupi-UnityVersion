using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class TilesModeController : MonoBehaviour
{
    public TilesModeNotesController notesController;
    public static float toleraciaError = 0.1f;
    public static event Action<CorrespondenciaTecla> NoteHit;
    public static event Action<CorrespondenciaTecla> NoteNoHit;
    public static event Action<CorrespondenciaTecla> NoteClick;
    public static event Action<CorrespondenciaTecla> NoteUnClick;

    private TilesModeInputController TilesInput;

    private void OnEnable()
    {
        TilesInput = TilesModeInputController.instance;

        TilesInput.SuscribePad(OnPad);
    }

    private void OnDisable()
    {
        if (TilesInput == null) return;

        TilesInput.UnsuscribePad(OnPad);
    }

    //se llama una vez se vincule con el inputController
    private void OnPad(InputAction.CallbackContext ctx)
    {
        if (ctx.action == TilesInput.LeftPad)
        {
            DetectClickCase(ctx,CorrespondenciaTecla.Left);
            //ButtonClicked(CorrespondenciaTecla.Left);
        }
        else if (ctx.action == TilesInput.RightPad)
        {
            DetectClickCase(ctx, CorrespondenciaTecla.Right);
            //ButtonClicked(CorrespondenciaTecla.Right);
        }
        else if (ctx.action == TilesInput.MiddlePad)
        {
            DetectClickCase(ctx, CorrespondenciaTecla.Middle);
            //ButtonClicked(CorrespondenciaTecla.Middle);
        }
    }

    private void DetectClickCase(InputAction.CallbackContext ctx,CorrespondenciaTecla tecla)
    {
        if (ctx.performed)
        {
            NoteClick?.Invoke(tecla);
        }
        if (ctx.canceled)
        {
            NoteUnClick?.Invoke(tecla);
        }
    }

    public static void MissNote(CorrespondenciaTecla tecla)
    {
        NoteNoHit?.Invoke(tecla);
    }

    public static void ClickNote(CorrespondenciaTecla tecla)
    {
        NoteHit?.Invoke(tecla);
    }
}