using UnityEngine;

public class BinTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Part"))
        {
            PartBehavior pb = other.GetComponent<PartBehavior>();

            if (pb != null && !pb.isPlaced)
            {
                pb.isPlaced = true;      // Prevent double counting
                PartManager.Instance.PartPlaced();
                Debug.Log("hello");
            }
        }
    }
}
