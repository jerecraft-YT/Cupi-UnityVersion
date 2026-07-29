using System.Collections.Generic;
using UnityEngine;

public class GameplayRenderer : MonoBehaviour
{
    private float scrollSpeed;
    private ModoJuego modoJuego;
    private TileModePlayStyle playStyleTile;
    private List<NotaInstance> chart;


    public void Initialize(LevelComposition level)
    {
        scrollSpeed = level.baseScrollSpeed;
        modoJuego = level.modoJuego;
        playStyleTile = level.tileModePlayStyle;
        chart = level.chart;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void NoteChange(int index, EstadoPuntuacion puntuacion)
    {

    } 


}