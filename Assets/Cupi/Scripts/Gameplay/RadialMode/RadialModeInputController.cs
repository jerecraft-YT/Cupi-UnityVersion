using UnityEngine;
using UnityEngine.InputSystem;

public class RadialModeInputController : MonoBehaviour
{
    public static RadialModeInputController instance;

    private RadialModeMaster _radialModeMaster;

    public LineRenderer escudoLine;

    public Camera mainCamera;

    private Vector3 referenceMouse;

    public InputActionReference mouse;

    public Vector3 virtualMouse;

    public float sensitivity = 1.0f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        Cursor.lockState = CursorLockMode.Locked;
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
        Vector3 mousePos = mouse.action.ReadValue<Vector2>();

        virtualMouse.x += mousePos.x * sensitivity;

        virtualMouse.y += mousePos.y * sensitivity;

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(virtualMouse.x, virtualMouse.y, 0.0f));

        Vector3 direction = virtualMouse - referenceMouse;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        escudoLine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        float distanceToChange = Vector3.Distance(virtualMouse, referenceMouse);

        if (distanceToChange > RadialModeMaster.instance.sensibilidadEscudo)
        {
            referenceMouse = virtualMouse + (referenceMouse - virtualMouse).normalized * RadialModeMaster.instance.sensibilidadEscudo;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(virtualMouse, Vector3.one);
        Gizmos.DrawCube(referenceMouse, Vector3.one);
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
