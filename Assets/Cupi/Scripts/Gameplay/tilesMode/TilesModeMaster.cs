using UnityEngine;

public class TilesModeMaster : MonoBehaviour
{
    public static TilesModeMaster instance;

    public SpriteRenderer sprite;

    [Header("Config Notas")]

    public TileModePlayStyle PlayStyle;

    [Tooltip("Margen de error para las notas (en segundos) ademas variara segun la velocidad")]
    public float toleranciaError;

    private float toleranciaErrorBase;

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

        toleranciaErrorBase = toleranciaError;
    }

    private void Update()
    {
        //actualizacion en tiempo real de la toleracia :3
        toleranciaError = toleranciaErrorBase / notaTileSpeed;

        sprite.transform.localScale = new Vector3 (sprite.transform.localScale.x, NotesVisibleRenderSize, sprite.transform.localScale.z);
    }

    public float RenderLimit => (limiteInferiorRender / notaTileSpeed) + extraRenderSize;

    public float NotesVisibleRenderSize => ((Camera.main.orthographicSize * 2.0f) + extraRenderSize) / notaTileSpeed;



}
