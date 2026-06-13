using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSingletons : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        SceneManager.LoadScene("PersistentGameplay", LoadSceneMode.Additive);

        GameObject go = new GameObject("GameManager");
        DontDestroyOnLoad(go);

        go.AddComponent<TimeController>();
        go.AddComponent<MusicController>();
        go.AddComponent<LevelDataController>();
    }
}
