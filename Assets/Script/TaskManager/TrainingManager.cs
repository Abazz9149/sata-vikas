using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Manages the overall training flow, step progression, audio cues, and indicators
/// </summary>
public class TrainingManager : MonoBehaviour
{
    [Serializable]
    public class TrainingStep
    {
        [Header("Step Configuration")]
        public string stepName;
        public int stepIndex;

        [Header("Interactables")]
        public List<XRBaseInteractable> stepInteractables = new List<XRBaseInteractable>();

        [Header("Audio")]
        // public AudioClip startAudio;
        // public AudioClip completeAudio;
        public VoicePlayer voicePlayerStart;
        public VoicePlayer voicePlayerComplete;

        [Header("Indicator")]
        public Transform indicatorTarget;
        public bool showIndicator = true;

        [Header("Events")]
        public UnityEvent OnStepStart;
        public UnityEvent OnStepComplete;

        [HideInInspector]
        public bool isCompleted = false;

    }

    [Header("Training Configuration")]
    [SerializeField] private List<TrainingStep> trainingSteps = new List<TrainingStep>();
    [SerializeField] private AudioSource audioSource;

    [Header("Manager References")]
    [SerializeField] private StepIndicator stepIndicator;

    [Header("Events")]
    public UnityEvent OnTrainingStart;
    public UnityEvent OnTrainingComplete;
    public UnityEvent<int> OnStepChanged;

    [HideInInspector]
    public int currentStepIndex = -1;
    private bool isTrainingActive = false;

    public int CurrentStepIndex => currentStepIndex;
    public TrainingStep CurrentStep => currentStepIndex >= 0 && currentStepIndex < trainingSteps.Count ?
        trainingSteps[currentStepIndex] : null;

    private bool isVoicePlaying = false;

    void Start()
    {
        InitializeTraining();
    }

    /// <summary>
    /// Initializes the training system and starts the first step
    /// </summary>
    public void InitializeTraining()
    {
        if (trainingSteps.Count == 0)
        {
            Debug.LogWarning("No training steps configured!");
            return;
        }

        // Lock all interactables initially
        LockAllInteractables();

        isTrainingActive = true;
        OnTrainingStart?.Invoke();
        StartStep(0);
    }

    /// <summary>
    /// Starts a specific training step
    /// </summary>
    public void StartStep(int stepIndex)
    {
        if (!isTrainingActive || stepIndex >= trainingSteps.Count)
            return;

        // Complete previous step if exists
        if (currentStepIndex >= 0 && currentStepIndex < trainingSteps.Count)
        {
            trainingSteps[currentStepIndex].isCompleted = true;
        }

        currentStepIndex = stepIndex;
        TrainingStep step = trainingSteps[currentStepIndex];

        // Update interactable locks
        UpdateInteractableLocks();

        // Play start audio

        if (step.voicePlayerStart != null)
        {
            // step.voicePlayerStart.PlayDialogue();
            StartCoroutine(PlayVoiceAndWait(step.voicePlayerStart));
        }

        // Update indicator
        if (stepIndicator != null)
        {
            if (step.showIndicator && step.indicatorTarget != null)
            {
                stepIndicator.SetTarget(step.indicatorTarget);
                stepIndicator.ShowIndicator();
            }
            else
            {
                stepIndicator.HideIndicator();
            }
        }

        // Invoke step start events
        step.OnStepStart?.Invoke();
        OnStepChanged?.Invoke(currentStepIndex);

        Debug.Log($"Started step: {step.stepName}");
    }

    /// <summary>
    /// Completes the current step and moves to the next one
    /// </summary>
    public void CompleteCurrentStep(int currentStep)
    {
        if (currentStepIndex < 0 || !isTrainingActive)
            return;

        if (currentStep != currentStepIndex) return;

        TrainingStep step = trainingSteps[currentStep];

        if (!step.isCompleted)
        {
            step.isCompleted = true;

            // Play complete audio
            if (step.voicePlayerComplete != null)
            {
                // step.voicePlayerComplete.PlayDialogue();
                StartCoroutine(PlayVoiceAndWait(step.voicePlayerComplete));
            }

            // Invoke step complete events
            step.OnStepComplete?.Invoke();

            Debug.Log($"Completed step: {step.stepName}");

            // Move to next step or complete training
            if (currentStepIndex < trainingSteps.Count - 1)
            {
                StartCoroutine(StartNextStepAfterDelay(1f));
            }
            else
            {
                CompleteTraining();
            }
        }
    }

    private IEnumerator PlayVoiceAndWait(VoicePlayer vp)
    {
        if (vp == null || vp.audioSource == null) yield break;

        // Wait until any previous audio finishes
        while (isVoicePlaying)
            yield return null;

        isVoicePlaying = true;

        vp.PlayDialogue();

        // Wait until this audio finishes
        while (vp.audioSource.isPlaying)
            yield return null;

        isVoicePlaying = false;
    }


    private IEnumerator StartNextStepAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartStep(currentStepIndex + 1);
    }

    /// <summary>
    /// Completes the entire training
    /// </summary>
    public void CompleteTraining()
    {
        isTrainingActive = false;

        if (stepIndicator != null)
        {
            stepIndicator.HideIndicator();
        }

        OnTrainingComplete?.Invoke();
        Debug.Log("Training completed!");
    }

    /// <summary>
    /// Updates interactable locks based on current step
    /// </summary>
    private void UpdateInteractableLocks()
    {
        for (int i = 0; i < trainingSteps.Count; i++)
        {
            bool shouldBeLocked = i != currentStepIndex;

            foreach (XRBaseInteractable interactable in trainingSteps[i].stepInteractables)
            {
                if (interactable != null)
                {
                    InteractableLock lockComponent = interactable.GetComponent<InteractableLock>();
                    if (lockComponent != null)
                    {
                        if (shouldBeLocked)
                        {
                            lockComponent.LockInteractable();
                        }
                        else
                        {
                            lockComponent.UnlockInteractable();
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Locks all interactables in the training
    /// </summary>
    private void LockAllInteractables()
    {
        foreach (TrainingStep step in trainingSteps)
        {
            foreach (XRBaseInteractable interactable in step.stepInteractables)
            {
                if (interactable != null)
                {
                    InteractableLock lockComponent = interactable.GetComponent<InteractableLock>();
                    if (lockComponent != null)
                    {
                        lockComponent.LockInteractable();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Skips to a specific step (for debugging or testing)
    /// </summary>
    public void SkipToStep(int stepIndex)
    {
        if (stepIndex >= 0 && stepIndex < trainingSteps.Count)
        {
            StartStep(stepIndex);
        }
    }
}