using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AutoSocketOnRelease : MonoBehaviour
{
    public XRSocketInteractor targetSocket;

    private XRGrabInteractable grabInteractable;
    private bool isBeingHeld = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }
    void OnGrab(SelectEnterEventArgs args)
    {
        isBeingHeld = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isBeingHeld = false;

        if (targetSocket != null)
            StartCoroutine(AttachNextFrame());
    }

    IEnumerator AttachNextFrame()
    {
        yield return null;

        if (!isBeingHeld && targetSocket != null && !targetSocket.hasSelection)
            targetSocket.StartManualInteraction((IXRSelectInteractable)grabInteractable);
    }
}
