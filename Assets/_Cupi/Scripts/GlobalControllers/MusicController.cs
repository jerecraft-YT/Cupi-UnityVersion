using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    public static MusicController instance;
    [SerializeField] private MusicDataSO config;

    [SerializeField] private bool _pitchRegulator;
    private AudioMixerGroup _musicGroup;
    private float _toleranciaSincronizacion;
    private bool _musicPaused;
    public static AudioSource mainMusic;
    public static event Action OnMusicChanged;

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

    public static async Task MusicFadeIn(float duracion)
    {
        float startVolumen = mainMusic.volume;

        for (float t = 0; t < duracion; t += Time.deltaTime)
        {
            mainMusic.volume = Mathf.Lerp(startVolumen, 0, t / duracion);
            await Awaitable.EndOfFrameAsync();
        }
        mainMusic.volume = 0;
    }

    public static async Task MusicFadeOut(float duracion)
    {
        for (float t = 0; t < duracion; t += Time.deltaTime)
        {
            mainMusic.volume = Mathf.Lerp(0.0f, 1.0f, t / duracion);
            await Awaitable.EndOfFrameAsync();
        }

        mainMusic.volume = 1.0f;
    }


    private void CargarDefaultConfig()
    {
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
        _musicGroup = config.audioMixerGroup;
        _pitchRegulator = config.pitchRegulator;
        _toleranciaSincronizacion = config.toleranciaSincronizacion;
        _musicPaused = config.musicPausedDefault;
        mainMusic.outputAudioMixerGroup = _musicGroup;
    }

    private void OnEnable()
    {
        TimeController.UpdateTimeScale += UpdateMusic;
    }

    private void OnDisable()
    {
        TimeController.UpdateTimeScale -= UpdateMusic;
    }

    private void UpdateMusic()
    {
        SincronizarMusica();
        ChangePitch();
    }

    public static void PlayMusic(AudioClip audio, float startTime = 0.0f)
    {
        mainMusic.clip = audio;
        mainMusic.time = startTime;
        mainMusic.Play();

        OnMusicChanged?.Invoke();
    }

    private void PausarMusica()
    {
        if (mainMusic.clip != null)
        {
            if (_musicPaused) mainMusic.Pause();
            else mainMusic.UnPause();
        }
    }

    public void SincronizarMusica(bool conTolerancia = true)
    {
        if (_musicPaused || mainMusic.clip == null) return;

        float additiveTime = (float)TimeController.instance.AdditiveTime;

        if (!conTolerancia)
        {
            if (!mainMusic.isPlaying) mainMusic.Play();

            //Debug.Log("resincronizando musica");

            mainMusic.time = additiveTime;

            return;
        }



        if (additiveTime < 0 || additiveTime > mainMusic.clip.length) return;

        if (Mathf.Abs(additiveTime - mainMusic.time) >= Mathf.Abs(_toleranciaSincronizacion * TimeController.instance.TimeScale))
        {
            if (!mainMusic.isPlaying) mainMusic.Play();

            //Debug.Log("resincronizando musica");

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
        if (_pitchRegulator)
        {
            _musicGroup.audioMixer.SetFloat("pitchShifter", 1.0f / Mathf.Abs(mainMusic.pitch));
        }
        else
        {
            _musicGroup.audioMixer.SetFloat("pitchShifter", 1.0f);
        }
    }

    public bool PitchRegulator
    {
        get
        {
            return _pitchRegulator;
        }
        set
        {
            if (_pitchRegulator != value)
            {
                _pitchRegulator = value;
                RegulatePitch();
            }
        }
    }

    public bool MusicPaused
    {
        get
        {
            return _musicPaused;
        }
        set
        {
            if(_musicPaused != value)
            {
                _musicPaused = value;
                PausarMusica();
            }
        }
    }

}
