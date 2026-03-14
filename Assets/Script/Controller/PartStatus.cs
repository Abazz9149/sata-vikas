using UnityEngine;

public class PartStatus : MonoBehaviour
{
    public enum DefectMode
    {
        ForceSafe,
        ForceDefect,
        RandomChance
    }

    [Header("Defect Settings")]
    public DefectMode mode = DefectMode.RandomChance;
    [Range(0f, 1f)]
    public float defectChance = 0.5f;

    public bool hasDefect;

    void Start()
    {
        if (mode == DefectMode.ForceSafe)
            hasDefect = false;

        else if (mode == DefectMode.ForceDefect)
            hasDefect = true;
    }

    public void EvaluateRandomDefect()
    {
        if (mode == DefectMode.RandomChance)
        {
            hasDefect = Random.value < defectChance;
            Debug.Log("Random defect evaluated → " + hasDefect);
        }
    }
}
