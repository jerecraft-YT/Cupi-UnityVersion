using System.Collections.Generic;
using UnityEngine;

public class testGeneradorPartida : MonoBehaviour
{
    public List<NotaInstance> chart1;
    public List<NotaInstance> chart2;

    void Start()
    {
        //Application.targetFrameRate = 10;

        if (!string.IsNullOrEmpty(LevelDataController.instance.levelName))
        {
            chart1 = LevelDataController.instance.actualLevel.notas;
        }

        CreateGameplay("gameplay1", chart1,ModoInput.Bot);
    }

    private void CreateGameplay(string name, List<NotaInstance> chart,ModoInput modoInput)
    {
        IInputDevice inputDevice = null;

        GameObject gameplayInstanceGO = new(name);

        GameplayInstance gameplayInstance = gameplayInstanceGO.AddComponent<GameplayInstance>();

        switch (modoInput)
        {
            case ModoInput.None:
                break;
            case ModoInput.Player:
                PlayerInputs playerInputs = new PlayerInputs(TileModePlayStyle.FourKeys);

                inputDevice = playerInputs;
                break;

            case ModoInput.Bot:
                BotInputs botInputs = new BotInputs(chart);
                gameplayInstance.GameTick += botInputs.BotTick;

                inputDevice = botInputs;

                break;
            case ModoInput.Custom:
                break;
            default:
                break;
        }

        gameplayInstance.Initialize(chart, ModoJuego.Tile, inputDevice);
    }
}
