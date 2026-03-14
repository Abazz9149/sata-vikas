using UnityEngine;

public class CoolandTank : MonoBehaviour
{
    public GameObject inspectionUI;

    void Start()
    {
        inspectionUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the Cooland Tank area.");
            inspectionUI.SetActive(true);
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inspectionUI.SetActive(false);
        }
    }
}
