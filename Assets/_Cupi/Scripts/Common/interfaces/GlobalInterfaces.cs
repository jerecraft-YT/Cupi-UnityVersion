using System;
using UnityEngine;

public interface IInputDevice : IDisposable
{
    bool ClickPressed(CorrespondenciaTecla tecla);
    event Action<CorrespondenciaTecla,double> OnButtonPressed;
    event Action<CorrespondenciaTecla,double> OnButtonReleased;
}

public interface ITimeProvider
{
    double GetCurrentTime();
    float GetCurrentTimeScale();
}

public interface INoteEntity
{
    void ChangeNoteState(EstadoPuntuacion puntuacion,EstadoNota estado);
    void DespawnNote();
    void InitializeNote(NoteIntialData intialData);
    void UpdateNote();
}