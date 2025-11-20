using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

public void PlayGame(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("game quit");
    }


    public GameObject settingPanelShadow; 
    public GameObject settingPanel;      

    
    public void SetPanelsTransparency()
    {
        
        if (settingPanelShadow != null)
        {
            Image shadowImg = settingPanelShadow.GetComponent<Image>();
            if (shadowImg != null)
            {
                Color c = shadowImg.color;
                c.a = 0.5f; 
                shadowImg.color = c;
            }
        }

   
        if (settingPanel != null)
        {
            Image panelImg = settingPanel.GetComponent<Image>();
            if (panelImg != null)
            {
                Color c = panelImg.color;
                c.a = 1f; 
                panelImg.color = c;
            }
        }
    }

}

