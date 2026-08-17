using System.Collections.Generic;
using UnityEngine;

public class GameplayRenderer : MonoBehaviour
{
    public float scrollSpeed;
    private ModoJuego modoJuego;
    private TileModePlayStyle playStyleTile;
    private List<NotaInstance> chart;
    private ITimeProvider timeProvider;

    private Dictionary<int, INoteEntity> notesToRender = new();

    public int startWindow;
    public float timeToProcess = 2;
    public int endWindow;

    //para poder procesar todas las notas si hubo un salto de tiempo muy abrupto
    public double oldSongTime;

    private static readonly Dictionary<DireccionesMovimientoNotas, Vector2> direccionesMovimientoNotas = new(){
        {DireccionesMovimientoNotas.Up ,Vector2.up},
        {DireccionesMovimientoNotas.Down,Vector2.down},
        {DireccionesMovimientoNotas.Left ,Vector2.left},
        {DireccionesMovimientoNotas.Right,Vector2.right}
    };

    public void Initialize(LevelComposition level,ITimeProvider timeProvider)
    {
        scrollSpeed = level.baseScrollSpeed;
        modoJuego = level.modoJuego;
        playStyleTile = level.tileModePlayStyle;
        chart = level.chart;

        this.timeProvider = timeProvider;
    }

    // Update is called once per frame
    void Update()
    {
        foreach(INoteEntity note in notesToRender.Values)
        {
            note.UpdateNote();
        }
    }

    public void EngineTick(double songTime)
    {
        SetWindowRenderRange(songTime);

        oldSongTime = songTime;

        UpdateRenderQueue();
    }

    private void UpdateRenderQueue()
    {
        for (int noteIndex = startWindow; noteIndex < endWindow; noteIndex++)
        {
            if (notesToRender.ContainsKey(noteIndex)) continue;

            NotaInstance noteData = chart[noteIndex];

            TipoNota tipoNota = noteData.tipoNota;
            ModoNota modoNota = noteData.modoNota;
            CorrespondenciaTecla tecla = noteData.correspondenciaTecla;

            if ((int)tecla > (int)playStyleTile) continue;

            GameObject nota = GetNoteEntity(tipoNota,modoNota,out TipoObjetoPool tipoObjetoPool);

            if (nota == null) continue;

            INoteEntity noteEntity = nota.GetComponent<INoteEntity>();

            Transform noteOrigin = PoolController.instance.RequestGroupPool(tipoObjetoPool).transform;

            Vector2 direccionMovimiento = EstablecerDireccionMovimiento(noteData.direccionMovimiento, noteData.direccionCustom);

            nota.transform.SetParent(transform);

            NoteIntialData intialData = new()
            {
                data = noteData,
                origin = noteOrigin,
                direccionMovimiento = direccionMovimiento,
                timeProvider = timeProvider,
                gameplayRenderer = this
            };

            noteEntity.InitializeNote(intialData);

            Debug.Log("se añadio una nota");
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
        }

        return PoolController.instance.RequestInstance(tipoObjetoPool);
    }

    private void NoValidNote()
    {
        Debug.LogWarning("este chart tiene una nota no valida");
    }

    private void SetWindowRenderRange(double songTime)
    {
        //ejemplo (si song time es 2 y el chart se procesa en 2 le sumamos un tiempo de procesado extra
        //por si hay un lag y si no aumentamos el index del startWindow)
        if (!IsFinalChart(startWindow))
        {
            while (oldSongTime > chart[startWindow].timeToArrive + chart[startWindow].duracion + timeToProcess)
            {
                startWindow++;

                if (IsFinalChart(startWindow))
                {
                    break;
                }
            }
        }

        if (!IsFinalChart(endWindow))
        {
            while (songTime > chart[endWindow].timeToArrive - chart[endWindow].duracion - timeToProcess)
            {
                endWindow++;

                if (IsFinalChart(endWindow))
                {
                    break;
                }
            }
        }
    }

    private bool IsFinalChart(int index)
    {
        return index >= chart.Count;
    }

    public void NoteChange(int index, EstadoPuntuacion puntuacion, EstadoNota estadoNota)
    {
        if (!notesToRender.ContainsKey(index))
        {
            Debug.LogWarning($"El engine proceso la nota {index} pero el renderer no la tenia activa. Revisar sincronizacion de ventanas.");
            return;
        }

        notesToRender[index].ChangeNoteState(puntuacion, estadoNota);
        
        //notesToRender.Remove(index);
    }
}