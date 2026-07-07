using UnityEngine;

public class NotaTileBaseLogic : MonoBehaviour
{
    protected TimeController timeController;
    protected TilesModeMaster tilesModeMaster;
    protected SpawnerNotas spawnerNotas;

    [SerializeField] private Transform note;

    public SpriteRenderer spriteNote;
    public NotaInstance data;

    public Vector2 DireccionMovimiento;
    public Vector2 finalPos;
    public Transform origin;
    public float progress;
    public bool lockProgress;
    public float offsetRendering;
    public int Myindex;
    public bool initialized;

    private void Awake()
    {
        timeController = TimeController.instance;
        tilesModeMaster = TilesModeMaster.instance;
        spawnerNotas = SpawnerNotas.instance;
    }

    protected virtual void OnEnable()
    {
        NotesController.NotasActivas += UpdateNote;
    }
    protected virtual void OnDisable()
    {
        NotesController.NotasActivas -= UpdateNote;
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
        if (!initialized) return;

        progress = 1 - InverseLerpUnclamped(0.0f, data.timeToArrive + offsetRendering, (float)timeController.AdditiveTime);

        if (lockProgress) progress = Mathf.Max(0, progress);

        float distancia = (progress * (data.timeToArrive + offsetRendering) * data.localSpeed * tilesModeMaster.notaTileSpeed);

        finalPos = data.offsetPositionToGo + (DireccionMovimiento * distancia);

        note.localPosition = finalPos;

        //esto hace que las notas se destruyan cuando se esta en reversa
        if (data.timeToArrive > spawnerNotas.NotesWindowEnd)
        {
            print("destruccion por reversa");
            DestroyNote();
        }
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
        //Debug.Log($"Pooling note {Myindex} {data.noteIndex}");
        spawnerNotas.RemoveNote(Myindex);
        Myindex = -1;
        transform.parent = origin;
        initialized = false;
        gameObject.SetActive(false);
    }
    public void Initialize(NotaInstance config)
    {
        data = config;

        Myindex = config.noteIndex;

        PostInitialize();

        initialized = true;
    }

    protected virtual void PostInitialize()
    {

    }
}
