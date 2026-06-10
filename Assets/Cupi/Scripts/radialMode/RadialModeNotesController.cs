using UnityEngine;
using UnityEngine.InputSystem;

public class RadialModeNotesController : MonoBehaviour
{
    public Transform testSeguimiento;

    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, mainCamera.nearClipPlane));

        Vector3 direction = mouseWorldPos - testSeguimiento.transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        testSeguimiento.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

    }
}
