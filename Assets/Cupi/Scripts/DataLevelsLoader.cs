using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;

public class DataLevelsLoader
{
    public static string nombreCarpetaJuego = "CUPI";
    public static int errorMusicLoad = -1;

    //si es una task es mas facil de saber cuando acabo en vez de ponerle variables de end
    public static async Task SaveAll(List<NotaInstance> notasToSave, string levelName, LevelMetadata metadata,string musicOriginalPath)
    {
        Debug.Log("-----GUARDANDO NIVEL COMPLETO-----");

        foreach(LevelData levelData in metadata.levelsFiles)
        {
            Debug.Log("guardando nivel");
            await SaveLevel(notasToSave, levelName,levelData.levelFileName);
        }

        Debug.Log("guardando musica");
        await SaveMusic(musicOriginalPath, levelName);

        if (errorMusicLoad != -1)
        {
            errorMusicLoad = -1;
        }
        else
        {
            Debug.Log("se sobreescribio la direccion de la musica en metadata");
            string fileName = Path.GetFileName(musicOriginalPath);
            metadata.musicFileName = fileName;
        }

        Debug.Log("guardando metadata");
        await SaveMetadata(metadata, levelName);

        Debug.Log("-----TERMINO DE GUARDAR NIVEL COMPLETO-----");
    }

    public static void FindGameFolder()
    {
        string dir = Path.Combine(MainPath, nombreCarpetaJuego);
        
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    private static void FindLevelFolder(string folderName)
    {
        string dir = Path.Combine(MainPath, nombreCarpetaJuego, folderName);

        if (!Directory.Exists(dir))
        {
            Debug.Log("crear carpeta");
            Directory.CreateDirectory(dir);
        }
    }

    public static async Task SaveLevel(List<NotaInstance> notasToSave,string levelName,string fileName)
    {
        FindGameFolder();
        FindLevelFolder(levelName);

        Level LevelToSave = new(notasToSave);

        string JsonString = JsonUtility.ToJson(LevelToSave, true);

        string dir = Path.Combine(MainPath, nombreCarpetaJuego, levelName, fileName);

        await Task.Run(() =>
        {
            File.WriteAllText(dir, JsonString);
        }); 
        Debug.Log("se guardo el nivel");
    }

    public static async Task SaveMusic(string originalPath,string levelName)
    {
        FindGameFolder();
        FindLevelFolder(levelName);

        string fileName = Path.GetFileName(originalPath);
        string finalDir = Path.Combine(MainPath, nombreCarpetaJuego, levelName, fileName);

        if (!File.Exists(originalPath))
        {
            errorMusicLoad = 1;
            Debug.LogWarning("direccion de musica no valido");
            return;
        }

        if (File.Exists(finalDir))
        {
            Debug.LogWarning("el archivo ya existe en la carpeta de nivel");
            return;
        }

        await Task.Run(() =>
        {
            File.Copy(originalPath, finalDir, true);
        });
    }

    public static async Task SaveMetadata(LevelMetadata metadata,string levelName)
    {
        FindGameFolder();
        FindLevelFolder(levelName);

        string dataName = levelName + ".meta";

        string JsonString = JsonUtility.ToJson(metadata, true);

        string dir = Path.Combine(MainPath,nombreCarpetaJuego, levelName,dataName);

        await Task.Run(() =>
        {
            File.WriteAllText(dir, JsonString);
        });

        Debug.Log("se guardo la metadata");
    }

    public static Level LoadDataLevel(string folderName,string levelName)
    {
        string dir = Path.Combine(MainPath , nombreCarpetaJuego, folderName, levelName);

        if (!File.Exists(dir))
        {
            return new(new());
        }

        Level notas;

        string JsonString = File.ReadAllText(dir);

        //hacer esto con task.run es pegriloso :c
        notas = JsonUtility.FromJson<Level>(JsonString);

        return notas;
    }

    public static bool LevelExist(string folderName, string levelName)
    {
        string dir = Path.Combine(MainPath, nombreCarpetaJuego, folderName, levelName);
        return File.Exists(dir);
    }

    public static LevelMetadata LoadMetadata(string levelName)
    {
        string dataName = levelName + ".meta";

        string dir = Path.Combine(MainPath, nombreCarpetaJuego, levelName, dataName);

        if (!File.Exists(dir))
        {
            Debug.Log("el archivo no existe |" + dir);
            return new LevelMetadata();
        }

        string JsonString = File.ReadAllText(dir);

        return JsonUtility.FromJson<LevelMetadata>(JsonString);
    }

    public static bool MetadataExists(string levelName)
    {
        string dataName = levelName + ".meta";
        string dir = Path.Combine(MainPath, nombreCarpetaJuego, levelName, dataName);
        return File.Exists(dir);
    }

    public static string MainPath => Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
}
