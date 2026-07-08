using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawnerNotas : MonoBehaviour
{
    public static SpawnerNotas instance;

    public static Dictionary<CorrespondenciaTecla, Transform> PosicionFinalNotaTile;

    private static readonly Dictionary<DireccionesMovimientoNotas, Vector2> direccionesMovimientoNotas = new(){
        {DireccionesMovimientoNotas.Up ,Vector2.up},
        {DireccionesMovimientoNotas.Down,Vector2.down},
        {DireccionesMovimientoNotas.Left ,Vector2.left},
        {DireccionesMovimientoNotas.Right,Vector2.right}
    };

    [SerializeField] private TilesModeMaster _tileModeMaster;

    [SerializeField] private ChunkController _chunkController;

    public Transform tileModeReference;

    const string capaTileMode = "TileMode";

    public float NotesWindowEnd;

    public int firstChunk;

    public int lastChunk;

    public float spawnWindowStart;

    public float spawnWindowEnd;

    public Transform radialModeReference;

    const string capaRadialMode = "RadialMode";

    public List<NotaInstance> notasToInstance;

    public Sprite spriteReference;

    private PoolController _poolController;

    private TimeController _timeController;

    private LevelDataController _levelDataController;

    //sirve para agregar elementos unicos
    private HashSet<int> _spawnedNotes = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        SpawnTileReferences();
    }

    private void Start()
    {
        _poolController = PoolController.instance;

        _timeController = TimeController.instance;

        _levelDataController = LevelDataController.instance;

        StartLevel();
    }

    private void SpawnTileReferences()
    {
        PosicionFinalNotaTile = new();

        int playStyle = (int)_tileModeMaster.PlayStyle;

        float posXCentrada = (playStyle * _tileModeMaster.separacionObjetivosNotas) / 2.0f;

        for (int i = 0; i < playStyle + 1; i++)
        {
            CorrespondenciaTecla tecla = (CorrespondenciaTecla)i;

            float posX = (_tileModeMaster.separacionObjetivosNotas * i) - posXCentrada;
            GameObject reference = new($"reference {tecla}");
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

    private void StartLevel()
    {
        if (!string.IsNullOrEmpty(_levelDataController.levelName))
        {
            notasToInstance = _levelDataController.actualLevel.notas;
        }

        SortNotes();

        _chunkController.GenerateBulletChunks(notasToInstance);

        ChunkSpawnController();
    }

    public void RemoveNote(int notaInstance)
    {
        _spawnedNotes.Remove(notaInstance);
    }

    private void ChunkSpawnController()
    {
        float currentTime = (float)_timeController.AdditiveTime;

        float travelTime = _tileModeMaster.NotesVisibleRender / _tileModeMaster.notaTileSpeed;

        spawnWindowStart = currentTime;
        spawnWindowEnd = currentTime + travelTime;

        NotesWindowEnd = spawnWindowEnd + _chunkController.ChunkSize;

        firstChunk = FloorChunk(spawnWindowStart, _chunkController.ChunkSize);

        lastChunk = FloorChunk(spawnWindowEnd, _chunkController.ChunkSize);

        foreach (var chunkGroup in _chunkController.Chunks)
        {
            var dict = chunkGroup.Value;

            for (int chunkTime = firstChunk; chunkTime <= lastChunk; chunkTime += _chunkController.ChunkSize)
            {
                if (!dict.TryGetValue(chunkTime, out var level))
                    continue;

                foreach (var noteData in level.notas)
                {
                    /*
                    if (noteData.nota.tipoNota == TipoNota.Sostenida && TimeController.instance.TimeScale < 0)
                    {
                        Debug.Log("podria generarse una nota sostenida");
                        Debug.Log($"en este chunk hay {level.notas.Count} notas");
                    }
                    */

                    if (_spawnedNotes.Contains(noteData.nota.noteIndex) || (noteData.timeToSpawn < currentTime))
                        continue;

                    SpawnNote(noteData.nota);
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
        switch (notaActual.modoNota)
        {
            case ModoNota.None:
                break;
            case ModoNota.Tile:
                SpawnTileNote(notaActual);
                break;
            case ModoNota.Radial:
                break;
            default:
                break;
        }

    }

    private void SpawnTileNote(NotaInstance notaActual)
    {
        int playStyle = (int)_tileModeMaster.PlayStyle;

        _spawnedNotes.Add(notaActual.noteIndex);

        TipoObjetoPool tipoObjetoPool = (TipoObjetoPool)(int)notaActual.tipoNota;

        GameObject nota = _poolController.RequestInstance(tipoObjetoPool);

        if (nota == null) return;

        CorrespondenciaTecla tecla = notaActual.correspondenciaTecla;

        NotaTileBaseLogic scriptNota = nota.GetComponent<NotaTileBaseLogic>();

        scriptNota.Initialize(notaActual);

        if ((int)tecla > playStyle) scriptNota.data.correspondenciaTecla = (CorrespondenciaTecla)playStyle;

        nota.transform.SetParent(DefinirCorrespondenciaTecla(scriptNota.data.correspondenciaTecla));
        nota.transform.localPosition = Vector2.zero;

        scriptNota.origin = _poolController.RequestGroupPool(tipoObjetoPool).transform;

        scriptNota.DireccionMovimiento = EstablecerDireccionMovimiento(notaActual.direccionMovimiento, notaActual.direccionCustom);
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
