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
        // ...then hide the mobile controls.
#if !UNITY_EDITOR
        if (!Application.isMobilePlatform)
        {
            gameObject.SetActive(false);
        }
#endif
    }

    public Vector2 GetMovement()
    {
        // If the object is disabled (hidden), return zero movement
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