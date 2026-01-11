using UnityEngine;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public bool IsHeld { get; private set; }
    private int lastPressedFrame = -1;

    public void OnPointerDown(PointerEventData eventData)
    {
        IsHeld = true;
        // Record exactly which frame the button was pressed
        lastPressedFrame = Time.frameCount;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsHeld = false;
    }

    public bool GetKeyDown()
    {
        // Returns true as long as we are still in the same frame as the click.
        // This allows multiple scripts (NPCs) to check this value in the same update loop.
        return Time.frameCount == lastPressedFrame;
    }
}