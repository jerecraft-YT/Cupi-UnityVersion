using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadMain : MonoBehaviour
{
    public Slider slider;
    public AsyncOperation carga;

    void Start()
    {
        StartCoroutine(EsperaCarga());
    }

    private IEnumerator EsperaCarga()
    {
        yield return new WaitUntil(() => Time.time > 3.0f);

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
        if (carga != null && carga.progress >= 0.9f)
        {
            carga.allowSceneActivation = true;
        }
    }
}
