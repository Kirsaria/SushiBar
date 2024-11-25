using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order 
{
    public List<GameObject> requiredIngredients;

    public Order(List<GameObject> ingredients)
    {
        requiredIngredients = ingredients;
    }
}
