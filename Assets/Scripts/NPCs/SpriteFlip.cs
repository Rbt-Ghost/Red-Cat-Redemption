using UnityEngine;

public class SpriteFlip : MonoBehaviour
{

    public Transform player; // Reference to the player's transform
    private SpriteRenderer spriteRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            // Flip the sprite based on the player's position
            if (player.position.x < transform.position.x)
            {
                spriteRenderer.flipX = true; // Face left
            }
            else
            {
                spriteRenderer.flipX = false; // Face right
            }
        }
    }
}
