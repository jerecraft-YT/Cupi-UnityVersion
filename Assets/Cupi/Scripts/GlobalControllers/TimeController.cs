using System;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController instance;

    private double _prevTime;

    private double _dspOffset;

    private double _progressTime;

    private double _additiveTime;

    private float _timeScale = 1.0f;

    private float _oldTimeScale = 1.0f;

    public static event Action UpdateTimeScale;

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

        if (_oldTimeScale != _timeScale)
        {
            UpdateTimeScale?.Invoke();
            _oldTimeScale = _timeScale;
        }
    }

    private void UpdateAdditiveTime()
    {
        _progressTime = ActualTime - _prevTime;

        _prevTime = ActualTime;

        _additiveTime += _progressTime * _timeScale;
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

    public double ActualTime => AudioSettings.dspTime - _dspOffset;
    public double AdditiveTime => _additiveTime;
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
                UpdateTimeScale?.Invoke();
                _timeScale = value;
            }
        }
    }
}
