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
    [Header("configuracion nota sostenida")]
    public float duracion;

    public Vector2 offsetPositionToGo;
    public Vector2 DireccionCustom;

    public NotaTileInstance(
        int noteIndex,
        TipoNota tipoNota,
        CorrespondenciaTecla CorrespondenciaTecla,
        DireccionesMovimientoNotas DireccionMovimiento,
        float timeToArrive,
        Vector2 offsetPositionToGo,
        Vector2 DireccionCustom,
        float duracion,
        float localSpeed = 1.0f)
    {
        this.noteIndex = noteIndex;
        this.tipoNota = tipoNota;
        this.CorrespondenciaTecla = CorrespondenciaTecla;
        this.timeToArrive = timeToArrive;
        this.localSpeed = localSpeed;
        this.duracion = duracion;
        this.offsetPositionToGo = offsetPositionToGo;
        this.DireccionMovimiento = DireccionMovimiento;
        this.DireccionCustom = DireccionCustom;
    }
}

[Serializable]
public class PrefabNote
{
    public GameObject prefab;
    public TipoNota tipoNota;
    public int IntialInstances;

    public PrefabNote(GameObject prefab, TipoNota tipoNota, int IntialInstances)
    {
        this.prefab = prefab;
        this.tipoNota = tipoNota;
        this.IntialInstances = IntialInstances;
    }
}

[Serializable]
public class NotaTileList
{
    public List<NotaTileInstance> notasTiles;
}