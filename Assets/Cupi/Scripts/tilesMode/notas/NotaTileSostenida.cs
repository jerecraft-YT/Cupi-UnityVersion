using UnityEngine;

public class NotaTileSostenida : NotaTileBaseLogic
{
    //public NotaTileBaseLogic notaTileMaster;
    public LineRenderer lineNote;
    public float timeToArriveForLine;
    public float consumoNota;
    public bool usarConsumo;

    protected override void OnEnable()
    {
        base.OnEnable();

        TilesModeController.NoteClick += ClampNote;
        TilesModeController.NoteUnClick += UnClampNote;
        lineNote.positionCount = 2;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        TilesModeController.NoteClick -= ClampNote;
        TilesModeController.NoteUnClick -= UnClampNote;
    }

    protected override void LogicUpdate()
    {
        TileNoteController();
    }

    private void Start()
    {
        timeToArriveForLine = data.duracion + data.timeToArrive;
        //notaTileMaster.lockProgress = true;
    }

    public void TileNoteController()
    {
        LogicLine();
        DrawLine();
    }

    public void ClampNote(CorrespondenciaTecla tecla)
    {
        if (tecla != data.CorrespondenciaTecla) return;

        float timeDiff = Mathf.Abs(data.timeToArrive - (float)TimeController.instance.AdditiveTime);

        if (timeDiff < TilesModeController.toleraciaError)
        {
            lockProgress = true;
            TilesModeController.ClickNote(data.CorrespondenciaTecla);
            //DestroyNote();
        }

    }

    public void UnClampNote(CorrespondenciaTecla tecla)
    {
        if (tecla != data.CorrespondenciaTecla) return;

        lockProgress = false;
    }

    public void LogicLine()
    {
        consumoNota = 1 - Mathf.InverseLerp(timeToArriveForLine, data.timeToArrive, (float)TimeController.instance.AdditiveTime);
        
        if (lockProgress)
        {
            offsetRendering = consumoNota * data.duracion;
            if (consumoNota >= 1)
            {
                DestroyNote();
            }
        }
    }

    public void DrawLine()
    {
        for (int i = 0; i < lineNote.positionCount; i++)
        {
            if (i == 0)
            {
                lineNote.SetPosition(i, finalPos);
            }
            else
            {
                float progress = 1 - InverseLerpUnclamped(0.0f, timeToArriveForLine, (float)TimeController.instance.AdditiveTime);

                if (lockProgress) progress = Mathf.Max(0, progress);

                float distancia = (progress * timeToArriveForLine * data.localSpeed * SpawnerNotas.instance.notaTileSpeed);

                Vector2 finalPos = data.offsetPositionToGo + (DireccionMovimiento * distancia);

                lineNote.SetPosition(i, finalPos);
            }
        }
    }
}
