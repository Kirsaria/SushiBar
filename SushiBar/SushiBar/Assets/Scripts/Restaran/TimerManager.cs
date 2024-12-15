using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Mono.Data.Sqlite;

public class TimerManager : MonoBehaviour
{
    private int sec = 0;
    private int min = 0;
    private TMP_Text TimerText;
    [SerializeField] private int delta = 0;
    [SerializeField] private Light2D[] lightLamps;  
    [SerializeField] private Light2D[] lightNights;  
    [SerializeField] private float transitionDuration = 5f; 
    [SerializeField] private AudioSource audioSource;
    private NPCManager npcManager;
    public Animator animatorButton;

    private bool isDay = false; 
    private bool hasDisplayedNPCsServed = false;

    private void Start()
    {
        TimerText = GameObject.Find("TimerText").GetComponent<TMP_Text>();
        npcManager = FindObjectOfType<NPCManager>();
        SetNightLighting(1f); 
        SetDayLighting(0f);  
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
                string currentDay = FindObjectOfType<UserDataSaver>().day;
                SaveLevelTime($"{min:D2}:{sec:D2}", currentDay);
            }

            if (min == 23)
            {
                npcManager.RemoveAllNPCs();
                animatorButton.SetTrigger("IsOn");
                yield break; 
            }

            if (min < 9)
            {
                SetNightLighting(1f); 
                SetDayLighting(0f);  
            }
            else if (min == 9 && !isDay)
            {
                PlayLightSwitchSound();
                yield return StartCoroutine(SwitchLighting(true));
                isDay = true;
            }
            if (min >18)
            {
                SetNightLighting(1f); 
                SetDayLighting(0f);  
            }
            else if (min == 18 && isDay)
            {
                PlayLightSwitchSound();
                yield return StartCoroutine(SwitchNighting(true));
                isDay = false; 
            }

            if (sec == 59)
            {
                min++;
                sec = -1;
            }
            sec += delta;
            TimerText.text = min.ToString("D2") + " : " + sec.ToString("D2");

            yield return new WaitForSeconds(0.3f);
        }
    }
    private int CompareTimes(string time1, string time2)
    {
        string[] parts1 = time1.Split(':');
        string[] parts2 = time2.Split(':');

        int minutes1 = int.Parse(parts1[0]);
        int seconds1 = int.Parse(parts1[1]);

        int minutes2 = int.Parse(parts2[0]);
        int seconds2 = int.Parse(parts2[1]);

        if (minutes1 != minutes2)
        {
            return minutes1.CompareTo(minutes2); 
        }
        return seconds1.CompareTo(seconds2); 
    }
    private void SaveLevelTime(string levelTime, string day)
    {
        string username = GlobalData.Instance.Username;
        string conn = "URI=file:Users.db"; 

        using (var connection = new SqliteConnection(conn))
        {
            connection.Open();
            using (var checkCommand = connection.CreateCommand())
            {
                checkCommand.CommandText = "SELECT LevelTime FROM LevelRecords WHERE username = @username AND Day = @day";
                checkCommand.Parameters.AddWithValue("@username", username);
                checkCommand.Parameters.AddWithValue("@day", day);

                string currentRecordTime = checkCommand.ExecuteScalar() as string;

                if (currentRecordTime != null)
                {
                    if (CompareTimes(levelTime, currentRecordTime) < 0) 
                    {
                        using (var updateCommand = connection.CreateCommand())
                        {
                            updateCommand.CommandText = "UPDATE LevelRecords SET LevelTime = @levelTime WHERE username = @username AND Day = @day";
                            updateCommand.Parameters.AddWithValue("@levelTime", levelTime);
                            updateCommand.Parameters.AddWithValue("@username", username);
                            updateCommand.Parameters.AddWithValue("@day", day);
                            updateCommand.ExecuteNonQuery();
                        }
                    }
                }
                else 
                {
                    using (var insertCommand = connection.CreateCommand())
                    {
                        insertCommand.CommandText = "INSERT INTO LevelRecords (username, Day, LevelTime) VALUES (@username, @day, @levelTime)";
                        insertCommand.Parameters.AddWithValue("@username", username);
                        insertCommand.Parameters.AddWithValue("@day", day);
                        insertCommand.Parameters.AddWithValue("@levelTime", levelTime);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }

    private IEnumerator SwitchLighting(bool toDay)
    {
        
        float startIntensity = toDay ? 0 : 1; 
        float endIntensity = toDay ? 1 : 0;   
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            float intensity = Mathf.Lerp(startIntensity, endIntensity, t);

            SetDayLighting(intensity);  
            SetNightLighting(1 - intensity); 

            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        SetDayLighting(1f);
        SetNightLighting(0f); 
    }
    private IEnumerator SwitchNighting(bool toDay)
    {
        float startIntensity = toDay ? 0 : 1; 
        float endIntensity = toDay ? 1 : 0;  
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            float intensity = Mathf.Lerp(startIntensity, endIntensity, t);

            SetDayLighting(1 - intensity);   
            SetNightLighting(intensity);

            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        SetDayLighting(0f); 
        SetNightLighting(1f); 
    }

    private void SetDayLighting(float intensity)
    {
        foreach (Light2D light in lightLamps)
        {
            light.intensity = intensity; 
        }
    }

    private void SetNightLighting(float intensity)
    {
        foreach (Light2D light in lightNights)
        {
            light.intensity = intensity;
        }
    }
    private void PlayLightSwitchSound()
    {
        Debug.Log("звук");
        audioSource.Play(); 
    }
    private void LoadAudioSettings()
    {
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f); 
        audioSource.volume = sfxVolume;
    }
}