using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class SlidingGate : MonoBehaviour
{
    [Header("Gate Settings")]
    public Vector3 openOffset = new Vector3(3f, 0f, 0f);
    public float moveTime = 1f;
    [Range(0f, 2f)] public float smoothing = 1f;

    [Header("Gate UI")]
    public string openPrompt = "Open Gate";
    public string closePrompt = "Close Gate";
    public TextMeshPro promptText;

    [Header("Auto Close")]
    public bool autoClose = false;
    public float autoCloseDelay = 3f;

    private Vector3 closedPos;
    private Vector3 openPos;
    private bool isOpen = false;
    private Coroutine moveRoutine;

    // Machine references (optional)
    private FanucRobodrillMachine machine1;
    private GrindingMachine machine2;

    private XRSimpleInteractable interactable;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        // Try to auto-detect machines in parent objects
        if (machine1 == null)
            machine1 = GetComponentInParent<FanucRobodrillMachine>();
        if (machine2 == null)
            machine2 = GetComponentInParent<GrindingMachine>();

        closedPos = transform.localPosition;
        openPos = closedPos + openOffset;

        // Safe event subscriptions
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelectEntered);
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
        }
    }

    void OnDestroy()
    {
        // Safe event unsubscriptions
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        bool isProcessing = false;

        if (machine1 != null && machine1.isProcessing)
            isProcessing = true;

        if (machine2 != null && machine2.isProcessing)
            isProcessing = true;

        // Don’t allow gate interaction while processing
        if (isProcessing)
            return;

        ToggleGate();
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (promptText != null)
            promptText.gameObject.SetActive(true);
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (promptText == null)
            return;

        bool isProcessing = false;

        if (machine1 != null && machine1.isProcessing)
            isProcessing = true;

        if (machine2 != null && machine2.isProcessing)
            isProcessing = true;

        if (isProcessing)
        {
            promptText.text = "Machine in Process";
        }
        else
        {
            promptText.text = isOpen ? closePrompt : openPrompt;
        }
    }

    public void ToggleGate()
    {
        if (isOpen)
            CloseGate();
        else
            OpenGate();
    }

    public void OpenGate()
    {
        if (isOpen) return;
        if (machine1 != null && machine1.isDoorClosed == false)
            machine1.SetDoorClosed(true);
        else if (machine2 != null && machine2.isDoorClosed == false)
            machine2.SetDoorClosed(true);

        StartMove(openPos);
        isOpen = true;

        if (autoClose)
            StartCoroutine(AutoClose());
    }

    public void CloseGate()
    {
        if (!isOpen) return;
        if (machine1 != null && machine1.isDoorClosed == true)
            machine1.SetDoorClosed(false);
        else if (machine2 != null && machine2.isDoorClosed == true)
            machine2.SetDoorClosed(false);

        StartMove(closedPos);
        isOpen = false;
    }

    private void StartMove(Vector3 target)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveTo(target));
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        Vector3 start = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < moveTime)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveTime);

            // Easing (smoothstep)
            if (smoothing > 0f)
            {
                float s = Mathf.SmoothStep(0f, 1f, t);
                t = Mathf.Lerp(t, s, Mathf.Clamp01(smoothing));
            }

            transform.localPosition = Vector3.Lerp(start, target, t);
            yield return null;
        }

        transform.localPosition = target;
        moveRoutine = null;
    }

    private IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(autoCloseDelay);
        if (isOpen) CloseGate();
    }

    public void UpdateText(string text)
    {
        if (promptText != null)
            promptText.text = text;
    }
}

