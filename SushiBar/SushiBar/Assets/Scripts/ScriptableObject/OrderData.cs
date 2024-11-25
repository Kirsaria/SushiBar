using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "OrderData", menuName = "ScriptableObjects/OrderData", order = 1)]
public class OrderData : ScriptableObject
{
    public List<Orders> orders = new List<Orders>();
    public Dictionary<int, GameObject> npcDictionary = new Dictionary<int, GameObject>();
}