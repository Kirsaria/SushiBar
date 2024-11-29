using UnityEngine;
using UnityEngine.UI;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.Analytics;
using System;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

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
    public GameObject loginWindow; // Добавьте это поле
    public InputField loginUsernameField; // Добавьте это поле
    public InputField loginPasswordField; // Добавьте это поле
    public Text loginErrorText; // Добавьте это поле

    private string dbName = "URI=file:Users.db";

    private void Start()
    {
        Debug.Log("Start method called");
        CreateDB();
        registerButton.onClick.AddListener(OnRegisterButtonClick);
        loginButton.onClick.AddListener(OnLoginButtonClick);
    }
    private bool ValidatePassword(string password)
    {
        Regex regex = new Regex("^(?=.*[A-Z])(?=.*\\d)(?!.*\\s).{8,}$");
        return regex.IsMatch(password);
    }
    private bool ValidateUsername(string username)
    {
        Regex regex = new Regex("^(?=.*[A-Z])[a-zA-Z]{3}[a-z]*$");
        return regex.IsMatch(username);
    }

    private void CreateDB()
    {
        Debug.Log("CreateDB method called");
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            Debug.Log("Database connection opened");

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS users (username VARCHAR(20), password VARCHAR(20));";
                command.ExecuteNonQuery();
                Debug.Log("Table creation command executed");
            }

            connection.Close();
            Debug.Log("Database connection closed");
        }
    }

    public void ConfirmPas()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        string doublePas = doublePas1.text;
        if (doublePas == "" || doublePas == null)
        {
            doublePas1.gameObject.SetActive(true);
            errorText.text = "CONFRIM YOUR PASSWORD";
        }
        else if (doublePas == password)
        {
            errorText.text = "You are registed";
            Register();
        }
        else if(doublePas != password)
        {
            errorText.text = "You are input the wrong password";
        }

    }

    public void OnRegisterButtonClick() // Добавьте этот метод
    {
        Register();
    }
    public void OnLoginButtonClick() // Добавьте этот метод
    {
        Login();
    }

    public void Login()
    {
        string username = loginUsernameField.text;
        string password = loginPasswordField.text;

        if (username != "" && password != "")
        {
            using (var connection = new SqliteConnection(dbName))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM users WHERE username='" + username + "' AND password='" + password + "';";
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            loginErrorText.text = "Welcome!";
                            Debug.Log("Login successful");
                            Enter = true;
                            mainMenu.SetActive(true);
                            loginWindow.SetActive(false);
                        }
                        else
                        {
                            loginErrorText.text = "Invalid username or password";
                            Debug.Log("Login failed");
                        }
                        reader.Close();
                    }
                }

                connection.Close();
            }
        }
        else
        {
            loginErrorText.text = "Please fill in all fields";
        }
    }

public void Register()
    {
        string username = usernameField.text;
        string password = passwordField.text;
        string confirmPassword = doublePas1.text;

        if (username != "" && password != "" && confirmPassword != "")
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
                            command.CommandText = "SELECT COUNT(*) FROM users WHERE username='" + username + "';";
                            int count = Convert.ToInt32(command.ExecuteScalar());
                            if (count > 0)
                            {
                                errorText.text = "User already exists";
                                Debug.Log("User already exists");
                                return;
                            }
                            command.CommandText = "INSERT INTO users (username, password) VALUES ('" + username + "', '" + password + "');";
                            command.ExecuteNonQuery();
                        }
                        connection.Close();
                    }
                    Enter = true;
                    errorText.text = "Registration successful! Redirecting to main menu...";
                    mainMenu.SetActive(true); // Активируем главное меню
                    registrationWindow.SetActive(false); // Деактивируем окно регистрации
                }
                else
                {
                    errorText.text = "Passwords do not match or do not meet the requirements";
                }
            }
            else
            {
                errorText.text = "Username must be at least 8 letters and contain only English alphabet letters";
            }
        }
        else
        {
            errorText.text = "Please fill in all fields";
        }
    }

}