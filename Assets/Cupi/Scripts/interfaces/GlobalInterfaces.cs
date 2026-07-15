using System;

public interface IInputDevice : IDisposable
{
    bool ClickPressed(CorrespondenciaTecla tecla);
    event Action<CorrespondenciaTecla> OnButtonPressed;
    event Action<CorrespondenciaTecla> OnButtonReleased;
}