using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "MusicConfigSO", menuName = "Scriptable Objects/MusicConfigSO")]
public class MusicConfigSO : ScriptableObject
{
    [Header("Configuracion de controlador de musica")]
    public AudioMixerGroup audioMixerGroup;
    [Tooltip("tiempo en segundos de tolerancia para resincronizar la musica")]
    public float toleranciaSincronizacion;
    public bool pitchRegulator;
    public bool musicPausedDefault;
}
