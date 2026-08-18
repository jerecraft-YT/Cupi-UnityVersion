//using System;
using UnityEngine;
//using UnityEngine;

public class NotaTileNormal : NotaTileBaseLogic
{
    //private bool _canHit;
    //private bool _needsRenderUpdate;

    /*
    protected override void OnEnable()
    {
        //_canHit = true;
        //_needsRenderUpdate = true;

        //TilesModeInputController.NoteClick += NoteHit;
    }
    protected override void OnDisable()
    {
        //TilesModeInputController.NoteClick -= NoteHit;
    }
    protected override void LogicUpdate()
    {
        //NoteMiss();
        //RenderControl();
    }
    */

    /*
    private void RenderControl()
    {
        if (!_canHit)
        {
            /*
            if (data.timeToArrive - tilesModeMaster.ToleranciaError > timeProvider.GetCurrentTime())
            {
                _canHit = true;
                _needsRenderUpdate = true;
            }
            */
        //}

        /*
        if (_needsRenderUpdate)
        {
            SetNoteVisibility(_canHit);

            _needsRenderUpdate = false;
        }
        */
    //}
    

    /*
    private void SetNoteVisibility(bool isVisible)
    {
        spriteNote.enabled = isVisible;
    }
    */

    private void NoteHit()
    {
        //if (timeProvider.GetCurrentTimeScale() < 0) return;

        //if (timeProvider.GetCurrentTimeScale() < 0 || !_canHit) return;

        //double timeDiff = Math.Abs(data.timeToArrive - timeProvider.GetCurrentTime());

        //NotesController.HitNote(data.correspondenciaTecla);
        //_canHit = false;
        //_needsRenderUpdate = true;

        /*
        if (timeDiff < tilesModeMaster.ToleranciaError)
        {
            NotesController.HitNote(data.correspondenciaTecla);
            _canHit = false;
            _needsRenderUpdate = true;
        }
        */

        DestroyNote();
    }

    private void NoteMiss()
    {
        //if (timeProvider.GetCurrentTimeScale() < 0) return;
        /*
        bool standardMiss = data.timeToArrive + tilesModeMaster.ToleranciaError < timeProvider.GetCurrentTime();

        if (standardMiss)
        {
            if (_canHit)
            {
                _canHit = false;
                _needsRenderUpdate = true;
                NotesController.MissNote(data.correspondenciaTecla);
            }
            DestroyNote();
        */

        //_canHit = false;
        //_needsRenderUpdate = true;
        DestroyNote();
        //NotesController.MissNote(data.correspondenciaTecla);
    }

    public override void ChangeNoteState(EstadoPuntuacion puntuacion, EstadoNota estado)
    {
        switch (estado)
        {
            case EstadoNota.None:
                break;
            case EstadoNota.EnProceso:
                break;
            case EstadoNota.Fallada:
                Debug.Log("nota fallada");
                NoteMiss();
                break;
            case EstadoNota.Procesada:
                Debug.Log("nota acertada");
                NoteHit();
                break;
            default:
                break;
        }
    }
}
