using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreScript : MonoBehaviour
{
    public GameObject username;
    public GameObject day;
    public GameObject time;
    public void SetScore(string username, string day, string time)
    {
        this.username.GetComponent<Text>().text = username;
        this.day.GetComponent<Text>().text = day;
        this.time.GetComponent<Text>().text = time;
    }
}
