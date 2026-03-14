using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Controls locking/unlocking of XR interactables while maintaining socket functionality
/// </summary>
[RequireComponent(typeof(XRBaseInteractable))]
public class InteractableLock : MonoBehaviour
{
    [Header("Lock Configuration")]
    [SerializeField] private bool startLocked = true;
    
    private XRBaseInteractable interactable;
    private bool isLocked = false;
    
    // Store original interaction layers
    private InteractionLayerMask originalInteractionLayers;
    
    void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        originalInteractionLayers = interactable.interactionLayers;
        
        if (startLocked)
        {
            LockInteractable();
        }
    }
    
    /// <summary>
    /// Locks the interactable from being grabbed while maintaining socket functionality
    /// </summary>
    public void LockInteractable()
    {
        if (interactable == null) return;
        
        isLocked = true;
        
        // Use interaction layers to disable grab but keep socket functionality
        // This prevents grabbing while allowing socket interactions
        interactable.interactionLayers = InteractionLayerMask.GetMask("Socket");
        
        // Force deselect if currently selected
        if (interactable.isSelected)
        {
            interactable.interactionManager.CancelInteractableSelection((IXRSelectInteractable)interactable);
        }
    }
    
    /// <summary>
    /// Unlocks the interactable for normal interaction
    /// </summary>
    public void UnlockInteractable()
    {
        if (interactable == null) return;
        
        isLocked = false;
        
        // Restore original interaction layers
        interactable.interactionLayers = originalInteractionLayers;
    }
    
    /// <summary>
    /// Toggles the lock state
    /// </summary>
    public void ToggleLock()
    {
        if (isLocked)
        {
            UnlockInteractable();
        }
        else
        {
            LockInteractable();
        }
    }
    
    /// <summary>
    /// Check if the interactable is currently locked
    /// </summary>
    public bool IsLocked()
    {
        return isLocked;
    }
    
    void OnValidate()
    {
        // Ensure we have the required interactable component
        if (GetComponent<XRBaseInteractable>() == null)
        {
            Debug.LogError("InteractableLock requires an XRBaseInteractable component!");
        }
    }
}