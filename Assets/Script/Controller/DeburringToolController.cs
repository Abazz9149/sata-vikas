using UnityEngine;

public class DeburringToolController : MonoBehaviour
{

    public TrainingManager trainingManager;
    public GameObject deburringEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Part"))
        {
            Debug.Log("Part collide");
            deburringEffect.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Part"))
        {
            deburringEffect.SetActive(false);
        }
    }

}
