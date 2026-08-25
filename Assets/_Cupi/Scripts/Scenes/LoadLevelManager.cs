using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelManager : MonoBehaviour
{
    [SerializeField] private float _tiempoBaseNivel;
    private AsyncOperation _carga;
    [SerializeField] private SceneField _sceneToLoad;
    [SerializeField] private SceneField[] scenesToUnload;
    //private MusicLoader _musicLoader;
    private bool _loaded = false;
    public float duracionTransicionMusica = 1.0f;

    void Start()
    {
        //_musicLoader = LevelDataController.instance.musicLoader;

        DontDestroyOnLoad(this);

        StartLoad();
    }

    private async void StartLoad()
    {
        await EsperaCarga();
    }

    private async Task EsperaCarga()
    {
        Debug.Log("-----CARGANDO-----");
        
        //yield return new WaitForSeconds(2.0f);

        await Awaitable.EndOfFrameAsync();

        Debug.Log("cargando nivel...");

        LevelDataController.instance.LoadDataLevel();

        //yield return new WaitForSeconds(2.0f);

        _carga = SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Additive);
        _carga.allowSceneActivation = false;
        UnloadScenes();

        Debug.Log("-----TERMINO CARGA-----");

        //yield return null;
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
        /*
        if (_musicLoader != null && _musicLoader.readyForNewLoad == true)
        {
            //elimina music loader porque ya no lo necesitamos para el resto del nivel
            Destroy(_musicLoader.gameObject);
        }
        */

        if (_carga != null && _carga.progress >= 0.9f && !_loaded)
        {
            _loaded = true;
            FinalLoading();
        }
    }

    private async void FinalLoading()
    {
        Debug.Log("finalizando Carga");
        await MusicController.MusicFadeIn(duracionTransicionMusica);

        TimeController.instance.SetTime(_tiempoBaseNivel);
        _carga.allowSceneActivation = true;

        Destroy(this);
    }
}
