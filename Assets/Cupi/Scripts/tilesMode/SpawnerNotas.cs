using UnityEngine;
using System.Collections.Generic;

public class SpawnerNotas : MonoBehaviour
{
    public static SpawnerNotas instance;

    //si se agrega la misma correspondencia de nota en la lista entonces
    //en el diccionario solo se tomara la ultima aparicion de esa
    [SerializeField] private List<PosicionNota> PosicionFinalNota;
    public static Dictionary<CorrespondenciaTecla, Transform> PosicionFinalNotaDic;

    public List<NotaTileInstance> notasTiles;

    public Sprite spriteReference;

    private TilesModeMaster tileModeMaster;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        tileModeMaster = GetComponent<TilesModeMaster>();

        PosicionFinalNotaDic = new();

        foreach (var posicionFinal in PosicionFinalNota)
        {
            PosicionFinalNotaDic[posicionFinal.tecla] = posicionFinal.posicion;
        }

        SpawnReferences();

        //LoadJson();
    }

    private void SpawnReferences()
    {
        int playStyle = (int)tileModeMaster.PlayStyle;

        float posXCentrada = (playStyle * tileModeMaster.separacionObjetivosNotas) / 2.0f;

        for (int i = 0; i < playStyle + 1; i++)
        {
            GameObject reference = Instantiate(new GameObject("reference"),transform);
            reference.transform.localPosition = new Vector3((tileModeMaster.separacionObjetivosNotas * i) - posXCentrada, 0.0f, 0.0f);
            reference.AddComponent<SpriteRenderer>().sprite = spriteReference;
            print(i);
        }
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
