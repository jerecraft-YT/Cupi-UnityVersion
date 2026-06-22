using UnityEngine;

public class NotaTileSostenida : NotaTileBaseLogic
{
    [SerializeField] private LineRenderer lineNote;
    private float timeToArriveForLine;
    private float consumoNota;
    //esto permitira tener efectos complejos mas adelante pero de momento lo dejo asi
    private int numberPoints = 2;
    private int framesToUpdate;
    private bool canMiss;
    private bool firstHit;
    private int actualPointGetter;
    private float getPointsEvery;
    //esto define cuantas secciones de un segundo daran puntos de la nota sostenida
    const int seccionesPorSegundos = 8;

    //const float margenDestruirNotaTile = 2.0f;

    protected override void OnEnable()
    {
        firstHit = true;
        canMiss = true;
        framesToUpdate = 0;
        lockProgress = false;
        actualPointGetter = 0;
        consumoNota = 0;
        offsetRendering = 0;

        base.OnEnable();

        TilesModeInputController.NoteClick += ClickNote;
        TilesModeInputController.NoteUnClick += UnClickNote;

        SetLinePoints();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        TilesModeInputController.NoteClick -= ClickNote;
        TilesModeInputController.NoteUnClick -= UnClickNote;
    }

    protected override void LogicUpdate()
    {
        TileNoteController();
    }

    protected override void PostInitialize()
    {
        timeToArriveForLine = data.duracion + data.timeToArrive;

        getPointsEvery = data.duracion > 0 ? 1.0f / (data.duracion * seccionesPorSegundos) : 0.125f;
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

    public void ClickNote(CorrespondenciaTecla tecla)
    {
        if (tecla != data.CorrespondenciaTecla || !canMiss) return;

        float timeDiff = Mathf.Abs((data.timeToArrive + (consumoNota * data.duracion)) - (float)timeController.AdditiveTime);

        if (timeDiff < tilesModeMaster.toleranciaError)
        {
            lockProgress = true;
            if (firstHit)
            {
                TilesModeNotesController.HitNote(data.CorrespondenciaTecla);
                firstHit = false;
            }
        }

    }

    public void UnClickNote(CorrespondenciaTecla tecla)
    {
        if (tecla != data.CorrespondenciaTecla || !canMiss) return;

        lockProgress = false;
    }

    public void LogicLine()
    {
        float currentTime = (float)timeController.AdditiveTime;

        consumoNota = 1 - Mathf.InverseLerp(timeToArriveForLine, data.timeToArrive, currentTime);

        float tiempoActual = data.timeToArrive + offsetRendering;

        if (tiempoActual + tilesModeMaster.toleranciaError < currentTime && canMiss)
        {
            canMiss = false;
            lockProgress = false;
        }

        while (actualPointGetter * getPointsEvery < consumoNota + getPointsEvery && lockProgress)
        {
            //print("ganaste puntos" +  actualPointGetter);
            actualPointGetter++;
        }

        if (timeController.TimeScale < 0) return;

        if (lockProgress)
        {
            // hit
            offsetRendering = consumoNota * data.duracion;
            if (consumoNota >= 1.0f)
            {
                DestroyNote();
            }
        }
        else
        {
            //hit por margen de soltar
            float margenNota = timeToArriveForLine + tilesModeMaster.RenderLimit;

            if (currentTime > margenNota || (consumoNota >= 1.0f - getPointsEvery && actualPointGetter > seccionesPorSegundos - 1))
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
                float progress = 1 - InverseLerpUnclamped(0.0f, timeToArriveForLine, (float)timeController.AdditiveTime);

                if (lockProgress) progress = Mathf.Max(0, progress);

                float distancia = (progress * timeToArriveForLine * data.localSpeed * tilesModeMaster.notaTileSpeed);

                Vector2 finalPos = data.offsetPositionToGo + (DireccionMovimiento * distancia);

                lineNote.SetPosition(i, finalPos);
            }
        }
    }
}
