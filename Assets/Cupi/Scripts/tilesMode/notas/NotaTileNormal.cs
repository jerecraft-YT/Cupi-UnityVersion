using UnityEngine;

public class NotaTileNormal : NotaTileBaseLogic
{
    private bool canMiss = true;
    protected override void OnEnable()
    {
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

        float timeDiff = Mathf.Abs(data.timeToArrive - (float)TimeController.instance.AdditiveTime);

        if (timeDiff < TilesModeMaster.instance.toleranciaError)
        {
            TilesModeNotesController.HitNote(data.CorrespondenciaTecla);
            DestroyNote();
        }
    }

    private void DetectMiss()
    {
        if (!canMiss) return;

        if (data.timeToArrive + TilesModeMaster.instance.toleranciaError < TimeController.instance.AdditiveTime)
        {
            TilesModeNotesController.MissNote(data.CorrespondenciaTecla);
            DestroyNote();
            canMiss = false;
        }
    }
}
