using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerNotas : MonoBehaviour
{
    public static SpawnerNotas instance;

    public static Dictionary<CorrespondenciaTecla, Transform> PosicionFinalNotaTile;

    private static Dictionary<DireccionesMovimientoNotas, Vector2> direccionesMovimientoNotas = new(){
        {DireccionesMovimientoNotas.Up ,Vector2.up},
        {DireccionesMovimientoNotas.Down,Vector2.down},
        {DireccionesMovimientoNotas.Left ,Vector2.left},
        {DireccionesMovimientoNotas.Right,Vector2.right}
    };

    public TilesModeMaster tileModeMaster;

    public Transform tileModeReference;

    const string capaTileMode = "TileMode";

    public int prevTimeForChunk = -1;
    public int prevSizeSearching = -1;

    public Transform radialModeReference;

    const string capaRadialMode = "RadialMode";

    public List<NotaInstance> notasToInstance;

    public Sprite spriteReference;

    private ChunkController chunkController;

    private PoolController PoolController;

    private TimeController timeController;

    //sirve para agregar elementos unicos
    //public HashSet<int> spawnedNotes = new();

    public HashSet<int> spawnedNotes = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        chunkController = GetComponent<ChunkController>();

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

    private void SortNotes()
    {
        notasToInstance = notasToInstance.OrderBy(t => t.timeToArrive).ToList();

        for (int i = 0; i < notasToInstance.Count; i++)
        {
            notasToInstance[i].noteIndex = i;
        }
    }

    private void Start()
    {
        PoolController = PoolController.instance;

        timeController = TimeController.instance;

        StartLevel();
    }

    private void StartLevel()
    {
        //TilesModeNotesController.RemoveNote += RemoveNote;

        if (!string.IsNullOrEmpty(LevelDataController.instance.levelName))
        {
            notasToInstance = LevelDataController.instance.actualLevel.notas;
        }

        SortNotes();

        chunkController.GenerateBulletChunks(notasToInstance);

        ChunkSpawnController();
    }

    public void RemoveNote(int notaInstance)
    {
        //Debug.Log("se quito nota");
        spawnedNotes.Remove(notaInstance);
    }

    public float NotesWindowEnd;

    private void ChunkSpawnController()
    {
        float currentTime = (float)timeController.AdditiveTime;

        float travelTime = tileModeMaster.NotesVisibleRender / tileModeMaster.notaTileSpeed;

        float spawnWindowStart = currentTime;
        float spawnWindowEnd = currentTime + travelTime;

        NotesWindowEnd = spawnWindowEnd + chunkController.ChunkSize;

        int firstChunk = FloorChunk(spawnWindowStart, chunkController.ChunkSize);

        int lastChunk = FloorChunk(spawnWindowEnd, chunkController.ChunkSize);

        foreach (var chunkGroup in chunkController.Chunks)
        {
            var dict = chunkGroup.Value;

            for (int chunkTime = firstChunk; chunkTime <= lastChunk; chunkTime += chunkController.ChunkSize)
            {
                if (!dict.TryGetValue(chunkTime, out var level))
                    continue;

                foreach (var nota in level.notas)
                {
                    if (spawnedNotes.Contains(nota.noteIndex) || (nota.timeToArrive < currentTime))
                        continue;

                    SpawnNote(nota);
                }
            }
        }
    }

    private int FloorChunk(float valor, int modulo)
    {
        return Mathf.FloorToInt(valor / modulo) * modulo;
    }

    private void Update()
    {
        ChunkSpawnController();
    }

    private void SpawnNote(NotaInstance notaActual)
    {
        int playStyle = (int)tileModeMaster.PlayStyle;

        spawnedNotes.Add(notaActual.noteIndex);

        TipoObjetoPool tipoObjetoPool = (TipoObjetoPool)(int)notaActual.tipoNota;

        GameObject nota = PoolController.RequestInstance(tipoObjetoPool);

        if (nota == null) return;

        CorrespondenciaTecla tecla = notaActual.CorrespondenciaTecla;

        NotaTileBaseLogic scriptNota = nota.GetComponent<NotaTileBaseLogic>();

        scriptNota.Initialize(notaActual);

        if ((int)tecla > playStyle) scriptNota.data.CorrespondenciaTecla = (CorrespondenciaTecla)playStyle;

        nota.transform.SetParent(DefinirCorrespondenciaTecla(scriptNota.data.CorrespondenciaTecla));
        nota.transform.localPosition = Vector2.zero;

        scriptNota.origin = PoolController.RequestGroupPool(tipoObjetoPool).transform;

        scriptNota.DireccionMovimiento = EstablecerDireccionMovimiento(notaActual.DireccionMovimiento, notaActual.DireccionCustom);
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
