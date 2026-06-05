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

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        //LoadJson();
    }

    private void Start()
    {
        foreach (NotaTileInstance notaActual in notasTiles)
        {
            GameObject nota = TilesModePoolController.instance.RequestInstance(notaActual.tipoNota);

            if (nota == null) continue;

            nota.transform.parent = DefinirCorrespondenciaTecla(notaActual.CorrespondenciaTecla);
            nota.transform.localPosition = Vector2.zero;


            NotaTileBaseLogic scriptNota = nota.GetComponent<NotaTileBaseLogic>();

            scriptNota.Initialize(notaActual);
            scriptNota.origin = TilesModePoolController.instance.RequestGroupPool(notaActual.tipoNota).transform;

            scriptNota.DireccionMovimiento = EstablecerDireccionMovimiento(notaActual.DireccionMovimiento, notaActual.DireccionCustom);
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
}
