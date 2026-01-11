using UnityEngine;

public class MobileInputManager : MonoBehaviour
{
    public static MobileInputManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private MobileButton shootButton;
    [SerializeField] private MobileButton interactButton;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // LOGIC TO HIDE ON PC:
        // If we are NOT in the Unity Editor, AND we are NOT on a mobile platform...
#if !UNITY_EDITOR
        if (!Application.isMobilePlatform)
        {
            // 1. Hide the visual elements
            if (joystick != null) joystick.gameObject.SetActive(false);
            if (shootButton != null) shootButton.gameObject.SetActive(false);
            if (interactButton != null) interactButton.gameObject.SetActive(false);

            // 2. Disable this manager object last (so it stops running updates)
            gameObject.SetActive(false);
        }
#endif
    }

    public Vector2 GetMovement()
    {
        // Safety check: if manager or joystick is missing/disabled, return zero
        if (!gameObject.activeInHierarchy || joystick == null) return Vector2.zero;
        return joystick.InputVector;
    }

    public bool IsShooting()
    {
        if (!gameObject.activeInHierarchy || shootButton == null) return false;
        return shootButton.IsHeld;
    }

    public bool IsInteracting()
    {
        if (!gameObject.activeInHierarchy || interactButton == null) return false;
        return interactButton.GetKeyDown();
    }
}