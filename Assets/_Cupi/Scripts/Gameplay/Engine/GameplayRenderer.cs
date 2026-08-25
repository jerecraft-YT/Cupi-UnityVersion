using System.Collections.Generic;
using UnityEngine;

public class GameplayRenderer : MonoBehaviour
{
    #region variables
    private ModoJuego modoJuego;
    private TileModePlayStyle playStyleTile;
    private List<NotaInstance> chart;
    private ITimeProvider timeProvider;
    //para poder procesar todas las notas si hubo un salto de tiempo muy abrupto
    private double oldSongTime;
    private int startWindow;
    private int endWindow;
    private static readonly Dictionary<DireccionesMovimientoNotas, Vector2> direccionesMovimientoNotas = new(){
        {DireccionesMovimientoNotas.Up ,Vector2.up},
        {DireccionesMovimientoNotas.Down,Vector2.down},
        {DireccionesMovimientoNotas.Left ,Vector2.left},
        {DireccionesMovimientoNotas.Right,Vector2.right}
    };
    private Dictionary<int, INoteEntity> notesToRender = new();
    private HashSet<int> notesProcess = new();
    private Dictionary<int, INoteEntity> notesToClean = new();
    private Dictionary<CorrespondenciaTecla, Transform> posicionFinalNotaTile = new();
    #endregion

    #region cosas de render
    //esto se podra cambiar dinamicamente desde eventos para ver mas o menos notas en pantalla
    [Header("RenderFeatures")]
    [SerializeField] private float timeToProcess = 3f;
    [SerializeField] private float referenceNotesSeparation = 1f;
    [SerializeField] private float scrollSpeed = 10.0f;
    [SerializeField] private float extraRenderTime = 2.0f;
    #endregion

    #region exposicion de variables
    //setter y getter para actualizar datos solo cuando se actualizan y no cada frame
    public float ScrollSpeed
    {
        get {  return scrollSpeed; }
        set
        {
            scrollSpeed = value;
        }
    }
    public float ReferenceNotesSeparation
    {
        get { return referenceNotesSeparation; }
        set
        {
            referenceNotesSeparation = value;
            UpdateRenderReferencesTransform();
        }
    }
    public float TimeToProcess
    {
        get { return timeToProcess; }
        set
        {
            timeToProcess = value;
        }
    }
    public float ExtraRenderTime
    {
        get { return  extraRenderTime; }
        set
        {
            extraRenderTime = value;
        }
    }
    #endregion

    //valores solo de debug para cambiar valores publicos de manera dinamica y optimizada :3
    private float oldReferenceNotesSeparation;

    public void Initialize(LevelComposition level,ITimeProvider timeProvider)
    {
        scrollSpeed = level.baseScrollSpeed;
        modoJuego = level.modoJuego;
        playStyleTile = level.tileModePlayStyle;
        chart = level.chart;

        this.timeProvider = timeProvider;

        SpawnRenderReferences();
    }

    private void SpawnRenderReferences()
    {
        posicionFinalNotaTile.Clear();

        int playStyleIndex = (int)playStyleTile + 1;

        for (int i = 0; i < playStyleIndex; i++)
        {
            CorrespondenciaTecla tecla = (CorrespondenciaTecla)i;

            GameObject referenciaInstanciada = new($"Referencia {tecla}");

            referenciaInstanciada.transform.SetParent(transform);
            posicionFinalNotaTile.Add(tecla, referenciaInstanciada.transform);
        }

        UpdateRenderReferencesTransform();
    }

    private void UpdateRenderReferencesTransform()
    {
        int playStyleIndex = (int)playStyleTile + 1;

        float maxDistanceReference = playStyleIndex * referenceNotesSeparation;

        for (int i = 0; i < playStyleIndex; i++)
        {
            CorrespondenciaTecla tecla = (CorrespondenciaTecla)i;

            posicionFinalNotaTile[tecla].transform.localPosition = new Vector2((i * referenceNotesSeparation) - referenceNotesSeparation, 0);
        }

    }
    // Update is called once per frame
    void Update()
    {
        foreach(INoteEntity note in notesToRender.Values)
        {
            note.UpdateNote();
        }

        foreach(INoteEntity note in notesToClean.Values)
        {
            note.UpdateNote();
        }

        #if UNITY_EDITOR
        DebugDinamicUpdate();
        #endif
    }

    private void DebugDinamicUpdate()
    {
        if (oldReferenceNotesSeparation != referenceNotesSeparation)
        {
            UpdateRenderReferencesTransform();
            oldReferenceNotesSeparation = referenceNotesSeparation;
        }
    }

    public void EngineTick(double songTime)
    {
        SetWindowRenderRange(songTime);

        oldSongTime = songTime;

        UpdateRenderQueue(songTime);

        RenderCleaner(songTime);
    }

    private List<int> indexToClean = new();

