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
        Debug.Log("����� Start ������");
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
        Debug.Log("����� CreateDB ������");
        using (var connection = new SqliteConnection(dbName))
        {
            connection.Open();
            Debug.Log("���������� � ����� ������ �������");

            using (var command = connection.CreateCommand())
            {
                command.CommandText = "CREATE TABLE IF NOT EXISTS users (username VARCHAR(20), password VARCHAR(20), Day TEXT, TotalTears INTEGER);";
                command.ExecuteNonQuery();
                Debug.Log("������� �� �������� ������� ���������");
            }

            connection.Close();
            Debug.Log("���������� � ����� ������ �������");
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
            errorText.text = "����������� ��� ������";
        }
        else if (doublePas == password)
        {
            errorText.text = "�� ����������������";
            Register();
        }
        else
        {
            errorText.text = "�� ����� ������������ ������";
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
                            loginErrorText.text = "����� ����������!";
                            Debug.Log("�������� ����");
                            Enter = true;
                            GlobalData.Instance.Username = username;
                            mainMenu.SetActive(true);
                            loginWindow.SetActive(false);
                        }
                        else
                        {
                            loginErrorText.text = "�������� ��� ������������ ��� ������";
                            Debug.Log("��������� ����");
                        }
                    }
                }

                connection.Close();
            }
        }
        else
        {
            loginErrorText.text = "����������, ��������� ��� ����";
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
                                errorText.text = "������������ ��� ����������";
                                Debug.Log("������������ ��� ����������");
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
                    errorText.text = "����������� �������! ��������������� � ������� ����...";
                    mainMenu.SetActive(true);
                    registrationWindow.SetActive(false);
                }
                else
                {
                    errorText.text = "������ �� ��������� ��� �� ������������� �����������";
                }
            }
            else
            {
                errorText.text = "��� ������������ ������ ��������� �� ����� 3 ���� � �� ������ ���� ������� ����������� �����";
            }
        }
        else
        {
            errorText.text = "����������, ��������� ��� ����";
        }
    }
}