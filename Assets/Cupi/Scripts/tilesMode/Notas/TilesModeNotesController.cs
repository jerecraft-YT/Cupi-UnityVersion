using UnityEngine;
using System;

public class TilesModeNotesController : MonoBehaviour
{
    public static TilesModeNotesController instance;

    //public List<NotaTileNormal> NotasActivas = new();
    public static event Action NotasActivas;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Update()
    {
        NotasActivas?.Invoke();
    }
}
