using System.Collections.Generic;
using UnityEngine;

public class GameplayConstructor
{
    public static void CreateGameplay(string gameplayName, LevelComposition level,Transform parent)
    {
        GameObject gameplayInstanceGO = new(gameplayName);
        gameplayInstanceGO.transform.SetParent(parent);
        gameplayInstanceGO.transform.localPosition = Vector3.zero;

        GameplayInstance gameplayInstance = gameplayInstanceGO.AddComponent<GameplayInstance>();

        IInputDevice inputDevice = SetInputDevice(level.modoInput, level.chart, gameplayInstance);

        ITimeProvider timeProvider = SetTimeProvider(level.modoTime);

        gameplayInstance.Initialize(level, inputDevice, timeProvider);
    }

    private static ITimeProvider SetTimeProvider(ModoTime modoTime)
    {
        switch (modoTime)
        {
            case ModoTime.None:
                break;
            case ModoTime.Global:
                return TimeController.instance;
            case ModoTime.Custom:
                break;
            default:
                break;
        }
        return null;
    }

    private static IInputDevice SetInputDevice(ModoInput modoInput, List<NotaInstance> chart, GameplayInstance gameplayInstance)
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
