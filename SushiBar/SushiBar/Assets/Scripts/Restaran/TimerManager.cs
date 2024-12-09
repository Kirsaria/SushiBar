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
    [SerializeField] private Light2D[] lightLamps;  // ������ �������� ����� (Light2D)
    [SerializeField] private Light2D[] lightNights;  // ������ ������� ����� (Light2D)
    [SerializeField] private float transitionDuration = 5f; // ������������ �������� ����� ���� � �����
    [SerializeField] private AudioSource audioSource;
    private NPCManager npcManager;
    public Animator animatorButton;

    private bool isDay = false; // ����, ����������� �� ��, ���� �� ������
    private bool hasDisplayedNPCsServed = false;

    private void Start()
    {
        TimerText = GameObject.Find("TimerText").GetComponent<TMP_Text>();
        npcManager = FindObjectOfType<NPCManager>();
        SetNightLighting(1f); // �������� ������ ����
        SetDayLighting(0f);   // ��������� ������� ����
        StartCoroutine(ITimer());
        LoadAudioSettings();
    }

    IEnumerator ITimer()
    {
        while (true)
        {
            if (!hasDisplayedNPCsServed && npcManager.AllNPCsServed())
            {
                Debug.Log("����� ���");
                animatorButton.SetTrigger("IsOn");
                hasDisplayedNPCsServed = true;
            }

            if (min == 23)
            {
                npcManager.RemoveAllNPCs();
                animatorButton.SetTrigger("IsOn");
                yield break; // ������������� ��������, ���� ������ 12 �����
            }

            // ������������ �����
            if (min < 9)
            {
                SetNightLighting(1f); // ������ ���� �������
                SetDayLighting(0f);   // ������� ���� ��������
            }
            else if (min == 9 && !isDay)
            {
                PlayLightSwitchSound();
                // ������� ������� �� ������� ����
                yield return StartCoroutine(SwitchLighting(true));
                isDay = true; // ������������� ����, ��� ������ ����
            }
            // ������������ �����
            if (min >18)
            {
                SetNightLighting(1f); // ������ ���� �������
                SetDayLighting(0f);   // ������� ���� ��������
            }
            else if (min == 18 && isDay)
            {
                PlayLightSwitchSound();
                // ������� ������� �� ������� ����
                yield return StartCoroutine(SwitchNighting(true));
                isDay = false; 
            }

            // ��������� ������
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
        
        float startIntensity = toDay ? 0 : 1; // ��������� �������������
        float endIntensity = toDay ? 1 : 0;   // �������� �������������
        float elapsedTime = 0f;

        // ������� �������
        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            float intensity = Mathf.Lerp(startIntensity, endIntensity, t);

            SetDayLighting(intensity);   // ������������� ������������� ��� �������� �����
            SetNightLighting(1 - intensity); // ������������� ������������� ��� ������� �����

            elapsedTime += Time.deltaTime;
            yield return null; // ���� ���������� �����
        }

        // ���������, ��� ������������� ����������� �����
        SetDayLighting(1f); // ������� ���� �������
        SetNightLighting(0f); // ������ ���� ��������
    }
    private IEnumerator SwitchNighting(bool toDay)
    {
        float startIntensity = toDay ? 0 : 1; // ��������� �������������
        float endIntensity = toDay ? 1 : 0;   // �������� �������������
        float elapsedTime = 0f;

        // ������� �������
        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            float intensity = Mathf.Lerp(startIntensity, endIntensity, t);

            SetDayLighting(1 - intensity);   // ������������� ������������� ��� �������� �����
            SetNightLighting(intensity); // ������������� ������������� ��� ������� �����

            elapsedTime += Time.deltaTime;
            yield return null; // ���� ���������� �����
        }

        // ���������, ��� ������������� ����������� �����
        SetDayLighting(0f); // ������� ���� �������
        SetNightLighting(1f); // ������ ���� ��������
    }

    private void SetDayLighting(float intensity)
    {
        foreach (Light2D light in lightLamps)
        {
            light.intensity = intensity; // ������������� ������������� ��� �������� �����
        }
    }

    private void SetNightLighting(float intensity)
    {
        foreach (Light2D light in lightNights)
        {
            light.intensity = intensity; // ������������� ������������� ��� ������� �����
        }
    }
    private void PlayLightSwitchSound()
    {
        Debug.Log("����");
        audioSource.Play(); // ������������� ����
    }
    private void LoadAudioSettings()
    {
        // ��������� ��������� �������� �������� �� PlayerPrefs
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f); // �������� �� ��������� 1f
        audioSource.volume = sfxVolume; // ������������� ��������� ��� AudioSource
    }
}