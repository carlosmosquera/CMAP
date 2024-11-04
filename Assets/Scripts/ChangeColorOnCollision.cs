using UnityEngine;

public class ChangeColorOnCollision : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("IT hiT");
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.green;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }
}
