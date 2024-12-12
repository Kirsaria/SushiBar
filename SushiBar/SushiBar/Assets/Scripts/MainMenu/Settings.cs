using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public Dropdown resolutionDropDown;
    private Resolution[] resolutions;
    public Toggle fullscreenToggle;
    public Toggle toggleMusic;
    public Slider sliderVolumeMusic;
    public AudioSource audioMusic;
    public Toggle toggleSFX;
    public Slider sliderVolumeSFX;
    public AudioSource audioSFX;
    public float volume;
    public float sfxVolume;
    public static Settings Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Удаляем дубликаты
        }
    }

    void Start()
    {
        Screen.fullScreen = true;
        fullscreenToggle.isOn = true;
        resolutions = Screen.resolutions;
        string[] strRes = new string[resolutions.Length];

        for (int i = 0; i < resolutions.Length; i++)
        {
            strRes[i] = resolutions[i].width.ToString() + "x" + resolutions[i].height.ToString();
        }
        resolutionDropDown.ClearOptions();
        resolutionDropDown.AddOptions(strRes.ToList());
        resolutionDropDown.value = resolutions.Length - 1;
        Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, Screen.fullScreen);
        UpdateVolumeSettings();
        UpdateSFXSettings();
    }

    private void UpdateVolumeSettings()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetMusicVolume(volume);
            sliderVolumeMusic.value = volume;
            toggleMusic.isOn = volume > 0;
        }
    }

    public void SliderMusic()
    {
        volume = sliderVolumeMusic.value;
        SaveSettings();
        UpdateVolumeSettings();
    }

    public void ToggleMusic()
    {
        volume = toggleMusic.isOn ? 1 : 0;
        SaveSettings();
        UpdateVolumeSettings();
    }

    public void SliderSFX()
    {
        sfxVolume = sliderVolumeSFX.value;
        SaveSettings();
        UpdateSFXSettings();
        audioSFX.Play();
    }

    public void ToggleSFX()
    {
        sfxVolume = toggleSFX.isOn ? 1 : 0;
        SaveSettings();
        UpdateSFXSettings();
        audioSFX.Play();
    }

    private void UpdateSFXSettings()
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetSFXVolume(sfxVolume);
            sliderVolumeSFX.value = sfxVolume;
            toggleSFX.isOn = sfxVolume > 0;
        }
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = fullscreenToggle.isOn;
        SaveSettings();
    }

    public void SetResolution()
    {
        Screen.SetResolution(resolutions[resolutionDropDown.value].width, resolutions[resolutionDropDown.value].height, Screen.fullScreen);
        SaveSettings(); 
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropDown.value);
        PlayerPrefs.SetInt("FullscreenToggle", System.Convert.ToInt32(fullscreenToggle.isOn));
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    //public void LoadSettings(int currentResolutionIndex)
    //{
    //    if (PlayerPrefs.HasKey("ResolutionPreference"))
    //    {
    //        resolutionDropDown.value = PlayerPrefs.GetInt("ResolutionPreference");
    //        SetResolution(resolutionDropDown.value); 
    //    }
    //    else
    //    {
    //        resolutionDropDown.value = currentResolutionIndex;
    //    }

    //    if (PlayerPrefs.HasKey("FullscreenToggle"))
    //    {
    //        fullscreenToggle.isOn = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenToggle"));
    //        Screen.fullScreen = fullscreenToggle.isOn;
    //    }

    //    volume = PlayerPrefs.GetFloat("volume", volume);
    //    sliderVolumeMusic.value = volume;
    //    sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    //    sliderVolumeSFX.value = sfxVolume;
    //}
}