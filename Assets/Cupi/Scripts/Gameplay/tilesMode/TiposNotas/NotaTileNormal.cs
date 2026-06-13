using UnityEngine;

public class NotaTileNormal : NotaTileBaseLogic
{
    private bool canMiss;

    protected override void OnEnable()
    {
        canMiss = true;

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
    }

    private void DetectClick(CorrespondenciaTecla tecla)
    {
        if (tecla != data.CorrespondenciaTecla) return;

        float timeDiff = Mathf.Abs(data.timeToArrive - (float)timeController.AdditiveTime);

        if (timeDiff < tilesModeMaster.toleranciaError)
        {
            TilesModeNotesController.HitNote(data.CorrespondenciaTecla);
            DestroyNote();
        }
    }

    private void DetectMiss()
    {
        if (!canMiss) return;

        if (data.timeToArrive + tilesModeMaster.toleranciaError < timeController.AdditiveTime)
        {
            canMiss = false;
            TilesModeNotesController.MissNote(data.CorrespondenciaTecla);
            DestroyNote();
        }
    }
}
