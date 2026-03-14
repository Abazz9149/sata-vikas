using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RotationSelectFilter : MonoBehaviour, IXRSelectFilter
{
    [Header("Target Rotation (in degrees)")]
    public Vector3 targetRotation = new Vector3(180f, -90f, 0f);

    [Header("Allowed Tolerance (degrees)")]
    public float tolerance = 10f;

    public bool canProcess => isActiveAndEnabled;

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        Quaternion relativeRot = Quaternion.Inverse(interactor.transform.rotation) * interactable.transform.rotation;
        Vector3 euler = relativeRot.eulerAngles;

        float diffX = Mathf.Abs(Mathf.DeltaAngle(euler.x, targetRotation.x));
        float diffY = Mathf.Abs(Mathf.DeltaAngle(euler.y, targetRotation.y));

        bool match = diffX <= tolerance && diffY <= tolerance;

        return match;
    }
}
