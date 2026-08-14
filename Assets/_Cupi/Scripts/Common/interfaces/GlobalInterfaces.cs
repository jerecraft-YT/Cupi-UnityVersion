using System;

public interface IInputDevice : IDisposable
{
    bool ClickPressed(CorrespondenciaTecla tecla);
    event Action<CorrespondenciaTecla,double> OnButtonPressed;
    event Action<CorrespondenciaTecla,double> OnButtonReleased;
}

public interface ITimeProvider
{
    double GetCurrentTime();
}