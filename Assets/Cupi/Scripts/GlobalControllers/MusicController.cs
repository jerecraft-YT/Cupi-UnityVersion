using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
    private AudioSource MainMusic;
    public static MusicController instance;
    [SerializeField] private bool pitchRegulator;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private float toleranciaSincronizacion = 0.01f;
    [SerializeField] private bool musicPaused = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        MainMusic = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        TimeController.updateTimeScale += SincronizarMusica;
        TimeController.updateTimeScale += ChangePitch;
    }

    private void OnDisable()
    {
        TimeController.updateTimeScale -= SincronizarMusica;
        TimeController.updateTimeScale -= ChangePitch;
    }

    public void PlayMusic(AudioClip audio)
    {
        MainMusic.generator = audio;
    }

    public void SincronizarMusica()
    {
        if (musicPaused || MainMusic.clip == null) return;

        float additiveTime = (float)TimeController.instance.AdditiveTime;

        if (additiveTime < 0 || additiveTime > MainMusic.clip.length) return;

        if (Mathf.Abs(additiveTime - MainMusic.time) >= toleranciaSincronizacion)
        {
            if (!MainMusic.isPlaying) MainMusic.Play();

            Debug.Log("resincronizando musica");

            MainMusic.time = additiveTime;
        }
    }

    public void ChangePitch()
    {
        MainMusic.pitch = TimeController.instance.TimeScale;
        RegulatePitch();
    }

    private void RegulatePitch()
    {


        if (pitchRegulator)
        {
            musicGroup.audioMixer.SetFloat("pitchShifter", 2.0f - Mathf.Abs(MainMusic.pitch));
        }
        else
        {
            musicGroup.audioMixer.SetFloat("pitchShifter", 1.0f);
        }
    }

    public bool PitchRegulator
    {
        get
        {
            return pitchRegulator;
        }
        set
        {
            if (pitchRegulator != value)
            {
                pitchRegulator = value;
                RegulatePitch();
            }
        }
    }

}
