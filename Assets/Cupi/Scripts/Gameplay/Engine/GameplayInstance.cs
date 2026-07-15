using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameplayInstance : MonoBehaviour
{
    public GameplayEngine gameplayEngine;
    public GameplayRenderer gameplayRenderer;
    public IInputDevice gameInput;

    public ModoJuego modoJuego;

    //referencia al chart original que se cargo al inicio de la partida
    public List<NotaInstance> levelChart;

    public float songTime;
    
    public void Initialize(List<NotaInstance> chart, ModoJuego modoJuego,IInputDevice input)
    {
        SortNotes(ref chart);

        levelChart = chart;
        this.modoJuego = modoJuego;
        gameInput = input;

        gameplayEngine = new GameplayEngine(input,chart);

        gameplayRenderer = gameObject.AddComponent<GameplayRenderer>();
        gameplayRenderer.gameplayEngine = gameplayEngine;
    }

    private void SortNotes(ref List<NotaInstance> chart)
    {
        chart = chart.OrderBy(t => t.timeToArrive).ToList();

        for (int i = 0; i < chart.Count; i++)
        {
            chart[i].noteIndex = i;
        }
    }


    // Update is called once per frame
    void Update()
    {
        songTime = (float)TimeController.instance.AdditiveTime;

        gameplayEngine.Tick(songTime);
    }

    private void OnDestroy()
    {
        gameInput?.Dispose();
        gameplayEngine?.Dispose();
    }
}
