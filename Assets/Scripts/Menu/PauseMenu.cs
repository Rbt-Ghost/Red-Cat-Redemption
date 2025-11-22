using static System.TimeZoneInfo;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;


public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    public Animator transition;
    public float transitionTime = 1f;
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;

    }
    public void Home()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex -1 ));
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

        IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }

    

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
}
