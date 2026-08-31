using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Scriptable Objects/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    public MainLevel levelChart;
    public LevelMetadata levelMetadata;
    public string levelFolder;
    public string levelName;

    public void Reset()
    {
        levelChart = null;
        levelMetadata = null;
        levelFolder = "";
        levelName = "";
    }
}
