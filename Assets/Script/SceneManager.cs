using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
    public Button startButton;
    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(() =>
            {
                OnMoveToNextScene();
            });
    }

    public void OnMoveToNextScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void OnMoveToResultScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(6);
    }

    public void OnRestart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(2);
    }
    public void LoadScene(int sceneID)
    {
        string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneID);
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

        PlayerPrefs.SetString("LastSceneName", sceneName);
        PlayerPrefs.Save();

         Debug.Log($"Loading Scene: {sceneName} (ID: {sceneID})");

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneID);

    }

    public void OnApplicationQuit()
    {
        Debug.Log("Application is quitting.");
        Application.Quit();
    }
}
