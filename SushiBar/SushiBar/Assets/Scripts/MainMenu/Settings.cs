using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Dropdown resolutionDropDown;
    Resolution[] resolutions;
    public Toggle fullscreenToggle;
    public Toggle toggleMusic;
    public Slider sliderVolumeMusic;
    public AudioSource audioMusic;
    public Toggle toggleSFX;
    public Slider sliderVolumeSFX;
    public AudioSource audioSFX;
    public float volume;
    public float sfxVolume;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        resolutionDropDown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for(int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRateRatio + "";
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropDown.AddOptions(options);
        resolutionDropDown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
        ValueMusic();
        ApplySFXSettings();
    }
    public void SliderMusic()
    {
        volume = sliderVolumeMusic.value;
        SaveSettings();
        ValueMusic();
    }
    public void ToggleMusic()
    {
        if (toggleMusic.isOn)
            volume = 1;
        else
            volume = 0;
        SaveSettings();
        ValueMusic();
    }
    private void ValueMusic()
    {
        audioMusic.volume = volume;
        sliderVolumeMusic.value = volume;
        toggleMusic.isOn = volume > 0;
    }
    public void SliderSFX()
    {
        sfxVolume = sliderVolumeSFX.value;
        SaveSettings();
        ApplySFXSettings();
        audioSFX.Play();
    }

    public void ToggleSFX()
    {
        sfxVolume = toggleSFX.isOn ? 1 : 0;
        SaveSettings();
        ApplySFXSettings();
        audioSFX.Play();
    }

    private void ApplySFXSettings()
    {
        audioSFX.volume = sfxVolume;
        sliderVolumeSFX.value = sfxVolume;
        toggleSFX.isOn = sfxVolume > 0;
    }
    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropDown.value);
        PlayerPrefs.SetInt("FullscreenPreference", System.Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetInt("FullscreenToggle", System.Convert.ToInt32(fullscreenToggle.isOn));
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        if(PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropDown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropDown.value = currentResolutionIndex;
        if (PlayerPrefs.HasKey("FullscreenTogglePreference"))
            fullscreenToggle.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenToggle"));
        fullscreenToggle.isOn = true;
        volume = PlayerPrefs.GetFloat("MusicVolume", volume);
        sliderVolumeMusic.value = volume;
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        sliderVolumeSFX.value = sfxVolume;
    }
}
