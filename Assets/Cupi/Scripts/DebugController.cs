using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;

    public AudioSource audioSource;

    private bool updateDebugInfo = true;

    private string textDebugInfo = "UnscaledCustomTime: {0:N2}\nCustomTime: {1:N2} \nMusicTime: {2:N2}\nTimeScale: {3:N2}\nPosicionMouse: {4:N2}";

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
            audioSource.time,
            TimeController.instance.TimeScale,
            RadialModeInputController.instance.mouseMovement.action.ReadValue<Vector2>()
            );

        yield return new WaitForSecondsRealtime(0.1f);

        updateDebugInfo = true;
    }
    public void ChangeTimeScale(float valor)
    {
        TimeController.instance.TimeScale = valor;

        audioSource.pitch = valor;
    }
}
