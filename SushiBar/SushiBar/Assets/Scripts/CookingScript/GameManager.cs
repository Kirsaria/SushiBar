using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Ingridients ingridientsToPlace;
    public CustomCursor cursor;
    public Transform canvasTransform; // Ссылка на Canvas
    public OrderSceneManager orderSceneManager;
    public List<GameObject> placedIngredients; // Список размещённых ингредиентов

    // Ссылка на объект Plate
    public Transform plateTransform;

    void Start()
    {
        placedIngredients = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && ingridientsToPlace != null)
        {
            // Получаем позицию мыши относительно Canvas
            Vector2 anchoredPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform as RectTransform, Input.mousePosition, Camera.main, out anchoredPosition);

            // Проверяем, есть ли уже такой ингредиент на доске
            bool ingredientExists = false;
            foreach (GameObject placedIngredient in placedIngredients)
            {
                if (placedIngredient.name == ingridientsToPlace.prefab.name + "(Clone)")
                {
                    ingredientExists = true;
                    break;
                }
            }

            if (!ingredientExists)
            {
                // Создаём новый ингредиент внутри Canvas и сразу устанавливаем его позицию
                GameObject newIngridient = Instantiate(ingridientsToPlace.prefab, canvasTransform);
                RectTransform rectTransform = newIngridient.GetComponent<RectTransform>();
                rectTransform.SetParent(canvasTransform, false);
                rectTransform.anchoredPosition = anchoredPosition;

                // Добавляем компонент Button и назначаем обработчик события
                Button button = newIngridient.AddComponent<Button>();
                button.onClick.AddListener(() => RemoveIngredient(newIngridient));

                placedIngredients.Add(newIngridient); // Добавляем ингредиент в список размещённых
                Debug.Log("Ингредиент добавлен: " + newIngridient.name); // Выводим в консоль
            }
            else
            {
                Debug.Log("Ингредиент уже добавлен.");
            }

            ingridientsToPlace = null;
            cursor.gameObject.SetActive(false);
            Cursor.visible = true;
            CheckOrder(); // Проверяем заказ
        }

        // Проверка на нажатие правой кнопки мыши
        if (Input.GetMouseButtonDown(1) && ingridientsToPlace != null)
        {
            // Отмена выбора ингредиента
            ingridientsToPlace = null;
            cursor.gameObject.SetActive(false);
            Cursor.visible = true;
            Debug.Log("Выбор ингредиента отменён.");
        }
    }
    private void RemoveIngredient(GameObject ingredient)
    {
        placedIngredients.Remove(ingredient);
        Destroy(ingredient);
        Debug.Log("Ингредиент удалён: " + ingredient.name);
    }
    public void AddIngridient(Ingridients ingridient)
    {
            cursor.gameObject.SetActive(true);
            cursor.GetComponent<Image>().sprite = ingridient.GetComponent<Image>().sprite;
            ingridientsToPlace = ingridient;
            Cursor.visible = false;
    }
    private void CheckOrder()
    {
        if (orderSceneManager.CheckOrder(placedIngredients))
        {
            MoveIngredientsToPlate(); // Перемещаем ингредиенты на тарелку
            placedIngredients.Clear(); // Очищаем список размещённых ингредиентов
        }
    }
    private void MoveIngredientsToPlate()
    {
        foreach (GameObject ingredient in placedIngredients)
        {
            ingredient.transform.SetParent(plateTransform); // Перемещаем ингредиент на тарелку
            ingredient.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Устанавливаем позицию на тарелке
            Destroy(ingredient, 2f); // Удаляем ингредиент через 2 секунды
        }
    }
}
