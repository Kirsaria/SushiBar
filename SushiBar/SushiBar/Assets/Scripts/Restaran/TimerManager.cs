using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class TimerManager : MonoBehaviour
{
    private int sec = 0;
    private int min = 0;
    private TMP_Text TimerText;
    [SerializeField] private int delta = 0;
    private void Start()
    {
        TimerText = GameObject.Find("TimerText").GetComponent<TMP_Text>();
        StartCoroutine(ITimer());
    }

    IEnumerator ITimer()
    {
        while (true)
        {
            if (min == 12)
            {
                yield break; // Останавливаем корутину, если прошло 5 минут
            }

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

}
