using UnityEngine;
using System.Collections.Generic;

public class SpawnerNotas : MonoBehaviour
{
    public static SpawnerNotas instance;

    public TilesModeNotesController notesController;
    //si se agrega la misma correspondencia de nota en la lista entonces
    //en el diccionario solo se tomara la ultima aparicion de esa
    [SerializeField] private List<PosicionNota> PosicionFinalNota;
    public static Dictionary<CorrespondenciaTecla, Transform> PosicionFinalNotaDic;

    public List<NotaTileInstance> notasTiles;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        PosicionFinalNotaDic = new();

        foreach (var posicionFinal in PosicionFinalNota)
        {
            PosicionFinalNotaDic[posicionFinal.tecla] = posicionFinal.posicion;
        }

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

    private Transform DefinirCorrespondenciaTecla(CorrespondenciaTecla Tecla)
    {
        if (!PosicionFinalNotaDic.ContainsKey(Tecla)) return transform;

        return PosicionFinalNotaDic[Tecla];
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
