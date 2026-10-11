using UnityEngine;

[CreateAssetMenu(fileName = "CombinedInputController", menuName = "InputController/CombinedInputController")]
public class CombinedInputController : InputController
{
    [Tooltip("All input sources are read together. Example: keyboard plus touch.")]
    [SerializeField] private InputController[] sources;

    public override float RetrieveMoveInput()
    {
        float strongestInput = 0f;

        foreach (InputController source in sources)
        {
            if (source == null)
            {
                continue;
            }

            float sourceInput = source.RetrieveMoveInput();
            if (Mathf.Abs(sourceInput) > Mathf.Abs(strongestInput))
            {
                strongestInput = sourceInput;
            }
        }

        return strongestInput;
    }

    // Every source is read each frame (no early exit) so touch taps are consumed and never go stale.
    public override bool RetriveJumpInput()
    {
        bool anyPressed = false;

        foreach (InputController source in sources)
        {
            if (source != null && source.RetriveJumpInput())
            {
                anyPressed = true;
            }
        }

        return anyPressed;
    }

    public override bool RetriveJumpHoldInput()
    {
        foreach (InputController source in sources)
        {
            if (source != null && source.RetriveJumpHoldInput())
            {
                return true;
            }
        }

        return false;
    }
}
