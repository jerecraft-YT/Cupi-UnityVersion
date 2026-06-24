using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    public AudioSource audioSource;

    private bool updateDebugInfo = true;

    private string textDebugInfo = "UnscaledCustomTime: {0:N2}\nCustomTime: {1:N2} \nMusicTime: {2:N2}\nTimeScale: {3:N2}";

    private void Start()
    {
        if (MusicController.instance.mainMusic.clip != null) audioSource = MusicController.instance.mainMusic;
    }

    private void Update()
    {
        if (updateDebugInfo)
        {
            StartCoroutine(UpdateDebug());
            updateDebugInfo = false;
        }
    }
    private IEnumerator UpdateDebug()
    {
        textMeshPro.text = string.Format(
            textDebugInfo,
            TimeController.instance.ActualTime,
            TimeController.instance.AdditiveTime,
            audioSource != null ? audioSource.time : "no hay musica XD",
            TimeController.instance.TimeScale
            );

        yield return new WaitForSecondsRealtime(0.1f);

        updateDebugInfo = true;
    }
    public void ChangeTimeScale(float valor)
    {
        TimeController.instance.TimeScale = (float)Math.Round(valor,2);
        
        if (audioSource == null) return;
        audioSource.pitch = valor;
    }
}
