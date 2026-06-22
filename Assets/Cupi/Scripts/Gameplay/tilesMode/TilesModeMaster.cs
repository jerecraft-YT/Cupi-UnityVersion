using UnityEngine;

public class TilesModeMaster : MonoBehaviour
{
    public static TilesModeMaster instance;

    [Header("Config Notas")]

    public TileModePlayStyle PlayStyle;

    [Tooltip("Margen de error para las notas (en segundos) ademas variara segun la velocidad")]
    public float toleranciaErrorBase;

    [Tooltip("Velocidad general de notas")]
    public float notaTileSpeed = 4;

    [Header("Config Render")]

    [Tooltip("Separacion de objetivos de notas")]
    public float separacionObjetivosNotas = 2.0f;

    public float limiteInferiorRender = 1.0f;

    public float extraRenderSize = 2.0f;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    //es mejor asi para no calcularlo en el update
    public float toleranciaError => toleranciaErrorBase / notaTileSpeed;

    public float RenderLimit => (limiteInferiorRender / notaTileSpeed) + extraRenderSize;

    [Tooltip("marca el rango de tiempo en que las notas seran visibles")]
    public float NotesVisibleRender => ((Camera.main.orthographicSize * 2.0f) + extraRenderSize);

    public float NotesMidleVisibleRender => (Camera.main.orthographicSize + extraRenderSize);
}
