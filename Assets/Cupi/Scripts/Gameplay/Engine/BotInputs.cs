using System.Collections.Generic;
using System;

public class BotInputs : IInputDevice
{
    public List<NotaInstance> chart;

    public event Action<CorrespondenciaTecla,float> OnButtonPressed;

    //temporal mientras no uso esto en el bot
    #pragma warning disable
    public event Action<CorrespondenciaTecla, float> OnButtonReleased;

    private int nextNote;

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
        ProcessBot(songTime);
    }

    private void ProcessBot(float songTime)
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
