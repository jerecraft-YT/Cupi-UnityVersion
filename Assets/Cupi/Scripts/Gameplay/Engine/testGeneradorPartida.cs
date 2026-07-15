using System.Collections.Generic;
using UnityEngine;

public class testGeneradorPartida : MonoBehaviour
{
    public List<NotaInstance> chart1;
    public List<NotaInstance> chart2;

    void Start()
    {
        if (!string.IsNullOrEmpty(LevelDataController.instance.levelName))
        {
            chart1 = LevelDataController.instance.actualLevel.notas;
        }

        CreateGameplay("gameplay1", chart1);
    }

    private void CreateGameplay(string name, List<NotaInstance> chart)
    {
        /*
        PlayerInputs playerInputs = new PlayerInputs();
        playerInputs.Initialize(TileModePlayStyle.FourKeys);
        */



        GameObject gameplayInstanceGO = new GameObject();
        gameplayInstanceGO.name = name;

        GameplayInstance gameplayInstance = gameplayInstanceGO.AddComponent<GameplayInstance>();

        gameplayInstance.Initialize(chart, ModoJuego.Tile, playerInputs);
    }
}
