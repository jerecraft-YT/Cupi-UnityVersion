using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadialModeInputController : MonoBehaviour
{
    public static RadialModeInputController instance;

    private RadialModeMaster _radialModeMaster;

    public LineRenderer escudoLine;

    public Camera mainCamera;

    private Vector3 referenceMouse;

    //public InputActionReference mouse;

    public Vector3 virtualCursor;

    public float sensitivityCursor = 0.01f;

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

    private void ControlSchemeChange()
    {

    }

    private void OnEnable()
    {
        _radialModeMaster = RadialModeMaster.instance;

        InputController.instance.gameInputs.RadialMode.Cursor.performed += MoveShield;

        CreateLine();
    }

    private void OnDisable()
    {
        InputController.instance.gameInputs.RadialMode.Cursor.performed -= MoveShield;
    }

    // Update is called once per frame
    void Update()
    {
        //MoveShield();
    }

    private void MoveShield(InputAction.CallbackContext ctx)
    {
        Vector3 mousePos = ctx.ReadValue<Vector2>();

        float sensitivity = ctx.control.device is Pointer ? sensitivityCursor : 1f;

        virtualCursor += mousePos * sensitivity;

        Vector3 direction = virtualCursor - referenceMouse;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        escudoLine.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        float distanceToChange = Vector3.Distance(virtualCursor, referenceMouse);

        if (distanceToChange > _radialModeMaster.sensibilidadEscudo)
        {
            referenceMouse = virtualCursor + (referenceMouse - virtualCursor).normalized * _radialModeMaster.sensibilidadEscudo;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.beige;
        Gizmos.DrawCube(virtualCursor, Vector3.one * 0.5f);
        Gizmos.color = Color.red;
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
