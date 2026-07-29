using System;
using UnityEngine;

public class NotaTileNormal : NotaTileBaseLogic
{

    private bool _canHit;
    private bool _needsRenderUpdate;

    protected override void OnEnable()
    {
        _canHit = true;
        _needsRenderUpdate = true;

        base.OnEnable();

        TilesModeInputController.NoteClick += DetectClick;
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        TilesModeInputController.NoteClick -= DetectClick;
    }
    protected override void LogicUpdate()
    {
        DetectMiss();
        RenderControl();
    }

    private void RenderControl()
    {
        if (!_canHit)
        {
            if (data.timeToArrive - tilesModeMaster.ToleranciaError > timeController.AdditiveTime)
            {
                _canHit = true;
                _needsRenderUpdate = true;
            }
        }

        if (_needsRenderUpdate)
        {
            SetNoteVisibility(_canHit);

            _needsRenderUpdate = false;
        }
    }

    private void SetNoteVisibility(bool isVisible)
    {
        spriteNote.enabled = isVisible;
    }

    private void DetectClick(CorrespondenciaTecla tecla)
    {
        if (tecla != data.correspondenciaTecla || timeController.TimeScale < 0 || !_canHit) return;

        double timeDiff = Math.Abs(data.timeToArrive - timeController.AdditiveTime);

        if (timeDiff < tilesModeMaster.ToleranciaError)
        {
            NotesController.HitNote(data.correspondenciaTecla);
            _canHit = false;
            _needsRenderUpdate = true;
        }
    }

    private void DetectMiss()
    {
        if (timeController.TimeScale < 0) return;

        bool standardMiss = data.timeToArrive + tilesModeMaster.ToleranciaError < timeController.AdditiveTime;

        if (standardMiss)
        {
            if (_canHit)
            {
                _canHit = false;
                _needsRenderUpdate = true;
                NotesController.MissNote(data.correspondenciaTecla);
            }
            DestroyNote();
        }
    }
}
