using UnityEngine;

public class BlueBin : MonoBehaviour
{

    public ResultManager resultManager;
    public VRMarksManager marksManager;
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Part"))
        {
            // marksManager.AddCorrectMarks(1);
            resultManager.MarkTaskDone("step19_bin_placement");

        }
    }
}
