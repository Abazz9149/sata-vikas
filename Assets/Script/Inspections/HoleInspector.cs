using UnityEngine;

public class HoleInspector : MonoBehaviour
{
    public string holeID;
    public AudioSource inspectionSound;
    public AudioClip passSound;

    public bool isInspected = false;
    public bool isDefective = false;
    public GameObject Scrap;

    private InspectionManager manager;

    void Start()
    {
        manager = GetComponentInParent<InspectionManager>();

        // 10% defect chance
        // isDefective = Random.value < 0.1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isInspected && other.CompareTag("InspectionTool"))
        {
            isInspected = true;
            if (Scrap != null)
            {
                Scrap.SetActive(true);
            }

            if (isDefective)
                Debug.Log($"{holeID} DEFECT DETECTED!");
            else
                inspectionSound.PlayOneShot(passSound);

            manager?.OnHoleInspected(this);
            return;
        }


        // Inspection 2 - Tool 1
        if (other.CompareTag("InspectionTool2-M2"))
        {
            inspectionSound.PlayOneShot(passSound);
            manager?.OnInspection2Tool2Used(this);
            return;
        }
    }
}
