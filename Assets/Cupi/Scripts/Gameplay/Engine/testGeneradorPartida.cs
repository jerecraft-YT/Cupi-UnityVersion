using System.Collections.Generic;
using UnityEngine;

public class testGeneradorPartida : MonoBehaviour
{
    public List<NotaInstance> chart1;
    public List<NotaInstance> chart2;

    void Start()
    {
        //Application.targetFrameRate = 1;

        if (!string.IsNullOrEmpty(LevelDataController.instance.levelName))
        {
            chart1 = LevelDataController.instance.actualLevel.notas;
        }

        CreateGameplay("gameplay1", chart1 , ModoInput.Bot);
    }

    private void CreateGameplay(string gameplayName, List<NotaInstance> chart,ModoInput modoInput)
    {
        GameObject gameplayInstanceGO = new (gameplayName);
        gameplayInstanceGO.transform.SetParent(transform);

        GameplayInstance gameplayInstance = gameplayInstanceGO.AddComponent<GameplayInstance>();

        IInputDevice inputDevice = SetInputDevice(modoInput , chart ,gameplayInstance);

        gameplayInstance.Initialize(chart, ModoJuego.Tile, inputDevice);
    }

    private IInputDevice SetInputDevice(ModoInput modoInput,List<NotaInstance> chart, GameplayInstance gameplayInstance)
    {
        switch (modoInput)
        {
            case ModoInput.None:
                break;
            case ModoInput.Player:
                PlayerInputs playerInputs = new PlayerInputs(TileModePlayStyle.FourKeys);

                return playerInputs;

            case ModoInput.Bot:
                BotInputs botInputs = new BotInputs(chart);
                gameplayInstance.GameTick += botInputs.BotTick;

                return botInputs;
            case ModoInput.Custom:
                break;
            default:
                break;
        }

        return null;
    }
}
