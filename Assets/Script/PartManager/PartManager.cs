using UnityEngine;

public class PartManager : MonoBehaviour
{
    public static PartManager Instance;
    public int totalParts = 3;
    private int completedParts = 0;

    public SceneManager sceneManager;

    private void Awake()
    {
        Instance = this;
    }

    public void PartPlaced()
    {
        completedParts++;

        Debug.Log("Part placed. Count = " + completedParts);

        if (completedParts >= totalParts)
        {
            ShowResult();
        }
    }

    private void ShowResult()
    {
        sceneManager.OnMoveToResultScene();
        Debug.Log("All parts done. Showing result.");
    }
}
