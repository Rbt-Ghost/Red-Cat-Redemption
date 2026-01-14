using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [Header("Conectare Mixer")]
    public AudioMixer myMixer;

    [Header("Slidere")]
    public Slider musicSlider;
    public Slider sfxSlider; 

    void Start()
    {
      
        float savedMusic = PlayerPrefs.GetFloat("MusicVolumePref", 1f);
        if (musicSlider != null) musicSlider.value = savedMusic;
        SetMusicVolume(savedMusic);
   
        float savedSFX = PlayerPrefs.GetFloat("SFXVolumePref", 1f);
        if (sfxSlider != null) sfxSlider.value = savedSFX;
        SetSFXVolume(savedSFX);
    }

    public void SetMusicVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;
        myMixer.SetFloat("MusicVol", dbValue);
        PlayerPrefs.SetFloat("MusicVolumePref", sliderValue);
    }

    public void SetSFXVolume(float sliderValue)
    {
        float dbValue = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20;

        myMixer.SetFloat("SFXVol", dbValue);

        PlayerPrefs.SetFloat("SFXVolumePref", sliderValue);
    }
}