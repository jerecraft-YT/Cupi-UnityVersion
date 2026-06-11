using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

public class LoadJsonLevel : MonoBehaviour
{
    public static string nombreCarpetaJuego = "CUPI";

    public static void SaveAll(List<NotaInstance> notasToSave, string levelName, LevelData metadata)
    {
        SaveLevel(notasToSave,levelName);
        SaveMetadata(metadata,levelName);
    }

    public static void FindGameFolder()
    {
        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        string[] carpetas = Directory.GetDirectories(MainPath);

        foreach (string carpet in carpetas)
        {
            string textoVerificar = Path.GetFileName(carpet.TrimEnd(Path.DirectorySeparatorChar));

            if (textoVerificar == nombreCarpetaJuego)
            {
                return;
            }
        }

        CreateBaseFolder();
    }

    private static void FindLevelFolder(string folderName)
    {
        string MainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),nombreCarpetaJuego);

        string[] carpetas = Directory.GetDirectories(MainPath);

        foreach (string carpet in carpetas)
        {
            string textoVerificar = Path.GetFileName(carpet.TrimEnd(Path.DirectorySeparatorChar));

            if (textoVerificar == folderName)
            {
                return;
            }
        }

        CreateLevelFolder(folderName);
    }

    private static void CreateLevelFolder(string folderName)
    {
        string MainPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),nombreCarpetaJuego);
        Directory.CreateDirectory(Path.Combine(MainPath, folderName));
    }

    private static void CreateBaseFolder()
    {
        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        Directory.CreateDirectory(Path.Combine(MainPath, nombreCarpetaJuego));
    }

    public static void SaveLevel(List<NotaInstance> notasToSave,string levelName)
    {
        FindGameFolder();

        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        NotasList conversor = new NotasList { notas = notasToSave};

        string JsonString = JsonUtility.ToJson(conversor, true);

        string dir = Path.Combine(MainPath, nombreCarpetaJuego, levelName);

        File.WriteAllText(dir, JsonString);
    }

    public static void SaveMetadata(LevelData metadata,string levelName)
    {
        FindGameFolder();
        FindLevelFolder(levelName);

        string dataName = levelName + ".meta";

        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        string JsonString = JsonUtility.ToJson(metadata, true);

        string dir = Path.Combine(MainPath,nombreCarpetaJuego, levelName,dataName);

        File.WriteAllText(dir, JsonString);
    }

    public static List<NotaInstance> LoadLevel(string levelName)
    {
        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        string dir = Path.Combine(MainPath , nombreCarpetaJuego, levelName, "dataTest.json");

        if (!File.Exists(dir))
        {
            return new List<NotaInstance>();
        }

        string JsonString = File.ReadAllText(dir);

        NotasList notas = JsonUtility.FromJson<NotasList>(JsonString);

        return notas.notas;
    }

    public static LevelData LoadMetadata(string levelName)
    {
        //un poco raro verificar esto pero es por si acaso
        FindGameFolder();
        FindLevelFolder(levelName);

        string dataName = levelName + ".meta";

        string MainPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        string dir = Path.Combine(MainPath, nombreCarpetaJuego, levelName, dataName);

        if (!File.Exists(dir))
        {
            print("el archivo no existe");
            return new LevelData();
        }

        string JsonString = File.ReadAllText(dir);

        return JsonUtility.FromJson<LevelData>(JsonString);
    }
}
