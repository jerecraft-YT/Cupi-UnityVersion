using System.Collections.Generic;
using System;
using UnityEngine;

public class BotInputs : IInputDevice
{
    public List<NotaInstance> chart;

    public event Action<CorrespondenciaTecla, double> OnButtonPressed;

    //temporal mientras no uso esto en el bot
    #pragma warning disable
    public event Action<CorrespondenciaTecla, double> OnButtonReleased;

    private int nextNote;

    public bool ClickPressed(CorrespondenciaTecla tecla)
    {
        return true;
    }

    public void Dispose()
    {
        return;
    }

    public BotInputs(List<NotaInstance> chart)
    {
        this.chart = chart;
    }

    public void BotTick(double songTime)
    {
        ProcessBot(songTime);
    }

    private void ProcessBot(double songTime)
    {
        RewindBot(songTime);

        while(nextNote < chart.Count)
        {
            var nota = chart[nextNote];

            if (songTime < nota.timeToArrive) break;

            OnButtonPressed?.Invoke(nota.correspondenciaTecla , nota.timeToArrive);

            nextNote++;
        }
    }

    /// <summary>
    /// devuelve el indice del bot si el tiempo retrocedio, si no las notas que el engine
    /// vuelve a poner por jugar quedan detras del indice y el bot no las presiona nunca mas
    /// </summary>
    private void RewindBot(double songTime)
    {
        while (nextNote > 0 && songTime < chart[nextNote - 1].timeToArrive)
        {
            nextNote--;
        }
    }
}
