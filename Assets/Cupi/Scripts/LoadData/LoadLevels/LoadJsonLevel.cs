using System.IO;
using UnityEngine;

public class LoadJsonLevel : MonoBehaviour
{
    public void SaveJson()
    {
        NotaTileList conversor = new NotaTileList { notasTiles = SpawnerNotas.instance.notasTiles };

        string JsonString = JsonUtility.ToJson(conversor, true);

        //print(JsonString);
        string dir = Application.persistentDataPath + "/dataTest.json";
        //print(dir);

        File.WriteAllText(dir, JsonString);
    }

    public void LoadJson()
    {
        string dir = Path.Combine(Application.persistentDataPath, "dataTest.json");

        if (!File.Exists(dir))
        {
            return;
        }

        string JsonString = File.ReadAllText(dir);

        NotaTileList notas = JsonUtility.FromJson<NotaTileList>(JsonString);

        SpawnerNotas.instance.notasTiles = notas.notasTiles;
    }
}
