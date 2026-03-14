using UnityEngine;

public class TurningResult : MonoBehaviour
{
    public VRMarksManager vRMarksManager;
    void Start()
    {
        vRMarksManager.CompleteTraining();
    }

}
