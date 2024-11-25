using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private List<Ingredient> riceIngredients;
    [SerializeField] private List<Ingredient> fishIngredients;
    [SerializeField] private List<Ingredient> cheeseIngredients;
    [SerializeField] private List<Ingredient> sousIngredients;
    [SerializeField] public Orders[] orders;
    public List<Orders> availableOrders;
    [SerializeField] private Dialogue[] dialogues;
    private void Start()
    {
        Orders order1 = new Orders
        {
            ingredients = new List<Ingredient> { riceIngredients[0], fishIngredients[0], cheeseIngredients[0] }
        };

        Orders order2 = new Orders
        {
            ingredients = new List<Ingredient> { riceIngredients[1], sousIngredients[0], cheeseIngredients[1] }
        };

        Orders order3 = new Orders
        {
            ingredients = new List<Ingredient> { fishIngredients[1], riceIngredients[2], sousIngredients[1] }
        };

        Orders order4 = new Orders
        {
            ingredients = new List<Ingredient> { fishIngredients[2], riceIngredients[2], sousIngredients[1], cheeseIngredients[1] }
        };

        orders = new Orders[] { order1, order2, order3, order4 };
        availableOrders = new List<Orders>(orders);
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
