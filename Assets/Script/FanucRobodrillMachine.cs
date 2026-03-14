using UnityEngine;
using System.Collections;

public class FanucRobodrillMachine : MonoBehaviour
{
    [Header("Socket Attachments")]
    public bool socket1Attached = false;
    public bool socket2Attached = false;

    [Header("Door")]
    public bool isDoorClosed = false;
    public SlidingGate doorGate;

    [Header("Start Button")]
    public Renderer startButtonRenderer;
    public Color readyColor = Color.green;
    public Color defaultColor = Color.red;

    [Header("Sounds Settings")]
    public AudioSource audioSource;
    public AudioClip machineStartClip;
    public AudioClip buttonClickClip;
    // public TrainingManager trainingManager;


    [HideInInspector]
    public bool isReady = false;
    [HideInInspector]
    public bool isProcessing = false;

    private void Start()
    {
        doorGate = GetComponentInChildren<SlidingGate>();
        audioSource = GetComponent<AudioSource>();
        if (startButtonRenderer != null)
            startButtonRenderer.material.color = defaultColor;
    }

    private void Update()
    {
        // Check readiness
        bool allAttached = socket1Attached && socket2Attached && isDoorClosed;

        if (allAttached && !isReady)
        {
            isReady = true;
            HighlightStartButton(true);
        }
        else if (!allAttached && isReady)
        {
            isReady = false;
            HighlightStartButton(false);
        }
    }

    /// Called when socket 1 attachment changes.
    public void SetSocket1Attached(bool state)
    {
        socket1Attached = state;
    }

    /// Called when socket 2 attachment changes.
    public void SetSocket2Attached(bool state)
    {
        socket2Attached = state;
    }

    /// Called when the door state changes.
    public void SetDoorClosed(bool state)
    {
        if (doorGate != null)
        {
            doorGate.UpdateText(state ? "Door Closed" : "Door Open");
        }
        isDoorClosed = state;
    }

    /// Called when player presses the start button.
    public void OnStartButtonPressed()
    {
        audioSource.PlayOneShot(buttonClickClip);
        if (isReady && !isProcessing)
        {
            StartCoroutine(ProcessRoutine());

        }
        else
        {
            Debug.Log("Machine not ready or already processing!");
        }
    }

    private IEnumerator ProcessRoutine()
    {
        isProcessing = true;
        doorGate.UpdateText("Machine in Process");
        audioSource.PlayOneShot(machineStartClip);
        yield return new WaitForSeconds(20f);
        // trainingManager.CompleteCurrentStep(6);
        audioSource.Stop();
        doorGate.UpdateText("Machine in Process");
        isProcessing = false;
        isReady = false;
        HighlightStartButton(false);
        
    }

    private void HighlightStartButton(bool highlight)
    {
        if (startButtonRenderer == null) return;

        startButtonRenderer.material.color = highlight ? readyColor : defaultColor;
    }
}
