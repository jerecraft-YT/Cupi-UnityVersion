using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class LevelViewer : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private string mainDirectory;

    public TextMeshProUGUI detallesNivel;

    public List<LevelData> levels = new();

    public List<string> nombreNiveles = new();

    public List<string> direccionesNiveles;

    public Dictionary<string, AudioClip> cacheAudio = new();

    private Coroutine loadMusicCoroutine;

    public string plantillaInfo = "Nombre: {0} \nDescripcion: {1}\nArtista: {2}\nAutor:{3}\nBpm:{4}";

    void Start()
    {
        //testGuardado();

        GetLevels();

        for (int  i = 0; i < nombreNiveles.Count; i++)
        {
            LoadDataLevel(nombreNiveles[i]);
        }

        if (nombreNiveles.Count != 0)
        {
            showLevels();
            ChangeOptionSelected(0);
        }
    }


    private void showLevels()
    {
        dropdown.AddOptions(new List<string>(nombreNiveles));
    }

    private void testGuardado()
    {
        LevelData testDataLevel = new LevelData();

        testDataLevel.Name = "beni";
        testDataLevel.Description = "esta es un prueba";
        testDataLevel.Artist = "benito";
        testDataLevel.Autor = "Yo";
        testDataLevel.Bpm = 123;
        testDataLevel.PreviewTimeMusic = -1;
        testDataLevel.Tags = "testeo|cool|nice";
        testDataLevel.MusicFileName = "test.ogg";

        LoadJsonLevel.SaveMetadata(testDataLevel, "otroTest");
    }

    private void LoadDataLevel(string levelName)
    {
        LevelData levelData = LoadJsonLevel.LoadMetadata(levelName);

        if (!string.IsNullOrEmpty(levelData.Name))
        {
            levels.Add(levelData);
        }
    }

    private void GetLevels()
    {
        LoadJsonLevel.FindGameFolder();

        mainDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        direccionesNiveles = new List<string>(Directory.GetDirectories(Path.Combine(mainDirectory, LoadJsonLevel.nombreCarpetaJuego)));

        //List<string> nombreNiveles = new();

        List<int> direccionesEliminar = new();

        for (int i = 0; i < direccionesNiveles.Count; i++)
        {
            string nombreCarpeta = Path.GetFileName(direccionesNiveles[i].TrimEnd(Path.DirectorySeparatorChar));

            if (LoadJsonLevel.MetadataExists(nombreCarpeta))
            {
                nombreNiveles.Add(nombreCarpeta);
            }
            else
            {
                direccionesEliminar.Add(i);
            }
        }
        
        for (int i = 0; i < direccionesEliminar.Count; i++)
        {
            direccionesNiveles.RemoveAt(direccionesEliminar[i]);
        }
    }

    public void ChangeOptionSelected(int option)
    {
        LevelData nivel = levels[option];

        detallesNivel.text = string.Format(plantillaInfo,nivel.Name, nivel.Description,nivel.Artist,nivel.Autor,nivel.Bpm);

        StartCoroutine(LoadMusic(option));
    }

    public IEnumerator LoadMusic(int index)
    {
        if (cacheAudio.ContainsKey(nombreNiveles[index]))
        {
            MusicController.instance.PlayMusic(cacheAudio[nombreNiveles[index]]);
            print("se cargo una musica de cache");
            yield break;
        }

        string path = Path.Combine(direccionesNiveles[index], levels[index].MusicFileName);

        UnityWebRequest audio = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.UNKNOWN);

        DownloadHandlerAudioClip AudioHandler = (DownloadHandlerAudioClip)audio.downloadHandler;
        AudioHandler.streamAudio = true;

        yield return audio.SendWebRequest();

        if (audio.result == UnityWebRequest.Result.ConnectionError || audio.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error al cargar la música: " + audio.error);
        }
        else
        {
            //Obtenemos el AudioClip descargado
            AudioClip clip = DownloadHandlerAudioClip.GetContent(audio);
            cacheAudio.Add(nombreNiveles[index], clip);

            MusicController.instance.PlayMusic(clip);
        }
    }
}
