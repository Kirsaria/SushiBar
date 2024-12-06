using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;

public class DayResult : MonoBehaviour
{
    public Text clientServedText;
    public Text clientTotalText;
    public Text tearsCollectText;
    public Text tearsTotalText;
    public int totalClients;
    public int totalTears;
    private void Start()
    {
        DisplayDayResults();
    }

    private void DisplayDayResults()
    {
        // ����������� � ���� ������
        string conn = "URI=file:Orders.db"; // ������� ���� � ����� ���� ������
        using (var connection = new SqliteConnection(conn))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // �������� ���������� ����������� ��������
                command.CommandText = "SELECT COUNT(*) FROM Orders WHERE IsCookingCompleted = 1";
                int clientsServed = Convert.ToInt32(command.ExecuteScalar());
                clientServedText.text = "����������� �������: " + clientsServed;

            }
        }
        clientTotalText.text = "����� ��������: " + totalClients;
        tearsTotalText.text = "����� ����: " + totalTears;
        tearsCollectText.text = "������� ����: " + PlayerStats.Instance.GetPoints();
    }
}
