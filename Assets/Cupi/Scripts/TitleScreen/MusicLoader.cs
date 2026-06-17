using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class MusicLoader : MonoBehaviour
{
    [SerializeField] private LevelViewer _levelViewer;

    [SerializeField] private BpmController bpmController;

    [SerializeField] private float _timeTransitionMusic = 0.3f;

    [SerializeField] private int _maxMusicInCache = 5;

    private AudioClip _emptyClip;

    private Dictionary<string, CacheAudio> _cacheAudio = new();

    private int _musicToLoad = -1;

    private bool _canCancelLoading = true;

    public bool readyForNewLoad = false;

    public string musicInUse = "";

    private UnityWebRequest _audioRequest;

    private bool _loading;

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        MusicController.instance.mainMusic.loop = true;

        _emptyClip = AudioClip.Create("EmptyMusic", 1, 1, 1000, true);
    }

    void Update()
    {
        if (!_loading)
        {
            MusicDinamicLoader();
        }
    }

    private async void MusicDinamicLoader()
    {
        if (_loading)
        {
            return;
        }

        if (_musicToLoad != -1 && readyForNewLoad == true)
        {
            _loading = true;

            try
            {
                readyForNewLoad = false;
                _canCancelLoading = true;

                //esto para que no pierda la referencia por si el numero se cambia fuera del task
                int musicIndex = _musicToLoad;
                _musicToLoad = -1;

                LevelInfo level = _levelViewer.levels[musicIndex];
                LevelMetadata levelMetadata = level.levelData;

                await LoadMusic(level, levelMetadata);
            }
            finally
            {
                _loading = false;
            }
        }
    }

    private void DinamicClearCache()
    {
        if (_cacheAudio.Count > _maxMusicInCache)
        {
            var old = _cacheAudio.OrderBy(x => x.Value.lastUse).First();

            Destroy(old.Value.clip);
            _cacheAudio.Remove(old.Key);
            Debug.Log("limpiando cache de audio");
        }
    }

    public void ClearMusicCache()
    {

        if (string.IsNullOrEmpty(musicInUse))
        {
            foreach (var item in _cacheAudio) Destroy(item.Value.clip);

            _cacheAudio.Clear();
            return;
        }

        var keysToRemove = _cacheAudio.Keys.Where(key => key != musicInUse).ToList();

        foreach (var key in keysToRemove)
        {
            Destroy(_cacheAudio[key].clip);
            _cacheAudio.Remove(key);
        }
    }

    public async Task LoadMusic(LevelInfo levelInfo,LevelMetadata levelMetadata)
    {
        MusicController _musicController = MusicController.instance;

        //algo obvio por el nombre :/
        await MusicFadeIn(_musicController);

        Debug.Log("-----CARGANDO AUDIO-----");

        await SelectMusic(levelInfo, levelMetadata, _musicController);

        Debug.Log("-----ACABO DE CARGAR AUDIO-----");

        _canCancelLoading = false;

        EndMusicLoad(levelMetadata,_musicController);

        await MusicFadeOut(_musicController);
    }

    private void EndMusicLoad(LevelMetadata levelMetadata,MusicController musicController)
    {
        float previewMusicTime = levelMetadata.PreviewTimeMusic;

        musicController.mainMusic.time = previewMusicTime;

        SincronizarMusica(previewMusicTime, levelMetadata.Bpm);

        readyForNewLoad = true;

        DinamicClearCache();
    }

    private async Task SelectMusic(LevelInfo levelInfo, LevelMetadata levelMetadata,MusicController musicController)
    {
        string levelName = levelInfo.name;

        if(_cacheAudio.TryGetValue(levelName,out CacheAudio cache))
        {
            musicController.PlayMusic(cache.clip);
            cache.lastUse = Time.time;
            musicInUse = levelName;

            Debug.Log("se cargo una musica de cache");
            return;
        }

        string levelDirectory = levelInfo.directory;

        string path = Path.Combine(levelDirectory, levelMetadata.MusicFileName);

        Debug.Log("obteniendo musica");

        AudioClip audioClip = await GetMusic(levelName, path);

        Debug.Log("cargo musica");

        if (audioClip == null)
        {
            audioClip = _emptyClip;
        }

        musicController.PlayMusic(audioClip);
        
    }

    public async Task<AudioClip> GetMusic(string levelName,string path)
    {

        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN);

        DownloadHandlerAudioClip audioHandler = (DownloadHandlerAudioClip)request.downloadHandler;
        audioHandler.streamAudio = true;

        _audioRequest = request;

        try
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al cargar la música: " + request.error);

                AudioClip clip = _emptyClip;

                return clip;
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                return null;
            }

            //Obtenemos el AudioClip descargado
            AudioClip loadedClip = DownloadHandlerAudioClip.GetContent(request);
            loadedClip.name = levelName;

            _cacheAudio[levelName] = new(loadedClip, Time.time);
            musicInUse = levelName;

            return loadedClip;
        }
        finally
        {
            if (_audioRequest == request)
            {
                _audioRequest = null;
            }

            request.Dispose();
        }
    }

    private async Task MusicFadeIn(MusicController musicController)
    {
        float startVolumen = musicController.mainMusic.volume;

        for (float t = 0; t < _timeTransitionMusic; t += Time.deltaTime)
        {
            musicController.mainMusic.volume = Mathf.Lerp(startVolumen, 0, t / _timeTransitionMusic);
            await Awaitable.EndOfFrameAsync();
        }
        musicController.mainMusic.volume = 0;
    }

    private async Task MusicFadeOut(MusicController musicController)
    {
        for (float t = 0; t < _timeTransitionMusic; t += Time.deltaTime)
        {
            musicController.mainMusic.volume = Mathf.Lerp(0.0f, 1.0f, t / _timeTransitionMusic);
            await Awaitable.EndOfFrameAsync();
        }

        musicController.mainMusic.volume = 1.0f;
    }

    private void SincronizarMusica(float previewMusicTime,float bpm)
    {
        float timeForBeat = 60.0f / bpm;

        float offset = previewMusicTime % timeForBeat;

        TimeController.instance.SetTime(previewMusicTime);
        if (bpmController != null)
        {
            bpmController.BPM = bpm;
            bpmController.ResetBpm(previewMusicTime - offset);
        }
    }

    public async void MusicChangeRequest(int option)
    {
        if (_loading)
        {
            _musicToLoad = option;
            return;
        }

        _loading = true;

        try
        {
            if (_canCancelLoading)
            {
                _audioRequest?.Abort();
                _audioRequest = null;

                int musicIndex = option;

                LevelInfo level = _levelViewer.levels[musicIndex];
                LevelMetadata levelMetadata = level.levelData;

                await LoadMusic(level, levelMetadata);
            }
            else
            {
                _musicToLoad = option;
            }
        }
        finally
        {

            _loading = false;
        }
    }
}
