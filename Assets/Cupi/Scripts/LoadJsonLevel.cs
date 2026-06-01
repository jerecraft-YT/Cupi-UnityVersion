using System.IO;
using UnityEngine;

public class LoadJsonLevel : MonoBehaviour
{
    public void SaveJson()
    {
        NotaNormalList conversor = new NotaNormalList { notasNormales = SpawnerNotas.instance.notasNormales };

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

        NotaNormalList notas = JsonUtility.FromJson<NotaNormalList>(JsonString);

        SpawnerNotas.instance.notasNormales = notas.notasNormales;
    }
}
