using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController instance;

    public double dspOffset;
    //public double actualTime;

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

    }
    void Start()
    {
        RestartTime();
    }

    void Update()
    {
        //actualTime = AudioSettings.dspTime - dspOffset;
    }

    public void RestartTime()
    {
        dspOffset = AudioSettings.dspTime;
    }

    public double ActualTime => AudioSettings.dspTime - dspOffset;
}
