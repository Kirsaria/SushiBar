using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScore 
{
    public int ID {  get; set; }
    public string Username { get; set; }
    public string Day {  get; set; }
    public string Time { get; set; }
    public HighScore(int ID, string Username, string Day, string Time)
    {
        this.ID = ID;
        this.Username = Username;
        this.Day = Day;
        this.Time = Time;
    }
}
