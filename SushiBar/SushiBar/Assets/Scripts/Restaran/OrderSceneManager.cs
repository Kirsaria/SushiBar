using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System.Data;

public class OrderSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject orderTextPrefab;
    [SerializeField] private Transform[] orderPoints;
    private string dbPath;
    private List<Orders> orders;
    private List<int> interactedNPCIDs;

    private void Start()
    {
        dbPath = "URI=file:Orders.db";
        LoadInteractedNPCIDs();
        LoadOrdersFromDatabase();
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
                    SELECT Orders.OrderID, Orders.NPCID, OrderItem.IngredientName, OrderItem.PrefabName
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

                        if (!ordersDict.ContainsKey(orderId))
                        {
                            ordersDict[orderId] = new Orders
                            {
                                ingredients = new List<Ingredient>(),
                                npcID = npcId,
                                HasTaken = true // Предположим, что все заказы взяты
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
                Debug.Log($"Проверка заказа: {order.npcID}, HasTaken: {order.HasTaken}");
                if (order.HasTaken && interactedNPCIDs.Contains(order.npcID))
                {
                    Debug.Log($"Заказ найден: {order.npcID}");
                    string orderText = GetIngridientsStr(order.ingredients);
                    Transform orderPoint = orderPoints[i];
                    GameObject orderTextObject = Instantiate(orderTextPrefab, orderPoint.position, Quaternion.identity, orderPoint);

                    // Найдите компонент Text внутри префаба
                    Text textComponent = orderTextObject.GetComponentInChildren<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = orderText;
                    }
                    else
                    {
                        Debug.LogError("Text компонент не найден в префабе!");
                    }
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
                // Если заказов меньше, чем точек, оставляем точку пустой
                Transform orderPoint = orderPoints[i];
                foreach (Transform child in orderPoint)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}
