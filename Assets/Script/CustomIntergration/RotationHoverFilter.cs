using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RotationHoverFilter : MonoBehaviour, IXRHoverFilter
{
    [Header("Target Rotation (in degrees)")]
    public Vector3 targetRotation = new Vector3(180f, -90f, 0f);

    [Header("Allowed Tolerance (degrees)")]
    public float tolerance = 10f;

    public bool canProcess => isActiveAndEnabled;

    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        Quaternion objectRot = interactable.transform.rotation;
        Vector3 euler = objectRot.eulerAngles;

        // Normalize angles (-180 to 180)
        euler.x = NormalizeAngle(euler.x);
        euler.y = NormalizeAngle(euler.y);
        euler.z = NormalizeAngle(euler.z);

        // Compare X and Y rotation difference with target
        float diffX = Mathf.Abs(Mathf.DeltaAngle(euler.x, targetRotation.x));
        float diffY = Mathf.Abs(Mathf.DeltaAngle(euler.y, targetRotation.y));
        bool match = diffX <= tolerance && diffY <= tolerance;

        Debug.Log($"[HoverFilter] rot=({euler.x:F1},{euler.y:F1}) diff=({diffX:F1},{diffY:F1}) -> {match}");

        // Allow hover only if both within tolerance
        return match;
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
}
