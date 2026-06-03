using UnityEngine;
using System.Collections.Generic;

public class SpawnerNotas : MonoBehaviour
{
    public static SpawnerNotas instance;

    public TilesModeNotesController notesController;
    public Transform finalPositionLeftNote;
    public Transform finalPositionMiddleNote;
    public Transform finalPositionRigthNote;

    public float notaTileSpeed = 4;

    public List<NotaTileInstance> notasTiles;
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
        foreach (NotaTileInstance notaActual in notasTiles)
        {
            GameObject nota = TilesModePoolController.instance.RequestInstance(notaActual.tipoNota);

            if (nota == null) continue;

            nota.transform.parent = DefinirCorrespondenciaTecla(notaActual.CorrespondenciaTecla);
            
            NotaTileNormal scriptNota = nota.GetComponent<NotaTileNormal>();

            AddNotesReferences(notaActual, scriptNota);

            scriptNota.Initialize(notaActual);
            scriptNota.origin = TilesModePoolController.instance.RequestGroupPool(notaActual.tipoNota).transform;

            scriptNota.DireccionMovimiento = EstablecerDireccionMovimiento(notaActual.DireccionMovimiento, notaActual.DireccionCustom);
        }
    }

    private void AddNotesReferences(NotaTileInstance nota, NotaTileNormal script)
    {
        notesController.NotasActivas.Add(script);

        switch (nota.CorrespondenciaTecla)
        {
            case CorrespondenciaTecla.Left:
                notesController.NotasTileLeft.Add(script);
                timeArriveLeftNotes.Add(nota.timeToArrive);
                break;
            case CorrespondenciaTecla.Right:
                notesController.NotasTileRight.Add(script);
                timeArriveRightNotes.Add(nota.timeToArrive);
                break;
            case CorrespondenciaTecla.Middle:
                notesController.NotasTileMiddle.Add(script);
                timeArriveMiddleNotes.Add(nota.timeToArrive);
                break;
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
