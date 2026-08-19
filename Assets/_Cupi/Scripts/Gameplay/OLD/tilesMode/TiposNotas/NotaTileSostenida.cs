using System;
using UnityEngine;

public class NotaTileSostenida : NotaTileBaseLogic
{
    [SerializeField] private LineRenderer _lineNote;
    private bool _renderNote;

    private double _timeToArriveForLine;
    private float _consumoNota;
    //esto permitira tener efectos complejos mas adelante pero de momento lo dejo asi
    private int _numberPoints = 2;
    private int _actualPointGetter;
    private float _getPointsEvery;
    private int _totalSecciones;
    //esto define cuantas secciones de un segundo daran puntos de la nota sostenida
    const int seccionesPorSegundos = 12;
    private bool _isPressed;

    const float defaultSection = 1f / 8f;

    protected override void LogicUpdate()
    {
        LogicLine();
        DrawLine();
    }

    private void SetNoteVisibility(bool isVisible)
    {
        spriteNote.enabled = isVisible;

        _lineNote.enabled = isVisible;
    }

    protected override void PostInitialize()
    {
        _renderNote = true;
        _isPressed = false;
        lockProgress = false;
        _actualPointGetter = 0;
        _consumoNota = 0;
        offsetRendering = 0;

        _timeToArriveForLine = data.duracion + data.timeToArrive;

        _getPointsEvery = data.duracion > 0 ? 1.0f / (data.duracion * seccionesPorSegundos) : defaultSection;

        _totalSecciones = Mathf.FloorToInt(seccionesPorSegundos * data.duracion);

        if (data.timeToArrive < timeProvider.GetCurrentTime())
        {
            offsetRendering = 1.0f;
        }

        SetLinePoints();

        //se hace esto para tener una referencia desde el inicio de la posicion base para el line
        NoteVisualUpdate();

        DrawLine();
    }

    protected override void ResetNoteData()
    {
        base.ResetNoteData();

        _lineNote.positionCount = 0;
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

    public void NoteProcess()
    {
        if (timeProvider.GetCurrentTimeScale() < 0) return;

        _isPressed = true;
        lockProgress = true;
    }

    public void NoteHit()
    {
        SetNoteVisibility(false);
    }

    public void NoteMiss()
    {
        _isPressed = false;
        lockProgress = false;
    }

    public void LogicLine()
    {
        double currentTime = timeProvider.GetCurrentTime();

        _consumoNota = 1 - Mathf.InverseLerp((float)_timeToArriveForLine, (float)data.timeToArrive, (float)currentTime);

        if (timeProvider.GetCurrentTimeScale() < 0)
        {
            if (_consumoNota < 1.0f && _consumoNota < offsetRendering)
            {
                lockProgress = _consumoNota is > 0.0f and < 1.0f;

                offsetRendering = _consumoNota * data.duracion;
                if (!_renderNote)
                {
                    _renderNote = true;
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
                //DestroyNote();
                SetNoteVisibility(false);
            }
        }
        else
        {
            lockProgress = false;



            //hit por margen de soltar
            if (_consumoNota >= 1.0f - _getPointsEvery && _actualPointGetter > _totalSecciones - 1 && _renderNote)
            {
                //Debug.Log("killNote");
                _renderNote = false;
                //DestroyNote();
            }
            /*
            //margen para destruir la nota si sale de pantalla
            double margenNota = _timeToArriveForLine + tilesModeMaster.RenderLimit;

            if (currentTime > margenNota)
            {
                DestroyNote();
            }
            */
        }
    }

    public void DrawLine()
    {
        //retardamos el update de la linea un frame
        //para que no haya erorres de renderizado
        /*
        if (_waitOneFrameBeforeRendering < 1)
        {
            _waitOneFrameBeforeRendering += 1;
            return;
        }
        */

        for (int i = 0; i < _lineNote.positionCount; i++)
        {
            if (i == 0)
            {
                _lineNote.SetPosition(i, finalPos);
            }
            else
            {
                double progress = 1 - InverseLerpUnclamped(0.0f, _timeToArriveForLine, timeProvider.GetCurrentTime());

                if (lockProgress) progress = Math.Max(0, progress);

                double distancia = (progress * _timeToArriveForLine * data.localSpeed * gameplayRenderer.scrollSpeed);

                Vector2 finalPos = data.offsetPositionToGo + (direccionMovimiento * (float)distancia);

                _lineNote.SetPosition(i, finalPos);
            }
        }
    }

    public override void ChangeNoteState(EstadoPuntuacion puntuacion, EstadoNota estado)
    {
        switch (estado)
        {
            case EstadoNota.None:
                break;
            case EstadoNota.EnProceso:
                Debug.Log("nota en proceso");
                NoteProcess();
                break;
            case EstadoNota.ProcesoFallado:
                Debug.Log("nota fallada");
                NoteMiss();
                break;
            case EstadoNota.Fallada:
                break;
            case EstadoNota.Procesada:
                Debug.Log("nota acertada");
                NoteHit();
                break;
            default:
                break;
        }
    }
}