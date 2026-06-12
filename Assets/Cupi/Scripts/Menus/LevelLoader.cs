using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private Button botonJugar;

    [SerializeField] private int ultimaDificultadSeleccionada;

    public int ultimoNivelSeleccionado;

    [SerializeField] private LevelViewer levelViewer;
    [SerializeField] private MusicLoader musicLoader;

    public void LoadLevel()
    {
        botonJugar.interactable = false;

        LevelDataController.instance.folderName = levelViewer.levels[ultimoNivelSeleccionado].name;

        LevelDataController.instance.levelName = levelViewer.levels[ultimoNivelSeleccionado].levelData.LevelsFiles[ultimaDificultadSeleccionada].levelFileName;

        LevelDataController.instance.actualMetadata = levelViewer.levels[ultimoNivelSeleccionado].levelData;

        LevelDataController.instance.musicLoader = musicLoader;

        musicLoader.ClearMusicCache();

        SceneManager.LoadSceneAsync("LoadLevel");
    }

    public void DificultadSeleccionada(int option)
    {
        ultimaDificultadSeleccionada = option;
    }
}
