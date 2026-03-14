using UnityEngine;
using TMPro;
using System;

public class CurrentDateTMP : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dateText;

    void Start()
    {
        // Example format: 19 Jan 2026
        dateText.text = DateTime.Now.ToString("dd MMM yyyy");
    }
}
