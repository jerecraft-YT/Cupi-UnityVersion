using UnityEngine;

public class MusicLevelManager : MonoBehaviour
{
    private bool musicIsPlaying = false;
    private TimeController _timeController;
    private AudioSource _mainMusic;

    private float startFadeDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _timeController = TimeController.instance;
        _mainMusic = MusicController.mainMusic;

        SetDefaultConfig();
    }

    private void SetDefaultConfig()
    {
        startFadeDuration = LevelDataController.defaultLevelConfig.startFadeDuration;
    }

    // Update is called once per frame
    void Update()
    {
        if (_mainMusic.clip == null) return;

        if (_mainMusic.time > 0.0f && _timeController.AdditiveTime > 0.0f && !musicIsPlaying)
        {
            musicIsPlaying = true;

            MusicController.instance.SincronizarMusica(false);

            FadeOutMusic();
        }
    }

    private async void FadeOutMusic()
    {
        await MusicController.MusicFadeOut(startFadeDuration);
    }
}
