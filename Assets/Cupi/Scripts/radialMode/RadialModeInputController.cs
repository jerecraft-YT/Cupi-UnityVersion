using UnityEngine;
using UnityEngine.InputSystem;

public class RadialModeInputController : MonoBehaviour
{
    public InputActionReference mouseMovement;

    public static RadialModeInputController instance;

    public LineRenderer escudoLine;

    private RadialModeMaster _radialModeMaster;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void OnEnable()
    {
        _radialModeMaster = RadialModeMaster.instance;

        CreateLine();
    }

    private void CreateLine()
    {
        int puntosLinea = GetPointsForShield();

        float coberturaEscudo = _radialModeMaster.coberturaEscudo;

        float radioEscudo = _radialModeMaster.radioEscudo;

        escudoLine.positionCount = puntosLinea;

        float progresoPorIteracion =  coberturaEscudo / puntosLinea;

        float centroCobertura = coberturaEscudo / 2.0f;

        for (int i = 0; i < puntosLinea; i++)
        {
            float anguloRad = ((i * progresoPorIteracion) - centroCobertura) * Mathf.Deg2Rad;

            Vector2 posicionPunto = new Vector2(Mathf.Cos(anguloRad) * radioEscudo,Mathf.Sin(anguloRad) * radioEscudo);
           
            escudoLine.SetPosition(i,posicionPunto);
        }
    }

    private int GetPointsForShield()
    {
        return Mathf.Max(2,(int)(_radialModeMaster.coberturaEscudo / _radialModeMaster.calidadEscudo));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
