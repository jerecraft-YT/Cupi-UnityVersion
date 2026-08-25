using System;
using System.Collections.Generic;
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

    [Header("variables de debug (solo visualización)")]
    public int startWindow;

    public int endWindow;

    public void Initialize(LevelComposition level,IInputDevice input,ITimeProvider timeProvider)
    {
        SortNotes(level.chart);

        //el chart manda sobre el modo de juego, y hay que ajustarlo antes de crear nada
        //porque el renderer arma sus carriles con este dato en su Initialize
        level.tileModePlayStyle = AjustarPlayStyleAlChart(level.chart, level.tileModePlayStyle);

        this.level = level;

        levelChart = level.chart;
        gameInput = input;
        gameTime = timeProvider;

        gameplayRenderer = gameObject.AddComponent<GameplayRenderer>();
        gameplayRenderer.Initialize(level,timeProvider);

        gameplayEngine = new GameplayEngine(gameTime,input, level.chart, level.tileModePlayStyle);
        gameplayEngine.NoteChange += gameplayRenderer.NoteChange;

        //el renderer va ANTES que el engine: asi la nota ya esta instanciada cuando el engine
        //la juzga en este mismo tick. al reves el engine juzgaba con la cola del tick anterior
        //y cualquier salto de tiempo grande la procesaba antes de que existiera en pantalla
        GameTick += gameplayRenderer.EngineTick;
        GameTick += gameplayEngine.EngineTick;
    }

    /// <summary>
    /// el numero de carriles lo manda el chart: si el nivel trae notas por encima del modo
    /// configurado se amplia el modo. si no, el renderer se salta esas notas (no tiene carril
    /// donde ponerlas) pero el engine las sigue juzgando, y de ahi salen los avisos de
    /// "el engine proceso la nota X pero el renderer no la tenia activa"
    /// </summary>
    private TileModePlayStyle AjustarPlayStyleAlChart(List<NotaInstance> chart, TileModePlayStyle playStyle)
    {
        int teclaMasAlta = -1;

        foreach (NotaInstance nota in chart)
        {
            int tecla = (int)nota.correspondenciaTecla;

            //None y cualquier valor raro no cuentan como tecla jugable
            if (tecla > (int)CorrespondenciaTecla.Ten) continue;

            if (tecla > teclaMasAlta) teclaMasAlta = tecla;
        }

        //chart vacio o sin ninguna tecla valida, no hay nada que ajustar
        if (teclaMasAlta < 0) return playStyle;

        bool modoSinDefinir = playStyle == TileModePlayStyle.None;

        if (!modoSinDefinir && teclaMasAlta <= (int)playStyle) return playStyle;

        TileModePlayStyle playStyleDelChart = (TileModePlayStyle)teclaMasAlta;

        Debug.LogWarning($"el chart llega hasta la tecla {(CorrespondenciaTecla)teclaMasAlta} pero el modo estaba en {playStyle}, se amplia a {playStyleDelChart}");

        return playStyleDelChart;
    }

    /// <summary>
    /// ordena la MISMA lista en el sitio, sin crear una nueva. el input ya recibio la
    /// referencia antes de llegar aca, si se reemplaza la lista se queda con la vieja
    /// sin ordenar y deja de ver lo mismo que el engine
    /// </summary>
    private void SortNotes(List<NotaInstance> chart)
    {
        chart.Sort((notaA, notaB) => notaA.timeToArrive.CompareTo(notaB.timeToArrive));

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

        SetDebugValues();
    }

    private void SetDebugValues()
    {
        endWindow = gameplayEngine.endWindow;
        startWindow = gameplayEngine.startWindow;
    }

    private void OnDestroy()
    {
        gameInput?.Dispose();
        gameplayEngine?.Dispose();
    }
}
