using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NotaInstance
{
    [Header("Definicion Inicial")]
    public int noteIndex;
    [Space(16)]
    public ModoNota modoNota;
    public TipoNota tipoNota;
    [Space(16)]
    public CorrespondenciaTecla CorrespondenciaTecla;
    public DireccionesMovimientoNotas DireccionMovimiento;
    [Space(16)]
    public Vector2 offsetPositionToGo;
    public Vector2 DireccionCustom;

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
    public TipoNota tipoNota;
    public int IntialInstances;
}

//esto se hace asi para que lo pueda guardar el json
[Serializable]
public class Level
{
    [Tooltip("datos de las notas del nivel")]
    public List<NotaInstance> notas;

    public Level(List<NotaInstance> notas)
    {
        this.notas = notas;
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
    public string Name;
    public string Description;
    public string Artist;
    public string Autor;
    public float PreviewTimeMusic;
    public string MusicFileName;
    public string Tags;
    public float Bpm;
    public List<LevelData> LevelsFiles;

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
        Name = name;
        Description = description;
        Artist = artist;
        Autor = autor;
        PreviewTimeMusic = previewTimeMusic;
        MusicFileName = musicFileName;
        Tags = tags;
        Bpm = bpm;
        LevelsFiles = levelsFiles;
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

public struct ChunkInfo
{
    public int start;
    public int end;

    public ChunkInfo(int start,int end)
    {
        this.start = start;
        this.end = end;
    }
}

public struct ChunkSeparation
{
    public TipoNota tipoNota;
    public ModoNota modoNota;

    public ChunkSeparation(TipoNota tipoNota,ModoNota modoNota)
    {
        this.tipoNota = tipoNota;
        this.modoNota = modoNota;
    }

    public ChunkSeparation(ChunkSeparation chunkSeparation)
    {
        tipoNota = chunkSeparation.tipoNota;
        modoNota = chunkSeparation.modoNota;
    }
}