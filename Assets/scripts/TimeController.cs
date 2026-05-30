using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController instance;

    public double dspOffset;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        RestartTime();
    }

    public void RestartTime()
    {
        dspOffset = AudioSettings.dspTime;
        print(Time.realtimeSinceStartup + "||" + System.Diagnostics.Stopwatch.GetTimestamp());
    }

    public double ActualTime => AudioSettings.dspTime - dspOffset;
}
