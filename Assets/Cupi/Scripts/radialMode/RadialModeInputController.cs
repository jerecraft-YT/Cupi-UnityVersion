using UnityEngine;
using UnityEngine.InputSystem;

public class RadialModeInputController : MonoBehaviour
{
    public InputActionReference mouseMovement;

    public static RadialModeInputController instance;

    public LineRenderer escudoLine;

    public RadialModeMaster radialModeMaster;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        CreateLine();
    }

    private void CreateLine()
    {
        int puntosLinea = GetPointsForShield();

        float coberturaEscudo = radialModeMaster.coberturaEscudo;

        float radioEscudo = radialModeMaster.radioEscudo;

        escudoLine.positionCount = puntosLinea;

        float progresoPorIteracion =  coberturaEscudo / puntosLinea;

        for (int i = 0; i < puntosLinea; i++)
        {
            float anguloRad = (i * progresoPorIteracion) * Mathf.Deg2Rad;

            Vector2 posicionPunto = new Vector2(Mathf.Cos(anguloRad) * radioEscudo,Mathf.Sin(anguloRad) * radioEscudo);
           
            escudoLine.SetPosition(i,posicionPunto);
        }
    }

    private int GetPointsForShield()
    {
        return Mathf.Max(2,(int)(radialModeMaster.coberturaEscudo / radialModeMaster.calidadEscudo));
    }

    // Update is called once per frame
    void Update()
    {

    }
}
