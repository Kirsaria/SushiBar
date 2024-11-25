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
    private int chairIndex; // Индекс стула
    public bool IsSitting = false;
    private NPCManager npcManager;
    [SerializeField] private OrderData orderData;

    public void AddInteractedNPC(int npcID, GameObject npc)
    {
        npcManager = FindObjectOfType<NPCManager>();
        if (!npcManager.npcDictionary.ContainsKey(npcID))
        {
            npcManager.npcDictionary.Add(npcID, npc);
            // Обновляем данные в OrderData
            Orders newOrder = new Orders { npcID = npcID, HasTaken = true, ingredients = new List<Ingredient>() };
            orderData.orders.Add(newOrder);
        }
    }


    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
            NPCManager npcManager = FindObjectOfType<NPCManager>();
            OrderSceneManager orderSceneManager = FindObjectOfType<OrderSceneManager>();
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
                AddInteractedNPC(npcID, gameObject);
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
