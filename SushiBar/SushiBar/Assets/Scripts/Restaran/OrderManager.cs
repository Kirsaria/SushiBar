using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class Dialogue
{
    public string[] sentences;
}

[System.Serializable]
public class Ingredient
{
    public string name;
    public GameObject prefab;
}

[System.Serializable]
public class Orders
{
    public List<Ingredient> ingredients;
    public bool HasTaken = false;
    public int npcID;
}

public class OrderManager : MonoBehaviour
{
    private string dbPath;
    public List<Orders> availableOrders;
    private List<int> interactedNPCs; // Новый список для хранения NPC
    public void SaveOrders()
    {
        // Логика сохранения заказов
        Debug.Log("Заказы сохранены.");
    }
    private void Start()
    {
        dbPath = "URI=file:Orders.db";
        LoadOrdersFromDatabase();
        interactedNPCs = new List<int>(); // Инициализация списка
    }

    public void InteractWithNPC(int npcId)
    {
        if (!interactedNPCs.Contains(npcId))
        {
            interactedNPCs.Add(npcId); // Добавляем NPC в список при взаимодействии
        }
    }

    public List<Orders> GetOrdersForInteractedNPCs()
    {
        List<Orders> filteredOrders = new List<Orders>();

        foreach (var order in availableOrders)
        {
            if (interactedNPCs.Contains(order.npcID))
            {
                filteredOrders.Add(order);
            }
        }

        return filteredOrders;
    }

    private void LoadOrdersFromDatabase()
    {
        availableOrders = new List<Orders>();

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
                                npcID = npcId
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

                    availableOrders = new List<Orders>(ordersDict.Values);
                }
            }
        }
    }
    public Orders GetOrderForNPC(int npcId)
    {
        foreach (var order in availableOrders)
        {
            if (order.npcID == npcId)
            {
                return order;
            }
        }
        return null;
    }

    public Dialogue CreateOrderDialogue(Orders order)
    {
        string[] sentences = new string[order.ingredients.Count + 1];
        sentences[0] = $"Привет! Я хочу заказать:";

        for (int i = 0; i < order.ingredients.Count; i++)
        {
            sentences[i + 1] = $"- {order.ingredients[i].name}";
        }

        return new Dialogue { sentences = sentences };
    }
}
