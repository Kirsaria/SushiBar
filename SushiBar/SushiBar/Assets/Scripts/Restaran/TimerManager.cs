using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    private int sec = 0;
    private int min = 0;
    private TMP_Text TimerText;
    [SerializeField] private int delta = 0;
    [SerializeField] private Light2D[] lightLamps;  // Массив дневного света (Light2D)
    [SerializeField] private Light2D[] lightNights;  // Массив ночного света (Light2D)
    [SerializeField] private float transitionDuration = 5f; // Длительность перехода между днем и ночью
    [SerializeField] private AudioSource audioSource;
    private NPCManager npcManager;
    public Animator animatorButton;

    private bool isDay = false; // Флаг, указывающий на то, день ли сейчас
    private bool hasDisplayedNPCsServed = false;

    private void Start()
    {
        TimerText = GameObject.Find("TimerText").GetComponent<TMP_Text>();
        npcManager = FindObjectOfType<NPCManager>();
        SetNightLighting(1f); // Включаем ночной свет
        SetDayLighting(0f);   // Выключаем дневной свет
        StartCoroutine(ITimer());
        LoadAudioSettings();
    }

    IEnumerator ITimer()
    {
        while (true)
        {
            if (!hasDisplayedNPCsServed && npcManager.AllNPCsServed())
            {
                Debug.Log("Конец дня");
                animatorButton.SetTrigger("IsOn");
                hasDisplayedNPCsServed = true;
            }

            if (min == 23)
            {
                npcManager.RemoveAllNPCs();
                animatorButton.SetTrigger("IsOn");
                yield break; // Останавливаем корутину, если прошло 12 минут
            }

            // Переключение света
            if (min < 9)
            {
                SetNightLighting(1f); // Ночной свет включен
                SetDayLighting(0f);   // Дневной свет выключен
            }
            else if (min == 9 && !isDay)
            {
                PlayLightSwitchSound();
                // Плавный переход на дневной свет
                yield return StartCoroutine(SwitchLighting(true));
                isDay = true; // Устанавливаем флаг, что сейчас день
            }
            // Переключение света
            if (min >18)
            {
                SetNightLighting(1f); // Ночной свет включен
                SetDayLighting(0f);   // Дневной свет выключен
            }
            else if (min == 18 && isDay)
            {
                PlayLightSwitchSound();
                // Плавный переход на дневной свет
                yield return StartCoroutine(SwitchNighting(true));
                isDay = false; 
            }

            // Обновляем таймер
            if (sec == 59)
            {
                min++;
                sec = -1;
            }
            sec += delta;
            TimerText.text = min.ToString("D2") + " : " + sec.ToString("D2");

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator SwitchLighting(bool toDay)
    {
        
        float startIntensity = toDay ? 0 : 1; // Начальная интенсивность
        float endIntensity = toDay ? 1 : 0;   // Конечная интенсивность
        float elapsedTime = 0f;

        // Плавный переход
        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            float intensity = Mathf.Lerp(startIntensity, endIntensity, t);

            SetDayLighting(intensity);   // Устанавливаем интенсивность для дневного света
            SetNightLighting(1 - intensity); // Устанавливаем интенсивность для ночного света

            elapsedTime += Time.deltaTime;
            yield return null; // Ждем следующего кадра
        }

        // Убедитесь, что интенсивности установлены точно
        SetDayLighting(1f); // Дневной свет включен
        SetNightLighting(0f); // Ночной свет выключен
    }
    private IEnumerator SwitchNighting(bool toDay)
    {
        float startIntensity = toDay ? 0 : 1; // Начальная интенсивность
        float endIntensity = toDay ? 1 : 0;   // Конечная интенсивность
        float elapsedTime = 0f;

        // Плавный переход
        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            float intensity = Mathf.Lerp(startIntensity, endIntensity, t);

            SetDayLighting(1 - intensity);   // Устанавливаем интенсивность для дневного света
            SetNightLighting(intensity); // Устанавливаем интенсивность для ночного света

            elapsedTime += Time.deltaTime;
            yield return null; // Ждем следующего кадра
        }

        // Убедитесь, что интенсивности установлены точно
        SetDayLighting(0f); // Дневной свет включен
        SetNightLighting(1f); // Ночной свет выключен
    }

    private void SetDayLighting(float intensity)
    {
        foreach (Light2D light in lightLamps)
        {
            light.intensity = intensity; // Устанавливаем интенсивность для дневного света
        }
    }

    private void SetNightLighting(float intensity)
    {
        foreach (Light2D light in lightNights)
        {
            light.intensity = intensity; // Устанавливаем интенсивность для ночного света
        }
    }
    private void PlayLightSwitchSound()
    {
        Debug.Log("звук");
        audioSource.Play(); // Воспроизводим звук
    }
    private void LoadAudioSettings()
    {
        // Загружаем громкость звуковых эффектов из PlayerPrefs
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f); // Значение по умолчанию 1f
        audioSource.volume = sfxVolume; // Устанавливаем громкость для AudioSource
    }
}