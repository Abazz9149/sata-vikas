using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VRWearManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject inputPanel;        // Panel with input fields
    public TMP_InputField empIdField;
    public TMP_InputField nameField;
    public Button submitButton;

    [Header("Sound References")]
    public AudioSource audioSource;
    public AudioClip itemEquipAudioClip;
    public VoicePlayer afterSafetyWering;
    private VoicePlayer voicePlayer;

    public GameObject correctKit;
    public GameObject redKit;
    public GameObject greenKit;

    private bool mainBodyWorn = false;
    private bool helmetWorn = false;
    private bool gogglesWorn = false;
    private bool glovesWorn = false;
    private bool bootsWorn = false;

    void Start()
    {
        if (inputPanel != null)
            inputPanel.SetActive(false);

        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmit);

        voicePlayer = GetComponent<VoicePlayer>();
        voicePlayer.PlayDialogue();
    }

    public void OnItemSelected(GameObject item)
    {
        HandleClick(item.GetComponent<Collider>());
    }

    public void OnWrongItemSelected(GameObject item)
    {
        Destroy(item);
        correctKit.SetActive(false);
        redKit.SetActive(false);
        greenKit.SetActive(false);
        ShowInputPanel();
    }

    private void HandleClick(Collider col)
    {
        switch (col.tag)
        {
            case "MainBody":
                if (!mainBodyWorn)
                {
                    mainBodyWorn = true;
                    audioSource.PlayOneShot(itemEquipAudioClip);
                    Destroy(col.gameObject);
                    Debug.Log("Main body worn");
                }
                break;

            case "Helmet":
                if (!helmetWorn)
                {
                    helmetWorn = true;
                    audioSource.PlayOneShot(itemEquipAudioClip);
                    Destroy(col.gameObject);
                    Debug.Log("Helmet worn");
                }
                break;
            case "Goggles":
                if (!gogglesWorn)
                {
                    gogglesWorn = true;
                    audioSource.PlayOneShot(itemEquipAudioClip);
                    Destroy(col.gameObject);
                    Debug.Log("Goggles worn");
                }
                break;

            case "Gloves":
                if (!glovesWorn)
                {
                    glovesWorn = true;
                    audioSource.PlayOneShot(itemEquipAudioClip);
                    Destroy(col.gameObject);
                    Debug.Log("Helmet worn");
                }
                break;

            case "Boots":
                if (!bootsWorn)
                {
                    bootsWorn = true;
                    audioSource.PlayOneShot(itemEquipAudioClip);
                    Destroy(col.gameObject);
                    Debug.Log("Boots worn");
                }
                break;
        }

        if (mainBodyWorn && helmetWorn && gogglesWorn && glovesWorn && bootsWorn)
            ShowInputPanel();
    }

    private void ShowInputPanel()
    {
        if (inputPanel != null && !inputPanel.activeSelf)
        {
            inputPanel.SetActive(true);
            afterSafetyWering.PlayDialogue();
            Debug.Log("All items worn. Showing input panel.");
        }
    }

    private void OnSubmit()
    {
        string empId = empIdField.text.Trim();
        string empName = nameField.text.Trim();

        if (string.IsNullOrEmpty(empId) || string.IsNullOrEmpty(empName))
        {
            Debug.LogWarning("Please enter both Employee ID and Name.");
            return;
        }

        PlayerPrefs.SetString("EmployeeID", empId);
        PlayerPrefs.SetString("EmployeeName", empName);
        PlayerPrefs.Save();

        int randomScene = UnityEngine.Random.Range(3, 5); // between 3,4,5


        Debug.Log($"Submitted: {empName} ({empId})");
        UnityEngine.SceneManagement.SceneManager.LoadScene(randomScene);
    }
}
