using UnityEngine;

public abstract class InputController : ScriptableObject
{
    public abstract float RetrieveMoveInput();
    public abstract bool RetriveJumpInput();

    public abstract bool RetriveJumpHoldInput();
}
