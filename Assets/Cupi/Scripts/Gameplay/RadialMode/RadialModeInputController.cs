using UnityEngine;
using UnityEngine.InputSystem;

public class RadialModeInputController : MonoBehaviour
{
    public static RadialModeInputController instance;

    private RadialModeMaster _radialModeMaster;

    public LineRenderer escudoLine;

    public Camera mainCamera;

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

    // Update is called once per frame
    void Update()
    {
        MoveShield();
    }

    private void MoveShield()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0.0f));

        Vector3 direction = mouseWorldPos - escudoLine.transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        escudoLine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void CreateLine()
    {
        int puntosLinea = GetPointsForShield();

        float coberturaEscudo = _radialModeMaster.coberturaEscudo;

        float radioEscudo = _radialModeMaster.radioEscudo;

        escudoLine.positionCount = puntosLinea;

        float progresoPorIteracion = coberturaEscudo / puntosLinea;

        float centroCobertura = coberturaEscudo / 2.0f;

        for (int i = 0; i < puntosLinea; i++)
        {
            float anguloRad = ((i * progresoPorIteracion) - centroCobertura) * Mathf.Deg2Rad;

            Vector2 posicionPunto = new Vector2(Mathf.Cos(anguloRad) * radioEscudo, Mathf.Sin(anguloRad) * radioEscudo);

            escudoLine.SetPosition(i, posicionPunto);
        }
    }

    private int GetPointsForShield()
    {
        return Mathf.Max(6, (int)(_radialModeMaster.coberturaEscudo / _radialModeMaster.calidadEscudo));
    }
}
