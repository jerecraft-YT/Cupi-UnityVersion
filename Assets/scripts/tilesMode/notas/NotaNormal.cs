using UnityEngine;

public class NotaNormal : MonoBehaviour
{
    public NotaNormalInstance data;
    private SpriteRenderer sprite;

    public void Initialize(NotaNormalInstance config)
    {
        data = config;
        sprite = GetComponent<SpriteRenderer>();
    }

    public Vector2 DireccionMovimiento;

    void Update()
    {
        float progress = 1 - Mathf.InverseLerp(0.0f,data.timeToArrive,(float)TimeController.instance.ActualTime);

        float distancia = (progress * data.timeToArrive * data.localSpeed * SpawnerNotas.instance.notaNormalSpeed);

        transform.localPosition = data.offsetPositionToGo + (DireccionMovimiento * distancia);

        if (progress <= 0) sprite.color = new Color(1.0f,1.0f,1.0f,0.0f);
    }
}
