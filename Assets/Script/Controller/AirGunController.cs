using System.Data.Common;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AirGunController : MonoBehaviour
{
    [Header("VR Input")]
    public InputActionProperty triggerValue; // Right-hand trigger input
    public InputActionProperty grabValue;    // Right-hand grab input

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip airSprayClip;

    [Header("Allowed Hand Interactors")]
    public XRBaseInteractor[] handInteractors;


    [Header("Task Manager")]
    public ResultManager resultManager;
    public TrainingManager trainingManager;
    private XRGrabInteractable grabInteractable;
    private IXRSelectInteractor currentInteractor;
    private bool isGrabbed = false;
    private bool isPlaying = false;
    public bool isStepOne = false;
    public bool isStepTen = false;
    public GameObject Scrap;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Subscribe to grab and release events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnDestroy()
    {
        // Clean up event listeners
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;

        XRBaseInteractor baseInteractor = currentInteractor as XRBaseInteractor;

        if (IsHandInteractor(currentInteractor))
        {
            isGrabbed = true;
        }
        else
        {
            isGrabbed = false;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        StopAir();
    }


    void Update()
    {
        if (!isGrabbed || currentInteractor == null)
            return;

        float trigger = triggerValue.action.ReadValue<float>();

        if (trigger > 0.8f && !isPlaying)
            StartAir();
        else if (trigger < 0.2f && isPlaying)
            StopAir();
    }

    private bool IsHandInteractor(IXRSelectInteractor interactor)
    {
        if (interactor == null) return false;

        XRBaseInteractor baseInteractor = interactor as XRBaseInteractor;
        if (baseInteractor == null) return false;

        for (int i = 0; i < handInteractors.Length; i++)
        {
            if (handInteractors[i] == baseInteractor)
                return true;
        }

        return false;
    }


    private void StartAir()
    {
        audioSource.loop = true;
        audioSource.clip = airSprayClip;
        audioSource.Play();
        isPlaying = true;
        if (Scrap != null)
            Scrap.SetActive(false);
        if (!isStepOne && trainingManager.currentStepIndex == 1)
        {
            trainingManager.CompleteCurrentStep(1);
            isStepOne = true;
        }
        else if (!isStepTen && trainingManager.currentStepIndex == 8)
        {
            trainingManager.CompleteCurrentStep(8);
            isStepTen = true;
        }
        else if (!isStepTen && trainingManager.currentStepIndex == 7)
        {
            trainingManager.CompleteCurrentStep(7);
            isStepTen = true;
        }
        if (Scrap != null)
            Scrap.SetActive(false);

        Debug.Log("Air Spray Started");
    }

    private void StopAir()
    {
        audioSource.Stop();
        isPlaying = false;
    }
}
