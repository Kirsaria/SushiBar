using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using System.Text.RegularExpressions;
using System;

public class Registration : MonoBehaviour
{
    public InputField usernameField;
    public InputField passwordField;
    public Text errorText;
    public InputField doublePas1;
    public static bool Enter = false;
    public Button registerButton;
    public Button loginButton;
    public GameObject mainMenu;
    public GameObject registrationWindow;
    public GameObject loginWindow;
    public InputField loginUsernameField;
    public InputField loginPasswordField;
    public Text loginErrorText;

    private string dbName = "URI=file:Users.db";

    private void Start()
    {
        Debug.Log("Метод Start вызван");
        CreateDB();
        registerButton.onClick.AddListener(OnRegisterButtonClick);
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }

    private bool ValidatePassword(string password)
    {
        Regex regex = new Regex("^(?=.*[A-Z])(?=.*\\d)(?!.*\\s).{8,20}$");
        return regex.IsMatch(password);
    }

    private bool ValidateUsername(string username)
    {
        Regex regex = new Regex("^(?=.*[A-Z])[a-zA-Z]{3,10}$");
        return regex.IsMatch(username) && !username.Contains("@") && !username.Contains(".");
    }

    private void CreateDB()
    {
        Debug.Log("Метод CreateDB вызван");
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            Debug.Log("Соединение с базой данных открыто");

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS users (username VARCHAR(20), password VARCHAR(20), Day TEXT, TotalTears INTEGER);";
                command.ExecuteNonQuery();
                Debug.Log("Команда на создание таблицы выполнена");
            }

            connection.Close();
            Debug.Log("Соединение с базой данных закрыто");
        }
    }

    public void ConfirmPas()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        string doublePas = doublePas1.text;
        if (string.IsNullOrEmpty(doublePas))
        {
            doublePas1.gameObject.SetActive(true);
            errorText.text = "ПОДТВЕРДИТЕ ВАШ ПАРОЛЬ";
        }
        else if (doublePas == password)
        {
            errorText.text = "Вы зарегистрированы";
            Register();
        }
        else
        {
            errorText.text = "Вы ввели неправильный пароль";
        }
    }

    public void OnRegisterButtonClick()
    {
        Register();
    }

    public void OnLoginButtonClick()
    {
        Login();
    }

    public void Login()
    {
        string username = loginUsernameField.text;
        string password = loginPasswordField.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM users WHERE username=@username AND password=@password;";
                    command.Parameters.Add(new SqliteParameter("@username", username));
                    command.Parameters.Add(new SqliteParameter("@password", password));
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            loginErrorText.text = "Добро пожаловать!";
                            Debug.Log("Успешный вход");
                            Enter = true;
                            GlobalData.Instance.Username = username;
                            mainMenu.SetActive(true);
                            loginWindow.SetActive(false);
                        }
                        else
                        {
                            loginErrorText.text = "Неверное имя пользователя или пароль";
                            Debug.Log("Неудачный вход");
                        }
                    }
                }

                connection.Close();
            }
        }
        else
        {
            loginErrorText.text = "Пожалуйста, заполните все поля";
        }
    }

    public void Register()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        string confirmPassword = doublePas1.text;

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(confirmPassword))
        {
            if (ValidateUsername(username))
            {
                if (password == confirmPassword && ValidatePassword(password))
                {
                    using (var connection = new SqliteConnection(dbName))
                    {
                        connection.Open();
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = "SELECT COUNT(*) FROM users WHERE username=@username;";
                            command.Parameters.Add(new SqliteParameter("@username", username));

                            int count = Convert.ToInt32(command.ExecuteScalar());
                            if (count > 0)
                            {
                                errorText.text = "Пользователь уже существует";
                                Debug.Log("Пользователь уже существует");
                                return;
                            }
                            command.CommandText = "INSERT INTO users (username, password, Day, TotalTears) VALUES (@username, @password, NULL, 0);";
                            command.Parameters.Add(new SqliteParameter("@password", password));
                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    Enter = true;
                    GlobalData.Instance.Username = username;
                    errorText.text = "Регистрация успешна! Перенаправление в главное меню...";
                    mainMenu.SetActive(true);
                    registrationWindow.SetActive(false);
                }
                else
                {
                    errorText.text = "Пароли не совпадают или не соответствуют требованиям";
                }
            }
            else
            {
                errorText.text = "Имя пользователя должно содержать не менее 3 букв и не должно быть адресом электронной почты";
            }
        }
        else
        {
            errorText.text = "Пожалуйста, заполните все поля";
        }
    }
}