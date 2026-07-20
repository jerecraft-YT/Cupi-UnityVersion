using System.Collections.Generic;
using System;

public class BotInputs : IInputDevice
{
    public List<NotaInstance> chart;

    public event Action<CorrespondenciaTecla> OnButtonPressed;
    public event Action<CorrespondenciaTecla> OnButtonReleased;

    public bool ClickPressed(CorrespondenciaTecla tecla)
    {
        return false;
    }

    public void Dispose()
    {
        return;
    }

    public void Initialize(List<NotaInstance> chart)
    {
        this.chart = chart;
    }

    public void BotTick(float songTime)
    {

    }


}
