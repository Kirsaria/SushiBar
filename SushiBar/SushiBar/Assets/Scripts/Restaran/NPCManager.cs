using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private int npcCounter = 0;
    public OrderData orderData;
    private void Start()
    {
        occupiedTargetPoints = new bool[targetPoints.Length];
        occupiedChairPoints = new bool[chairPositions.Length];
        orderManager = FindObjectOfType<OrderManager>();
        SpawnNPC();
    }

    public void SpawnNPC()
    {
        if (AllPointsOccupied())
        {
            return;
        }

        int spawnIndex = Random.Range(0, spawnPoints.Length);
        int npcIndex = spawnedNPCs.Count % npcPrefabs.Length;
        GameObject npc = Instantiate(npcPrefabs[npcIndex], spawnPoints[spawnIndex].position, Quaternion.identity);
        npc.GetComponent<NPCInteraction>().animator = GameObject.FindGameObjectWithTag("HintTag").GetComponent<Animator>();
        npc.GetComponent<NPCInteraction>().npcID = npcCounter; // Присваиваем уникальный идентификатор
        npcDictionary.Add(npcCounter, npc); // Добавляем NPC в словарь
        orderData.npcDictionary.Add(npcCounter, npc);
        npcCounter++; // Увеличиваем счетчик для следующего NPC
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
        int orderIndex = Random.Range(0, orderManager.availableOrders.Count);
        Orders order = orderManager.availableOrders[orderIndex];
        orderManager.availableOrders.RemoveAt(orderIndex);
        order.npcID = npc.GetComponent<NPCInteraction>().npcID; // Присваиваем npcID заказу
        npc.GetComponent<NPCInteraction>().order = order; // Присваиваем заказ NPC
        npc.GetComponent<NPCInteraction>().dialogue = orderManager.CreateOrderDialogue(order);
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
