using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameplayEngine : IDisposable
{
    public event Action OnLevelEnd;

    public event Action<int,EstadoPuntuacion,EstadoNota> NoteChange;

    private RuntimeStateNote[] estadoNotas;

    private List<NotaInstance> chart;

    private List<BufferedInput> inputBuffer = new();

    private IInputDevice inputDevice;

    private ITimeProvider timeProvider;

    //el engine tiene que saber cuantos carriles hay para no juzgar notas que el renderer
    //no puede mostrar, si no las dos partes dejan de estar de acuerdo sobre que notas existen
    private TileModePlayStyle playStyleTile;

    public int startWindow;

    public int endWindow;

    //para poder procesar todas las notas si hubo un salto de tiempo muy abrupto
    private double oldSongTime;

    //para que el fin de nivel no se dispare otra vez si se rebobina y se vuelve a avanzar
    private bool levelEndInvoked;

    #region Constantes
    //publica para que el renderer pueda asegurarse de nunca mostrar menos tiempo que el engine
    public const float maxProcessTime = 2;

    const float margenPuntuacionPerfecta = 0.045f;

    const float margenPuntuacionBueno = 0.090f;

    const float margenPuntuacionMalo = 0.135f;

    const float margenPuntuacionPesimo = 0.16f;
    #endregion

    //constructor de gameplay con lo esencial
    public GameplayEngine(ITimeProvider timeProvider,IInputDevice inputDevice,List<NotaInstance> chart,TileModePlayStyle playStyleTile)
    {
        this.inputDevice = inputDevice;
        this.chart = chart;
        this.timeProvider = timeProvider;
        this.playStyleTile = playStyleTile;

        this.inputDevice.OnButtonPressed += OnButtonPressed;
        this.inputDevice.OnButtonReleased += OnButtonReleased;

        estadoNotas = new RuntimeStateNote[chart.Count];
    }

    public void Dispose()
    {
        inputDevice.OnButtonPressed -= OnButtonPressed;
        inputDevice.OnButtonReleased -= OnButtonReleased;
    }

    public void EngineTick(double songTime)
    {
        SetWindowRange(songTime);

        ProcessNotes(songTime);

        //para guardar el tiempo anterior por si pasa mucho tiempo entre ticks
        oldSongTime = songTime;
    }

    private void ProcessNotes(double songTime)
    {
        if (inputBuffer.Count != 0 && timeProvider.GetCurrentTimeScale() < 0)
        {
            inputBuffer.Clear();
        }

        foreach (var actualInput in inputBuffer)
        {
            if (!actualInput.isPressed) continue;

            CheckNoteHit(actualInput.songTime, actualInput.tecla);
        }

        inputBuffer.Clear();
        //es none para no usar ningun input ya que es para detectar errores
        CheckNoteHit(songTime, CorrespondenciaTecla.None);
    }

    private void CheckNoteHit(double songTime,CorrespondenciaTecla tecla)
    {
        bool isReversed = timeProvider.GetCurrentTimeScale() < 0;

        for (int noteIndex = startWindow; noteIndex < endWindow; noteIndex++)
        {
            ref var estadoNota = ref estadoNotas[noteIndex]; //referencia para no copiar

            NotaInstance nota = chart[noteIndex]; // para mas facil acceso a la nota actual

            //si el renderer no tiene carril para esta nota tampoco se puede juzgar aca,
            //seria puntuar (o fallar) una nota que el jugador nunca llego a ver
            if (!EsTeclaJugable(nota.correspondenciaTecla)) continue;

            TipoNota tipoNota = nota.tipoNota;

            //en reversa no se juzga ninguna nota, solo se deshace lo que el tiempo ya des-paso
            if (isReversed)
            {
                ReverseNoteReset(noteIndex, nota, songTime);
                continue;
            }

            bool isProcess = estadoNota.estadoNota == EstadoNota.Fallada || estadoNota.estadoNota == EstadoNota.Procesada || estadoNota.estadoNota == EstadoNota.ProcesoFallado;

            if (isProcess) continue; // continuar a la siguiente iteracion si la nota ya fue procesada

            double diferencia = nota.timeToArrive - songTime;
            
            if (estadoNota.estadoNota == EstadoNota.EnProceso)
            {
                diferencia = nota.timeToArrive + nota.duracion - songTime;

                bool estaEnRango = diferencia > margenPuntuacionPerfecta;

                if (!inputDevice.ClickPressed(nota.correspondenciaTecla) && estaEnRango)
                {
                    Debug.Log("soltaste muy pronto");
                    RegistrarResultado(noteIndex, EstadoNota.ProcesoFallado, EstadoPuntuacion.Fallaste);
                    continue;
                }

                if (!estaEnRango)
                {
                    RegistrarResultado(noteIndex, EstadoNota.Procesada, EstadoPuntuacion.Perfecto);
                    continue;
                }
            }
            else
            {
                if (diferencia < -margenPuntuacionPerfecta)
                {
                    Debug.Log("no le diste a tiempo");
                    RegistrarResultado(noteIndex, EstadoNota.Fallada, EstadoPuntuacion.Fallaste);
                    continue;
                }
            }

            if (nota.correspondenciaTecla != tecla) continue; // continuar si no coincide la tecla con la nota

            EstadoPuntuacion puntuacion = EstadoPuntuacion.None;

            switch (tipoNota)
            {
                case TipoNota.None:
                    break;
                case TipoNota.Normal:
                    puntuacion = ObtenerPuntaje(diferencia);

                    break;
                case TipoNota.Sostenida:
                    puntuacion = ObtenerPuntaje(diferencia);

                    break;
                default:
                    break;
            }

            if (puntuacion == EstadoPuntuacion.None) continue; // continuar si no estaba en el margen de puntos

            switch (tipoNota)
            {
                case TipoNota.None:
                    break;
                case TipoNota.Normal:
                    RegistrarResultado(noteIndex, EstadoNota.Procesada, puntuacion);
                    break;
                case TipoNota.Sostenida:
                    RegistrarResultado(noteIndex, EstadoNota.EnProceso, puntuacion);
                    break;
                default:
                    break;
            }

            if (tecla != CorrespondenciaTecla.None) return; //si la tecla fue presionada y fue valida la descartamos
        
        }
    }

    /// <summary>
    /// una tecla por encima del modo de juego no existe para el gameplay: el renderer no
    /// tiene donde ponerla, asi que el engine tiene que ignorarla exactamente igual
    /// </summary>
    private bool EsTeclaJugable(CorrespondenciaTecla tecla)
    {
        return (int)tecla <= (int)playStyleTile;
    }

    /// <summary>
    /// deshace el resultado de una nota cuando el tiempo retrocedio por detras de ella,
    /// asi se puede volver a jugar tal cual estaba antes
    /// </summary>
    private void ReverseNoteReset(int noteIndex,NotaInstance nota,double songTime)
    {
        //si nunca se toco no hay nada que deshacer
        if (estadoNotas[noteIndex].estadoNota == EstadoNota.None) return;

        //el momento mas temprano en el que la nota se podia tocar, mientras el tiempo
        //no pase por detras de ahi el resultado que ya tenia sigue siendo valido
        //double inicioVentanaGolpe = nota.timeToArrive - margenPuntuacionPesimo;

        
        double inicioVentanaGolpe = nota.timeToArrive;
        
        if (songTime >= inicioVentanaGolpe) return;

        RegistrarResultado(noteIndex, EstadoNota.None, EstadoPuntuacion.None);
    }

    private void RegistrarResultado(int noteIndex,EstadoNota estado, EstadoPuntuacion puntuacion)
    {
        Debug.Log(puntuacion+ "|" + estado);
        estadoNotas[noteIndex].estadoNota = estado;
        estadoNotas[noteIndex].estadoPuntuacion = puntuacion;
        NoteChange?.Invoke(noteIndex, puntuacion, estado);
    }

    private EstadoPuntuacion ObtenerPuntaje(double diferencia)
    {
        if (diferencia <= margenPuntuacionPerfecta) return EstadoPuntuacion.Perfecto;
        else if (diferencia <= margenPuntuacionBueno) return EstadoPuntuacion.Bueno;
        else if (diferencia <= margenPuntuacionMalo) return EstadoPuntuacion.Malo;
        else if (diferencia <= margenPuntuacionPesimo) return EstadoPuntuacion.Pesimo;

        return EstadoPuntuacion.None;
    }

    private void SetWindowRange(double songTime)
    {
        if (timeProvider.GetCurrentTimeScale() < 0)
        {
            ReverseWindowSetter(songTime);
            return;
        }

        NormalWindowSetter(songTime);
    }

    //en reversa los bordes cambian de rol: startWindow pasa a ser el que va llegando y endWindow
    //el que se va, por eso start mira el tiempo actual y end el anterior (al reves que en normal)
    private void ReverseWindowSetter(double songTime)
    {
        if (startWindow > 0)
        {
            while (songTime <= chart[startWindow - 1].timeToArrive + chart[startWindow - 1].duracion + maxProcessTime)
            {
                startWindow--;

                if (startWindow <= 0) break;
            }
        }

        if (endWindow > 0)
        {
            while (oldSongTime <= chart[endWindow - 1].timeToArrive - chart[endWindow - 1].duracion - maxProcessTime)
            {
                endWindow--;

                if (endWindow <= 0) break;
            }
        }

        //si el tiempo retrocede el nivel deja de estar terminado y se puede volver a avisar
        if (!IsFinalChart(startWindow)) levelEndInvoked = false;
    }

    private void NormalWindowSetter(double songTime)
    {
        //ejemplo (si song time es 2 y el chart se procesa en 2 le sumamos un tiempo de procesado extra
        //por si hay un lag y si no aumentamos el index del startWindow)
        if (!IsFinalChart(startWindow))
        {
            while (oldSongTime > chart[startWindow].timeToArrive + chart[startWindow].duracion + maxProcessTime)
            {
                startWindow++;

                if (IsFinalChart(startWindow))
                {
                    InvokeLevelEnd();
                    break;
                }
            }
        }

        if (!IsFinalChart(endWindow))
        {
            while (songTime > chart[endWindow].timeToArrive - chart[endWindow].duracion - maxProcessTime)
            {
                endWindow++;

                if (IsFinalChart(endWindow))
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// avisa del fin del nivel una sola vez, si se rebobina se vuelve a armar
    /// desde <see cref="ReverseWindowSetter"/>
    /// </summary>
    private void InvokeLevelEnd()
    {
        if (levelEndInvoked) return;

        levelEndInvoked = true;

        OnLevelEnd?.Invoke();
    }

    private bool IsFinalChart(int index)
    {
        return index >= chart.Count;
    }

    private void OnButtonPressed(CorrespondenciaTecla tecla, double customInputTime)
    {
        if (timeProvider.GetCurrentTimeScale() < 0f) return;

        //mas facil para no perder inputs antes del tick :3
        BufferedInput input = new(tecla, customInputTime == -1f ? timeProvider.GetCurrentTime() : customInputTime, true);

        inputBuffer.Add(input);

        //Debug.Log("Pressed :" + tecla);
    }

#pragma warning disable
    private void OnButtonReleased(CorrespondenciaTecla tecla, double customInputTime)
    {
        //aun no le encuentro uso al released
        return;

        //-1f es valor por defecto
        BufferedInput input = new(tecla, customInputTime == -1f ? timeProvider.GetCurrentTime() : customInputTime, false);

        inputBuffer.Add(input);

        //Debug.Log("Released :" + tecla);
    }
}
