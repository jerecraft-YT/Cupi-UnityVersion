using UnityEngine;

public class LevelDataController : MonoBehaviour
{
    public static LevelDataController instance;

    public Level actualLevel;

    public LevelMetadata actualMetadata;

    public string folderName;

    public string levelName;

    public MusicLoader musicLoader;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    [Tooltip("convierte los datos del nivel de este script a null pero no elimina los archivos en carpetas")]
    public void RemoveAllDataLevel()
    {
        actualLevel = null;
        actualMetadata = null;
    }

    public void LoadDataLevel()
    {
        actualLevel = DataLevelsLoader.LoadDataLevel(folderName, levelName);
    }
}
