using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct NotaNormalInstance
{
    public int noteIndex;
    public CorrespondenciaTecla CorrespondenciaTecla;
    public DireccionesMovimientoNotas DireccionMovimiento;

    public float timeToArrive;
    public float localSpeed;

    public Vector2 offsetPositionToGo;
    public Vector2 DireccionCustom;

    public NotaNormalInstance(
        int noteIndex,
        CorrespondenciaTecla CorrespondenciaTecla,
        DireccionesMovimientoNotas DireccionMovimiento,
        float timeToArrive,
        Vector2 offsetPositionToGo,
        Vector2 DireccionCustom,
        float localSpeed = 1.0f)
    {
        this.noteIndex = noteIndex;
        this.CorrespondenciaTecla = CorrespondenciaTecla;
        this.timeToArrive = timeToArrive;
        this.localSpeed = localSpeed;
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
public class NotaNormalList
{
    public List<NotaNormalInstance> notasNormales;
}