    private void RenderCleaner(double songTime)
    {
        foreach (var note in notesToClean)
        {
            //la nota ocupa un rango de tiempo (una sostenida dura), no un solo instante,
            //asi que se mide la distancia contra el rango entero y no contra el final
            (double timeToArrive, float duracion) = note.Value.GetNoteTimeData();

            bool muyAdelante = songTime > timeToArrive + duracion + extraRenderTime;

            bool muyAtras = songTime < timeToArrive - extraRenderTime;

            if (muyAdelante || muyAtras)
            {
                indexToClean.Add(note.Key);
            }
        }

        foreach(int index in indexToClean)
        {
            notesToClean[index].DespawnNote();
            notesToClean.Remove(index);
        }

        indexToClean.Clear();
    }

    private void UpdateRenderQueue(double songTime)
    {
        bool isReversed = timeProvider.GetCurrentTimeScale() < 0;

        for (int noteIndex = startWindow; noteIndex < endWindow; noteIndex++)
        {
            //si ya esta en pantalla, jugandose o esperando en la cola de limpieza,
            //no hay nada que instanciar
            if (notesToRender.ContainsKey(noteIndex) || notesToClean.ContainsKey(noteIndex)) continue;

            NotaInstance noteData = chart[noteIndex];

            //el veto de notesProcess se levanta solo si el tiempo va en reversa y volvio a
            //meterse dentro de la nota, asi una sostenida ya consumida reaparece en pantalla
            //para poder ir recuperandose. una nota normal dura 0 asi que nunca entra aca
            bool tiempoDentroDeLaNota = isReversed
                && songTime >= noteData.timeToArrive
                && songTime < noteData.timeToArrive + noteData.duracion;

            bool yaProcesada = notesProcess.Contains(noteIndex);

            if (yaProcesada && !tiempoDentroDeLaNota) continue;

            TipoNota tipoNota = noteData.tipoNota;
            ModoNota modoNota = noteData.modoNota;
            CorrespondenciaTecla tecla = noteData.correspondenciaTecla;

            //los dos descartes de abajo marcan la nota como procesada aunque no se instancie:
            //no se puede mostrar nunca, asi que no hay que reintentarlo (ni volver a avisar)
            //en cada tick mientras siga dentro de la ventana
            if ((int)tecla > (int)playStyleTile)
            {
                notesProcess.Add(noteIndex);
                Debug.LogError($"la nota {noteIndex} usa la tecla {tecla} pero el modo es {playStyleTile}, no hay carril donde ponerla");
                continue;
            }

            GameObject nota = GetNoteEntity(tipoNota,modoNota,out TipoObjetoPool tipoObjetoPool);

            if (nota == null)
            {
                notesProcess.Add(noteIndex);
                Debug.LogError($"la nota {noteIndex} no pudo salir de la pool ({tipoObjetoPool}), revisar los prefabs del PoolController");
                continue;
            }

            INoteEntity noteEntity = nota.GetComponent<INoteEntity>();

            Transform noteOrigin = PoolController.instance.RequestGroupPool(tipoObjetoPool).transform;

            Vector2 direccionMovimiento = EstablecerDireccionMovimiento(noteData.direccionMovimiento, noteData.direccionCustom);

            nota.transform.SetParent(posicionFinalNotaTile[noteData.correspondenciaTecla]);

            NoteIntialData intialData = new()
            {
                data = noteData,
                origin = noteOrigin,
                direccionMovimiento = direccionMovimiento,
                timeProvider = timeProvider,
                gameplayRenderer = this
            };

            noteEntity.InitializeNote(intialData);

            //Debug.Log("se añadio una nota");

            //las ya procesadas que reaparecen van directo a la cola de limpieza: solo tienen
            //que mostrarse mientras el tiempo las tape y despues irse solas, no son jugables
            if (yaProcesada)
            {
                notesToClean.Add(noteIndex, noteEntity);
                continue;
            }

            notesToRender.Add(noteIndex, noteEntity);
        }
    }

    private Vector2 EstablecerDireccionMovimiento(DireccionesMovimientoNotas DireccionMovimiento, Vector2 DireccionCustom)
    {
        if (DireccionMovimiento == DireccionesMovimientoNotas.Custom) return DireccionCustom;

        return direccionesMovimientoNotas[DireccionMovimiento];
    }

    private GameObject GetNoteEntity(TipoNota tipoNota, ModoNota modoNota, out TipoObjetoPool tipoObjetoPool)
    {
        tipoObjetoPool = TipoObjetoPool.None;

        switch (modoNota)
        {
            case ModoNota.None:
                NoValidNote();
                break;

            case ModoNota.Tile:

                switch (tipoNota)
                {
                    case TipoNota.None:
                        NoValidNote();
                        break;
                    case TipoNota.Normal:
                        tipoObjetoPool = TipoObjetoPool.NotaNormalTile;
                        break;
                    case TipoNota.Sostenida:
                        tipoObjetoPool = TipoObjetoPool.NotaSostenidaTile;
                        break;
                }
                break;
            case ModoNota.Radial:
                switch (tipoNota)
                {
                    case TipoNota.None:
                        NoValidNote();
                        break;
                    case TipoNota.Normal:
                        tipoObjetoPool = TipoObjetoPool.NotaNormalRadial;
                        break;
                    case TipoNota.Sostenida:
                        tipoObjetoPool = TipoObjetoPool.NotaSostenidaRadial;
                        break;
                }
                break;
            default:
                NoValidNote();
                break;
        }

        return PoolController.instance.RequestInstance(tipoObjetoPool);
    }

