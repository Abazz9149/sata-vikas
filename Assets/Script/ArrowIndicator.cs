// using UnityEngine;

// public class ArrowIndicator : MonoBehaviour
// {
//     [SerializeField] private float floatAmplitude = 0.5f;
//     [SerializeField] private float floatSpeed = 2f;
//     [SerializeField] private float rotationSpeed = 50f;
//     private Vector3 startPos;

//     void Start()
//     {
//         startPos = transform.position;
//     }

// <<<<<<< HEAD
//     // Update is called once per frame
// =======
// >>>>>>> dae1dd232ae35fe4a1b4456d39c97e29e6134908
//     void Update()
//     {
//         float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
//         transform.Rotate(Vector3.left, rotationSpeed * Time.deltaTime);
//         transform.position = new Vector3(startPos.x, newY, startPos.z);
//     }
// }
