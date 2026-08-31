using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "MusicConfigSO", menuName = "Scriptable Objects/MusicConfigSO")]
public class MusicDataSO : ScriptableObject
{
    [Header("Configuracion de controlador de musica")]
    public AudioMixerGroup audioMixerGroup;
    [Tooltip("tiempo en segundos de tolerancia para resincronizar la musica")]
    public float toleranciaSincronizacion;
    public bool pitchRegulator;
    public bool musicPausedDefault;

    [Header("datos persistentes")]
    public Dictionary<string, CacheAudio> cacheAudio = new();

    public void ClearAudioCache()
    {
        cacheAudio.Clear();
    }

    public void ClearAudioCache(string exception)
    {
        //crea un array pequeñito solo con lo que queremos borrar
        ClearAudioCache(new[] { exception });
    }

    public void ClearAudioCache(IEnumerable<string> exceptions)
    {
        var exceptionsSet = exceptions.ToHashSet();

        foreach (string key in cacheAudio.Keys.ToList())
        {
            if (!exceptionsSet.Contains(key))
            {
                cacheAudio.Remove(key);
            }
        }
    }
}
