using System.Collections.Generic;
using System;
using UnityEngine;

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


}
