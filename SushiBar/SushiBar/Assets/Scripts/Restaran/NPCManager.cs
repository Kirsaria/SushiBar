using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using Mono.Data.Sqlite;

public class NPCManager : MonoBehaviour
{
    [SerializeField] private GameObject[] npcPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] targetPoints;
    [SerializeField] private Transform[] chairPositions;
    private List<GameObject> spawnedNPCs = new List<GameObject>();
    private bool[] occupiedTargetPoints;
    private bool[] occupiedChairPoints;
    private OrderManager orderManager;
    public Dictionary<int, GameObject> npcDictionary = new Dictionary<int, GameObject>(); // Словарь для хранения NPC по ID
    public OrderData orderData;
    private List<int> interactedNPCIDs = new List<int>(); // Список ID взаимодействовавших NPC
    public void SaveInteractedNPCIDs()
    {
        string ids = string.Join(",", interactedNPCIDs);
        PlayerPrefs.SetString("InteractedNPCIDs", ids);
        PlayerPrefs.Save();
        Debug.Log($"Сохранено взаимодействующих NPC: {ids}");

        // Сохранение заказов
        if (orderManager != null)
        {
            orderManager.SaveOrders();
        }
        else
        {
            Debug.LogError("OrderManager не найден!");
        }
    }

    public void AddInteractedNPC(int npcID)
    {
        if (!interactedNPCIDs.Contains(npcID))
        {
            interactedNPCIDs.Add(npcID);
            Debug.Log($"NPC с ID {npcID} добавлен в список interactedNPCIDs.");
        }
        else
        {
            Debug.Log($"NPC с ID {npcID} уже существует в списке interactedNPCIDs.");
        }
    }

    private void Start()
    {
        occupiedTargetPoints = new bool[targetPoints.Length];
        occupiedChairPoints = new bool[chairPositions.Length];
        orderManager = FindObjectOfType<OrderManager>();
        LoadInteractedNPCs();
        SpawnNPC();
    }

    private void LoadInteractedNPCs()
    {
        string conn = "URI=file:Orders.db";
        using (var connection = new SqliteConnection(conn))
        {
            connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "SELECT NPCID FROM Orders WHERE IsCookingCompleted = 0";
                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int npcId = reader.GetInt32(0);
                        interactedNPCIDs.Add(npcId);
                    }
                }
            }
        }
    }


    public void SpawnNPC()
    {
        if (AllPointsOccupied() || interactedNPCIDs.Count == 0)
        {
            return;
        }

        int spawnIndex = Random.Range(0, spawnPoints.Length);
        int npcId = interactedNPCIDs[0]; // Берем первый ID из списка
        interactedNPCIDs.RemoveAt(0); // Удаляем его из списка после использования

        GameObject npc = Instantiate(npcPrefabs[npcId % npcPrefabs.Length], spawnPoints[spawnIndex].position, Quaternion.identity);
        npc.GetComponent<NPCInteraction>().animator = GameObject.FindGameObjectWithTag("HintTag").GetComponent<Animator>();
        npc.GetComponent<NPCInteraction>().npcID = npcId; // Присваиваем NPCID из базы данных
        npcDictionary.Add(npcId, npc); // Добавляем NPC в словарь
        orderData.npcDictionary.Add(npcId, npc);
        AssignOrderToNPC(npc);
        spawnedNPCs.Add(npc);

        for (int i = 0; i < targetPoints.Length; i++)
        {
            if (!occupiedTargetPoints[i])
            {
                occupiedTargetPoints[i] = true;
                StartCoroutine(MoveNPCToPosition(npc, targetPoints[i].position, i));
                break;
            }
        }

        Invoke("SpawnNPC", 2f);
    }

    private void AssignOrderToNPC(GameObject npc)
    {
        int npcId = npc.GetComponent<NPCInteraction>().npcID;
        Orders order = orderManager.GetOrderForNPC(npcId); // Получаем заказ для конкретного NPC
        npc.GetComponent<NPCInteraction>().order = order; // Присваиваем заказ NPC
        npc.GetComponent<NPCInteraction>().dialogue = orderManager.CreateOrderDialogue(order);
        orderData.orders.Add(order); // Добавляем заказ в OrderData
    }

    private IEnumerator MoveNPCToPosition(GameObject npc, Vector3 targetPosition, int targetIndex)
    {
        Animator npcAnimator = npc.GetComponent<Animator>();

        while (Vector3.Distance(npc.transform.position, targetPosition) > 0.1f)
        {
            Vector3 direction = (targetPosition - npc.transform.position).normalized;
            float speed = 2.0f;

            npcAnimator.SetFloat("Speed", speed);
            npcAnimator.SetFloat("Horizontal", direction.x);
            npcAnimator.SetFloat("Vertical", direction.y);

            npc.transform.position = Vector3.MoveTowards(npc.transform.position, targetPosition, Time.deltaTime * speed);
            yield return null;
        }

        npcAnimator.SetFloat("Speed", 0);
        occupiedTargetPoints[targetIndex] = true; // Отмечаем точку как занятую
    }

    private bool AllPointsOccupied()
    {
        foreach (bool occupied in occupiedTargetPoints)
        {
            if (!occupied)
            {
                return false;
            }
        }
        return true;
    }

    public void SitOnChair(GameObject npc)
    {
        NPCInteraction npcInteraction = npc.GetComponent<NPCInteraction>();
        if (npcInteraction.IsSitting)
        {
            return;
        }

        for (int i = 0; i < chairPositions.Length; i++)
        {
            if (!occupiedChairPoints[i])
            {
                occupiedChairPoints[i] = true;
                StartCoroutine(SitRoutine(npc, chairPositions[i].position, i));
                break;
            }
        }
    }

    private IEnumerator SitRoutine(GameObject npc, Vector3 chairPosition, int chairIndex)
    {
        NPCInteraction npcInteraction = npc.GetComponent<NPCInteraction>();
        npcInteraction.IsSitting = true;
        Animator npcAnimator = npc.GetComponent<Animator>();
        Collider2D npcCollider = npc.GetComponent<Collider2D>();
        npcCollider.enabled = false;

        while (Vector3.Distance(npc.transform.position, chairPosition) > 0.1f)
        {
            Vector3 direction = (chairPosition - npc.transform.position).normalized;
            float speed = 2.0f;

            npcAnimator.SetFloat("Speed", speed);
            npcAnimator.SetFloat("Horizontal", direction.x);
            npcAnimator.SetFloat("Vertical", direction.y);

            npc.transform.position = Vector3.MoveTowards(npc.transform.position, chairPosition, Time.deltaTime * speed);
            yield return null;
        }

        float sitDirection = chairPosition.x < npc.transform.position.x ? 0 : 1;
        npcAnimator.SetFloat("SitDirection", sitDirection);
        npcAnimator.SetTrigger("Sit");

        // Выключаем коллайдер NPC
        npcCollider.enabled = false;
        UpdateDialogueToWaiting(npc);
        occupiedChairPoints[chairIndex] = true;
    }

    private void UpdateDialogueToWaiting(GameObject npc)
    {
        npc.GetComponent<NPCInteraction>().dialogue = new Dialogue
        {
            sentences = new string[] { "Я жду свой заказ." }
        };
    }
}
