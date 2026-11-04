using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInput : MonoBehaviour
{
    private const float RayLength = 100f;
    private Camera mainCamera;

    void Awake()
    {
        Cursor.visible = true;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        Ray ray = CreateRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(ray.origin, ray.direction * RayLength, Color.red);
    }

    private Ray CreateRay(Vector2 cursorPosition)
    {
        return mainCamera.ScreenPointToRay(cursorPosition);
    }
}
