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

    public LevelData[] levels;

    public string[] nombreNiveles;

    public string[] direccionesNiveles;

    public string plantillaInfo = "Nombre: {0} \nDescripcion: {1}\nArtista: {2}\nAutor:{3}\nBpm:{4}";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //FindLevels();
        //testGuardado();

        nombreNiveles = GetLevels();

        levels = new LevelData[nombreNiveles.Length];

        for (int  i = 0; i < nombreNiveles.Length; i++)
        {
            LoadDataLevel(nombreNiveles[i],i);
        }

        showLevels();
        ChangeOptionSelected(0);
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

    private void LoadDataLevel(string levelName,int indice)
    {
        levels[indice] = LoadJsonLevel.LoadMetadata(levelName);
    }

    private string[] GetLevels()
    {
        LoadJsonLevel.FindGameFolder();

        mainDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        direccionesNiveles = Directory.GetDirectories(Path.Combine(mainDirectory, LoadJsonLevel.nombreCarpetaJuego));

        string[] nombreNiveles = new string[direccionesNiveles.Length];

        for (int i = 0; i < direccionesNiveles.Length; i++)
        {

            string nombreCarpeta = Path.GetFileName(direccionesNiveles[i].TrimEnd(Path.DirectorySeparatorChar));


            nombreNiveles[i] += nombreCarpeta;
        }
        
        return nombreNiveles;
    }

    public void ChangeOptionSelected(int option)
    {
        LevelData nivel = levels[option];

        detallesNivel.text = string.Format(plantillaInfo,nivel.Name, nivel.Description,nivel.Artist,nivel.Autor,nivel.Bpm);

        StartCoroutine(LoadMusic(option));
    }

    public IEnumerator LoadMusic(int index)
    {
        print(Path.Combine(direccionesNiveles[index], levels[index].MusicFileName));

        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(Path.Combine(direccionesNiveles[index], levels[index].MusicFileName), AudioType.UNKNOWN))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error al cargar la música: " + request.error);
            }
            else
            {
                // Obtenemos el AudioClip descargado
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                print("recurso si cargo");
                MusicController.instance.PlayMusic(clip);
            }
        }
    }
    
}
