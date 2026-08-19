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

    public int startWindow;

    public int endWindow;

    //para poder procesar todas las notas si hubo un salto de tiempo muy abrupto
    private double oldSongTime;

    #region Constantes
    const float maxProcessTime = 2;

    const float margenPuntuacionPerfecta = 0.045f;

    const float margenPuntuacionBueno = 0.090f;

    const float margenPuntuacionMalo = 0.135f;

    const float margenPuntuacionPesimo = 0.16f;
    #endregion

    //constructor de gameplay con lo esencial
    public GameplayEngine(ITimeProvider timeProvider,IInputDevice inputDevice,List<NotaInstance> chart)
    {
        this.inputDevice = inputDevice;
        this.chart = chart;
        this.timeProvider = timeProvider;

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
        for (int noteIndex = startWindow; noteIndex < endWindow; noteIndex++)
        {
            ref var estadoNota = ref estadoNotas[noteIndex]; //referencia para no copiar

            NotaInstance nota = chart[noteIndex]; // para mas facil acceso a la nota actual

            TipoNota tipoNota = nota.tipoNota;

            bool estaProcesada = estadoNota.estadoNota == EstadoNota.Fallada || estadoNota.estadoNota == EstadoNota.Procesada || estadoNota.estadoNota == EstadoNota.ProcesoFallado;

            //resetea el estado de la nota cuando esta en reversa
            if (estaProcesada && timeProvider.GetCurrentTimeScale() < 0)
            {
                estadoNota.estadoNota = EstadoNota.None;
                estadoNota.estadoPuntuacion = EstadoPuntuacion.None;

                continue;
            }



            if (estaProcesada) continue; // continuar si la nota ya fue procesada

            double diferencia = nota.timeToArrive - songTime;
            
            if (estadoNota.estadoNota == EstadoNota.EnProceso)
            {
                diferencia = (nota.timeToArrive + nota.duracion) - songTime;

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

    private void RegistrarResultado(int noteIndex,EstadoNota estado, EstadoPuntuacion puntuacion)
    {
        NoteChange?.Invoke(noteIndex, puntuacion, estado);
        Debug.Log(puntuacion+ "|" + estado);
        estadoNotas[noteIndex].estadoNota = estado;
        estadoNotas[noteIndex].estadoPuntuacion = puntuacion;
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

    private void ReverseWindowSetter(double songTime)
    {
        if (startWindow - 1 > 0)
        {
            while (oldSongTime <= chart[startWindow - 1].timeToArrive + chart[startWindow - 1].duracion + maxProcessTime)
            {
                startWindow--;

                if (startWindow - 1 <= 0) break;
            }
        }

        if (endWindow - 1 > 0)
        {
            //Debug.Log(endWindow + "|" + chart.Count);

            while (songTime <= chart[endWindow - 1].timeToArrive - chart[endWindow - 1].duracion - maxProcessTime)
            {
                Debug.Log(endWindow + "|" + chart.Count);

                endWindow--;

                if (endWindow - 1 <= 0) break;
                
            }
        }
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
                    OnLevelEnd?.Invoke();
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
