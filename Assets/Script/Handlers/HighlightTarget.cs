using UnityEngine;

public class HighlightTarget : MonoBehaviour
{
    [HideInInspector] public int originalLayer;

    void Awake()
    {
        // Store the original layer of this object
        originalLayer = gameObject.layer;
    }

    public void SetHighlighted(bool isHighlighted, int outlineLayer)
    {
        gameObject.layer = isHighlighted ? outlineLayer : originalLayer;
    }
}
