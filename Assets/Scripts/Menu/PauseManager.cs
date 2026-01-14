using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Meniul Principal de Pauza")]
    public GameObject mainPauseButtons; 

    [Header("Panoul de Setari (Posterul)")]
    public GameObject settingsPanel;       
    public GameObject settingsButtonsGroup; 

    [Header("Sub-Panouri")]
    public GameObject soundPanel;  
    public GameObject controlsPanel; 

    public void OpenSettings()
    {
        mainPauseButtons.SetActive(false);
        settingsPanel.SetActive(true);
        settingsButtonsGroup.SetActive(true); 

 
        soundPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainPauseButtons.SetActive(true);
    }

    public void OpenSoundPanel()
    {
        settingsButtonsGroup.SetActive(false); 
        soundPanel.SetActive(true);          
    }

    public void CloseSoundPanel() 
    {
        soundPanel.SetActive(false);
        settingsButtonsGroup.SetActive(true);  
    }

    public void OpenControlsPanel()
    {
        settingsButtonsGroup.SetActive(false);
        controlsPanel.SetActive(true);
    }

    public void CloseControlsPanel() 
    {
        controlsPanel.SetActive(false);
        settingsButtonsGroup.SetActive(true);
    }
}