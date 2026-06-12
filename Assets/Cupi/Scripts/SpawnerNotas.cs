using UnityEngine;
using System.Collections.Generic;

public class SpawnerNotas : MonoBehaviour
{
    public TilesModeMaster tileModeMaster;

    public Transform tileModeReference;

    const string capaTileMode = "TileMode";

    public Transform radialModeReference;

    const string capaRadialMode = "RadialMode";

    public static SpawnerNotas instance;

    public static Dictionary<CorrespondenciaTecla, Transform> PosicionFinalNotaTile;

    public List<NotaInstance> notasToInstance;

    public Sprite spriteReference;

    private static Dictionary<DireccionesMovimientoNotas, Vector2> direccionesMovimientoNotas = new(){
        {DireccionesMovimientoNotas.Up ,Vector2.up},
        {DireccionesMovimientoNotas.Down,Vector2.down},
        {DireccionesMovimientoNotas.Left ,Vector2.left},
        {DireccionesMovimientoNotas.Right,Vector2.right}
    };

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        SpawnReferences();
    }

    private void SpawnReferences()
    {
        SpawnTileReferences();
    }

    private void SpawnTileReferences()
    {
        PosicionFinalNotaTile = new();

        int playStyle = (int)tileModeMaster.PlayStyle;

        float posXCentrada = (playStyle * tileModeMaster.separacionObjetivosNotas) / 2.0f;

        for (int i = 0; i < playStyle + 1; i++)
        {
            CorrespondenciaTecla tecla = (CorrespondenciaTecla)i;

            float posX = (tileModeMaster.separacionObjetivosNotas * i) - posXCentrada;
            GameObject reference = new GameObject($"reference {tecla}");
            reference.transform.SetParent(tileModeReference);

            reference.transform.localPosition = new Vector3(posX, 0.0f, 0.0f);
            SpriteRenderer spriteRenderer = reference.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = spriteReference;
            spriteRenderer.sortingLayerName = capaTileMode;

            PosicionFinalNotaTile.Add(tecla, reference.transform);
        }
    }

    private void Start()
    {
        if (LevelDataController.instance.actualLevel != null)
        {
            notasToInstance = LevelDataController.instance.actualLevel.notas;
        }

        int playStyle = (int)tileModeMaster.PlayStyle;

        foreach (NotaInstance notaActual in notasToInstance)
        {
            GameObject nota = TilesModePoolController.instance.RequestInstance(notaActual.tipoNota);

            if (nota == null) continue;

            CorrespondenciaTecla tecla = notaActual.CorrespondenciaTecla;

            NotaTileBaseLogic scriptNota = nota.GetComponent<NotaTileBaseLogic>();

            scriptNota.Initialize(notaActual);

            if ((int)tecla > playStyle) scriptNota.data.CorrespondenciaTecla = (CorrespondenciaTecla)playStyle;

            nota.transform.parent = DefinirCorrespondenciaTecla(scriptNota.data.CorrespondenciaTecla);
            nota.transform.localPosition = Vector2.zero;

            scriptNota.origin = TilesModePoolController.instance.RequestGroupPool(notaActual.tipoNota).transform;

            scriptNota.DireccionMovimiento = EstablecerDireccionMovimiento(notaActual.DireccionMovimiento, notaActual.DireccionCustom);
        }
    }

    private Transform DefinirCorrespondenciaTecla(CorrespondenciaTecla Tecla)
    {
        if (PosicionFinalNotaTile.TryGetValue(Tecla, out Transform result))
        {
            return result;
        }

        return PosicionFinalNotaTile[0];
    }

    private Vector2 EstablecerDireccionMovimiento(DireccionesMovimientoNotas DireccionMovimiento, Vector2 DireccionCustom)
    {
        if (DireccionMovimiento == DireccionesMovimientoNotas.Custom) return DireccionCustom;

        return direccionesMovimientoNotas[DireccionMovimiento];
    }
}
