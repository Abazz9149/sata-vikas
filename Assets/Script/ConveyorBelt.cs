using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ConveyorBelt : MonoBehaviour
{
    [Header("Conveyor Settings")]
    public List<Transform> beltPositions; 
    public List<GameObject> objectsOnBelt = new List<GameObject>(); 
    public float moveSpeed = 3f; 

    void Update()
    {
        // Smoothly move each object to its assigned belt position
        for (int i = 0; i < objectsOnBelt.Count; i++)
        {
            if (objectsOnBelt[i] == null) continue;
            objectsOnBelt[i].transform.position = Vector3.MoveTowards(
                objectsOnBelt[i].transform.position,
                beltPositions[i].position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    // Call this when an object is picked up
    public void OnObjectPickedUp(GameObject pickedObject)
    {
        int index = objectsOnBelt.IndexOf(pickedObject);
        if (index == -1) return;

        // Remove the picked object
        objectsOnBelt.RemoveAt(index);

        // Shift the next objects forward automatically
        for (int i = index; i < objectsOnBelt.Count; i++)
        {
            StartCoroutine(MoveToNextSlot(objectsOnBelt[i], beltPositions[i]));
        }

    }

    private IEnumerator MoveToNextSlot(GameObject obj, Transform targetPos)
    {
        while (Vector3.Distance(obj.transform.position, targetPos.position) > 0.01f)
        {
            obj.transform.position = Vector3.MoveTowards(
                obj.transform.position,
                targetPos.position,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }
    }

    // Optional helper: add a new object to the last slot
    public void AddObject(GameObject newObj)
    {
        if (objectsOnBelt.Count >= beltPositions.Count) return;
        objectsOnBelt.Add(newObj);
        newObj.transform.position = beltPositions[objectsOnBelt.Count - 1].position;
    }

}
