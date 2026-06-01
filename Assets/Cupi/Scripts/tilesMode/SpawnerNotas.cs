using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public struct NotaNormalInstance
{
    public CorrespondenciaTecla CorrespondenciaTecla;
    public DireccionesMovimientoNotas DireccionMovimiento;

    public float timeToArrive;
    public float localSpeed;

    public Vector2 offsetPositionToGo;
    public Vector2 DireccionCustom;

    public NotaNormalInstance(
        CorrespondenciaTecla CorrespondenciaTecla,
        DireccionesMovimientoNotas DireccionMovimiento,
        float timeToArrive,
        Vector2 offsetPositionToGo,
        Vector2 DireccionCustom,
        float localSpeed = 1.0f)
    {
        this.CorrespondenciaTecla = CorrespondenciaTecla;
        this.timeToArrive = timeToArrive;
        this.localSpeed = localSpeed;
        this.offsetPositionToGo = offsetPositionToGo;
        this.DireccionMovimiento = DireccionMovimiento;
        this.DireccionCustom = DireccionCustom;
    }
}

[Serializable]
public class NotaNormalList
{
    public List<NotaNormalInstance> notasNormales;
}

public class SpawnerNotas : MonoBehaviour
{
    public TilesModeNotesController notesController;
    public Transform finalPositionLeftNote;
    public Transform finalPositionMiddleNote;
    public Transform finalPositionRigthNote;

    public static SpawnerNotas instance;
    public float notaNormalSpeed = 4;

    public List<NotaNormalInstance> notasNormales;
    private List<float> timeArriveLeftNotes;
    private List<float> timeArriveRightNotes;
    private List<float> timeArriveMiddleNotes;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        timeArriveLeftNotes = new();
        timeArriveMiddleNotes = new();
        timeArriveRightNotes = new();

        //LoadJson();
    }

    private void Start()
    {
        SeparateNotesForInput();

        foreach (NotaNormalInstance notaActual in notasNormales)
        {
            GameObject nota = TilesModePoolController.instance.RequestInstance(TipoNota.NormalTile);
            nota.transform.parent = DefinirCorrespondenciaTecla(notaActual.CorrespondenciaTecla);
            
            NotaNormal scriptNota = nota.GetComponent<NotaNormal>();

            AddNotesReferences(notaActual.CorrespondenciaTecla, scriptNota);

            scriptNota.Initialize(notaActual);
            scriptNota.origin = TilesModePoolController.instance.RequestGroupPool(TipoNota.NormalTile).transform;

            scriptNota.DireccionMovimiento = EstablecerDireccionMovimiento(notaActual.DireccionMovimiento, notaActual.DireccionCustom);
        }
    }

    private void AddNotesReferences(CorrespondenciaTecla tecla , NotaNormal script)
    {
        notesController.activeNotes.Add(script);

        switch (tecla)
        {
            case CorrespondenciaTecla.Left:
                notesController.NotaNormalLeft.Add(script);
                break;
            case CorrespondenciaTecla.Right:
                notesController.NotaNormalRight.Add(script);
                break;
            case CorrespondenciaTecla.Middle:
                notesController.NotaNormalMiddle.Add(script);
                break;
        }
    }

    private void SeparateNotesForInput()
    {
        foreach(NotaNormalInstance notaActual in notasNormales)
        {
            switch (notaActual.CorrespondenciaTecla)
            {
                case CorrespondenciaTecla.Left:
                    timeArriveLeftNotes.Add(notaActual.timeToArrive);
                    break;
                case CorrespondenciaTecla.Right:
                    timeArriveRightNotes.Add(notaActual.timeToArrive);
                    break;
                case CorrespondenciaTecla.Middle:
                    timeArriveMiddleNotes.Add(notaActual.timeToArrive);
                    break;
            }
        }
    }

    private Transform DefinirCorrespondenciaTecla(CorrespondenciaTecla CorrespondenciaTecla)
    {
        switch (CorrespondenciaTecla)
        {
            case CorrespondenciaTecla.Left:
                return finalPositionLeftNote != null ? finalPositionLeftNote : transform;
            case CorrespondenciaTecla.Middle:
                return finalPositionMiddleNote != null ? finalPositionMiddleNote : transform;
            case CorrespondenciaTecla.Right:
                return finalPositionRigthNote != null ? finalPositionRigthNote : transform;
        }
        return transform;
    }

    private Vector2 EstablecerDireccionMovimiento(DireccionesMovimientoNotas DireccionMovimiento, Vector2 DireccionCustom)
    {
        switch (DireccionMovimiento)
        {
            case DireccionesMovimientoNotas.Up:
                return Vector2.up;
            case DireccionesMovimientoNotas.Down:
                return Vector2.down;
            case DireccionesMovimientoNotas.Left:
                return Vector2.left;
            case DireccionesMovimientoNotas.Right:
                return Vector2.right;
            case DireccionesMovimientoNotas.Custom:
                return DireccionCustom;
        }
        return Vector2.zero;
    }

    public List<float> TimeArriveLeftNotes => timeArriveLeftNotes;
    public List<float> TimeArriveRightNotes => timeArriveRightNotes;
    public List<float> TimeArriveMiddleNotes => timeArriveMiddleNotes;
}
