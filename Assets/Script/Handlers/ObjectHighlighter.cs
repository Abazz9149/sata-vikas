using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ObjectHighlighter : MonoBehaviour
{
    [SerializeField] private XRBaseInteractor interactor;
    [SerializeField] private string outlineLayerName = "Outline";

    private int outlineLayer;

    private void Awake()
    {
        outlineLayer = LayerMask.NameToLayer(outlineLayerName);

        if (interactor == null)
        {
            interactor = GetComponent<XRBaseInteractor>();
        }
    }

    private void OnEnable()
    {
        interactor.hoverEntered.AddListener(OnHoverEnter);
        interactor.hoverExited.AddListener(OnHoverExit);
    }

    private void OnDisable()
    {
        interactor.hoverEntered.RemoveListener(OnHoverEnter);
        interactor.hoverExited.RemoveListener(OnHoverExit);
    }


    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        // Debug.Log("Object Highlighed");
        if (args.interactableObject is XRBaseInteractable interactable)
        {
            HighlightTarget target = interactable.GetComponent<HighlightTarget>();
            if (target != null)
                target.SetHighlighted(true, outlineLayer);
        }
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (args.interactableObject is XRBaseInteractable interactable)
        {
            HighlightTarget target = interactable.GetComponent<HighlightTarget>();
            if (target != null)
                target.SetHighlighted(false, outlineLayer);
        }
    }

}
