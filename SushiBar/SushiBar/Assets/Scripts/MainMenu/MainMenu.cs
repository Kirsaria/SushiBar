using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button continueButton;
    public Text sceneNameText;
    private string sceneToLoad;

    private void Start()
    {
        CheckUserData();
    }

    public void PlayGame()
    {
        GameState.LastAction = "StartGame";
        ResetUserData();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Continue()
    {
        GameState.LastAction = "ContinueGame";
        SceneManager.LoadScene(sceneToLoad);
    }

    private void CheckUserData()
    {
        string username = GlobalData.Instance.Username; // �������� ��� ������������
        Debug.Log($"������������: {username}");

        // �������� ������ �� ���� ������
        using (var connection = new SqliteConnection("URI=file:Users.db")) // ������� ���������� ���� � ����� ��
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT Day, TotalTears FROM users WHERE username=@username;";
                command.Parameters.Add(new SqliteParameter("@username", username));

                using (IDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string day = reader["Day"] as string;
                        int totalTears = reader["TotalTears"] != DBNull.Value ? Convert.ToInt32(reader["TotalTears"]) : 0;

                        // �������� ���������� ��������
                        Debug.Log($"����������� �������� - Day: '{day}', TotalTears: {totalTears}");

                        // ������� �������
                        day = day?.Trim();

                        // ���� �������� NULL ��� ������, ��������� ������
                        if (string.IsNullOrEmpty(day) || totalTears <= 0)
                        {
                            Debug.LogWarning("������ ��������.");
                            continueButton.interactable = false;
                        }
                        else
                        {
                            continueButton.interactable = true; // �������� ������, ���� ������ �������
                            sceneToLoad = day; // ��������� �������� �����
                            sceneNameText.text = "����������: " + sceneToLoad; // ��������� ��������� ���� � ��������� �����
                        }
                    }
                    else
                    {
                        Debug.LogWarning("������ ������������ �� �������.");
                        continueButton.interactable = false;
                    }
                }
            }
            connection.Close();
        }
    }
    public void ResetUserData()
    {
        string username = GlobalData.Instance.Username; // �������� ��� ������������
        Debug.Log($"���������� ������ ��� ������������: {username}");

        using (var connection = new SqliteConnection("URI=file:Users.db")) // ������� ���������� ���� � ����� ��
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE users SET Day = 'Restaurant', TotalTears = 0 WHERE username = @username;";
                command.Parameters.Add(new SqliteParameter("@username", username));

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    Debug.Log("������ ������������ ������� ��������.");
                    // ����� ������������� ������� CheckUserData, ����� �������� ���������
                    CheckUserData();
                }
                else
                {
                    Debug.LogWarning("�� ������� �������� ������ ������������.");
                }
            }
            connection.Close();
        }
    }
    public void ExitGame()
    {
        Debug.Log("���� ���������");
        Application.Quit();
    }
}