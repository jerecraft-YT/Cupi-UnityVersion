using System;
using UnityEngine;

public class TimeController : MonoBehaviour , ITimeProvider
{
    public static TimeController instance;

    //usado para obtener el tiempo del frame anterior
    private double _prevTime;

    /// <summary>
    /// offset para que <see cref="TimeController"/> pueda comenzar desde 0 al restablecer el tiempo
    /// </summary>
    private double _dspOffset;

    private double _additiveTime;

    private float _timeScale = 1.0f;

    public static event Action OnUpdateTimeScale;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        RestartTime();
    }

    private void FixedUpdate()
    {
        UpdateAdditiveTime();
    }

    private void UpdateAdditiveTime()
    {
        double progressTime = ActualTime - _prevTime;

        _prevTime = ActualTime;

        _additiveTime += progressTime * _timeScale;
    }

    public void RestartTime(AudioSource source = null)
    {
        _dspOffset = AudioSettings.dspTime;

        _additiveTime = 0.0f;

        _prevTime = ActualTime;

        if (source != null) source.time = 0.0f;
    }

    public void SetTime(float time ,AudioSource source = null)
    {
        _dspOffset = AudioSettings.dspTime;

        _additiveTime = time;

        _prevTime = ActualTime;

        if (source != null) source.time = time;
    }

    /// <summary>
    /// Metodo de interfaz usado para obtener de manera alternativa <see cref="AdditiveTime"/>
    /// </summary>
    /// <returns><see cref="AdditiveTime"/></returns>
    public double GetCurrentTime()
    {
        return AdditiveTime;
    }

    /// <summary>
    /// metodo de interfaz usado para obtener de manera alternativa el valor de TimeScale
    /// </summary>
    /// <returns>Time Scale</returns>
    public float GetCurrentTimeScale()
    {
        return _timeScale;
    }

    /// <summary>
    /// modificador de Escala de tiempo de <see cref="AdditiveTime"/>
    /// </summary>
    public void SetTimeScale(float value)
    {
        if (value != _timeScale)
        {
            OnUpdateTimeScale?.Invoke();
            _timeScale = value;
        }
    }

    /// <summary>
    /// Medida de tiempo determinista
    /// </summary>
    public double ActualTime => AudioSettings.dspTime - _dspOffset;

    /// <summary>
    /// medida de tiempo creciente alterable por <see cref="TimeScale"/>
    /// </summary>
    public double AdditiveTime => _additiveTime;

    /// <summary>
    /// medida que altera la manera de crecer de <see cref="AdditiveTime"/>
    /// </summary>
    public float TimeScale
    {
        get
        {
            return _timeScale;
        }
        set
        {
            if (value != _timeScale)
            {
                OnUpdateTimeScale?.Invoke();
                _timeScale = value;
            }
        }
    }
}
