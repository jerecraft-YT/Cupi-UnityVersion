using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textoDebug;
    [SerializeField] private float _timeUpdateDebugInfo;

    private AudioSource _audioSource;

    private bool _updateDebugInfo = true;

    private const string TEXT_DEBUG_INFO = "UnscaledCustomTime: {0:N2}\nCustomTime: {1:N2} \nMusicTime: {2:N2}\nTimeScale: {3:N2}";

    private void Start()
    {
        MusicController.OnMusicChanged += setMainMusic;
        
        InputController.instance.gameInputs.UI.Enable();

        InputController.instance.gameInputs.UI.ScrollWheel.performed += ScrollMouse;
    }

    private void OnDisable()
    {
        MusicController.OnMusicChanged -= setMainMusic;
    }

    private void setMainMusic()
    {
        _audioSource = MusicController.mainMusic.clip != null ? MusicController.mainMusic : null;
    }

    private async void Update()
    {
        if (_updateDebugInfo)
        {
            _updateDebugInfo = false;
            await UpdateDebug();
        }
    }

    private void ScrollMouse(InputAction.CallbackContext ctx)
    {
        TimeController.instance.TimeScale += ctx.ReadValue<Vector2>().y * 0.1f;

        ChangeTimeScale(TimeController.instance.TimeScale);
    }

    private async Task UpdateDebug()
    {
        _textoDebug.text = string.Format(
            TEXT_DEBUG_INFO,
            TimeController.instance.ActualTime,
            TimeController.instance.AdditiveTime,
            _audioSource != null ? _audioSource.time : "no hay musica XD",
            TimeController.instance.TimeScale
            );

        await Awaitable.WaitForSecondsAsync(_timeUpdateDebugInfo);

        _updateDebugInfo = true;
    }
    public void ChangeTimeScale(float valor)
    {
        TimeController.instance.TimeScale = (float)Math.Round(valor,2);
        
        if (_audioSource == null) return;
        _audioSource.pitch = valor;
    }
}
