using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TilesModeMaster : MonoBehaviour
{
    public static TilesModeMaster instance;

    [Header("Config")]

    [Tooltip("Margen de error para las notas (en segundos)")]
    public float toleranciaError;

    [Tooltip("Velocidad general de notas")]
    public float notaTileSpeed = 4;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

}
