using UnityEngine;

public class GameplayGenerator : MonoBehaviour
{
    public LevelComposition[] modosActivos;

    void Start()
    {
        //Application.targetFrameRate = 1;

        if (!string.IsNullOrEmpty(LevelDataController.instance.levelName))
        {
            modosActivos[0].chart = LevelDataController.instance.actualLevel.notas;
        }

        GameplayConstructor.CreateGameplay("gameplay1", modosActivos[0], transform);
    }
}
