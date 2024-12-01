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
    public int OrderID;
    public List<Ingredient> ingredients;
    public bool HasTaken = false;
    public bool IsCookingCompleted = false;
    public int npcID;
}

public class OrderManager : MonoBehaviour
{
    private string dbPath;
    public List<Orders> availableOrders;
    private List<int> interactedNPCs; // Новый список для хранения NPC
    public void SaveOrders()
    {
        Debug.Log("Заказы сохранены.");
    }
    private void Start()
    {
        dbPath = "URI=file:Orders.db";
        LoadOrdersFromDatabase();
        interactedNPCs = new List<int>(); // Инициализация списка
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
                                HasTaken = isCompleted,
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

                    availableOrders = new List<Orders>(ordersDict.Values);
                    Debug.Log($"Загружено заказов: {availableOrders.Count}");
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
            sentences[i + 1] = $"{order.ingredients[i].name}";
        }

        return new Dialogue { sentences = sentences };
    }

}
