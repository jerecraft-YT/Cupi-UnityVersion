using System.Collections.Generic;
using System;

public class BotInputs : IInputDevice
{
    public List<NotaInstance> chart;

    public event Action<CorrespondenciaTecla> OnButtonPressed;
    //temporal mientras no uso esto en el bot
    #pragma warning disable
    public event Action<CorrespondenciaTecla> OnButtonReleased;

    private int startWindow;
    private int endWindow;
    const float maxProcessTime = 2.0f;

    const float margenPuntuacionPerfecta = 0.045f;

    public bool ClickPressed(CorrespondenciaTecla tecla)
    {
        return false;
    }

    public void Dispose()
    {
        return;
    }

    public BotInputs(List<NotaInstance> chart)
    {
        this.chart = chart;
    }

    public void BotTick(float songTime)
    {
        SetWindowRange(songTime);

        ProcessBot(songTime);
    }

    private void ProcessBot(float songTime)
    {
        for (int noteIndex = startWindow; noteIndex < endWindow; noteIndex++)
        {
            NotaInstance nota = chart[noteIndex];

            float diferencia = nota.timeToArrive - songTime;

            if (diferencia <= margenPuntuacionPerfecta && diferencia >= -margenPuntuacionPerfecta)
            {
                OnButtonPressed?.Invoke(nota.correspondenciaTecla);
            }
        }
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
}
