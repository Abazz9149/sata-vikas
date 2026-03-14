using UnityEngine;

/// <summary>
/// Handles the 3D arrow indicator that points to active objects
/// </summary>
public class StepIndicator : MonoBehaviour
{
    [Header("Indicator References")]
    [SerializeField] private GameObject arrowIndicator;
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private float heightOffset = 0.5f;
    [SerializeField] private float rotationSpeed = 2f;
    
    [Header("Animation")]
    [SerializeField] private float bobHeight = 0.1f;
    [SerializeField] private float bobSpeed = 2f;
    
    private Transform target;
    private bool isShowing = false;
    private Vector3 originalArrowPosition;
    
    void Start()
    {
        if (arrowIndicator != null)
        {
            originalArrowPosition = arrowIndicator.transform.localPosition;
            arrowIndicator.SetActive(false);
        }
        else
        {
            Debug.LogError("StepIndicator: Arrow indicator reference is missing!");
        }
    }
    
    void Update()
    {
        if (isShowing && target != null && arrowIndicator != null)
        {
            UpdateIndicatorPosition();
            UpdateIndicatorAnimation();
        }
    }
    
    /// <summary>
    /// Sets the target for the indicator to point to
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        
        if (target != null && arrowIndicator != null)
        {
            // Immediately position the indicator
            Vector3 targetPosition = target.position + Vector3.up * heightOffset;
            arrowIndicator.transform.position = targetPosition;
        }
    }
    
    /// <summary>
    /// Shows the indicator
    /// </summary>
    public void ShowIndicator()
    {
        if (arrowIndicator != null)
        {
            arrowIndicator.SetActive(true);
            isShowing = true;
        }
    }
    
    /// <summary>
    /// Hides the indicator
    /// </summary>
    public void HideIndicator()
    {
        if (arrowIndicator != null)
        {
            arrowIndicator.SetActive(false);
            isShowing = false;
        }
    }
    
    /// <summary>
    /// Updates the indicator position to follow the target smoothly
    /// </summary>
    private void UpdateIndicatorPosition()
    {
        if (target == null) return;
        
        Vector3 targetPosition = target.position + Vector3.up * heightOffset;
        arrowIndicator.transform.position = Vector3.Lerp(
            arrowIndicator.transform.position, 
            targetPosition, 
            followSpeed * Time.deltaTime
        );
        
        // Make the arrow face the camera (player)
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 directionToCamera = mainCamera.transform.position - arrowIndicator.transform.position;
            directionToCamera.y = 0; // Keep arrow upright
            
            if (directionToCamera != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
                arrowIndicator.transform.rotation = Quaternion.Slerp(
                    arrowIndicator.transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
    
    /// <summary>
    /// Updates the bobbing animation of the indicator
    /// </summary>
    private void UpdateIndicatorAnimation()
    {
        if (arrowIndicator == null) return;
        
        float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        Vector3 animatedPosition = originalArrowPosition + Vector3.up * bobOffset;
        
        // Apply local position for animation while maintaining world position for targeting
        Transform arrowTransform = arrowIndicator.transform;
        Vector3 localOffset = arrowTransform.TransformDirection(animatedPosition - originalArrowPosition);
        arrowTransform.position += localOffset;
    }
    
    /// <summary>
    /// Toggles the indicator visibility
    /// </summary>
    public void ToggleIndicator()
    {
        if (isShowing)
        {
            HideIndicator();
        }
        else
        {
            ShowIndicator();
        }
    }
    
    /// <summary>
    /// Checks if the indicator is currently showing
    /// </summary>
    public bool IsShowing()
    {
        return isShowing;
    }
    
    void OnValidate()
    {
        if (arrowIndicator == null)
        {
            // Try to find a child arrow object
            Transform arrowChild = transform.Find("Arrow");
            if (arrowChild != null)
            {
                arrowIndicator = arrowChild.gameObject;
            }
        }
    }
}