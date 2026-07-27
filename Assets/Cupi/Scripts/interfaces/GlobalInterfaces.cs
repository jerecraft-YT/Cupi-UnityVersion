using System;

public interface IInputDevice : IDisposable
{
    bool ClickPressed(CorrespondenciaTecla tecla);
    event Action<CorrespondenciaTecla,float> OnButtonPressed;
    event Action<CorrespondenciaTecla,float> OnButtonReleased;
}