using UnityEngine;

public class TouchInputBinder : MonoBehaviour
{
    [SerializeField] private TouchInputController touchInput;
    [SerializeField] private Joystick moveJoystick;
    [SerializeField] private TouchJumpZone jumpZone;

    private void OnEnable()
    {
        if (touchInput == null || moveJoystick == null || jumpZone == null)
        {
            Debug.LogError("TouchInputBinder is missing a reference.", this);
            return;
        }

        touchInput.Bind(moveJoystick, jumpZone);
    }

    private void OnDisable()
    {
        if (touchInput != null)
        {
            touchInput.Unbind();
        }
    }
}
