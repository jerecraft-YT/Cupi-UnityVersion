using UnityEngine;

public class TilesModeMaster : MonoBehaviour
{
    public static TilesModeMaster instance;

    [Header("Config")]

    [Tooltip("Margen de error para las notas (en segundos)")]
    public float toleranciaError;

    [Tooltip("Velocidad general de notas")]
    public float notaTileSpeed = 4;

    [Tooltip("Separacion de objetivos de notas")]
    public float separacionObjetivosNotas = 2.0f;

    public TileModePlayStyle PlayStyle;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

}
