using UnityEngine;

public class VoicePlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip englishClip;
    public AudioClip hindiClip;

    public void PlayDialogue()
    {
        if (LanguageManager.Instance.currentLanguage == Language.English)
            audioSource.clip = englishClip;
        else
            audioSource.clip = hindiClip;

        audioSource.Play();
    }

    public bool IsPlaying => audioSource.isPlaying;
}
