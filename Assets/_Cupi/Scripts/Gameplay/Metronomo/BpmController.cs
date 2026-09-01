using System;
using UnityEngine;

public class BpmController : MonoBehaviour
{
    [SerializeField] private bool animateBPM = true;
    [SerializeField] private float fuerzaMaxBeat = 1.4f;
    [SerializeField] private float fuerzaMinBeat = 1.2f;
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
    public static Action OnBeat;
    public bool puedeSincronizar = true;

    private void UpdateTimeForBeat()
    {
        timeForBeat = 60.0f / bpm;
    }

    private void Start()
    {
        UpdateTimeForBeat();
        ResetBpm();
    }

    public void ResetBpm(float customStart = 0.0f)
    {
        if (customStart != 0.0f)
        {
            prevBeatTime = customStart;
            return;
        }

        prevBeatTime = TimeController.instance.AdditiveTime - offset;
    }

    private void FixedUpdate()
    {
        if (bpmAnterior != bpm)
        {
            UpdateTimeForBeat();
            bpmAnterior = bpm;
        }

        while (TimeController.instance.AdditiveTime > prevBeatTime + timeForBeat)
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

        if (puedeSincronizar) MusicController.instance.SincronizarMusica();

        OnBeat?.Invoke();
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
            if (animateBPM) ScaleBPM(fuerzaMaxBeat, fuerzaMaxBeat);
        }
        else
        {
            musicPlayer.PlayOneShot(beatSuave);
            if (animateBPM) ScaleBPM(fuerzaMinBeat, fuerzaMinBeat);
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
