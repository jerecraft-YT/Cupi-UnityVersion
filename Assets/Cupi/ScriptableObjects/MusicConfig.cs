using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "MusicConfig", menuName = "Scriptable Objects/MusicConfig")]
public class MusicConfig : ScriptableObject
{
    [Header("Configuracion de controlador de musica")]
    public AudioMixerGroup audioMixerGroup;
    [Tooltip("tiempo en segundos de tolerancia para resincronizar la musica")]
    public float toleranciaSincronizacion;
    public bool pitchRegulator;
    public bool musicPausedDefault;
}
