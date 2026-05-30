using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

public class TilesModeController : MonoBehaviour
{

    public float toleraciaError = 0.1f;
    private int actualViewLeftPadNotes = 0;
    private int actualViewRigthPadNotes = 0;
    private int actualViewMiddlePadNotes = 0;
    public Action<CorrespondenciaTecla> NoteHit;

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
    private void Update()
    {
        DetectMissNote(ref actualViewLeftPadNotes, SpawnerNotas.instance.timeArriveLeftNotes);
        DetectMissNote(ref actualViewRigthPadNotes, SpawnerNotas.instance.timeArriveRigthNotes);
        DetectMissNote(ref actualViewMiddlePadNotes, SpawnerNotas.instance.timeArriveMiddleNotes);
    }

    //se llama una vez se vincule con el inputController
    private void OnPad(InputAction.CallbackContext ctx)
    {
        if (ctx.action == TilesInput.LeftPad)
        {
            ButtonClicked(CorrespondenciaTecla.Left);
        }
        else if (ctx.action == TilesInput.RightPad)
        {
            ButtonClicked(CorrespondenciaTecla.Right);
        }
        else if (ctx.action == TilesInput.MiddlePad)
        {
            ButtonClicked(CorrespondenciaTecla.Middle);
        }
    }
    private void DetectMissNote(ref int index, List<float> notesGroup)
    {
        if (index >= notesGroup.Count) return;
        if (notesGroup[index] + toleraciaError < TimeController.instance.ActualTime)
        {
            index++;
        }
    }
    private void DetectHitNote(ref int index, List<float> notesGroup, CorrespondenciaTecla tecla)
    {
        if (index >= notesGroup.Count) return;

        float timeDiff = Mathf.Abs(notesGroup[index] - (float)TimeController.instance.ActualTime);

        if (timeDiff < toleraciaError)
        {
            index++;
            NoteHit?.Invoke(tecla);
        }
    }
    private void ButtonClicked(CorrespondenciaTecla tecla)
    {
        switch (tecla)
        {
            case CorrespondenciaTecla.Left:
                DetectHitNote(ref actualViewLeftPadNotes, SpawnerNotas.instance.timeArriveLeftNotes, tecla);
                break;

            case CorrespondenciaTecla.Right:
                DetectHitNote(ref actualViewRigthPadNotes, SpawnerNotas.instance.timeArriveRigthNotes, tecla);
                break;

            case CorrespondenciaTecla.Middle:
                DetectHitNote(ref actualViewMiddlePadNotes, SpawnerNotas.instance.timeArriveMiddleNotes, tecla);
                break;
        }
    }
}