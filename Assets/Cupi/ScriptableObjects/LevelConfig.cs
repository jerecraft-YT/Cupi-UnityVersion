using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Scriptable Objects/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [Header("Config inicio nivel")]
    public float startFadeDuration = 0.15f;

    [Space(10)]

    [Header("Base Config nivel")]

    [Tooltip("Margen de error para las notas (en segundos)")]
    public float toleranciaErrorBase = 0.5f;

    [Header("Config Modo tile")]
    public TileModePlayStyle PlayStyle = TileModePlayStyle.FourKeys;

    [Tooltip("Velocidad general de notas")]
    public float notaTileSpeed = 7;

    [Space(10)]

    [Header("Config Render")]

    [Tooltip("Separacion de objetivos de notas por defecto")]
    public float separacionObjetivosNotas = 2.0f;

    public float limiteInferiorRender = 1.0f;

    public float extraRenderSize = 3.0f;

    [Header("Config chunks")]

    [Tooltip("tamaño de los chunks en segundos")]
    public int chunkSize = 3;

    private void Reset()
    {
        startFadeDuration = 0.15f;
        toleranciaErrorBase = 0.5f;
        PlayStyle = TileModePlayStyle.FourKeys;
        notaTileSpeed = 7;
        separacionObjetivosNotas = 2.0f;
        limiteInferiorRender = 1.0f;
        extraRenderSize = 3.0f;
        chunkSize = 3;
    }
}
