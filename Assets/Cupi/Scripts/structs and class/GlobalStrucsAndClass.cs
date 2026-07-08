using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NotaInstance
{
    [Header("Definicion Inicial")]
    public int noteIndex;
    [Space(16)]
    public ModoNota modoNota;
    public TipoNota tipoNota;
    [Space(16)]
    public CorrespondenciaTecla correspondenciaTecla;
    public DireccionesMovimientoNotas direccionMovimiento;
    [Space(16)]
    public Vector2 offsetPositionToGo;
    public Vector2 direccionCustom;

    [Header("configuracion Base")]
    public float timeToArrive;
    public float localSpeed;
    [Header("configuracion Nota Sostenida")]
    public float duracion;

    [Header("configuracion Modo Radial")]
    public float angulo;
    [Header("configuracion Extra Nota Sostenida Modo Radial")]
    public float anguloFinal;
}

[Serializable]
public class PrefabNote
{
    public GameObject prefab;
    public TipoObjetoPool tipoObjetoPool;
    public int intialInstances;
}

//esto se hace asi para que lo pueda guardar el json
[Serializable]
public class MainLevel
{
    [Tooltip("datos de las notas del nivel")]
    public List<NotaInstance> notas;

    public MainLevel(List<NotaInstance> notas)
    {
        this.notas = notas;
    }
}

[Tooltip("dato de nota individiual para usar en chunks")]
public class ChunkNoteData
{
    public NotaInstance nota;

    //este es un valor configurable para no cambiar los datos bases de la nota
    //asi se puede tener offset de spawn o varias notas con varios puntos de aparicion
    //muy util para la nota sostenida si se ve en reversa
    public float timeToSpawn;

    public ChunkNoteData(NotaInstance nota, float timeToSpawn)
    {
        this.nota = nota;
        this.timeToSpawn = timeToSpawn;
    }
}

[Tooltip("conjunto de notas usable para chunk")]
public class ChunkLevelData
{
    public List<ChunkNoteData> notas;

    public ChunkLevelData (List<ChunkNoteData> notas)
    {
        this.notas = notas;
    }
}

public struct ChunkSeparation
{
    public TipoNota tipoNota;
    public ModoNota modoNota;

    public ChunkSeparation(TipoNota tipoNota, ModoNota modoNota)
    {
        this.tipoNota = tipoNota;
        this.modoNota = modoNota;
    }
}


[Serializable]
public class LevelData
{
    public string nombreDificultad;
    public float valorDificultad;
    [Tooltip("direccion del nivel en esa dificultad")]
    public string levelFileName;

    public LevelData (string nombreDificultad, float valorDificultad, string levelFileName)
    {
        this.nombreDificultad = nombreDificultad;
        this.valorDificultad = valorDificultad;
        this.levelFileName = levelFileName;
    }
}

[Serializable]
public class LevelMetadata
{
    [Tooltip("nombre interno del nivel en general")]
    public string name;
    public string description;
    public string artist;
    public string autor;
    public float previewTimeMusic;
    public string musicFileName;
    public string tags;
    public float bpm;
    public List<LevelData> levelsFiles;

    public LevelMetadata(
        string name = "",
        string artist = "",
        string autor = "",
        float previewTimeMusic = 0,
        string musicFileName = "",
        float bpm = 0,
        List<LevelData> levelsFiles = null,
        string tags = "",
        string description = "")
    {
        this.name = name;
        this.description = description;
        this.artist = artist;
        this.autor = autor;
        this.previewTimeMusic = previewTimeMusic;
        this.musicFileName = musicFileName;
        this.tags = tags;
        this.bpm = bpm;
        this.levelsFiles = levelsFiles;
    }
}

[Serializable]
public class LevelInfo
{
    [Tooltip("nombre de la carpeta del nivel")]
    public string name;
    [Tooltip("direcion del nivel")]
    public string directory;
    [Tooltip("metadata del nivel")]
    public LevelMetadata levelData;

    //constructor para crearlo mas facil desde codigo :3
    public LevelInfo (string name, string directory, LevelMetadata levelData)
    {
        this.name = name;
        this.directory = directory;
        this.levelData = levelData;
    }
}

[Serializable]
public class CacheAudio
{
    public AudioClip clip;
    public float lastUse;

    public CacheAudio(AudioClip clip,float lastUse)
    {
        this.clip = clip;
        this.lastUse = lastUse;
    }
}