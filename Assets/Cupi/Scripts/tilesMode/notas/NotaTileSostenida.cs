using UnityEngine;

public class NotaTileSostenida : NotaTileBaseLogic
{
    [SerializeField] private LineRenderer lineNote;
    private float timeToArriveForLine;
    private float consumoNota;
    private int numberPoints = 2;
    private int framesToUpdate;

    protected override void OnEnable()
    {
        base.OnEnable();

        TilesModeController.NoteClick += ClampNote;
        TilesModeController.NoteUnClick += UnClampNote;

        SetLinePoints();
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

    private void SetLinePoints()
    {
        lineNote.positionCount = numberPoints;
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

        if (timeDiff < TilesModeMaster.instance.toleranciaError)
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
        //retardamos el update de la linea un frame
        //para que no haya erorres de renderizado
        if (framesToUpdate < 1)
        {
            framesToUpdate += 1;
            return;
        }

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

                float distancia = (progress * timeToArriveForLine * data.localSpeed * TilesModeMaster.instance.notaTileSpeed);

                Vector2 finalPos = data.offsetPositionToGo + (DireccionMovimiento * distancia);

                lineNote.SetPosition(i, finalPos);
            }
        }
    }
}
