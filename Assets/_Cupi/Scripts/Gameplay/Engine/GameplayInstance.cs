using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayInstance : MonoBehaviour
{
    public event Action<double> GameTick;

    public GameplayEngine gameplayEngine;
    public GameplayRenderer gameplayRenderer;

    //referencia de momento para debug :P
    public LevelComposition level;

    public IInputDevice gameInput;
    public ITimeProvider gameTime;

    //referencia al chart original que se cargo al inicio de la partida
    public List<NotaInstance> levelChart;

    public double songTime;
    
    public void Initialize(LevelComposition level,IInputDevice input,ITimeProvider timeProvider)
    {
        SortNotes(ref level.chart);

        this.level = level;

        levelChart = level.chart;
        gameInput = input;
        gameTime = timeProvider;
 
        gameplayRenderer = gameObject.AddComponent<GameplayRenderer>();
        gameplayRenderer.Initialize(level,timeProvider);

        gameplayEngine = new GameplayEngine(gameTime,input, level.chart);
        gameplayEngine.NoteChange += gameplayRenderer.NoteChange;

        GameTick += gameplayEngine.EngineTick;
        GameTick += gameplayRenderer.EngineTick;
    }

    private void SortNotes(ref List<NotaInstance> chart)
    {
        chart = chart.OrderBy(t => t.timeToArrive).ToList();

        for (int i = 0; i < chart.Count; i++)
        {
            chart[i].noteIndex = i;
        }
    }

    //mejor para tener todo de manera mas consistente
    private void FixedUpdate()
    {
        songTime = gameTime.GetCurrentTime();

        GameTick?.Invoke(songTime);
    }

    private void OnDestroy()
    {
        gameInput?.Dispose();
        gameplayEngine?.Dispose();
    }
}
