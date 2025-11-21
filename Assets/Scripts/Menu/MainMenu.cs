using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;

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
}

