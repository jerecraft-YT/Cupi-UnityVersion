using System.IO;
using UnityEngine;

public class LoadJsonLevel : MonoBehaviour
{

    public static void SaveJson()
    {
        NotasList conversor = new NotasList { notas = SpawnerNotas.instance.notasToInstance };

        string JsonString = JsonUtility.ToJson(conversor, true);

        //print(JsonString);
        string dir = Application.persistentDataPath + "/dataTest.json";
        //print(dir);

        File.WriteAllText(dir, JsonString);
    }

    public static void LoadJson()
    {
        string dir = Path.Combine(Application.persistentDataPath, "dataTest.json");

        if (!File.Exists(dir))
        {
            return;
        }

        string JsonString = File.ReadAllText(dir);

        print(JsonString);

        NotasList notas = JsonUtility.FromJson<NotasList>(JsonString);

        print(notas.ToString());

        SpawnerNotas.instance.notasToInstance = notas.notas;
    }
}
