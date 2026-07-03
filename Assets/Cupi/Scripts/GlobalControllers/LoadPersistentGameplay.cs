using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadPersistentGameplay : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (VerificarEscena())
        {
            SceneManager.LoadScene("PersistentGameplay", LoadSceneMode.Additive);
        }


        GameObject go = new GameObject("GameManager");

        DontDestroyOnLoad(go);

        go.AddComponent<TimeController>();
        go.AddComponent<MusicController>();
        go.AddComponent<LevelDataController>();
        go.AddComponent<InputController>();
    }

    static bool VerificarEscena()
    {
        for (int i = SceneManager.sceneCount; i < 0; i--)
        {
            Scene scenesInGameplay = SceneManager.GetSceneAt(i);

            if (scenesInGameplay.name == "PersistentGameplay")
            {
                return false;
            }
        }

        return true;
    }
}
