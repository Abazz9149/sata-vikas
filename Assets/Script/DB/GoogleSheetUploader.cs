using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class GoogleSheetUploader : MonoBehaviour
{
    [SerializeField]
    private string googleScriptURL =
        "https://script.google.com/macros/s/AKfycbz7IAGKC3YuzOJfutsNkevyFttO02Vf9KTM4ER9a9E/dev";


    public void TestUpload()
    {
        Debug.Log("▶ Test upload started");

        UploadResult(
            empName: "Test User",
            empId: "TEST001",
            companyName: "Sata Vikas",
            trainingName: "CB Turning 10",
            score: 22,
            maxScore: 30
        );
    }


    public void UploadResult(
        string empName,
        string empId,
        string companyName,
        string trainingName,
        int score,
        int maxScore
    )
    {
        string date = DateTime.Now.ToString("dd-MM-yyyy");
        string scoreText = $"{score}/{maxScore} ({date})";

        var json = JsonUtility.ToJson(new SheetData
        {
            empName = empName,
            empId = empId,
            companyName = companyName,
            trainingName = trainingName,
            scoreText = scoreText
        });

        StartCoroutine(PostData(json));
    }

    IEnumerator PostData(string json)
    {
        var request = new UnityWebRequest(googleScriptURL, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Data uploaded to Google Sheet");
            Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ Upload failed: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
    }

    [Serializable]
    class SheetData
    {
        public string empName;
        public string empId;
        public string companyName;
        public string trainingName;
        public string scoreText;
    }
}
