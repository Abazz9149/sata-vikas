using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class GrindingMachine : MonoBehaviour
{
    [Header("Socket Attachments")]
    public bool socketAttached = false;
    public XRSocketInteractor socket;


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

    [Header("Machine Grinding State")]
    public Renderer grindingButtonRenderer;
    public GameObject grindingMachineObject;
    public float moveDistance = 0.1f;
    public float moveSpeed = 2f;
    public float rotationSpeed = 360f;
    private bool isGrindingEnabled = false;

    [Header("Machine Rotator State")]
    public Renderer rotatorButtonRenderer;
    public GameObject rotatorMachineObject;
    public float moveDistanceForRotator = 0.1f;

    [Header("Light")]
    public Light lampLight;

    [Header("Traning manager")]
    public TrainingManager trainingManager;

    [HideInInspector]
    public bool isReady = false;
    [HideInInspector]
    public bool isProcessing = false;

    [Header("Defect Settings")]
    [Range(0f, 1f)]
    public AudioClip defectAlarmClip;
    private bool alarmPlaying = false;


    public Mesh processedMesh;

    private Vector3 initialPosition;
    private Vector3 initialPositionForRotator;
    private bool isGrinding = false;

    private PartStatus currentPart;
    public GameObject Scrap;

    private void Start()
    {
        doorGate = GetComponentInChildren<SlidingGate>();
        lampLight.color = Color.yellow;
        audioSource = GetComponent<AudioSource>();
        if (socket != null)
            socket.selectEntered.AddListener(OnSelectEntered);
        socket.selectExited.AddListener((args) => SetSocketAttached(false));

        if (startButtonRenderer != null)
            startButtonRenderer.material.color = defaultColor;
        if (rotatorButtonRenderer != null)
            rotatorButtonRenderer.material.color = defaultColor;

        if (grindingMachineObject != null)
            initialPosition = grindingMachineObject.transform.localPosition;
        if (rotatorMachineObject != null)
            initialPositionForRotator = rotatorMachineObject.transform.localPosition;
    }

    private void Update()
    {
        // Check readiness
        bool allAttached = socketAttached && isDoorClosed && isGrindingEnabled;

        if (socketAttached)
        {
            grindingButtonRenderer.material.color = Color.green;
            rotatorButtonRenderer.material.color = Color.green;
        }
        else
        {
            grindingButtonRenderer.material.color = Color.red;
            rotatorButtonRenderer.material.color = Color.red;
        }

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

        if (isGrinding && grindingMachineObject != null)
        {
            AnimateGrinding();
        }
    }

    /// Called when socket 1 attachment changes.
    public void GetObject(PartStatus go)
    {
        currentPart = go;

        Debug.Log("Part attached to machine: " + currentPart.gameObject.name);

        if (currentPart.mode == PartStatus.DefectMode.RandomChance)
            currentPart.EvaluateRandomDefect();
    }

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        PartStatus part = args.interactableObject.transform
        .GetComponentInParent<PartStatus>();

        if (part == null)
        {
            Debug.LogError("No PartStatus found on object inserted!");
            return;
        }

        socketAttached = true;
        GetObject(part);
    }

    public void SetSocketAttached(bool state)
    {
        socketAttached = state;

        if (!state)
        {
            // Reset on detach
            lampLight.color = Color.yellow;
            isReady = false;
            StopAlarm();
            return;
        }

        CheckPartDefect();

    }

    private void CheckPartDefect()
    {
        if (currentPart == null)
            return;

        if (currentPart.hasDefect)
        {
            lampLight.color = Color.red;

            // Machine blocked
            isReady = false;
            HighlightStartButton(false);

            Debug.Log("⚠ DEFECT FOUND — MACHINE LOCKED");

            PlayAlarm();
        }
        else
        {
            lampLight.color = Color.green;
            Debug.Log("Part OK — Ready to process");

            StopAlarm();
        }
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
        if (isReady && isGrindingEnabled && !isProcessing)
        {
            StartCoroutine(ProcessRoutine());
        }
        else
        {
            Debug.Log("Machine not ready or already processing!");
        }
    }

    public void OnGrindingButtonPressed()
    {
        if (!socketAttached) return;
        isGrindingEnabled = true;
        isGrinding = true;
        StartCoroutine(MoveOnce(grindingMachineObject, initialPosition, moveDistance));

    }
    public void OnRotatorButtonPressed()
    {
        if (!socketAttached) return;

        StartCoroutine(MoveOnce(rotatorMachineObject, initialPositionForRotator, moveDistanceForRotator));

    }

    public void OnGrindingButtonReleased()
    {
        isGrinding = false;
        isGrindingEnabled = false;

        grindingButtonRenderer.material.color = Color.red;

        // Reset position
        if (grindingMachineObject != null)
            grindingMachineObject.transform.localPosition = initialPosition;
    }
    public void OnRotatorButtonReleased()
    {

        // Reset position
        if (rotatorMachineObject != null)
            rotatorMachineObject.transform.localPosition = initialPositionForRotator;
    }

    private IEnumerator ProcessRoutine()
    {
        isProcessing = true;
        doorGate.UpdateText("Machine in Process");
        audioSource.PlayOneShot(machineStartClip);
        lampLight.color = Color.green;
        yield return new WaitForSeconds(20f);

        audioSource.Stop();
        lampLight.color = Color.yellow;
        OnGrindingButtonReleased();
        OnRotatorButtonReleased();
        ApplyProcessedMesh();
        trainingManager.CompleteCurrentStep(6); // Complete grinding step
        isProcessing = false;
        isReady = false;
        HighlightStartButton(false);
        if (Scrap != null)
        {
            Scrap.SetActive(true);
        }
    }
    private void AnimateGrinding()
    {
        // Continuous rotation
        grindingMachineObject.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);
    }

    private IEnumerator MoveOnce(GameObject go, Vector3 intPos, float md)
    {
        Vector3 targetPos = intPos + Vector3.forward * md;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            go.transform.localPosition = Vector3.Lerp(intPos, targetPos, t);
            yield return null;
        }
    }


    private void HighlightStartButton(bool highlight)
    {
        if (startButtonRenderer == null) return;

        startButtonRenderer.material.color = highlight ? readyColor : defaultColor;
    }

    private void ApplyProcessedMesh()
    {
        if (currentPart == null)
        {
            Debug.LogWarning("No part attached — cannot apply processed mesh.");
            return;
        }

        // Replace final mesh
        MeshFilter filter = currentPart.gameObject.transform.Find("main body").GetComponent<MeshFilter>();

        if (filter != null)
            filter.mesh = processedMesh;

        Debug.Log("Mesh replaced — part successfully processed." + currentPart.gameObject);
    }


    private void PlayAlarm()
    {
        if (alarmPlaying) return;
        if (defectAlarmClip == null) return;

        audioSource.loop = true;
        audioSource.clip = defectAlarmClip;
        audioSource.Play();

        alarmPlaying = true;
    }

    private void StopAlarm()
    {
        if (!alarmPlaying) return;

        audioSource.Stop();
        audioSource.loop = false;
        alarmPlaying = false;
    }

}
