using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

[Serializable]
public struct NotaNormalInstance
{
    public CorrespondenciaTecla CorrespondenciaTecla;
    public DireccionesMovimientoNotas DireccionMovimiento;

    public float timeToArrive;
    public float localSpeed;

    public Vector2 offsetPositionToGo;
    public Vector2 DireccionCustom;

    public NotaNormalInstance(
        CorrespondenciaTecla CorrespondenciaTecla,
        DireccionesMovimientoNotas DireccionMovimiento,
        float timeToArrive,
        Vector2 offsetPositionToGo,
        Vector2 DireccionCustom,
        float localSpeed = 1.0f)
    {
        this.CorrespondenciaTecla = CorrespondenciaTecla;
        this.timeToArrive = timeToArrive;
        this.localSpeed = localSpeed;
        this.offsetPositionToGo = offsetPositionToGo;
        this.DireccionMovimiento = DireccionMovimiento;
        this.DireccionCustom = DireccionCustom;
    }
}

[Serializable]
public class NotaNormalList
{
    public List<NotaNormalInstance> notasNormales;
}

public class SpawnerNotas : MonoBehaviour
{
    public GameObject notaNormal;
    public Transform finalPositionLeftNote;
    public Transform finalPositionMidleNote;
    public Transform finalPositionRigthNote;

    public static SpawnerNotas instance;
    public float notaNormalSpeed = 4;

    public List<NotaNormalInstance> notasNormales;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        foreach (NotaNormalInstance notaActual in notasNormales)
        {
            GameObject nota = Instantiate(notaNormal,DefinirCorrespondenciaTecla(notaActual.CorrespondenciaTecla));

            NotaNormal scriptNota = nota.GetComponent<NotaNormal>();

            scriptNota.Initialize(notaActual);

            scriptNota.DireccionMovimiento = EstablecerDireccionMovimiento(notaActual.DireccionMovimiento, notaActual.DireccionCustom);
        }

    }

    void SaveJson()
    {
        NotaNormalList conversor = new NotaNormalList { notasNormales = notasNormales };

        string JsonString = JsonUtility.ToJson(conversor, true);

        print(JsonString);
        string dir = Application.persistentDataPath + "/dataTest.json";
        //print(dir);

        File.WriteAllText(dir, JsonString);
    }

    Transform DefinirCorrespondenciaTecla(CorrespondenciaTecla CorrespondenciaTecla)
    {
        switch (CorrespondenciaTecla)
        {
            case CorrespondenciaTecla.Left:
                return finalPositionLeftNote;
            case CorrespondenciaTecla.Midle:
                return finalPositionMidleNote;
            case CorrespondenciaTecla.Right:
                return finalPositionRigthNote;
        }

        return transform;
    }

    Vector2 EstablecerDireccionMovimiento(DireccionesMovimientoNotas DireccionMovimiento, Vector2 DireccionCustom)
    {
        switch (DireccionMovimiento)
        {
            case DireccionesMovimientoNotas.Up:
                return Vector2.up;
            case DireccionesMovimientoNotas.Down:
                return Vector2.down;
            case DireccionesMovimientoNotas.Left:
                return Vector2.left;
            case DireccionesMovimientoNotas.Right:
                return Vector2.right;
            case DireccionesMovimientoNotas.Custom:
                return DireccionCustom;
        }
        return Vector2.zero;
    }
}
