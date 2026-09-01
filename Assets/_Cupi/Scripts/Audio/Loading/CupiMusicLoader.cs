using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Cupi.ResourceLoader.Audio
{

    public static class CupiMusicLoader
    {
        private static BpmController actualBpmController;
        const float timeTransitionMusic = 0.3f;
        const int maxMusicInCache = 5;
        private static UnityWebRequest audioRequest;
        private static AudioClip emptyClip;
        private static Dictionary<string, CacheAudio> cacheAudio = new();
        private static string musicInUse = "";
        private static LevelInfo musicToLoad = null;
        private static bool loadingNewMusic;


        private static void DinamicClearCache()
        {
            if (cacheAudio.Count > maxMusicInCache)
            {
                var old = cacheAudio.OrderBy(x => x.Value.lastUse).First();

                Object.Destroy(old.Value.clip);

                cacheAudio.Remove(old.Key);
                Debug.Log("limpiando cache de audio");
            }
        }

        public static void ClearMusicCache()
        {

            if (string.IsNullOrEmpty(musicInUse))
            {
                foreach (var item in cacheAudio) Object.Destroy(item.Value.clip);

                cacheAudio.Clear();
                return;
            }

            var keysToRemove = cacheAudio.Keys.Where(key => key != musicInUse).ToList();

            foreach (var key in keysToRemove)
            {
                Object.Destroy(cacheAudio[key].clip);
                cacheAudio.Remove(key);
            }
        }

        public static async Task LoadMusic(LevelInfo levelInfo, LevelMetadata levelMetadata)
        {
            //algo obvio por el nombre :/
            //await MusicFadeIn();
            await MusicController.MusicFadeIn(timeTransitionMusic);

            Debug.Log("-----CARGANDO AUDIO-----");
            
            await SelectMusic(levelInfo, levelMetadata);
            
            Debug.Log("-----ACABO DE CARGAR AUDIO-----");

            //_canCancelLoading = false;

            EndMusicLoad(levelMetadata);

            await MusicController.MusicFadeOut(timeTransitionMusic);
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
            string levelName = levelInfo.folderName;

            if (cacheAudio.TryGetValue(levelName, out CacheAudio cache))
            {
                MusicController.PlayMusic(cache.clip);
                cache.lastUse = Time.time;
                musicInUse = levelName;

                Debug.Log($"se cargo una musica de cache {levelName}");
                return;
            }

            string levelDirectory = levelInfo.directory;

            string path = Path.Combine(levelDirectory, levelMetadata.musicFileName);

            Debug.Log("obteniendo musica");

            AudioClip audioClip = await GetMusic(levelName, path);
            //AudioClip audioClip = await OldGetMusic(levelName, path);

            Debug.Log("cargo musica");

            if (audioClip == null)
            {
                audioClip = emptyClip;
            }

            MusicController.PlayMusic(audioClip);

        }

        public static async Task<AudioClip> GetMusic(string levelName, string path)
        {
            UnityWebRequest request = UnityWebRequest.Get(path);

            audioRequest = request;

            try
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error al cargar la música: " + request.error);
                    return emptyClip;
                }

                byte[] fileBytes = request.downloadHandler.data;

                var forwardClip = await AudioLoaderPipeline.LoadAndPrepare(fileBytes, levelName);

                if (forwardClip == null)
                {
                    // fallback al método viejo si el formato no fue reconocido
                    return await OldGetMusic(path, levelName);
                }

                cacheAudio[levelName] = new CacheAudio(forwardClip, Time.time);
                musicInUse = levelName;

                return forwardClip;
            }
            finally
            {
                if (audioRequest == request)
                    audioRequest = null;

                request.Dispose();
            }
        }

        public static async Task<AudioClip> OldGetMusic(string levelName, string path)
        {
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN);

            DownloadHandlerAudioClip audioHandler = (DownloadHandlerAudioClip)request.downloadHandler;
            audioHandler.streamAudio = true;

            audioRequest = request;

            try
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.ConnectionError ||
                    request.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error al cargar la música: " + request.error);

                    AudioClip clip = emptyClip;

                    return clip;
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    return null;
                }

                //Obtenemos el AudioClip descargado
                AudioClip loadedClip = DownloadHandlerAudioClip.GetContent(request);

                loadedClip.name = levelName;

                cacheAudio[levelName] = new(loadedClip, Time.time);
                musicInUse = levelName;

                return loadedClip;
            }
            finally
            {
                if (audioRequest == request)
                {
                    audioRequest = null;
                }

                request.Dispose();
            }
        }

        private static void SincronizarMusica(float previewMusicTime, float bpm)
        {
            float timeForBeat = 60.0f / bpm;

            float offset = previewMusicTime % timeForBeat;

            TimeController.instance.SetTime(previewMusicTime);

            if (actualBpmController != null)
            {
                actualBpmController.BPM = bpm;
                actualBpmController.ResetBpm(previewMusicTime - offset);
            }
        }

        public static async void MusicChangeRequest(
            LevelInfo levelInfo,
            BpmController bpmController = null)
        {
            actualBpmController = bpmController;

            // Si ya estamos cargando, simplemente guardamos
            // la última música solicitada.
            if (loadingNewMusic)
            {
                musicToLoad = levelInfo;
                return;
            }

            loadingNewMusic = true;

            try
            {
                await LoadMusic(levelInfo, levelInfo.levelData);

                // Cuando termina, comprobamos si llegó otra petición
                while (musicToLoad != null)
                {
                    LevelInfo nextMusic = musicToLoad;
                    musicToLoad = null;

                    await LoadMusic(nextMusic, nextMusic.levelData);
                }
            }
            finally
            {
                loadingNewMusic = false;
            }
        }
    }
}
