using UnityEngine;

public class LoadSingletons : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        GameObject go = new GameObject("GameManager");
        DontDestroyOnLoad(go);

        go.AddComponent<TimeController>();
       //go.AddComponent<LoadAndSaveData>();
        go.AddComponent<MusicController>();
    }
}
