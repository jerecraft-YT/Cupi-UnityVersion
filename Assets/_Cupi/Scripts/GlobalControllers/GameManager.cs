using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelDataSO levelData;

    private void OnApplicationQuit()
    {
        ClearGameSO();
    }

    private void ClearGameSO()
    {
        levelData.Reset();
        Debug.Log("LevelDataSO limpiado");
    }
}
