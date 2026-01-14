using UnityEngine;
using UnityEngine.UI; 

public class PlatformIconSwitcher : MonoBehaviour
{
    [Header("Setari Imagini")]
    public Sprite imaginePC;      // imagine taste
    public Sprite imagineMobil;   // imagine telefon

    [Header("Componenta")]
    public Image imageComponent;  

    void Start()
    {  
        if (imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
        }

        CheckPlatform();
    }

    void CheckPlatform()
    {
        if (Application.isMobilePlatform)
        {
            if (imagineMobil != null)
                imageComponent.sprite = imagineMobil;
        }
        else
        {
            if (imaginePC != null)
                imageComponent.sprite = imaginePC;
        }
    }
}