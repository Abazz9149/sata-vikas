using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class AutoMultiSocketOnRelease : MonoBehaviour
{
    [Tooltip("Assign all potential sockets for this object.")]
    public List<XRSocketInteractor> targetSockets = new List<XRSocketInteractor>();

    private XRGrabInteractable grabInteractable;
    private bool isBeingHeld = false;

    void Start()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isBeingHeld = true;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isBeingHeld = false;

        if (targetSockets != null && targetSockets.Count > 0)
            StartCoroutine(AttachToAvailableSocket());
    }

    IEnumerator AttachToAvailableSocket()
    {
        yield return null; // Wait a frame to ensure release completes.

        if (isBeingHeld) yield break;

        foreach (var socket in targetSockets)
        {
            if (socket == null) continue;

            // Check if socket is active and available
            if (socket.isActiveAndEnabled && !socket.hasSelection)
            {
                // Attach this object to the available socket
                socket.StartManualInteraction((IXRSelectInteractable)grabInteractable);
                yield break; // Stop after first valid socket
            }
        }

        Debug.LogWarning($"{name}: No available sockets found to attach.");
    }
}
