using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputController : MonoBehaviour
{
    public static InputController instance;

    public GameInputs gameInputs;

    public InputDevice lastDevice;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        gameInputs = new GameInputs();

        gameInputs.Enable();
    }

    //posible solucion para detectar el cambio de
    //input seria suscribirse a cada accion y detectar su control device
}
