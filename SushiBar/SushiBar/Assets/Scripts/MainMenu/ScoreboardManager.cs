using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;

public class ScoreboardManager : MonoBehaviour
{
    public GameObject recordPrefab; 
    private List<HighScore> highScores = new List<HighScore>();
    public Transform scoreParent;
    private void Start()
    {
        //LoadScores();
        ShowScores(); 
    }

    private void LoadScores()
    {
        highScores.Clear();
        string conn = "URI=file:Users.db"; 
        using (var connection = new SqliteConnection(conn))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT id, username, Day, LevelTime FROM LevelRecords ORDER BY LevelTime ASC"; 
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        highScores.Add(new HighScore(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));

                    }
                }
            }
        }
    }

    private void ShowScores()
    {
        LoadScores();
        for(int i = 0; i < highScores.Count; i++)
        {
            GameObject tmpObjec = Instantiate(recordPrefab);
            HighScore tmpScore = highScores[i];
            tmpObjec.GetComponent<HighScoreScript>().SetScore(tmpScore.Username, tmpScore.Day, tmpScore.Time);
            tmpObjec.transform.SetParent(scoreParent);
            tmpObjec.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        }
    }
}