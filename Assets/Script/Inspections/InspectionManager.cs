using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class InspectionManager : MonoBehaviour
{
    public HoleInspector[] holes;

    [Header("Main Inspection Progress (M1)")]
    public Image progressBar;

    [Header("Inspection 2 Tool Progress")]
    public Image progressBarForDrillingTool2;

    public XRGrabInteractable grabInteractable;
    public TrainingManager trainingManager;
     [Range(0f, 1f)]
    public float randomChanceForDefect = 0.2f;

    private int inspectedCount = 0;
    private int tool1Count = 0;
    private int tool2Count = 0;

    private bool hasDefect = false;

    void Start()
    {
        holes = GetComponentsInChildren<HoleInspector>();

        foreach (var hole in holes)
        {
            hole.isInspected = false;
            hole.isDefective = Random.value < randomChanceForDefect;
        }

        UpdateMainProgress();
        HideAllProgressBars();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void HideAllProgressBars()
    {
        progressBar.transform.parent.gameObject.SetActive(false);
        progressBarForDrillingTool2.transform.parent.gameObject.SetActive(false);
    }

    // ------------------------ MAIN INSPECTION ------------------------

    public void OnHoleInspected(HoleInspector hole)
    {
        progressBar.transform.parent.gameObject.SetActive(true);

        inspectedCount = holes.Count(h => h.isInspected);

        if (hole.isDefective)
            hasDefect = true;

        UpdateMainProgress();
    }

    private void UpdateMainProgress()
    {
        float progress = (float)inspectedCount / holes.Length;
        progressBar.fillAmount = progress;

        if (hasDefect)
        {
            progressBar.color = Color.red;
            progressBar.transform.parent.gameObject.SetActive(true);
        }
        else if (inspectedCount == holes.Length)
        {
            trainingManager?.CompleteCurrentStep(7);
        }
    }

    // ---------------------- INSPECTION 2 TOOL 1 ----------------------


    // ---------------------- INSPECTION 2 TOOL 1 ----------------------

    public void OnInspection2Tool2Used(HoleInspector hole)
    {
        if (!hole.isInspected)
            tool2Count++;

        progressBarForDrillingTool2.transform.parent.gameObject.SetActive(true);
        progressBarForDrillingTool2.fillAmount = (float)tool2Count / holes.Length;

        Debug.Log($"Inspection2 Tool2 progress: {tool2Count}/{holes.Length}");
    }

    // --------------------------- XR EVENTS ---------------------------

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        ResetAllProgress();
        progressBar.transform.parent.gameObject.SetActive(true);
        progressBarForDrillingTool2.transform.parent.gameObject.SetActive(true);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        HideAllProgressBars();
    }

    private void ResetAllProgress()
    {
        inspectedCount = 0;
        tool1Count = 0;
        tool2Count = 0;
        hasDefect = false;

        foreach (var hole in holes)
        {
            hole.isInspected = false;
            hole.isDefective = Random.value < 0.1f;
        }

        progressBar.fillAmount = 0f;
        progressBar.color = Color.white;

        progressBarForDrillingTool2.fillAmount = 0f;
    }
}
