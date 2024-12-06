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
        // Подключение к базе данных
        string conn = "URI=file:Orders.db"; // Укажите путь к вашей базе данных
        using (var connection = new SqliteConnection(conn))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                // Получаем количество обслуженных клиентов
                command.CommandText = "SELECT COUNT(*) FROM Orders WHERE IsCookingCompleted = 1";
                int clientsServed = Convert.ToInt32(command.ExecuteScalar());
                clientServedText.text = "Обслуженные клиенты: " + clientsServed;

            }
        }
        clientTotalText.text = "Всего клиентов: " + totalClients;
        tearsTotalText.text = "Всего слез: " + totalTears;
        tearsCollectText.text = "Собрано слез: " + PlayerStats.Instance.GetPoints();
    }
}
