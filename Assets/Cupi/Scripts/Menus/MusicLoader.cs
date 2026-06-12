using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class MusicLoader : MonoBehaviour
{
    public BpmController bpmController;

    [SerializeField] private LevelViewer levelViewer;

    [SerializeField] private float duracionTransicionMusica = 0.3f;

    [SerializeField] private int maxMusicInCache = 5;

    private Dictionary<string, CacheAudio> cacheAudio = new();

    private Coroutine loadMusicCoroutine;

    private Queue<int> musicToLoad = new();

    public bool musicToLoadComplete = false;

    private bool canCancelLoading = true;

    public string musicInUse = "";


    void Start()
    {
        DontDestroyOnLoad(gameObject);

        MusicController.instance.mainMusic.loop = true;
    }

    void Update()
    {
        DinamicMusicLoader();
        DinamicClearCache();
    }

    private void DinamicMusicLoader()
    {
        if (musicToLoad.Count != 0 && musicToLoadComplete == true)
        {
            musicToLoadComplete = false;
            canCancelLoading = true;
            LevelInfo level = levelViewer.levels[musicToLoad.Peek()];
            LevelMetadata levelData = level.levelData;

            loadMusicCoroutine = StartCoroutine(
                LoadMusic(
                    duracionTransicionMusica, level.name, levelData.MusicFileName, level.directory, levelData.PreviewTimeMusic, levelData.Bpm
                    ));
            musicToLoad.Clear();
        }
    }

    private void DinamicClearCache()
    {
        if (cacheAudio.Count > maxMusicInCache)
        {
            var old = cacheAudio.OrderBy(x => x.Value.lastUse).First();

            Destroy(old.Value.clip);
            cacheAudio.Remove(old.Key);
            Debug.Log("limpiando cache de audio");
        }
    }

    public void ClearMusicCache()
    {
        foreach (var item in cacheAudio)
        {
            if (item.Key != musicInUse) Destroy(item.Value.clip);
        }
        cacheAudio.Clear();
    }

    public IEnumerator LoadMusic(float duracion,string levelName,string musicFileName,string levelDirectory,float previewMusicTime,float bpm)
    {
        MusicController _musicController = MusicController.instance;
        float startVolumen = _musicController.mainMusic.volume;

        for (float t = 0; t < duracion; t += Time.deltaTime)
        {
            _musicController.mainMusic.volume = Mathf.Lerp(startVolumen, 0, t / duracion);
            yield return null;
        }
        _musicController.mainMusic.volume = 0;

        //LevelInfo level = levelViewer.levels[index];
        //LevelMetadata levelData = level.levelData;

        if (cacheAudio.ContainsKey(levelName))
        {
            _musicController.PlayMusic(cacheAudio[levelName].clip);
            musicInUse = levelName;
            cacheAudio[levelName].lastUse = Time.time;
            print("se cargo una musica de cache");
        }
        else
        {
            string path = Path.Combine(levelDirectory, musicFileName);

            UnityWebRequest audio = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN);

            DownloadHandlerAudioClip AudioHandler = (DownloadHandlerAudioClip)audio.downloadHandler;
            AudioHandler.streamAudio = true;

            yield return audio.SendWebRequest();

            if (audio.result == UnityWebRequest.Result.ConnectionError || audio.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al cargar la música: " + audio.error);
                AudioClip clip = AudioClip.Create("EmptyMusic", 1, 1, 1000, true);
                cacheAudio.Add(levelName, new CacheAudio(clip, Time.time));
                _musicController.PlayMusic(clip);
            }
            else
            {
                //Obtenemos el AudioClip descargado
                AudioClip clip = DownloadHandlerAudioClip.GetContent(audio);
                //AudioClip clip = AudioClip.Create("EmptyMusic", 1, 1, 1000, true);
                clip.name = levelName;
                cacheAudio.Add(levelName, new CacheAudio(clip, Time.time));
                musicInUse = levelName;
                _musicController.PlayMusic(clip);
            }
        }

        _musicController.mainMusic.time = previewMusicTime;

        SincronizarMusica(previewMusicTime,bpm);
        canCancelLoading = false;

        for (float t = 0; t < duracion; t += Time.deltaTime)
        {
            _musicController.mainMusic.volume = Mathf.Lerp(0.0f, 1.0f, t / duracion);
            yield return null;
        }

        _musicController.mainMusic.volume = 1.0f;

        musicToLoadComplete = true;
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
        if (canCancelLoading)
        {
            if (loadMusicCoroutine != null) StopCoroutine(loadMusicCoroutine);

            LevelInfo level = levelViewer.levels[option];
            LevelMetadata levelData = level.levelData;

            loadMusicCoroutine = StartCoroutine(
                LoadMusic(
                    duracionTransicionMusica, level.name, levelData.MusicFileName, level.directory, levelData.PreviewTimeMusic, levelData.Bpm
                    ));
        }
        else
        {

            musicToLoad.Enqueue(option);
        }
    }
}
