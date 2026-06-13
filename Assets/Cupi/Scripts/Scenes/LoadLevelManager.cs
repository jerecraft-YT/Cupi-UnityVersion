using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadLevelManager : MonoBehaviour
{
    public Slider slider;
    public AsyncOperation carga;
    public SceneField sceneToLoad;
    public SceneField[] scenesToUnload;

    void Start()
    {
        StartCoroutine(EsperaCarga());
    }

    private IEnumerator EsperaCarga()
    {
        yield return new WaitForSeconds(3.0f);

        Debug.Log("cargando nivel...");

        LevelDataController.instance.LoadDataLevel();

        yield return new WaitForSeconds(3.0f);

        carga = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        carga.allowSceneActivation = false;
        UnloadScenes();
    }

    private void UnloadScenes()
    {
        foreach(string scene in scenesToUnload)
        {
            SceneManager.UnloadSceneAsync(scene);
        }
    }

    private IEnumerator CargarEscena(string escena)
    {
        carga = SceneManager.LoadSceneAsync(escena);

        carga.allowSceneActivation = false;

        while (!carga.isDone)
        {
            yield return null;
        }
    }

    private void Update()
    {
        if (LevelDataController.instance.musicLoader != null && LevelDataController.instance.musicLoader.readyForNewLoad == true)
        {
            Destroy(LevelDataController.instance.musicLoader.gameObject);
        }
        if (carga != null && carga.progress >= 0.9f)
        {
            TimeController.instance.SetTime(0.0f);
            carga.allowSceneActivation = true;
        }
    }
}
