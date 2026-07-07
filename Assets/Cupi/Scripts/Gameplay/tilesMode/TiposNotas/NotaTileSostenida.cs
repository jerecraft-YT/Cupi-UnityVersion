using UnityEngine;

public class NotaTileSostenida : NotaTileBaseLogic
{
    [SerializeField] private LineRenderer _lineNote;
    private bool _canHit;
    private bool _firstHit;

    private float _timeToArriveForLine;
    public float _consumoNota;
    //esto permitira tener efectos complejos mas adelante pero de momento lo dejo asi
    private int _numberPoints = 2;
    private int _framesToUpdate;
    public int _actualPointGetter;
    public float _getPointsEvery;
    public int totalSecciones;
    //esto define cuantas secciones de un segundo daran puntos de la nota sostenida
    const int seccionesPorSegundos = 8;

    //const float margenDestruirNotaTile = 2.0f;

    protected override void OnEnable()
    {
        _firstHit = true;
        _canHit = true;
        _framesToUpdate = 0;
        lockProgress = false;
        _actualPointGetter = 0;
        _consumoNota = 0;
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
        _timeToArriveForLine = data.duracion + data.timeToArrive;

        _getPointsEvery = data.duracion > 0 ? 1.0f / (data.duracion * seccionesPorSegundos) : 0.125f;

        totalSecciones = Mathf.FloorToInt(seccionesPorSegundos * data.duracion);
    }

    private void SetLinePoints()
    {
        _lineNote.positionCount = _numberPoints;
    }

    public void TileNoteController()
    {
        LogicLine();
        DrawLine();
    }

    public void ClickNote(CorrespondenciaTecla tecla)
    {
        if (tecla != data.correspondenciaTecla || !_canHit) return;

        float timeDiff = Mathf.Abs((data.timeToArrive + (_consumoNota * data.duracion)) - (float)timeController.AdditiveTime);

        if (timeDiff < tilesModeMaster.ToleranciaError)
        {
            lockProgress = true;
            if (_firstHit)
            {
                NotesController.HitNote(data.correspondenciaTecla);
                _firstHit = false;
            }
        }

    }

    public void UnClickNote(CorrespondenciaTecla tecla)
    {
        if (tecla != data.correspondenciaTecla || !_canHit) return;

        lockProgress = false;
    }

    public void LogicLine()
    {
        float currentTime = (float)timeController.AdditiveTime;

        _consumoNota = 1 - Mathf.InverseLerp(_timeToArriveForLine, data.timeToArrive, currentTime);

        float tiempoActual = data.timeToArrive + offsetRendering;

        if (tiempoActual + tilesModeMaster.ToleranciaError < currentTime && _canHit)
        {
            _canHit = false;
            lockProgress = false;
        }

        while (_actualPointGetter * _getPointsEvery < _consumoNota + _getPointsEvery && lockProgress)
        {
            //print("ganaste puntos" +  actualPointGetter);
            _actualPointGetter++;
        }

        if (timeController.TimeScale < 0) return;

        if (lockProgress)
        {
            // hit
            offsetRendering = _consumoNota * data.duracion;
            if (_consumoNota >= 1.0f)
            {
                print("destruccionFijaNotaSostenida");
                DestroyNote();
            }
        }
        else
        {
            //hit por margen de soltar
            float margenNota = _timeToArriveForLine + tilesModeMaster.RenderLimit;

            if (currentTime > margenNota || (_consumoNota >= 1.0f - _getPointsEvery && _actualPointGetter > totalSecciones - 1))
            {
                DestroyNote();
            }
        }
    }

    public void DrawLine()
    {
        //retardamos el update de la linea un frame
        //para que no haya erorres de renderizado
        if (_framesToUpdate < 1)
        {
            _framesToUpdate += 1;
            return;
        }

        for (int i = 0; i < _lineNote.positionCount; i++)
        {
            if (i == 0)
            {
                _lineNote.SetPosition(i, finalPos);
            }
            else
            {
                float progress = 1 - InverseLerpUnclamped(0.0f, _timeToArriveForLine, (float)timeController.AdditiveTime);

                if (lockProgress) progress = Mathf.Max(0, progress);

                float distancia = (progress * _timeToArriveForLine * data.localSpeed * tilesModeMaster.notaTileSpeed);

                Vector2 finalPos = data.offsetPositionToGo + (DireccionMovimiento * distancia);

                _lineNote.SetPosition(i, finalPos);
            }
        }
    }
}
