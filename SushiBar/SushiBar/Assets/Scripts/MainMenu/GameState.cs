using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    private static GameState instance;
    public static string LastAction { get; set; }
    private void Awake()
    {
        // Проверяем, существует ли уже экземпляр
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Не уничтожать объект при загрузке новой сцены
        }
        else
        {
            Destroy(gameObject); // Уничтожаем дубликат
        }
    }
}
