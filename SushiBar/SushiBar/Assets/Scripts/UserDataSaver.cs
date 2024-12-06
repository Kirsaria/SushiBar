using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class UserDataSaver : MonoBehaviour
{
    private string dbName = "URI=file:Users.db";
    private string username;
    public string day; 
    public int totalTears;

    void Start()
    {
        if (FindObjectOfType<UserDataSaver>() == null)
        {
            GameObject userDataSaverObject = new GameObject("UserDataSaver");
            userDataSaverObject.AddComponent<UserDataSaver>();
        }
    }
    public void HandleUserData()
    {
        switch (GameState.LastAction)
        {
            case "StartGame":
                SaveUserDataAfterLoading();
                LoadUserData();
                break;
            case "ContinueGame":
                LoadUserData();
                SaveUserDataAfterLoading();
                break;
            case "NextDay":
                SaveUserDataAfterLoading();
                LoadUserData();
                break;
            default:
                Debug.LogWarning("Неизвестное действие");
                break;
        }
    }

    public void SaveUserData(string day, int totalTears)
    {
        string username = GlobalData.Instance.Username;
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE users SET Day = @day, TotalTears = @totalTears WHERE username = @username;";
                command.Parameters.Add(new SqliteParameter("@username", username));
                command.Parameters.Add(new SqliteParameter("@day", day));
                command.Parameters.Add(new SqliteParameter("@totalTears", totalTears));

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Debug.Log("Данные пользователя успешно обновлены");
                }
                else
                {
                    Debug.Log("Не удалось обновить данные пользователя");
                }
            }
            connection.Close();
        }
    }
    private void LoadUserData()
    {
        username = GlobalData.Instance.Username; // Получаем имя пользователя

        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Day, TotalTears FROM users WHERE username = @username;";
                command.Parameters.Add(new SqliteParameter("@username", username));

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        day = reader["Day"] as string;
                        totalTears = reader["TotalTears"] != DBNull.Value ? Convert.ToInt32(reader["TotalTears"]) : 0;
                        PlayerStats.Instance.SetTotalTears(totalTears);
                        Debug.Log($"Загруженные данные - Day: '{day}', TotalTears: {totalTears}");
                    }
                    else
                    {
                        Debug.Log("Пользователь не найден, инициализация данных по умолчанию.");
                        day = "Restaurant";
                        totalTears = 0;
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        // Подписываемся на событие загрузки сцены
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Отписываемся от события при отключении объекта
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        HandleUserData();
    }

    private void SaveUserDataAfterLoading()
    {
        string currentDay = GetCurrentDay(); 
        int totalTears = GetTotalTears(); 

        SaveUserData(currentDay, totalTears);
    }

    private string GetCurrentDay()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        return sceneName; 
    }

    private int GetTotalTears()
    {
        return PlayerStats.Instance.GetPoints();
    }
}