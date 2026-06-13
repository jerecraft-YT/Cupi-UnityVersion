using UnityEngine;

public class RadialModeMaster : MonoBehaviour
{
    public static RadialModeMaster instance;

    public float coberturaEscudo = 30.0f;

    public float radioEscudo = 1f;

    [Tooltip("valores mas altos son una calidad mas baja")]
    public float calidadEscudo = 1.0f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
