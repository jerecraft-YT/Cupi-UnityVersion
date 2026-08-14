using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Button _botonJugar;

    [SerializeField] private int _ultimaDificultadSeleccionada;

    [SerializeField] private GameObject[] _objectsToHide;
    [SerializeField] private SceneField _sceneToLoad;

    [SerializeField] private LevelViewer _levelViewer;
    [SerializeField] private MusicLoader _musicLoader;

    public int ultimoNivelSeleccionado;

    public void LoadLevel()
    {
        if (_levelViewer.levels.Count == 0)
        {
            Debug.Log("no hay niveles para jugar");
            return;
        }

        _botonJugar.interactable = false;

        LevelDataController dataLevel = LevelDataController.instance;

        LevelInfo infoActualLevel = _levelViewer.levels[ultimoNivelSeleccionado];

        dataLevel.folderName = infoActualLevel.name;

        dataLevel.levelName = infoActualLevel.levelData.levelsFiles[_ultimaDificultadSeleccionada].levelFileName;

        dataLevel.actualMetadata = infoActualLevel.levelData;

        dataLevel.musicLoader = _musicLoader;

        _musicLoader.ClearMusicCache();

        HideMenu();

        SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Additive);
    }

    private void HideMenu()
    {
        foreach(var obj in _objectsToHide)
        {
            obj.SetActive(false);
        }
    }

    public void DificultadSeleccionada(int option)
    {
        _ultimaDificultadSeleccionada = option;
    }
}
