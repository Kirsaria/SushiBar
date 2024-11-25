using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Ingridients ingridientsToPlace;
    public CustomCursor cursor;
    public Transform canvasTransform; // ������ �� Canvas
    public List<Order> orders; // ������ �������
    public List<GameObject> placedIngredients; // ������ ����������� ������������

    // ������ �� ������� ������������
    public GameObject Rice1Prefab;
    public GameObject Cheese1Prefab;
    public GameObject Fish1Prefab;

    // ������ �� ������ Plate
    public Transform plateTransform;

    void Start()
    {
        orders = new List<Order>();
        placedIngredients = new List<GameObject>();
        // ������ ���������� ������
        orders.Add(new Order(new List<GameObject> { Rice1Prefab, Cheese1Prefab, Fish1Prefab }));
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && ingridientsToPlace != null)
        {
            // �������� ������� ���� ������������ Canvas
            Vector2 anchoredPosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasTransform as RectTransform, Input.mousePosition, Camera.main, out anchoredPosition);

            // ���������, ���� �� ��� ����� ���������� �� �����
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
                // ������ ����� ���������� ������ Canvas � ����� ������������� ��� �������
                GameObject newIngridient = Instantiate(ingridientsToPlace.prefab, canvasTransform);
                RectTransform rectTransform = newIngridient.GetComponent<RectTransform>();
                rectTransform.SetParent(canvasTransform, false);
                rectTransform.anchoredPosition = anchoredPosition;

                // ��������� ��������� Button � ��������� ���������� �������
                Button button = newIngridient.AddComponent<Button>();
                button.onClick.AddListener(() => RemoveIngredient(newIngridient));

                placedIngredients.Add(newIngridient); // ��������� ���������� � ������ �����������
                Debug.Log("���������� ��������: " + newIngridient.name); // ������� � �������
            }
            else
            {
                Debug.Log("���������� ��� ��������.");
            }

            ingridientsToPlace = null;
            cursor.gameObject.SetActive(false);
            Cursor.visible = true;
            CheckOrder(); // ��������� �����
        }

        // �������� �� ������� ������ ������ ����
        if (Input.GetMouseButtonDown(1) && ingridientsToPlace != null)
        {
            // ������ ������ �����������
            ingridientsToPlace = null;
            cursor.gameObject.SetActive(false);
            Cursor.visible = true;
            Debug.Log("����� ����������� ������.");
        }
    }
    private void RemoveIngredient(GameObject ingredient)
    {
        placedIngredients.Remove(ingredient);
        Destroy(ingredient);
        Debug.Log("���������� �����: " + ingredient.name);
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
        if (orders.Count > 0)
        {
            Order currentOrder = orders[0];
            bool orderComplete = true;

            foreach (GameObject ingredient in currentOrder.requiredIngredients)
            {
                bool ingredientFound = false;
                foreach (GameObject placedIngredient in placedIngredients)
                {
                    // ������� ������� "(Clone)" �� �����
                    string placedIngredientName = placedIngredient.name.Replace("(Clone)", "").Trim();
                    if (ingredient.name == placedIngredientName)
                    {
                        ingredientFound = true;
                        break;
                    }
                }
                if (!ingredientFound)
                {
                    orderComplete = false;
                    break;
                }
            }

            if (orderComplete)
            {
                Debug.Log("����� ��������!");
                MoveIngredientsToPlate(); // ���������� ����������� �� �������
                orders.RemoveAt(0); // ������� ����������� �����
                placedIngredients.Clear(); // ������� ������ ����������� ������������
            }
            else
            {
                Debug.Log("����� �� ��������. �� ��� ����������� �� �����.");
            }
        }
    }

    private void MoveIngredientsToPlate()
    {
        foreach (GameObject ingredient in placedIngredients)
        {
            ingredient.transform.SetParent(plateTransform); // ���������� ���������� �� �������
            ingredient.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // ������������� ������� �� �������
            Destroy(ingredient, 2f); // ������� ���������� ����� 2 �������
        }
    }
}
