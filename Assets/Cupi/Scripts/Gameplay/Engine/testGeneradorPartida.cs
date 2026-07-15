using System.Collections.Generic;
using UnityEngine;

public class testGeneradorPartida : MonoBehaviour
{
    public List<NotaInstance> chart;

    void Start()
    {
        if (!string.IsNullOrEmpty(LevelDataController.instance.levelName))
        {
            chart = LevelDataController.instance.actualLevel.notas;
        }

        PlayerInputs playerInputs = new PlayerInputs();
        playerInputs.Initialize(TileModePlayStyle.FourKeys);

        GameObject gameplayInstanceGO = new GameObject();
        gameplayInstanceGO.name = "gameplay1";

        GameplayInstance gameplayInstance = gameplayInstanceGO.AddComponent<GameplayInstance>();

        gameplayInstance.Initialize(chart, ModoJuego.Tile, playerInputs);
    }
}
