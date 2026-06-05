using UnityEngine;

public class NotaTileNormal : NotaTileBaseLogic
{
    //public NotaTileBaseLogic notaTileMaster;
    public bool canMiss = true;

    protected override void OnEnable()
    {
        base.OnEnable();

        TilesModeController.NoteClick += DetectClick;
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        TilesModeController.NoteClick -= DetectClick;
    }

    protected override void LogicUpdate()
    {
        DetectMiss();
    }

    private void DetectClick(CorrespondenciaTecla tecla)
    {
        if (tecla != data.CorrespondenciaTecla) return;

        float timeDiff = Mathf.Abs(data.timeToArrive - (float)TimeController.instance.AdditiveTime);

        if (timeDiff < TilesModeController.toleraciaError)
        {
            TilesModeController.ClickNote(data.CorrespondenciaTecla);
            DestroyNote();
        }
    }

    private void DetectMiss()
    {
        if (!canMiss) return;

        if (data.timeToArrive + TilesModeController.toleraciaError < TimeController.instance.AdditiveTime)
        {
            TilesModeController.MissNote(data.CorrespondenciaTecla);
            DestroyNote();
            canMiss = false;
        }
    }
}
