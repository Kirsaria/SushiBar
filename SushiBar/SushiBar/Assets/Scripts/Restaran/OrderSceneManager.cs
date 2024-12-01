using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;
using UnityEngine.SceneManagement;

public class OrderSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject orderTextPrefab;
    [SerializeField] private Transform[] orderPoints;
    private string dbPath;
    private List<Orders> orders;
    private List<int> interactedNPCIDs;
    private Dictionary<int, GameObject> orderObjects; // Словарь для хранения объектов заказов
    public GameObject cookingCanvas;
    public GameObject cameraMain;
    public GameObject cameraCooking;

    private void Start()
    {
        dbPath = "URI=file:Orders.db";
        LoadInteractedNPCIDs();
        LoadOrdersFromDatabase();
        orderObjects = new Dictionary<int, GameObject>();
        DisplayOrders();
    }

    private void LoadInteractedNPCIDs()
    {
        interactedNPCIDs = new List<int>();
        string ids = PlayerPrefs.GetString("InteractedNPCIDs", "");
        if (!string.IsNullOrEmpty(ids))
        {
            string[] idArray = ids.Split(',');
            foreach (string id in idArray)
            {
                if (int.TryParse(id, out int npcId))
                {
                    interactedNPCIDs.Add(npcId);
                }
            }
        }
        Debug.Log($"Загружено взаимодействующих NPC: {string.Join(",", interactedNPCIDs)}");
    }

    private void LoadOrdersFromDatabase()
    {
        orders = new List<Orders>();

        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
            SELECT Orders.OrderID, Orders.NPCID, OrderItem.IngredientName, OrderItem.PrefabName, Orders.IsCompleted, Orders.IsCookingCompleted
            FROM Orders
            JOIN OrderItem ON Orders.OrderID = OrderItem.OrderID
        ";

                using (var reader = command.ExecuteReader())
                {
                    Dictionary<int, Orders> ordersDict = new Dictionary<int, Orders>();

                    while (reader.Read())
                    {
                        int orderId = reader.GetInt32(0);
                        int npcId = reader.GetInt32(1);
                        string ingredientName = reader.GetString(2);
                        string prefabName = reader.GetString(3);
                        bool isCompleted = reader.GetInt32(4) == 1;
                        bool isCookingCompleted = reader.GetInt32(5) == 1;

                        if (!ordersDict.ContainsKey(orderId))
                        {
                            ordersDict[orderId] = new Orders
                            {
                                OrderID = orderId,
                                npcID = npcId,
                                IsCompleted = isCompleted,
                                IsCookingCompleted = isCookingCompleted,
                                ingredients = new List<Ingredient>()
                            };
                        }

                        // Загрузка префаба из папки Resources
                        GameObject prefab = Resources.Load<GameObject>($"Prefabs/Ingridients Prefabs/{prefabName}");
                        if (prefab != null)
                        {
                            ordersDict[orderId].ingredients.Add(new Ingredient
                            {
                                name = ingredientName,
                                prefab = prefab
                            });
                        }
                        else
                        {
                            Debug.LogError($"Prefab with name {prefabName} not found in Resources/Prefabs/Ingridients Prefabs folder!");
                        }
                    }

                    orders = new List<Orders>(ordersDict.Values);
                    Debug.Log($"Загружено заказов: {orders.Count}");
                }
            }
        }
    }

    public string GetIngridientsStr(List<Ingredient> ingredients)
    {
        string strIngridients = "";
        for (int i = 0; i < ingredients.Count; i++)
        {
            strIngridients += ingredients[i].name;
            if (i != ingredients.Count - 1)
            {
                strIngridients += ", ";
            }
        }
        return strIngridients;
    }

    private void DisplayOrders()
    {
        Debug.Log("DisplayOrders вызван");
        int orderIndex = 0;
        for (int i = 0; i < orderPoints.Length; i++)
        {
            bool orderDisplayed = false;
            while (orderIndex < orders.Count && !orderDisplayed)
            {
                var order = orders[orderIndex];
                Debug.Log($"Проверка заказа: {order.npcID}, IsCompleted: {order.IsCompleted}");
                if (order.IsCompleted && interactedNPCIDs.Contains(order.npcID))
                {
                    Debug.Log($"Заказ найден: {order.npcID}");
                    string orderText = GetIngridientsStr(order.ingredients);
                    Transform orderPoint = orderPoints[i];
                    GameObject orderTextObject = Instantiate(orderTextPrefab, orderPoint.position, Quaternion.identity, orderPoint);

                    Text textComponent = orderTextObject.GetComponentInChildren<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = orderText;
                    }
                    else
                    {
                        Debug.LogError("Text компонент не найден в префабе!");
                    }

                    orderObjects[order.npcID] = orderTextObject;
                    orderDisplayed = true;
                }
                else
                {
                    Debug.Log($"Заказ не найден: {order.npcID}");
                }
                orderIndex++;
            }

            if (!orderDisplayed)
            {
                Transform orderPoint = orderPoints[i];
                foreach (Transform child in orderPoint)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public bool CheckOrder(List<GameObject> placedIngredients)
    {
        foreach (var order in orders)
        {
            if (order.IsCompleted && interactedNPCIDs.Contains(order.npcID))
            {
                bool orderComplete = true;

                foreach (Ingredient ingredient in order.ingredients)
                {
                    bool ingredientFound = false;
                    foreach (GameObject placedIngredient in placedIngredients)
                    {
                        string placedIngredientName = placedIngredient.name.Replace("(Clone)", "").Trim();
                        if (ingredient.prefab.name == placedIngredientName)
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
                    Debug.Log("Заказ выполнен!");
                    RemoveOrderFromScene(order.npcID);
                    order.IsCompleted = true;
                    SaveOrderState(order.OrderID); // Сохраняем состояние заказа
                    cameraCooking.SetActive(false);
                    cameraMain.SetActive(true);
                    cookingCanvas.SetActive(false); // Переход на сцену ресторана
                    return true;
                }
                else
                {
                    Debug.Log("Заказ не выполнен. Не все ингредиенты найдены.");
                }
            }
        }
        return false;
    }

    private void RemoveOrderFromScene(int npcID)
    {
        if (orderObjects.TryGetValue(npcID, out GameObject orderObject))
        {
            Destroy(orderObject);
            orderObjects.Remove(npcID); // Удаляем ссылку из словаря
        }
    }

    private void SaveOrderState(int OrderID)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "UPDATE Orders SET IsCookingCompleted = 1 WHERE OrderID = @orderID";
                command.Parameters.AddWithValue("@orderID", OrderID);
                command.ExecuteNonQuery();
            }
        }
    }
}
