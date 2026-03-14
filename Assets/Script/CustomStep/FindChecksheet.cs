using UnityEngine;

public class FindChecksheet : MonoBehaviour
{
    public VoicePlayer voicePlayer;
    void Start()
    {
        Invoke(nameof(ShowDebug), 8f);
    }

    void ShowDebug()
    {
        
        voicePlayer.PlayDialogue();
    }
}
