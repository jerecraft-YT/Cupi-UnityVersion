using UnityEngine;

public class NotaNormal : MonoBehaviour
{
    public NotaNormalInstance data;
    public Transform origin;

    public void Initialize(NotaNormalInstance config)
    {
        data = config;
    }

    public Vector2 DireccionMovimiento;

    public void UpdateNotePosition()
    {
        float progress = 1 - InverseLerpUnclamped(0.0f,data.timeToArrive,(float)TimeController.instance.AdditiveTime);

        float distancia = (progress * data.timeToArrive * data.localSpeed * SpawnerNotas.instance.notaNormalSpeed);

        transform.localPosition = data.offsetPositionToGo + (DireccionMovimiento * distancia);

        //if (progress <= 0) sprite.color = new Color(1.0f,1.0f,1.0f,0.0f);
    }

    private float InverseLerpUnclamped(float a,float b,float valor)
    {
        if (b != a) return (valor - a) / (b - a);

        return 0.0f;
    }

    public void DestroyNote()
    {
        TilesModeNotesController.instance.activeNotes.Remove(this);
        transform.parent = origin;
        gameObject.SetActive(false);
    }
}
