using UnityEngine;

public class TilesModeMaster : MonoBehaviour
{
    public static TilesModeMaster instance;

    [Tooltip("Modo de juego actual de tile mode")]
    private TileModePlayStyle playStyle;

    [Tooltip("Margen de error para las notas (en segundos)")]
    private float toleranciaErrorBase;

    [Tooltip("Velocidad general de notas")]
    private float notaTileSpeed;

    [Tooltip("Separacion de objetivos de notas")]
    private float separacionObjetivosNotas;

    private float limiteInferiorRender;

    private float extraRenderSize;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        SetDefaultConfig();
    }

    private void SetDefaultConfig()
    {
        LevelConfigSO levelConfig = LevelDataController.defaultLevelConfig;

        playStyle = levelConfig.PlayStyle;

        toleranciaErrorBase = levelConfig.toleranciaErrorBase;

        notaTileSpeed = levelConfig.notaTileSpeed;

        separacionObjetivosNotas = levelConfig.separacionObjetivosNotas;

        limiteInferiorRender = levelConfig.limiteInferiorRender;

        extraRenderSize = levelConfig.extraRenderSize;
    }



    public float ExtraRenderSize => extraRenderSize;
    public float SeparacionObjetivosNotas => separacionObjetivosNotas;
    public TileModePlayStyle PlayStyle => playStyle;
    public float NotaTileSpeed => notaTileSpeed;
    //es mejor asi para no calcularlo en el update
    public float ToleranciaError => toleranciaErrorBase;
    public float RenderLimit => (limiteInferiorRender / NotaTileSpeed) + ExtraRenderSize;
    public float NotesVisibleRender => ((Camera.main.orthographicSize * 2.0f) + ExtraRenderSize);
    public float NotesMidleVisibleRender => (Camera.main.orthographicSize + ExtraRenderSize);
}
