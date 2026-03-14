using System.Collections;
using UnityEngine;

[RequireComponent(typeof(VoicePlayer))]
public class MachineOneIntro : MonoBehaviour
{
    public TrainingManager trainingManager;

    private VoicePlayer voicePlayer;
    private bool isIntroDone = false;
    private bool isChecklistDone = false;
    private bool coolantTankCheck = false;
    private bool lubricantOilCheck = false;
    private bool hydraulicOilCheck = false;
    private bool isPlayerInside = false;
    void Start()
    {
        voicePlayer = GetComponent<VoicePlayer>();
    }

    public void MarkChecklistDone()
    {
        isChecklistDone = true;
        TryPlayIntro();
    }

    public void MarkCoolantTankCheckDone()
    {
        coolantTankCheck = true;
        ValidateAllTanks();
    }

    public void MarkLubricantTankCheckDone()
    {
        lubricantOilCheck = true;
        ValidateAllTanks();
    }

    public void MarkHydraulicTankCheckDone()
    {
        hydraulicOilCheck = true;
        ValidateAllTanks();
    }

    private void ValidateAllTanks()
    {
        if (coolantTankCheck && lubricantOilCheck && hydraulicOilCheck)
        {
            CompleteTankStepIfPossible();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInside = true;

        TryPlayIntro();
        CompleteTankStepIfPossible();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerInside = false;
    }

    private void TryPlayIntro()
    {
        if (isPlayerInside && isChecklistDone && !isIntroDone)
        {
            isIntroDone = true;
            voicePlayer.PlayDialogue();
        }
    }

    private void CompleteTankStepIfPossible()
    {
        if (isPlayerInside &&
            coolantTankCheck &&
            lubricantOilCheck &&
            hydraulicOilCheck)
        {
            trainingManager.CompleteCurrentStep(0);
        }
    }
}
