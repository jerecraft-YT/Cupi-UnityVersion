using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using CupiEngine.ResourceLoader.Audio;
using CupiEngine.ResourceLoader.Levels;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] private Button _botonJugar;
    [SerializeField] private int _ultimaDificultadSeleccionada;
    [SerializeField] private GameObject[] _objectsToHide;
    [SerializeField] private SceneField _sceneToLoad;

    [SerializeField] private LevelViewer _levelViewer;

    [SerializeField] private LevelDataSO _levelDataSO;

    public int ultimoNivelSeleccionado;

    public void LoadLevel()
    {
        if (_levelViewer.levels.Count == 0)
        {
            Debug.Log("no hay niveles para jugar");
            return;
        }

        _botonJugar.interactable = false;

        LevelInfo infoActualLevel = _levelViewer.levels[ultimoNivelSeleccionado];

        string folderName = infoActualLevel.folderName;

        string levelName = infoActualLevel.levelData.levelsFiles[_ultimaDificultadSeleccionada].levelFileName;

        LevelMetadata levelMetadata = infoActualLevel.levelData;

        CupiLevelsLoader.SetAllLevelData(_levelDataSO, levelName, levelMetadata, folderName);

        CupiMusicLoader.ClearMusicCache();

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
