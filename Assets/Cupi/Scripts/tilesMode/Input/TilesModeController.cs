using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class TilesModeController : MonoBehaviour
{
    public static TilesModeController instance;
    public static event Action<CorrespondenciaTecla> NoteHit;
    public static event Action<CorrespondenciaTecla> NoteNoHit;
    public static event Action<CorrespondenciaTecla> NoteClick;
    public static event Action<CorrespondenciaTecla> NoteUnClick;

    private TilesModeInputController TilesInput;

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
        //para que funcione bien el nombre de la accion debe ser el mismo del enum
        DetectClickCase(ctx,Enum.Parse<CorrespondenciaTecla>(ctx.action.name));
        return;
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