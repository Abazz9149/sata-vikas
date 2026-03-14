using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    [Header("Task Toggles")]
    // Step 1 - 6: Pre-Operation Checks
    // public Toggle ppeSelectionToggle;
    public Toggle amChecksheetVerifyToggle;
    public Toggle coolantTankCheckToggle;
    public Toggle lubricantOilCheckToggle;
    public Toggle hydraulicOilCheckToggle;
    public Toggle machineCleanlinessCheckToggle;

    // Step 7 - 13: Part Loading and Start
    public Toggle inputOutputIdentifyToggle;
    public Toggle correctPartOrientationToggle;

    public Toggle partPickRestIdentifyToggle;
    public Toggle clampinPartToggle;

    public Toggle machineRestingToggle; // Resting
    public Toggle machineClampingToggle;//clamping
    public Toggle machineOrientationToggle;  //Orientation

    public Toggle doorCloseToggle;
    public Toggle machineStartToggle;

    // Step 14 - 18: Post-Machining
    public Toggle partUnloadingToggle;
    public Toggle deburringProcessToggle;
    public Toggle preDeburringCleaningToggle; // Air, Fixture, Part Loading
    public Toggle inspectionToggle;
    public Toggle correctBinPlacementToggle;


    [Header("UI Colors")]
    public Color doneColor = new Color(0.2f, 0.8f, 0.2f); // Green
    public Color notDoneColor = new Color(0.8f, 0.2f, 0.2f); // Red

    [Header("Sprites")]
    public Sprite notDoneSprite;
    public Sprite doneSprite;

    [Header("Storage Option")]
    // public bool testModeToggle;
    private Dictionary<string, (Toggle toggle, bool isDone)> tasks;
    private DateTime sessionStart;
    private bool sessionEnded = false;


    void Awake()
    {
        // Initialize all tasks with default (false)
        tasks = new Dictionary<string, (Toggle, bool)>()
        {
            // Step 1 - 6
            // { "step1_ppe_select", (ppeSelectionToggle, false) },
            { "step2_am_check", (amChecksheetVerifyToggle, false) },
            { "step3_coolant_check", (coolantTankCheckToggle, false) },
            { "step4_lubricant_check", (lubricantOilCheckToggle, false) },
            { "step5_hydraulic_check", (hydraulicOilCheckToggle, false) },
            { "step6_machine_clean", (machineCleanlinessCheckToggle, false) },
            
            // Step 7 - 13
            { "step7_input_output", (inputOutputIdentifyToggle, false) },
            { "step8_part_resting", (partPickRestIdentifyToggle, false) },
            { "step9_part_orient", (correctPartOrientationToggle, false) },
            { "step10_part_clamping", (clampinPartToggle, false) },
            { "step11_machine_resting", (machineRestingToggle, false) },
            { "step12_machine_clamping", (machineClampingToggle, false) },
            { "step13_machine_orient", (machineOrientationToggle, false) },
            { "step14_door_close", (doorCloseToggle, false) },
            { "step13.1_machine_start", (machineStartToggle, false) },

            // Step 14 - 18
            { "step15_part_unload", (partUnloadingToggle, false) },
            { "step16_deburr_process", (deburringProcessToggle, false) },
            { "step17_pre_deburr_clean", (preDeburringCleaningToggle, false) },
            { "step18_inspection", (inspectionToggle, false) },
            { "step19_bin_placement", (correctBinPlacementToggle, false) }
        };

        sessionStart = DateTime.Now;
    }

    void Start()
    {
        UpdateResults();
    }

    public void MarkTaskDone(string taskName)
    {
        if (tasks.ContainsKey(taskName))
        {
            tasks[taskName] = (tasks[taskName].toggle, true);
            UpdateResults();
        }
        else
        {
            Debug.LogWarning($"No task found with name: {taskName}");
        }
    }


    public void UpdateResults()
    {
        foreach (var kvp in tasks)
            SetTaskUI(kvp.Value.toggle, kvp.Value.isDone);

        UpdatePassFailResult();
    }

    private void UpdatePassFailResult()
    {
        bool allTasksDone = true;
        foreach (var task in tasks)
        {
            if (!task.Value.isDone)
            {
                allTasksDone = false;
                break;
            }
        }

        string result = allTasksDone ? "PASS" : "FAIL";

        PlayerPrefs.SetString("FinalResult", result);
        PlayerPrefs.Save();
    }

    private void SetTaskUI(Toggle toggle, bool isDone)
    {
        if (toggle == null) return;

        Transform bgTransform = toggle.transform.Find("Background");
        if (bgTransform == null) return;

        Image bgImage = bgTransform.GetComponent<Image>();
        if (bgImage != null)
            bgImage.color = isDone ? doneColor : notDoneColor;

        Transform checkmark = bgTransform.Find("Checkmark");
        if (checkmark != null)
        {
            Image checkmarkImage = checkmark.GetComponent<Image>();
            if (checkmarkImage != null)
            {
                checkmarkImage.enabled = true;
                checkmarkImage.sprite = isDone ? doneSprite : notDoneSprite;
            }
        }
    }


    public void IsTraningEnd(bool isEnd)
    {
        sessionEnded = isEnd;
    }


}
