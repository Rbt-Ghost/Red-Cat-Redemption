using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;

    [Header("Settings")]
    [SerializeField] private float handleRange = 1f;
    [SerializeField, Range(0, 1f)] private float deadZone = 0.1f; // Prevents jitter
    [SerializeField] private bool isFloating = false; // If true, joystick moves to where you touch

    public Vector2 InputVector { get; private set; } = Vector2.zero;

    private Vector2 initialPosition;
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        // Store the original position to snap back to if using Floating mode
        initialPosition = background.anchoredPosition;

        // Ensure the handle is centered at start
        handle.anchoredPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Optional: Move the joystick to the finger position (Floating Joystick)
        if (isFloating)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint
            );
            background.anchoredPosition = localPoint;
        }

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        // Calculate position relative to the background
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out position))
        {
            // FIX: This math works for a CENTER PIVOT (0.5, 0.5), which is standard for UI.
            // We divide by (size / 2) to get a range of -1 to 1 directly.
            float x = position.x / (background.sizeDelta.x / 2);
            float y = position.y / (background.sizeDelta.y / 2);

            InputVector = new Vector2(x, y);

            // Normalize if length > 1 so we don't move faster diagonally
            InputVector = (InputVector.magnitude > 1.0f) ? InputVector.normalized : InputVector;

            // Apply movement to the visual Handle
            handle.anchoredPosition = new Vector2(
                InputVector.x * (background.sizeDelta.x / 2),
                InputVector.y * (background.sizeDelta.y / 2)
            ) * handleRange;

            // Apply Deadzone to the public Output only (visuals remain smooth)
            if (InputVector.magnitude < deadZone)
            {
                InputVector = Vector2.zero;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        // Reset position if floating
        if (isFloating)
        {
            background.anchoredPosition = initialPosition;
        }
    }
}