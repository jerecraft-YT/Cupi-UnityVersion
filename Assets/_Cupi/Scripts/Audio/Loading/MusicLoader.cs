using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

using Cupi.ResourceLoader;

public static class MusicLoader
{
    private static BpmController _bpmController;
    const float _timeTransitionMusic = 0.3f;
    const int _maxMusicInCache = 5;
    private static UnityWebRequest _audioRequest;
    private static AudioClip _emptyClip;
    private static Dictionary<string, CacheAudio> _cacheAudio = new();
    private static string _musicInUse = "";
    private static LevelInfo _musicToLoad = null;
    private static bool _loadingNewMusic;


    private static void DinamicClearCache()
    {
        if (_cacheAudio.Count > _maxMusicInCache)
        {
            var old = _cacheAudio.OrderBy(x => x.Value.lastUse).First();

            Object.Destroy(old.Value.clip);

            _cacheAudio.Remove(old.Key);
            Debug.Log("limpiando cache de audio");
        }
    }

    public static void ClearMusicCache()
    {

        if (string.IsNullOrEmpty(_musicInUse))
        {
            foreach (var item in _cacheAudio) Object.Destroy(item.Value.clip);

            _cacheAudio.Clear();
            return;
        }

        var keysToRemove = _cacheAudio.Keys.Where(key => key != _musicInUse).ToList();

        foreach (var key in keysToRemove)
        {
            Object.Destroy(_cacheAudio[key].clip);
            _cacheAudio.Remove(key);
        }
    }

    public static async Task LoadMusic(LevelInfo levelInfo,LevelMetadata levelMetadata)
    {
        //algo obvio por el nombre :/
        //await MusicFadeIn();
        await MusicController.MusicFadeIn(_timeTransitionMusic);

        Debug.Log("-----CARGANDO AUDIO-----");

        await SelectMusic(levelInfo, levelMetadata);

        Debug.Log("-----ACABO DE CARGAR AUDIO-----");

        //_canCancelLoading = false;

        EndMusicLoad(levelMetadata);

        await MusicController.MusicFadeOut(_timeTransitionMusic);
    }

    private static void EndMusicLoad(LevelMetadata levelMetadata)
    {
        float previewMusicTime = levelMetadata.previewTimeMusic;

        MusicController.mainMusic.time = previewMusicTime;

        SincronizarMusica(previewMusicTime, levelMetadata.bpm);

        DinamicClearCache();
    }

    private static async Task SelectMusic(LevelInfo levelInfo, LevelMetadata levelMetadata)
    {
        string levelName = levelInfo.name;

        if(_cacheAudio.TryGetValue(levelName,out CacheAudio cache))
        {
            MusicController.PlayMusic(cache.clip);
            cache.lastUse = Time.time;
            _musicInUse = levelName;

            Debug.Log("se cargo una musica de cache");
            return;
        }

        string levelDirectory = levelInfo.directory;

        string path = Path.Combine(levelDirectory, levelMetadata.musicFileName);

        Debug.Log("obteniendo musica");

        AudioClip audioClip = await GetMusic(levelName, path);

        Debug.Log("cargo musica");

        if (audioClip == null)
        {
            audioClip = _emptyClip;
        }

        MusicController.PlayMusic(audioClip);
        
    }

    public static async Task<AudioClip> GetMusic(string levelName, string path)
    {
        UnityWebRequest request = UnityWebRequest.Get(path);

        _audioRequest = request;

        try
        {
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al cargar la música: " + request.error);
                return _emptyClip;
            }

            byte[] fileBytes = request.downloadHandler.data;

            var forwardClip = await AudioLoaderPipeline.LoadAndPrepare(fileBytes, levelName);

            if (forwardClip == null)
            {
                // fallback al método viejo si el formato no fue reconocido
                return await OldGetMusic(path, levelName);
            }

            _cacheAudio[levelName] = new CacheAudio(forwardClip, Time.time);
            _musicInUse = levelName;

            return forwardClip;
        }
        finally
        {
            if (_audioRequest == request)
                _audioRequest = null;

            request.Dispose();
        }
    }

    public static async Task<AudioClip> OldGetMusic(string levelName,string path)
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
            _musicInUse = levelName;

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

    private static void SincronizarMusica(float previewMusicTime,float bpm)
    {
        float timeForBeat = 60.0f / bpm;

        float offset = previewMusicTime % timeForBeat;

        TimeController.instance.SetTime(previewMusicTime);

        if (_bpmController != null)
        {
            _bpmController.BPM = bpm;
            _bpmController.ResetBpm(previewMusicTime - offset);
        }
    }

    public static async void MusicChangeRequest(
        LevelInfo levelInfo,
        BpmController bpmController = null)
    {
        _bpmController = bpmController;

        // Si ya estamos cargando, simplemente guardamos
        // la última música solicitada.
        if (_loadingNewMusic)
        {
            _musicToLoad = levelInfo;
            return;
        }

        _loadingNewMusic = true;

        try
        {
            await LoadMusic(levelInfo, levelInfo.levelData);

            // Cuando termina, comprobamos si llegó otra petición
            while (_musicToLoad != null)
            {
                LevelInfo nextMusic = _musicToLoad;
                _musicToLoad = null;

                await LoadMusic(nextMusic, nextMusic.levelData);
            }
        }
        finally
        {
            _loadingNewMusic = false;
        }
    }
}
