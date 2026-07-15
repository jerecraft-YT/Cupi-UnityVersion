using System;
using System.Collections.Generic;

[Serializable]
public class GameplayEngine : IDisposable
{
    public event Action OnLevelEnd;

    public RuntimeStateNote[] estadoNotas;

    public List<NotaInstance> chart;

    public List<BufferedInput> inputBuffer = new();

    public ModoJuego modoJuego;

    public IInputDevice input;

    public int startWindow;

    public int endWindow;

    const float maxProcessTime = 2;

    const float margenPuntuacionPerfecta = 0.045f;

    const float margenPuntuacionBueno = 0.090f;

    const float margenPuntuacionMalo = 0.135f;

    const float margenPuntuacionPesimo = 0.16f;

    //constructor de gameplay con lo esencial
    public GameplayEngine(IInputDevice inputDevice,List<NotaInstance> chart)
    {
        input = inputDevice;
        this.chart = chart;

        input.OnButtonPressed += OnButtonPressed;
        input.OnButtonReleased += OnButtonReleased;

        estadoNotas = new RuntimeStateNote[chart.Count];
    }

    public void Dispose()
    {
        input.OnButtonPressed -= OnButtonPressed;
        input.OnButtonReleased -= OnButtonReleased;
    }

    private float GetCurrentSongTime()
    {
        return (float)TimeController.instance.AdditiveTime;
    }

    public void Tick(float songTime)
    {
        SetWindowRange(songTime);

        ProcessNotes(songTime);
    }

    private void ProcessNotes(float songTime)
    {
        foreach (var actualInput in inputBuffer)
        {
            if (!actualInput.isPressed) continue;

            CheckNoteHit(actualInput.songTime, actualInput.tecla);
        }

        inputBuffer.Clear();
        //es none para no usar ningun input ya que es para detectar errores
        CheckNoteHit(songTime, CorrespondenciaTecla.None);
    }

    private void CheckNoteHit(float songTime,CorrespondenciaTecla tecla)
    {
        for (int noteIndex = startWindow; noteIndex < endWindow; noteIndex++)
        {
            ref var estadoNota = ref estadoNotas[noteIndex]; //referencia para no copiar

            NotaInstance nota = chart[noteIndex]; // para mas facil acceso a la nota actual

            bool estaProcesada = estadoNota.estadoNota == EstadoNota.Fallada || estadoNota.estadoNota == EstadoNota.Procesada;

            if (estaProcesada) continue; // continuar si la nota ya fue procesada

            float diferencia = nota.timeToArrive - songTime;

            if (diferencia < -margenPuntuacionPerfecta)
            {
                RegistrarResultado(noteIndex,EstadoNota.Fallada,EstadoPuntuacion.Fallaste);
                continue;
            }

            if (nota.correspondenciaTecla != tecla) continue; // continuar si no coincide la tecla con la nota

            EstadoPuntuacion puntuacion = ObtenerPuntaje(diferencia);

            if (puntuacion == EstadoPuntuacion.None) continue; // continuar si no estaba en el margen de puntos

            RegistrarResultado(noteIndex, EstadoNota.Procesada, puntuacion);

            if (tecla != CorrespondenciaTecla.None) return; //si la tecla fue presionada y fue valida la descartamos
        }
    }

    private void RegistrarResultado(int noteIndex,EstadoNota estado, EstadoPuntuacion puntuacion)
    {
        estadoNotas[noteIndex].estadoNota = estado;
        estadoNotas[noteIndex].estadoPuntuacion = puntuacion;
    }

    private EstadoPuntuacion ObtenerPuntaje(float diferencia)
    {
        if (diferencia <= margenPuntuacionPerfecta) return EstadoPuntuacion.Perfecto;
        else if (diferencia <= margenPuntuacionBueno) return EstadoPuntuacion.Bueno;
        else if (diferencia <= margenPuntuacionMalo) return EstadoPuntuacion.Malo;
        else if (diferencia <= margenPuntuacionPesimo) return EstadoPuntuacion.Pesimo;

        return EstadoPuntuacion.None;
    }

    private void SetWindowRange(float songTime)
    {
        //ejemplo (si song time es 2 y el chart se procesa en 2 le sumamos un tiempo de procesado extra
        //por si hay un lag y si no aumentamos el index del startWindow)
        if (!IsFinalChart(startWindow))
        {
            while (songTime > chart[startWindow].timeToArrive + maxProcessTime)
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
            while (songTime > chart[endWindow].timeToArrive - maxProcessTime)
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

    private void OnButtonPressed(CorrespondenciaTecla tecla)
    {
        //mas facil para no perder inputs antes del tick :3
        BufferedInput input = new(tecla,GetCurrentSongTime(),true);

        inputBuffer.Add(input);

        //Debug.Log("Pressed :" + tecla);
    }

    private void OnButtonReleased(CorrespondenciaTecla tecla)
    {
        BufferedInput input = new BufferedInput(tecla, GetCurrentSongTime(), false);

        inputBuffer.Add(input);

        //Debug.Log("Released :" + tecla);
    }
}
