using UnityEngine;

public class RedBin : MonoBehaviour
{
    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.gameObject.CompareTag("Part"))
    //     {
    //         // Destroy(other.gameObject);
    //     }
    // }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Part"))
        {
            // Destroy(other.gameObject);
        }
    }
}
