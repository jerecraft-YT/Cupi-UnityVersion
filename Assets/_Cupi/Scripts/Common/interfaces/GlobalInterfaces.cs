using System;
//using UnityEngine;

public interface IInputDevice : IDisposable
{
    public bool ClickPressed(CorrespondenciaTecla tecla);
    public event Action<CorrespondenciaTecla,double> OnButtonPressed;
    public event Action<CorrespondenciaTecla,double> OnButtonReleased;
}

public interface ITimeProvider
{
    public double GetCurrentTime();
    public float GetCurrentTimeScale();
}

public interface INoteEntity
{
    public void ChangeNoteState(EstadoPuntuacion puntuacion,EstadoNota estado);
    public void DespawnNote();
    public void InitializeNote(NoteIntialData intialData);
    public NotaInstance GetNoteData();
    public (double timeToArrive, float duracion) GetNoteTimeData();
    public double GetTimeToDespawn();
    public void UpdateNote();
}