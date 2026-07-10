using UnityEngine;

public class NotaTileSostenida : NotaTileBaseLogic
{

    private bool _canHit;
    private bool _needsRenderUpdate;
    private bool _firstHit;
    [SerializeField] private LineRenderer _lineNote;
    private bool _renderNote;

    private float _timeToArriveForLine;
    private float _consumoNota;
    //esto permitira tener efectos complejos mas adelante pero de momento lo dejo asi
    private int _numberPoints = 2;
    private int _waitOneFrameBeforeRendering;
    private int _actualPointGetter;
    private float _getPointsEvery;
    private int _totalSecciones;
    //esto define cuantas secciones de un segundo daran puntos de la nota sostenida
    const int seccionesPorSegundos = 12;
    private bool _isPressed;

    const float defaultSection = 1f / 8f;

    protected override void OnEnable()
    {
        base.OnEnable();

        _firstHit = true;
        _canHit = true;
        _renderNote = true;
        _needsRenderUpdate = true;
        _isPressed = false;
        lockProgress = false;
        _actualPointGetter = 0;
        _waitOneFrameBeforeRendering = 0;
        _consumoNota = 0;
        offsetRendering = 0;

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
        LogicLine();
        DrawLine();
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
                _renderNote = true;
            }
        }

        if (_needsRenderUpdate)
        {
            SetNoteVisibility(_renderNote);

            _needsRenderUpdate = false;
        }
    }

    private void SetNoteVisibility(bool isVisible)
    {
        spriteNote.enabled = isVisible;

        _lineNote.enabled = isVisible;
    }

    protected override void PostInitialize()
    {
        _timeToArriveForLine = data.duracion + data.timeToArrive;

        _getPointsEvery = data.duracion > 0 ? 1.0f / (data.duracion * seccionesPorSegundos) : defaultSection;

        _totalSecciones = Mathf.FloorToInt(seccionesPorSegundos * data.duracion);

        if (data.timeToArrive < timeController.AdditiveTime)
        {
            offsetRendering = 1.0f;
        }
    }

    protected override void SetDefaultConfig()
    {
        base.SetDefaultConfig();

        _lineNote.positionCount = 0;
        _firstHit = true;
        _canHit = true;
        _waitOneFrameBeforeRendering = 0;
        lockProgress = false;
        _isPressed = false;
        _actualPointGetter = 0;
        _consumoNota = 0;
        offsetRendering = 0;
    }

    private void SetLinePoints()
    {
        _lineNote.positionCount = _numberPoints;
    }

    public void ClickNote(CorrespondenciaTecla tecla)
    {
        if (tecla != data.correspondenciaTecla || timeController.TimeScale < 0 || !_canHit) return;

        float timeDiff = Mathf.Abs((data.timeToArrive + (_consumoNota * data.duracion)) - (float)timeController.AdditiveTime);

        if (timeDiff < tilesModeMaster.ToleranciaError)
        {
            _isPressed = true;
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

        _isPressed = false;
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
            _isPressed = false;
        }

        while (_actualPointGetter * _getPointsEvery < _consumoNota + _getPointsEvery && _isPressed)
        {
            //print("ganaste puntos" +  actualPointGetter);
            _actualPointGetter++;
        }


        if (timeController.TimeScale < 0)
        {
            //recupera el conteo de puntos para que la nota no desaparezca antes de
            //tiempo por tener ya muchos puntos recogidos
            while (_actualPointGetter * _getPointsEvery >= _consumoNota + _getPointsEvery)
            {
                _actualPointGetter--;
            }

            if (_consumoNota < 1.0f && _consumoNota < offsetRendering)
            {
                lockProgress = _consumoNota is > 0.0f and < 1.0f;

                offsetRendering = _consumoNota * data.duracion;
                if (!_renderNote)
                {
                    _renderNote = true;
                    _needsRenderUpdate = true;
                }
            }

            //si el tiempo esta en reversa no necesitamos detectar si presionaste la nota
            return;
        }

        if (_isPressed)
        {
            // hit
            lockProgress = true;
            offsetRendering = _consumoNota * data.duracion;
            if (_consumoNota >= 1.0f)
            {
                //print("destruccionFijaNotaSostenida");
                //_renderNote = false;
                DestroyNote();
            }
        }
        else
        {
            lockProgress = false;

            //margen para destruir la nota si sale de pantalla
            float margenNota = _timeToArriveForLine + tilesModeMaster.RenderLimit;

            //hit por margen de soltar
            if (_consumoNota >= 1.0f - _getPointsEvery && _actualPointGetter > _totalSecciones - 1 && _renderNote)
            {
                //Debug.Log("killNote");
                _renderNote = false;
                _needsRenderUpdate = true;
                //DestroyNote();
            }

            if (currentTime > margenNota)
            {
                DestroyNote();
            }
        }
    }

    public void DrawLine()
    {
        //retardamos el update de la linea un frame
        //para que no haya erorres de renderizado
        if (_waitOneFrameBeforeRendering < 1)
        {
            _waitOneFrameBeforeRendering += 1;
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

                float distancia = (progress * _timeToArriveForLine * data.localSpeed * tilesModeMaster.NotaTileSpeed);

                Vector2 finalPos = data.offsetPositionToGo + (DireccionMovimiento * distancia);

                _lineNote.SetPosition(i, finalPos);
            }
        }
    }
}