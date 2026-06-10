using UnityEngine;

public class NotaTileBaseLogic : MonoBehaviour
{
    protected TimeController timeController;
    protected TilesModeMaster tilesModeMaster;

    [SerializeField] private Transform note;
    public NotaInstance data;
    public Vector2 DireccionMovimiento;
    public Vector2 finalPos;
    public Transform origin;
    private float progress;
    public bool lockProgress;
    public float offsetRendering;

    private void Awake()
    {
        timeController = TimeController.instance;
        tilesModeMaster = TilesModeMaster.instance;
    }

    protected virtual void OnEnable()
    {
        TilesModeNotesController.NotasActivas += UpdateNote;
        NoteVisualUpdate();
    }
    protected virtual void OnDisable()
    {
        TilesModeNotesController.NotasActivas -= UpdateNote;
    }
    public void UpdateNote()
    {
        LogicUpdate();
        NoteVisualUpdate();
    }
    protected virtual void LogicUpdate()
    {
    }

    public void NoteVisualUpdate()
    {
        progress = 1 - InverseLerpUnclamped(0.0f, data.timeToArrive + offsetRendering, (float)TimeController.instance.AdditiveTime);

        if (lockProgress) progress = Mathf.Max(0, progress);

        float distancia = (progress * (data.timeToArrive + offsetRendering) * data.localSpeed * TilesModeMaster.instance.notaTileSpeed);

        finalPos = data.offsetPositionToGo + (DireccionMovimiento * distancia);

        note.localPosition = finalPos;
    }

    public void DestroyNote()
    {
        GoToPool();
    }

    public float InverseLerpUnclamped(float a, float b, float valor)
    {
        if (b != a) return (valor - a) / (b - a);

        return 0.0f;
    }

    public void GoToPool()
    {
        transform.parent = origin;
        gameObject.SetActive(false);
    }
    public void Initialize(NotaInstance config)
    {
        data = config;

        PostInitialize();
    }

    protected virtual void PostInitialize()
    {

    }
}
