using UnityEngine;

public class BpmController : MonoBehaviour
{
    [SerializeField] private bool animateBPM = true;
    [SerializeField] private float bpm = 120.0f;
    [SerializeField] private int tiempos = 4;
    [SerializeField] private float offset = 0.0f;
    [SerializeField] private AudioClip beatFuerte;
    [SerializeField] private AudioClip beatSuave;
    [SerializeField] private AudioSource musicPlayer;
    private double prevBeatTime;
    private float bpmAnterior = 60.0f;
    private float timeForBeat;
    private int numberBeats;

    private void UpdateTimeForBeat()
    {
        timeForBeat = 60.0f / bpm;
    }

    private void Start()
    {
        UpdateTimeForBeat();
        prevBeatTime = AudioSettings.dspTime - offset;
    }

    private void Update()
    {
        // por lo visto dsp time es la mejor opcion porque no se puede trabar o bueno eso creo
        //print(AudioSettings.dspTime + " | " + Time.time);

        if (bpmAnterior != bpm)
        {
            UpdateTimeForBeat();
            bpmAnterior = bpm;
        }

        while (AudioSettings.dspTime > prevBeatTime + timeForBeat)
        {
            prevBeatTime += timeForBeat;

            Beat();
        }

        if (animateBPM)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, 10.0f * Time.deltaTime);
        }
    }

    private void Beat()
    {
        playSoundBeat(numberBeats, tiempos);

        numberBeats += 1;

        //print("beat: " + numberBeats);
    }

    public void playSoundBeat(int numBeat , int tiempo)
    {
        if (musicPlayer == null || beatFuerte == null || beatSuave == null)
        {
            Debug.LogError("Bpm Controller debe tener asignado un audioSource y sus audioclips para funcionar correctamente.");
            return;
        }

        if (numBeat % tiempo == 0)
        {
            musicPlayer.PlayOneShot(beatFuerte);
            if (animateBPM) ScaleBPM(1.5f, 1.5f);
        }
        else
        {
            musicPlayer.PlayOneShot(beatSuave);
            if (animateBPM) ScaleBPM(1.3f,1.3f);
        }
    }

    void ScaleBPM(float scaleX, float scaleY)
    {
        transform.localScale = new Vector3(scaleX, scaleY, 1.0f);
    }

    public float BPM
    {
        get { return bpm; }
        set
        {
            if (value != bpm)
            {
                bpm = value;
                UpdateTimeForBeat();
            }
        }
    }
}
