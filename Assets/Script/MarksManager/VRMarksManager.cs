using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;

public class VRMarksManager : MonoBehaviour
{
    [Header("Marks Settings")]
    public int maxMarks = 30;
    public int currentMarks = 0;

    [Header("Scoring Values")]
    // public int wrongActionPenalty = 0;
    // public int timePenaltyPerSecond = 0; // optional

    [Header("Time Tracking (Optional)")]
    // public bool useTimer = false;
    // private float elapsedTime = 0f;

    [Header("UI References")]
    public TextMeshProUGUI marksText;
    public TextMeshProUGUI resultStatus; // pass or fail
    // public TextMeshProUGUI timeText;

    [Header("Training State")]
    public bool trainingCompleted = false;

    [Header("Cloud Database (Google sheet)")]
    public GoogleSheetUploader sheetUploader;


    void Start()
    {
        currentMarks = 0;
        UpdateUI(currentMarks);
    }

    // void Update()
    // {
    //     if (!useTimer || trainingCompleted) return;

    //     elapsedTime += Time.deltaTime;

    //     if (timeText != null)
    //         timeText.text = $"Time: {Mathf.FloorToInt(elapsedTime)}s";
    // }

    //  Call when user performs correct step
    public void AddCorrectMarks(int correctActionMarks)
    {
        if (trainingCompleted) return;

        currentMarks += correctActionMarks;
        currentMarks = Mathf.Clamp(currentMarks, 0, maxMarks);

        PlayerPrefs.SetInt("m01_marks", currentMarks);
        PlayerPrefs.Save();

        UpdateUI(currentMarks);
    }

    //  Call when user performs wrong step
    public void DeductWrongMarks()
    {
        if (trainingCompleted) return;

        // currentMarks -= wrongActionPenalty;
        currentMarks = Mathf.Clamp(currentMarks, 0, maxMarks);

        UpdateUI(currentMarks);
    }

    //  Call once at training end (if using timer)
    // public void ApplyTimePenalty()
    // {
    //     if (!useTimer) return;

    //     int penalty = Mathf.FloorToInt(elapsedTime) * timePenaltyPerSecond;
    //     currentMarks -= penalty;
    //     currentMarks = Mathf.Clamp(currentMarks, 0, maxMarks);
    // }

    //  Call when training is finished
    public void CompleteTraining()
    {
        int finalMarks = PlayerPrefs.GetInt("m01_marks", 0);
        string trainingName = PlayerPrefs.GetString("LastSceneName", "");
        string empName = PlayerPrefs.GetString("EmployeeName", "");
        string empID = PlayerPrefs.GetString("EmployeeID", "");
        string companyName = "Sata Vikas";


        trainingCompleted = true;

        // ApplyTimePenalty();

        UpdateUI(finalMarks * 4);

        sheetUploader.UploadResult(
       empName,
       empID,
       companyName,
       trainingName,
       finalMarks,
       maxMarks
   );

        Debug.Log("Training completed & data sent to Google Sheet");
    }

    void UpdateUI(int finalMarks)
    {
        if (marksText != null)
        {
            marksText.text = $"{finalMarks}/{maxMarks}";
        }

        float percentage = (float)finalMarks / maxMarks * 100f;

        if (percentage >= 80)
        {
            resultStatus.text = "Pass";
            resultStatus.color = Color.green;

        }
        else
        {
            resultStatus.text = "Fail";
            resultStatus.color = Color.red;
        }
    }



}