    private void NoValidNote()
    {
        Debug.LogWarning("este chart tiene una nota no valida");
    }

    private void SetWindowRenderRange(double songTime)
    {
        //el render nunca puede ver menos tiempo que el engine, si no habria notas
        //procesadas que ya no tienen objeto en pantalla
        timeToProcess = Mathf.Max(timeToProcess, GameplayEngine.maxProcessTime);

        extraRenderTime = Mathf.Max(extraRenderTime, GameplayEngine.maxProcessTime);

        if (timeProvider.GetCurrentTimeScale() < 0)
        {
            ReverseWindowSetter(songTime);
            return;
        }

        NormalWindowSetter(songTime);
    }

    //en reversa los bordes cambian de rol: startWindow pasa a ser el que va llegando y endWindow
    //el que se va, por eso start mira el tiempo actual y end el anterior (al reves que en normal).
    //tiene que ser igual que el ReverseWindowSetter del engine o las ventanas se desincronizan
    private void ReverseWindowSetter(double songTime)
    {
        if (startWindow > 0)
        {
            while (songTime <= chart[startWindow - 1].timeToArrive + chart[startWindow - 1].duracion + timeToProcess)
            {
                startWindow--;

                if (startWindow <= 0) break;
            }
        }

        if (endWindow > 0)
        {
            while (oldSongTime <= chart[endWindow - 1].timeToArrive - chart[endWindow - 1].duracion - timeToProcess)
            {
                endWindow--;

                if (endWindow <= 0) break;
            }
        }
    }

    private void NormalWindowSetter(double songTime)
    {
        //ejemplo (si song time es 2 y el chart se procesa en 2 le sumamos un tiempo de procesado extra
        //por si hay un lag y si no aumentamos el index del startWindow)
        if (!IsFinalChart(startWindow))
        {
            while (oldSongTime > chart[startWindow].timeToArrive + chart[startWindow].duracion + timeToProcess)
            {
                startWindow++;

                if (IsFinalChart(startWindow)) break;
            }
        }

        if (!IsFinalChart(endWindow))
        {
            while (songTime > chart[endWindow].timeToArrive - chart[endWindow].duracion - timeToProcess)
            {
                endWindow++;

                if (IsFinalChart(endWindow)) break;
            }
        }
    }

    private bool IsFinalChart(int index)
    {
        return index >= chart.Count;
    }

    public void NoteChange(int index, EstadoPuntuacion puntuacion, EstadoNota estadoNota)
    {
        //el engine deshizo la nota porque el tiempo retrocedio por detras de ella,
        //esto no es una desincronizacion asi que se atiende antes de cualquier warning
        if (estadoNota == EstadoNota.None)
        {
            ResetProcess(index);
            return;
        }

        if (!notesToRender.ContainsKey(index))
        {
            Debug.LogWarning($"El engine proceso la nota {index} pero el renderer no la tenia activa. Revisar sincronizacion de ventanas.");
            return;
        }

        notesToRender[index].ChangeNoteState(puntuacion, estadoNota);

        switch (estadoNota)
        {
            //asi se pueden agrugar varios cases en uno solo :O
            case EstadoNota.ProcesoFallado:
            case EstadoNota.Fallada:
            case EstadoNota.Procesada:
                AddToCleanProcess(index);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// devuelve la nota al estado de "todavia no jugada", este en la cola que este,
    /// para que <see cref="UpdateRenderQueue"/> la pueda volver a instanciar desde cero
    /// </summary>
    private void ResetProcess(int index)
    {
        //siempre se levanta el veto, aunque la nota ya no estuviera en ninguna cola porque
        //RenderCleaner se le adelanto, si no la nota no volveria a instanciarse nunca
        notesProcess.Remove(index);

        //si todavia tiene objeto en pantalla basta con avisarle del cambio,
        //no hace falta devolverla al pool para volver a pedirla en el mismo frame
        if (notesToRender.TryGetValue(index, out INoteEntity notaActiva))
        {
            notaActiva.ChangeNoteState(EstadoPuntuacion.None, EstadoNota.None);
            return;
        }

        //si estaba en la cola de limpieza se devuelve al pool para que vuelva a salir
        //entera desde cero por UpdateRenderQueue
        if (notesToClean.TryGetValue(index, out INoteEntity notaEnCola))
        {
            notaEnCola.DespawnNote();
            notesToClean.Remove(index);
        }

        Debug.Log($"se reseteo la nota {index} del renderer");
    }

    private void AddToCleanProcess(int index)
    {
        //el indexer y no Add porque una nota puede volver a procesarse despues de un rebobinado
        notesToClean[index] = notesToRender[index];
        notesToRender.Remove(index);
        notesProcess.Add(index);
    }
}