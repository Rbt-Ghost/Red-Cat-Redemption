using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    [Header("References")]
    [Tooltip("The visual circle background of the joystick")]
    [SerializeField] private RectTransform background;
    [Tooltip("The knob that moves inside the background")]
    [SerializeField] private RectTransform handle;

    [Header("Settings")]
    [SerializeField] private float handleRange = 1f;
    [SerializeField] private float deadZone = 0.1f; // Prevents jitter
    [Tooltip("If true, the joystick snaps to where you touch.")]
    [SerializeField] private bool isFloating = true;
    [Tooltip("If true, the joystick disappears when you release.")]
    [SerializeField] private bool hideOnRelease = true;

    public Vector2 InputVector { get; private set; } = Vector2.zero;

    private Vector2 initialBackgroundPosition;
    private CanvasGroup canvasGroup; // Used for hiding/showing smoothly
    private RectTransform baseRect; // The RectTransform this script is attached to

    private void Start()
    {
        baseRect = GetComponent<RectTransform>();
        initialBackgroundPosition = background.anchoredPosition;

        // Setup transparency handling
        if (hideOnRelease)
        {
            canvasGroup = background.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                // Add a CanvasGroup if one is missing so we can control opacity
                canvasGroup = background.gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0f; // Start hidden
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isFloating)
        {
            // 1. Move the visual background to exactly where the finger touched
            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                baseRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint))
            {
                background.anchoredPosition = localPoint;
            }
        }

        // 2. Show the joystick visuals
        if (hideOnRelease && canvasGroup != null) canvasGroup.alpha = 1f;

        // 3. Calculate initial input (in case they tap exactly on the edge)
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 position;
        // Calculate the position of the touch relative to the BACKGROUND (not the screen)
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(background, eventData.position, eventData.pressEventCamera, out position))
        {
            Vector2 size = background.sizeDelta;

            // Normalize values to range -1 to 1
            InputVector = new Vector2(
                (position.x / size.x) * 2,
                (position.y / size.y) * 2
            );

            // If the magnitude > 1, normalize it so diagonal movement isn't faster
            InputVector = (InputVector.magnitude > 1.0f) ? InputVector.normalized : InputVector;

            // Apply Deadzone (ignore tiny movements)
            if (InputVector.magnitude < deadZone)
            {
                InputVector = Vector2.zero;
                handle.anchoredPosition = Vector2.zero;
            }
            else
            {
                // Move the handle visually
                handle.anchoredPosition = new Vector2(
                    InputVector.x * (size.x / 2) * handleRange,
                    InputVector.y * (size.y / 2) * handleRange
                );
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset everything on release
        InputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;

        if (!isFloating)
        {
            // Only reset position if not floating (or reset to default if you prefer)
            background.anchoredPosition = initialBackgroundPosition;
        }

        // Hide joystick visuals
        if (hideOnRelease && canvasGroup != null) canvasGroup.alpha = 0f;
    }
}