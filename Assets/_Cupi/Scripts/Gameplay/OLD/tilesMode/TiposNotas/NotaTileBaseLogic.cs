using UnityEngine;

public abstract class NotaTileBaseLogic : MonoBehaviour,INoteEntity
{
    protected ITimeProvider timeProvider;
    protected GameplayRenderer gameplayRenderer;

    [SerializeField] private Transform note;

    public SpriteRenderer spriteNote;
    public NotaInstance data;

    public Vector2 direccionMovimiento;
    public Vector2 finalPos;
    public Transform origin;
    public double progress;
    public bool lockProgress;
    public float offsetRendering;
    public int Myindex;
    public bool initialized;
    public float timeToLastStateUpdate;

    protected virtual void OnEnable() { }

    protected virtual void OnDisable() { }

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

        progress = 1 - InverseLerpUnclamped(0.0f, data.timeToArrive + offsetRendering, timeProvider.GetCurrentTime());

        if (lockProgress) progress = Mathf.Max(0);

        double distancia = (progress * (data.timeToArrive + offsetRendering) * data.localSpeed * gameplayRenderer.scrollSpeed);

        finalPos = data.offsetPositionToGo + (direccionMovimiento * (float)distancia);

        note.localPosition = finalPos;

        //esto hace que las notas se destruyan cuando se esta en reversa
        /*
        if (data.timeToArrive > spawnerNotas.NotesWindowEnd)
        {
            if (data.tipoNota == TipoNota.Sostenida)
            {
                print("destruccion por reversa");
            }

            DestroyNote();
        }
        */
    }

    public void DestroyNote()
    {
        GoToPool();
    }

    public double InverseLerpUnclamped(double a, double b, double valor)
    {
        if (b != a) return (valor - a) / (b - a);

        return 0.0f;
    }

    public void GoToPool()
    {
        //Debug.Log($"Pooling note {Myindex} {data.noteIndex}");
        //spawnerNotas.RemoveNote(Myindex);

        SetDefaultConfig();

        Debug.Log("quitando nota");

        gameObject.SetActive(false);
    }

    protected virtual void SetDefaultConfig()
    {
        Myindex = -1;
        transform.parent = origin;
        transform.position = Vector2.one * 1000.0f;
        initialized = false;
        finalPos = Vector2.one * 1000.0f;
    }

    protected virtual void PostInitialize()
    {

    }

    public virtual void ChangeNoteState(EstadoPuntuacion puntuacion, EstadoNota estado)
    {
        
    }

    public void DespawnNote()
    {
        throw new System.NotImplementedException();
    }

    public void InitializeNote(NoteIntialData intialData)
    {
        data = intialData.data;

        Myindex = data.noteIndex;

        origin = intialData.origin;

        direccionMovimiento = intialData.direccionMovimiento;

        timeProvider = intialData.timeProvider;

        gameplayRenderer = intialData.gameplayRenderer;

        transform.localPosition = Vector2.zero;

        PostInitialize();

        initialized = true;
    }
}
