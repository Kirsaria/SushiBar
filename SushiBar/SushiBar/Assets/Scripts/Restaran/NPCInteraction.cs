using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    public int npcID;
    public Animator animator;
    private bool playerInTrigger = false;
    public Dialogue dialogue;
    public Animator animatorDialog;
    public Orders order;
    private bool isPlayerNearby = false;
    private bool isDialogueActive = false;
    private int chairIndex; // ������ �����
    public bool IsSitting = false;
    private NPCManager npcManager;
    [SerializeField] private OrderData orderData;
    private List<int> interactedNPCs;

    private void Awake()
    {
        interactedNPCs = new List<int>();
        DontDestroyOnLoad(gameObject); // ��������� ������ ����� �������
    }

    private void Start()
    {
        npcManager = FindObjectOfType<NPCManager>();
    }

    public void InteractWithNPC(int npcId)
    {
        if (!interactedNPCs.Contains(npcId))
        {
            interactedNPCs.Add(npcId);
            Debug.Log($"NPC � ID {npcId} �������� � ������ ��������������.");
            // ��������� ������ � OrderData
            Orders newOrder = new Orders { npcID = npcId, HasTaken = true, ingredients = new List<Ingredient>() };
            orderData.orders.Add(newOrder);

            // ��������� NPC � ������ �������������� � NPCManager
            if (npcManager != null)
            {
                npcManager.AddInteractedNPC(npcId);
            }
            else
            {
                Debug.LogError("NPCManager �� ������!");
            }
        }
        else
        {
            Debug.Log($"NPC � ID {npcId} ��� ���������� � ������ ��������������.");
        }
    }

    public List<int> GetInteractedNPCs()
    {
        return interactedNPCs;
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
            if (isDialogueActive)
            {
                dialogueUI.EndDialogue();
                isDialogueActive = false;
                npcManager.SitOnChair(gameObject);
            }
            else
            {
                dialogueUI.StartDialogue(dialogue);
                isDialogueActive = true;
                InteractWithNPC(npcID);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger("IsTriggered");
            playerInTrigger = true;
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger("IsTriggered");
            playerInTrigger = false;
            isPlayerNearby = false;
            if (isDialogueActive)
            {
                DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
                dialogueUI.EndDialogue();
                isDialogueActive = false;
            }
        }
    }

    public void EnableInteraction(int index)
    {
        chairIndex = index;
        isPlayerNearby = true;
    }
}
