using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    private AudioSource mainMusic;
    public static MusicController instance;
    private bool pitchRegulator;
    private AudioMixerGroup musicGroup;
    private float toleranciaSincronizacion;
    private bool musicPaused;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        CargarDefaultConfig();
    }

    private void CargarDefaultConfig()
    {
        MusicConfig config = Resources.Load<MusicConfig>("ScriptableObjects/MusicBaseConfig");

        if (config == null)
        {
            Debug.LogError("configuracion de musica no encontrado");
            enabled = false;
            return;
        }
        if (config.audioMixerGroup == null)
        {
            Debug.LogError("audioMixerGroup de musica no encontrado");
            enabled = false;
            return;
        }

        mainMusic = GetComponent<AudioSource>();
        musicGroup = config.audioMixerGroup;
        pitchRegulator = config.pitchRegulator;
        toleranciaSincronizacion = config.toleranciaSincronizacion;
        musicPaused = config.musicPausedDefault;
        mainMusic.outputAudioMixerGroup = musicGroup;
    }

    private void OnEnable()
    {
        TimeController.updateTimeScale += UpdateMusic;
    }

    private void OnDisable()
    {
        TimeController.updateTimeScale -= UpdateMusic;
    }

    private void UpdateMusic()
    {
        SincronizarMusica();
        ChangePitch();
    }

    public void PlayMusic(AudioClip audio, float startTime = 0.0f)
    {
        mainMusic.clip = audio;
        mainMusic.time = startTime;
        mainMusic.Play();
    }

    private void PausarMusica()
    {
        if (mainMusic.clip != null)
        {
            if (musicPaused) mainMusic.Pause();
            else mainMusic.UnPause();
        }
    }

    public void SincronizarMusica()
    {
        if (musicPaused || mainMusic.clip == null) return;

        float additiveTime = (float)TimeController.instance.AdditiveTime;

        if (additiveTime < 0 || additiveTime > mainMusic.clip.length) return;

        if (Mathf.Abs(additiveTime - mainMusic.time) >= toleranciaSincronizacion)
        {
            if (!mainMusic.isPlaying) mainMusic.Play();

            Debug.Log("resincronizando musica");

            mainMusic.time = additiveTime;
        }
    }

    private void ChangePitch()
    {
        mainMusic.pitch = TimeController.instance.TimeScale;
        RegulatePitch();
    }

    private void RegulatePitch()
    {
        if (pitchRegulator)
        {
            musicGroup.audioMixer.SetFloat("pitchShifter", 2.0f - Mathf.Abs(mainMusic.pitch));
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

    public bool MusicPaused
    {
        get
        {
            return musicPaused;
        }
        set
        {
            if(musicPaused != value)
            {
                musicPaused = value;
                PausarMusica();
            }
        }
    }

}
