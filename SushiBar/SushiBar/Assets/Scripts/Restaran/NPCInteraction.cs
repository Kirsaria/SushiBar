using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteraction : MonoBehaviour
{
    public int npcID;
    public Animator animator;
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
        animator = GetComponent<Animator>();
        AnimatorManager.Instance.SetNPCAnimator(animator);
        AnimatorManager.Instance.SetDialogAnimator(animatorDialog);
    }

    private void Start()
    {
        npcManager = FindObjectOfType<NPCManager>();
        if (animator == null)
        {
            animator = AnimatorManager.Instance.npcAnimator; // ����������� ��������, ���� �� �� ��� ����������
        }
        if (animatorDialog == null)
        {
            animatorDialog = AnimatorManager.Instance.dialogAnimator; // ����������� �������� �������, ���� �� �� ��� ����������
        }
    }

    public void InteractWithNPC(int npcId)
    {
        if (!interactedNPCs.Contains(npcId))
        {
            interactedNPCs.Add(npcId);
            Debug.Log($"NPC � ID {npcId} �������� � ������ ��������������.");

            // ��������� ������ ������ � ���� ������
            string conn = "URI=file:Orders.db";
            using (var connection = new SqliteConnection(conn))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Orders SET IsCompleted = 1 WHERE NPCID = @npcId";
                    command.Parameters.Add(new SqliteParameter("@npcId", npcId));
                    command.ExecuteNonQuery();
                }
            }

            // ��������� ������ � OrderData
            Orders newOrder = new Orders { npcID = npcId, IsCompleted = true, ingredients = new List<Ingredient>() };
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

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
            if (dialogueUI == null)
            {
                Debug.LogError("DialogueUI �� ������!");
                return;
            }

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
            isPlayerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger("IsTriggered");
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
