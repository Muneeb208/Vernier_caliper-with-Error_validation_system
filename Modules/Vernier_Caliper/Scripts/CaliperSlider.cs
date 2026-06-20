using UnityEngine;
using UnityEngine.InputSystem;

public class CaliperSlider : MonoBehaviour
{
    public float minX = -4f;
    public float maxX = 3f;
    private bool isDragging = false;
    private Vector3 offset;
    private Camera cam;

    // Callback event triggered when dragging ends
    public System.Action OnDragReleased;

    public bool IsDragging => isDragging;

    void Start()
    {
        cam = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            isDragging = false;
            OnDragReleased?.Invoke();
        }
    }

    void Update()
    {
        if (!isDragging) return;

        Vector3 newPos = GetMouseWorldPos() + offset;
        newPos.y = transform.position.y;
        newPos.z = transform.position.z;
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        transform.position = newPos;
    }

    Vector3 GetMouseWorldPos()
    {
        if (cam == null) cam = Camera.main;
        if (cam == null) return Vector3.zero;

        // Use the new Input System to read mouse position
        Vector2 mouseScreen = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : Vector2.zero;

        Vector3 mouse = new Vector3(mouseScreen.x, mouseScreen.y,
            Mathf.Abs(cam.transform.position.z));

        return cam.ScreenToWorldPoint(mouse);
    }
}
