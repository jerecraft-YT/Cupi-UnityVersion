using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class MusicLoader : MonoBehaviour
{
    [SerializeField] private LevelViewer _levelViewer;

    [SerializeField] private float _timeTransitionMusic = 0.3f;

    [SerializeField] private int _maxMusicInCache = 5;

    private Dictionary<string, CacheAudio> _cacheAudio = new();

    private Coroutine _loadMusicCoroutine;

    private int _musicToLoad = -1;

    private bool _canCancelLoading = true;

    public BpmController bpmController;

    public bool readyForNewLoad = false;

    public string musicInUse = "";

    private UnityWebRequest _audioRequest;


    void Start()
    {
        DontDestroyOnLoad(gameObject);

        MusicController.instance.mainMusic.loop = true;
    }

    void Update()
    {
        MusicDinamicLoader();
    }

    private void MusicDinamicLoader()
    {
        if (_musicToLoad != -1 && readyForNewLoad == true)
        {
            readyForNewLoad = false;
            _canCancelLoading = true;
            LevelInfo level = _levelViewer.levels[_musicToLoad];
            LevelMetadata levelData = level.levelData;

            _loadMusicCoroutine = StartCoroutine(
                LoadMusic(
                    _timeTransitionMusic,
                    level.name,
                    levelData.MusicFileName,
                    level.directory,
                    levelData.PreviewTimeMusic,
                    levelData.Bpm
                    ));
            _musicToLoad = -1;
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

    public IEnumerator LoadMusic(float duracion,string levelName,string musicFileName,string levelDirectory,float previewMusicTime,float bpm)
    {
        MusicController _musicController = MusicController.instance;

        yield return StartCoroutine(MusicFadeIn(_musicController, duracion));

        if (_cacheAudio.ContainsKey(levelName))
        {
            _musicController.PlayMusic(_cacheAudio[levelName].clip);
            musicInUse = levelName;
            _cacheAudio[levelName].lastUse = Time.time;
            Debug.Log("se cargo una musica de cache");
        }
        else
        {
            string path = Path.Combine(levelDirectory, musicFileName);

            _audioRequest = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN);

            DownloadHandlerAudioClip audioHandler = (DownloadHandlerAudioClip)_audioRequest.downloadHandler;
            audioHandler.streamAudio = true;

            yield return _audioRequest.SendWebRequest();

            if (_audioRequest == null) yield break;

            if (_audioRequest.result == UnityWebRequest.Result.ConnectionError || _audioRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al cargar la música: " + _audioRequest.error);
                AudioClip clip = AudioClip.Create("EmptyMusic", 1, 1, 1000, true);
                _cacheAudio.Add(levelName, new CacheAudio(clip, Time.time));
                musicInUse = levelName;
                _musicController.PlayMusic(clip);
            }
            else
            {
                //Obtenemos el AudioClip descargado
                AudioClip clip = DownloadHandlerAudioClip.GetContent(_audioRequest);
                clip.name = levelName;
                _cacheAudio.Add(levelName, new CacheAudio(clip, Time.time));
                musicInUse = levelName;
                _musicController.PlayMusic(clip);
            }

            _audioRequest?.Dispose();
            _audioRequest = null;
        }

        _musicController.mainMusic.time = previewMusicTime;

        SincronizarMusica(previewMusicTime,bpm);
        _canCancelLoading = false;

        yield return StartCoroutine(MusicFadeOut(_musicController, duracion));

        readyForNewLoad = true;

        DinamicClearCache();
    }

    private IEnumerator MusicFadeIn(MusicController musicController,float duracion)
    {
        float startVolumen = musicController.mainMusic.volume;

        for (float t = 0; t < duracion; t += Time.deltaTime)
        {
            musicController.mainMusic.volume = Mathf.Lerp(startVolumen, 0, t / duracion);
            yield return null;
        }
        musicController.mainMusic.volume = 0;
    }

    private IEnumerator MusicFadeOut(MusicController musicController, float duracion)
    {
        for (float t = 0; t < duracion; t += Time.deltaTime)
        {
            musicController.mainMusic.volume = Mathf.Lerp(0.0f, 1.0f, t / duracion);
            yield return null;
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

    public void MusicChangeRequest(int option)
    {
        if (_canCancelLoading)
        {
            if (_loadMusicCoroutine != null) StopCoroutine(_loadMusicCoroutine);

            _audioRequest?.Abort();
            _audioRequest?.Dispose();
            _audioRequest = null;

            LevelInfo level = _levelViewer.levels[option];
            LevelMetadata levelData = level.levelData;

            _loadMusicCoroutine = StartCoroutine(
                LoadMusic(
                    _timeTransitionMusic, level.name, levelData.MusicFileName, level.directory, levelData.PreviewTimeMusic, levelData.Bpm
                    ));
        }
        else
        {

            _musicToLoad = option;
        }
    }
}
