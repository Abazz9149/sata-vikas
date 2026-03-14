using UnityEngine;

public class LanguageSelector : MonoBehaviour
{
    public void SelectEnglish()
    {
        LanguageManager.Instance.SetLanguage(Language.English);
        Debug.Log("Language set to English");
    }

    public void SelectHindi()
    {
        LanguageManager.Instance.SetLanguage(Language.Hindi);
        Debug.Log("Language set to Hindi");
    }
}
