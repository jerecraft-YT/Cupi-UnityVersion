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

    public void UpdateNote()
    {
        LogicUpdate();
        NoteVisualUpdate();
    }

    /// <summary>
    /// logica sobreescribible de la nota actual (reemplazar por lo que necesites)
    /// </summary>
    protected virtual void LogicUpdate()
    {

    }

    protected void NoteVisualUpdate()
    {
        if (!initialized) return;

        progress = 1 - InverseLerpUnclamped(0.0f, data.timeToArrive + offsetRendering, timeProvider.GetCurrentTime());

        if (lockProgress) progress = Mathf.Max(0);

        double distancia = (progress * (data.timeToArrive + offsetRendering) * data.localSpeed * gameplayRenderer.scrollSpeed);

        finalPos = data.offsetPositionToGo + (direccionMovimiento * (float)distancia);

        note.localPosition = finalPos;
    }

    public NotaInstance GetNoteData()
    {
        return data;
    }

    public (double timeToArrive, float duracion) GetNoteTimeData()
    {
        return (data.timeToArrive,data.duracion);
    }

    public double GetTimeToDespawn()
    {
        return data.timeToArrive + data.duracion;
    }

    //Por temas de conveniencia solo el render puede decidir cuando quitar la nota,
    //los scripts que hereden solo deben deshabilitar su render pero no quitarlo

    /// <summary>
    /// funcion que fuerza el despawneo de una nota
    /// </summary>
    public void DespawnNote()
    {
        //reseteamos todo antes de quitarlo
        ResetNoteData();

        Debug.Log("quitando nota");

        gameObject.SetActive(false);
    }

    /// <summary>
    /// funcion alternativa a <see cref="Mathf.InverseLerp"/> que no devuelve un valor clampeado
    /// </summary>
    protected double InverseLerpUnclamped(double a, double b, double valor)
    {
        if (b != a) return (valor - a) / (b - a);

        return 0.0f;
    }

    /// <summary>
    /// funcion que se llamara al quitar una nota,
    /// esto puede sobreescribirse para agregar mas comportamientos de reinicio
    /// </summary>
    protected virtual void ResetNoteData()
    {
        Myindex = -1;
        transform.parent = origin;
        transform.position = Vector2.one * 1000.0f;
        initialized = false;
        finalPos = Vector2.one * 1000.0f;
    }

    /// <summary>
    /// funcion sobreescribible que puede usarse para reiniciar variables de la nota
    /// </summary>
    protected virtual void PostInitialize()
    {
    }

    /// <summary>
    /// funcion que se llama cuando una nota cambia de estado, la gestion de llamadas es controlada por <see cref="GameplayEngine"/>
    /// </summary>
    public virtual void ChangeNoteState(EstadoPuntuacion puntuacion, EstadoNota estado)
    {
        //esto usaran los scripts que lo hereden para reaccionar a los cambios
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

        initialized = true;

        PostInitialize();
    }
}
