using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

/// <summary>
/// Completes a training step when the correct object is placed in a socket
/// </summary>
public class SocketCompleteTrigger : MonoBehaviour
{
    [Header("Socket Configuration")]
    [SerializeField] private XRSocketInteractor socketInteractor;
    [SerializeField] private string expectedObjectTag = "";
    [SerializeField] private bool requireExactObject = false;
    [SerializeField] private GameObject expectedSpecificObject;
    [SerializeField] private int requiredStepIndex = 0; 
    
    [Header("Completion Events")]
    public UnityEvent OnCorrectObjectPlaced;
    public UnityEvent OnWrongObjectPlaced;
    
    private TrainingManager trainingManager;
    private bool isCompleted = false;

    [System.Obsolete]
    void Start()
    {
        trainingManager = FindObjectOfType<TrainingManager>();
        
        if (socketInteractor == null)
        {
            socketInteractor = GetComponent<XRSocketInteractor>();
        }
        
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.AddListener(OnObjectPlaced);
            socketInteractor.selectExited.AddListener(OnObjectRemoved);
        }
        else
        {
            Debug.LogError("SocketCompleteTrigger: No XRSocketInteractor found!");
        }
    }
    
    void OnDestroy()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnObjectPlaced);
            socketInteractor.selectExited.RemoveListener(OnObjectRemoved);
        }
    }
    
    private void OnObjectPlaced(SelectEnterEventArgs args)
    {
        if (isCompleted) return;
        
        GameObject placedObject = args.interactableObject.transform.gameObject;
        bool isCorrectObject = CheckIfCorrectObject(placedObject);
        
        if (isCorrectObject)
        {
            CompleteStep();
        }
        else
        {
            OnWrongObjectPlaced?.Invoke();
            Debug.LogWarning("Wrong object placed in socket!");
        }
    }
    
    private void OnObjectRemoved(SelectExitEventArgs args)
    {
        // Optional: Handle when correct object is removed
        // You might want to reset completion status here
    }
    
    private bool CheckIfCorrectObject(GameObject placedObject)
    {
        if (requireExactObject && expectedSpecificObject != null)
        {
            return placedObject == expectedSpecificObject;
        }
        
        if (!string.IsNullOrEmpty(expectedObjectTag))
        {
            return placedObject.CompareTag(expectedObjectTag);
        }
        
        // If no specific requirements, any object is considered correct
        return true;
    }
    
    private void CompleteStep()
    {
        isCompleted = true;
        OnCorrectObjectPlaced?.Invoke();
        
        if (trainingManager != null)
        {
            trainingManager.CompleteCurrentStep(requiredStepIndex);
        }
        else
        {
            Debug.LogError("TrainingManager not found!");
        }
        
        Debug.Log("Socket completion trigger activated!");
    }
    
    /// <summary>
    /// Resets the completion status (useful for training restarts)
    /// </summary>
    public void ResetCompletion()
    {
        isCompleted = false;
    }
    
    void OnValidate()
    {
        if (socketInteractor == null)
        {
            socketInteractor = GetComponent<XRSocketInteractor>();
        }
    }
}