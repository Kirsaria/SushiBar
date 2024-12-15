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
    public Transform canvasTransform; 
    public OrderSceneManager orderSceneManager;
    public List<GameObject> placedIngredients; 
    public Transform plateTransform;

    void Start()
    {
        placedIngredients = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && ingridientsToPlace != null)
        {
            Vector2 anchoredPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform as RectTransform, Input.mousePosition, Camera.main, out anchoredPosition);
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
                GameObject newIngridient = Instantiate(ingridientsToPlace.prefab, canvasTransform);
                RectTransform rectTransform = newIngridient.GetComponent<RectTransform>();
                rectTransform.SetParent(canvasTransform, false);
                rectTransform.anchoredPosition = anchoredPosition;
                Button button = newIngridient.AddComponent<Button>();
                button.onClick.AddListener(() => RemoveIngredient(newIngridient));

                placedIngredients.Add(newIngridient);
                Debug.Log("Ингредиент добавлен: " + newIngridient.name); 
            }
            else
            {
                Debug.Log("Ингредиент уже добавлен.");
            }

            ingridientsToPlace = null;
            cursor.gameObject.SetActive(false);
            Cursor.visible = true;
            CheckOrder(); 
        }

        if (Input.GetMouseButtonDown(1) && ingridientsToPlace != null)
        {
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
            MoveIngredientsToPlate();
            placedIngredients.Clear();
        }
    }
    private void MoveIngredientsToPlate()
    {
        foreach (GameObject ingredient in placedIngredients)
        {
            ingredient.transform.SetParent(plateTransform);
            ingredient.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; 
            Destroy(ingredient, 2f); 
        }
    }
}
