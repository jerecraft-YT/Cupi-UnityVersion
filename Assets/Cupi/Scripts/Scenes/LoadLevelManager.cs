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
    private MusicLoader musicLoader;

    void Start()
    {
        musicLoader = LevelDataController.instance.musicLoader;

        StartCoroutine(EsperaCarga());
    }

    private IEnumerator EsperaCarga()
    {
        Debug.Log("-----CARGANDO-----");

        yield return new WaitForSeconds(3.0f);

        Debug.Log("cargando nivel...");

        LevelDataController.instance.LoadDataLevel();

        yield return new WaitForSeconds(3.0f);

        carga = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        carga.allowSceneActivation = false;
        UnloadScenes();

        Debug.Log("-----TERMINO CARGA-----");
    }

    private void UnloadScenes()
    {
        foreach(string scene in scenesToUnload)
        {
            SceneManager.UnloadSceneAsync(scene);
        }
    }

    private void Update()
    {
        if (musicLoader != null && musicLoader.readyForNewLoad == true)
        {
            Destroy(musicLoader.gameObject);
        }

        if (carga != null && carga.progress >= 0.9f)
        {
            TimeController.instance.SetTime(0.0f);
            carga.allowSceneActivation = true;
        }

    }
}
