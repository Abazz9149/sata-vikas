using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance;

    public Language currentLanguage = Language.Hindi; // default

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // keep language choice across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetLanguage(Language lang)
    {
        currentLanguage = lang;
        PlayerPrefs.SetInt("Language", (int)lang);
        PlayerPrefs.Save();
    }

    public void LoadLanguage()
    {
        if (PlayerPrefs.HasKey("Language"))
            currentLanguage = (Language)PlayerPrefs.GetInt("Language");
    }
}
