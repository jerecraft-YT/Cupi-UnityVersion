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
public class NotasList
{
    public List<NotaInstance> notas;
}

[Serializable]
public struct LevelData
{
    [Header("Datos")]
    public string Name;
    public string Description;
    public string Artist;
    public string Autor;
    public float PreviewTimeMusic;
    public string MusicFileName;
    public string Tags;
    public float Bpm;
}