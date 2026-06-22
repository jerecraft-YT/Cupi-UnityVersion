using System;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController instance;

    private double prevTime;

    private double dspOffset;

    private double progressTime;

    private double additiveTime;

    private float timeScale = 1.0f;

    private float oldTimeScale = 1.0f;

    public static event Action updateTimeScale;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            print("oh no");
            Destroy(gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        RestartTime();
    }

    private void Update()
    {
        UpdateAdditiveTime();

        if (oldTimeScale != timeScale)
        {
            updateTimeScale?.Invoke();
            oldTimeScale = timeScale;
        }
    }

    private void UpdateAdditiveTime()
    {
        progressTime = ActualTime - prevTime;

        prevTime = ActualTime;

        additiveTime += progressTime * timeScale;
    }

    public void RestartTime(AudioSource source = null)
    {
        dspOffset = AudioSettings.dspTime;

        additiveTime = 0.0f;

        prevTime = ActualTime;

        if (source != null) source.time = 0.0f;
    }

    public void SetTime(float time ,AudioSource source = null)
    {
        dspOffset = AudioSettings.dspTime;

        additiveTime = time;

        prevTime = ActualTime;

        if (source != null) source.time = time;
    }

    public double ActualTime => AudioSettings.dspTime - dspOffset;
    public double AdditiveTime => additiveTime;
    public float TimeScale
    {
        get
        {
            return timeScale;
        }
        set
        {
            if (value != timeScale)
            {
                updateTimeScale?.Invoke();
                timeScale = value;
            }
        }
    }
}
