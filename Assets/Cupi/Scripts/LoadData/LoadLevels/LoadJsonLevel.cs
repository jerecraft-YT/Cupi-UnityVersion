using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class LoadJsonLevel
{
    public static string nombreCarpetaJuego = "CUPI";

    public static void SaveAll(List<NotaInstance> notasToSave, string levelName, LevelMetadata metadata)
    {
        SaveLevel(notasToSave,levelName);
        SaveMetadata(metadata,levelName);
    }

    public static void FindGameFolder()
    {
        string MainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),nombreCarpetaJuego);

        if (!Directory.Exists(MainPath))
        {
            Directory.CreateDirectory(MainPath);
        }
    }

    private static void FindLevelFolder(string folderName)
    {
        string MainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),nombreCarpetaJuego,folderName);

        if (!Directory.Exists(MainPath))
        {
            Directory.CreateDirectory(MainPath);
        }
    }

    public static void SaveLevel(List<NotaInstance> notasToSave,string levelName)
    {
        FindGameFolder();
        FindLevelFolder(levelName);

        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


        Level LevelToSave = new(notasToSave);

        string JsonString = JsonUtility.ToJson(LevelToSave, true);

        string dir = Path.Combine(MainPath, nombreCarpetaJuego, levelName);

        File.WriteAllText(dir, JsonString);
    }

    public static void SaveMetadata(LevelMetadata metadata,string levelName)
    {
        FindGameFolder();
        FindLevelFolder(levelName);

        string dataName = levelName + ".meta";

        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        string JsonString = JsonUtility.ToJson(metadata, true);

        string dir = Path.Combine(MainPath,nombreCarpetaJuego, levelName,dataName);

        File.WriteAllText(dir, JsonString);
    }

    public static Level LoadLevel(string folderName,string levelName)
    {
        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        string dir = Path.Combine(MainPath , nombreCarpetaJuego, folderName, levelName);

        if (!File.Exists(dir))
        {
            return new(new());
        }

        string JsonString = File.ReadAllText(dir);

        Level notas = JsonUtility.FromJson<Level>(JsonString);

        return notas;
    }

    public static bool LevelExist(string folderName, string levelName)
    {
        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string dir = Path.Combine(MainPath, nombreCarpetaJuego, folderName, levelName);
        return File.Exists(dir);
    }

    public static LevelMetadata LoadMetadata(string levelName)
    {
        string dataName = levelName + ".meta";

        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

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
        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string dir = Path.Combine(MainPath, nombreCarpetaJuego, levelName, dataName);
        return File.Exists(dir);
    }
}
