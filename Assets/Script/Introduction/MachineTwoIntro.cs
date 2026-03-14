using UnityEngine;

public class MachineTwoIntro : MonoBehaviour
{
    private VoicePlayer voicePlayer;
    public TrainingManager trainingManager;
    private bool isIntroDone = false;
    private bool isTankCheck = false;
    void Start()
    {
        voicePlayer = GetComponent<VoicePlayer>();
    }

    public void MarkAllTanksAsDone()
    {
        isTankCheck = true;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isIntroDone)
        {
            isIntroDone = true;
            voicePlayer.PlayDialogue();
             trainingManager.CompleteCurrentStep(0);

        }
        if (other.CompareTag("Player") && isTankCheck)
        {
            trainingManager.CompleteCurrentStep(0);
        }
    }
}
