using UnityEngine;
using UnityEngine.TextCore.Text;

public class FadeRenderer : MonoBehaviour
{
    const float fadeInAlpha = 1f;
    const float fadeOutAlpha = 0.5f;

    private bool isFading = false;
    private SpriteRenderer[] spriteRenderers; // Array to hold all SpriteRenderers in children

    public SpriteRenderer playerSprite;
    private int layerOrder;

    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // Example of accessing PlayerStats if needed
        layerOrder = playerSprite.sortingOrder;
    }

    void Update()
    {
        if (isFading)
        {
            FadeOut();
        }
        else
        {
            FadeIn();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFading) // to prevent multiple triggers
            return;
        if (collision.TryGetComponent<PlayerStats>(out var playerStats)) // calling the PlayerStats script from the player object
        {
            //Debug.Log("OnTriggerEnter2D ");
            isFading = true;
            playerStats.GetComponent<SpriteRenderer>().sortingOrder = spriteRenderers[0].sortingOrder - 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isFading) // to prevent multiple triggers
                return; 
        if (collision.TryGetComponent<PlayerStats>(out var playerStats)) // calling the PlayerStats script from the player object
        {
            //Debug.Log("OnTriggerExit2D ");
            isFading = false;
            playerStats.GetComponent<SpriteRenderer>().sortingOrder = layerOrder;
        }
    }

    private void FadeOut()
    {
        foreach( var rederer in spriteRenderers )
        {
            changeOpacity(rederer, fadeOutAlpha);
        }
    }

    private void FadeIn()
    {
        foreach (var rederer in spriteRenderers)
        {
            changeOpacity(rederer, fadeInAlpha);
        }
    }

    private void changeOpacity(SpriteRenderer obj, float targetAlpha)
    {
        Color color = obj.color;
        Color smoothColor = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, targetAlpha, Time.deltaTime * 5f)); // Smooth transition
        obj.color = smoothColor;
    }
}
