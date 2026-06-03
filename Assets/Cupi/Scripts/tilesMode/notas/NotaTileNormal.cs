using System;
using UnityEngine;

public class NotaTileNormal : MonoBehaviour
{
    public NotaTileInstance data;
    public Transform origin;
    public event Action<Vector2> UpdateNote;

    public void Initialize(NotaTileInstance config)
    {
        data = config;
    }

    public Vector2 DireccionMovimiento;

    public void UpdateNotePosition()
    {

        float progress = 1 - InverseLerpUnclamped(0.0f, data.timeToArrive, (float)TimeController.instance.AdditiveTime);

        float distancia = (progress * data.timeToArrive * data.localSpeed * SpawnerNotas.instance.notaTileSpeed);

        Vector2 finalPos = data.offsetPositionToGo + (DireccionMovimiento * distancia);

        transform.localPosition = finalPos;

        UpdateNote?.Invoke(finalPos);

        //if (progress <= 0) sprite.color = new Color(1.0f,1.0f,1.0f,0.0f);
    }

    private float InverseLerpUnclamped(float a, float b, float valor)
    {
        if (b != a) return (valor - a) / (b - a);

        return 0.0f;
    }

    public void DestroyNote()
    {
        TilesModeNotesController.instance.NotasActivas.Remove(this);
        transform.parent = origin;
        gameObject.SetActive(false);
    }
}
