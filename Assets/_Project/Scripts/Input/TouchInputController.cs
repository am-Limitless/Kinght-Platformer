using UnityEngine;

[CreateAssetMenu(fileName = "TouchInputController", menuName = "InputController/TouchInputController")]
public class TouchInputController : InputController
{
    [Header("Left Stick (Move)")]
    [Tooltip("Stick push below this amount counts as no movement.")]
    [SerializeField, Range(0f, 1f)] private float moveDeadZone = 0.25f;

    private Joystick moveJoystick;
    private TouchJumpZone jumpZone;

    public void Bind(Joystick newMoveJoystick, TouchJumpZone newJumpZone)
    {
        moveJoystick = newMoveJoystick;
        jumpZone = newJumpZone;
    }

    public void Unbind()
    {
        moveJoystick = null;
        jumpZone = null;
    }

    public override float RetrieveMoveInput()
    {
        if (moveJoystick == null)
        {
            return 0f;
        }

        float horizontal = moveJoystick.Horizontal;
        if (Mathf.Abs(horizontal) < moveDeadZone)
        {
            return 0f;
        }

        // Full speed in either direction, same feel as keyboard GetAxisRaw.
        return Mathf.Sign(horizontal);
    }

    public override bool RetriveJumpInput()
    {
        return jumpZone != null && jumpZone.ConsumePress();
    }

    public override bool RetriveJumpHoldInput()
    {
        return jumpZone != null && jumpZone.IsHeld;
    }
}
