using UnityEngine;

public class NotaTileNormal : NotaTileBaseLogic
{
    private bool canHit;
    private bool changeNoteState;

    protected override void OnEnable()
    {
        canHit = true;
        changeNoteState = true;

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

        if (!canHit)
        {
            if (data.timeToArrive - tilesModeMaster.ToleranciaError > timeController.AdditiveTime)
            {
                canHit = true;
                changeNoteState = true;
            }
        }

        if (changeNoteState)
        {
            if (!canHit)
            {
                Color colorActual = spriteNote.color;

                colorActual.a = 0f;

                spriteNote.color = colorActual;
            }
            else
            {
                Color colorActual = spriteNote.color;

                colorActual.a = 1f;

                spriteNote.color = colorActual;
            }

            changeNoteState = false;
        }
    }

    private void DetectClick(CorrespondenciaTecla tecla)
    {
        if (tecla != data.correspondenciaTecla || timeController.TimeScale < 0 || !canHit) return;

        float timeDiff = Mathf.Abs(data.timeToArrive - (float)timeController.AdditiveTime);

        if (timeDiff < tilesModeMaster.ToleranciaError)
        {
            NotesController.HitNote(data.correspondenciaTecla);
            canHit = false;
            changeNoteState = true;
        }
    }

    private void DetectMiss()
    {
        if (timeController.TimeScale < 0) return;

        bool standartMiss = data.timeToArrive + tilesModeMaster.ToleranciaError < timeController.AdditiveTime;

        if (standartMiss)
        {
            if (canHit)
            {
                canHit = false;
                changeNoteState = true;
                NotesController.MissNote(data.correspondenciaTecla);
            }
            DestroyNote();
        }
    }
}
