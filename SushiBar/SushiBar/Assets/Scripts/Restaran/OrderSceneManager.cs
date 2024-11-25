using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderSceneManager : MonoBehaviour
{
    [SerializeField] private OrderData orderData;
    [SerializeField] private GameObject orderTextPrefab;
    [SerializeField] private Transform[] orderPoints;
    private NPCManager npcManager;

    private void Start()
    {
        npcManager = FindObjectOfType<NPCManager>();
        Debug.Log($"���������� �������: {orderData.orders.Count}");
        foreach (var order in orderData.orders)
        {
            Debug.Log($"�����: NPC ID = {order.npcID}, HasTaken = {order.HasTaken}");
        }
        DisplayOrders();
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
        Debug.Log("DisplayOrders ������");
        int orderIndex = 0;
        for (int i = 0; i < orderPoints.Length; i++)
        {
            if (orderIndex < orderData.orders.Count)
            {
                var order = orderData.orders[orderIndex];
                Debug.Log($"�������� ������: {order.npcID}");
                if (order.HasTaken && orderData.npcDictionary.ContainsKey(order.npcID))
                {
                    Debug.Log($"����� ������: {order.npcID}");
                    string orderText = GetIngridientsStr(order.ingredients);
                    Transform orderPoint = orderPoints[i];
                    GameObject orderTextObject = Instantiate(orderTextPrefab, orderPoint.position, Quaternion.identity, orderPoint);

                    // ������� ��������� Text ������ �������
                    Text textComponent = orderTextObject.GetComponentInChildren<Text>();
                    if (textComponent != null)
                    {
                        textComponent.text = orderText;
                    }
                    else
                    {
                        Debug.LogError("Text ��������� �� ������ � �������!");
                    }
                    orderIndex++;
                }
                else
                {
                    Debug.Log($"����� �� ������: {order.npcID}");
                }
            }
            else
            {
                // ���� ������� ������, ��� �����, ��������� ����� ������
                Transform orderPoint = orderPoints[i];
                foreach (Transform child in orderPoint)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }
}