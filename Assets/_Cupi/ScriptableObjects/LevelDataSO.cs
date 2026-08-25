using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Scriptable Objects/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    public MainLevel levelChart;

    private void Reset()
    {
        levelChart = null;
    }
}
