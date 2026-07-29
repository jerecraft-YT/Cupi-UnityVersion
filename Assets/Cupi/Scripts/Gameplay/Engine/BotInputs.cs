using System.Collections.Generic;
using System;

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
        while(nextNote < chart.Count)
        {
            var nota = chart[nextNote];

            if (songTime < nota.timeToArrive) break;

            OnButtonPressed?.Invoke(nota.correspondenciaTecla , nota.timeToArrive);

            nextNote++;
        }
    }
}
