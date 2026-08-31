using UnityEngine;

public class GameplayGenerator : MonoBehaviour
{
    [SerializeField] private LevelDataSO levelData;
    public LevelComposition[] modosActivos;
    

    void Start()
    {
        //Application.targetFrameRate = 1;

        if (!string.IsNullOrEmpty(levelData.levelName))
        {
            modosActivos[0].chart = levelData.levelChart.notas;
        }

        GameplayConstructor.CreateGameplay("gameplay1", modosActivos[0], transform);
    }
}
