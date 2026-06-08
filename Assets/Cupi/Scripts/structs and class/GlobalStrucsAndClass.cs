using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NotaTileInstance
{
    [Header("Definicion Inicial")]
    public int noteIndex;
    public TipoNota tipoNota;
    public CorrespondenciaTecla CorrespondenciaTecla;
    public DireccionesMovimientoNotas DireccionMovimiento;
    [Header("configuracion Base")]
    public float timeToArrive;
    public float localSpeed;
    [Header("configuracion Nota Sostenida")]
    public float duracion;

    public Vector2 offsetPositionToGo;
    public Vector2 DireccionCustom;
}

[Serializable]
public class PrefabNote
{
    public GameObject prefab;
    public TipoNota tipoNota;
    public int IntialInstances;
}

[Serializable]
public class NotaTileList
{
    public List<NotaTileInstance> notasTiles;
}

[Serializable]
public class PosicionNota
{
    public CorrespondenciaTecla tecla;
    public Transform posicion;
}