using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteOutline : MonoBehaviour
{
    public Color outlineColor = Color.black;
    public float outlineSize = 1.1f;

    private SpriteRenderer spriteRenderer;
    private GameObject outlineObject;
    private SpriteRenderer outlineSpriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        outlineObject = new GameObject("Outline");
        outlineObject.transform.parent = transform;
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localScale = Vector3.one;

        outlineSpriteRenderer = outlineObject.AddComponent<SpriteRenderer>();
        outlineSpriteRenderer.sprite = spriteRenderer.sprite;
        outlineSpriteRenderer.color = outlineColor;
        outlineSpriteRenderer.sortingLayerID = spriteRenderer.sortingLayerID;
        outlineSpriteRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

        UpdateOutline();
    }

    void UpdateOutline()
    {
        outlineObject.transform.localScale = new Vector3(outlineSize, outlineSize, 1);
    }
}
