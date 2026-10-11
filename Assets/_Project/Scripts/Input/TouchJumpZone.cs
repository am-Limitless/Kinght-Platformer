using UnityEngine;
using UnityEngine.EventSystems;

public class TouchJumpZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private int fingersHolding;
    private bool pressPending;

    public bool IsHeld => fingersHolding > 0;

    public void OnPointerDown(PointerEventData eventData)
    {
        fingersHolding++;
        pressPending = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        fingersHolding = Mathf.Max(0, fingersHolding - 1);
    }

    // Returns true once per tap, so a quick second tap in the air becomes the double jump.
    public bool ConsumePress()
    {
        bool wasPressed = pressPending;
        pressPending = false;
        return wasPressed;
    }

    private void OnDisable()
    {
        fingersHolding = 0;
        pressPending = false;
    }
}
