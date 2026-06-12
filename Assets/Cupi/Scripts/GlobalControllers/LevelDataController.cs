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
    
    public void RemoveAllDataLevel()
    {
        actualLevel = null;
        actualMetadata = null;
    }

    public void LoadLevel()
    {
        actualLevel = LoadJsonLevel.LoadLevel(folderName, levelName);
    }
}
