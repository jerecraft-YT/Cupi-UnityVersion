using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadMainLevel : MonoBehaviour
{
    public Slider slider;
    public AsyncOperation carga;
    public BpmController bpm;

    void Start()
    {
        StartCoroutine(EsperaCarga());
        LevelDataController.instance.musicLoader.bpmController = bpm;
    }

    private IEnumerator EsperaCarga()
    {
        yield return new WaitUntil(() => Time.timeSinceLevelLoad > 2.0f);

        Debug.Log("cargando nivel...");

        LevelDataController.instance.LoadLevel();

        yield return new WaitUntil(() => Time.timeSinceLevelLoad > 1.0f && LevelDataController.instance.musicLoader == null);

        StartCoroutine(CargarEscena("TestZone"));
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
        if (LevelDataController.instance.musicLoader != null && LevelDataController.instance.musicLoader.musicToLoadComplete == true)
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
