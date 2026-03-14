using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Completes a training step when a specific object is grabbed
/// </summary>
public class GrabCompleteTrigger : MonoBehaviour
{
     [Header("Grab Completion")]
    [SerializeField] private bool requireSpecificGrabbable = false;
    [SerializeField] private GameObject specificGrabbableObject;
     [SerializeField] private int requiredStepIndex = 0; 
    
    [Header("Events")]
    public UnityEvent OnCorrectObjectGrabbed;
    
    private TrainingManager trainingManager;
    private bool isCompleted = false;
    private XRBaseInteractable interactable;

    void Start()
    {
        trainingManager = FindObjectOfType<TrainingManager>();
        interactable = GetComponent<XRBaseInteractable>();
        
        if (interactable == null)
        {
            Debug.LogError("GrabCompleteTrigger requires an XRBaseInteractable component!");
            return;
        }
        
        interactable.selectEntered.AddListener(OnObjectGrabbed);
    }
    
    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnObjectGrabbed);
        }
    }
    
    private void OnObjectGrabbed(SelectEnterEventArgs args)
    {
        if (isCompleted) return;
        
        // Only trigger for direct hand grabs (not sockets)
        if (args.interactorObject is XRSocketInteractor)
        {
            return; // Ignore non-direct interactions (sockets)
        }
        
        GameObject grabbedObject = args.interactableObject.transform.gameObject;
        bool isCorrectObject = CheckIfCorrectObject(grabbedObject);
        
        if (isCorrectObject)
        {
            CompleteStep();
        }
    }
    
    private bool CheckIfCorrectObject(GameObject checkObject)
    {
        if (requireSpecificGrabbable && specificGrabbableObject != null)
        {
            return checkObject == specificGrabbableObject;
        }
        
        return checkObject == gameObject;
    }
    
    private void CompleteStep()
    {
        isCompleted = true;
        OnCorrectObjectGrabbed?.Invoke();
        
        if (trainingManager != null)
        {
            trainingManager.CompleteCurrentStep(requiredStepIndex);
        }
        
        Debug.Log("Grab completion trigger activated!");
    }
    
    public void ResetCompletion()
    {
        isCompleted = false;
    }
}
