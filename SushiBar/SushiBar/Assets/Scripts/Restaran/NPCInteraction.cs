using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    private int chairIndex; 
    public bool IsSitting = false;
    private NPCManager npcManager;
    private List<int> interactedNPCs;
    public bool isWaitingForOrder = true; 
    public bool isOrderComplete = false;
    public bool hasCompletedDialogue = false;
    public int rewardPoint;
    private Text scoreText;
    public PlayerControler playerControler;
    private UserDataSaver userDataSaver;

    private void Awake()
    {
        interactedNPCs = new List<int>();
        animator = GetComponent<Animator>();
        AnimatorManager.Instance.SetNPCAnimator(animator);
        AnimatorManager.Instance.SetDialogAnimator(animatorDialog);
    }
    void LoadData()
    {
        userDataSaver = FindObjectOfType<UserDataSaver>(); 

        if (userDataSaver != null)
        {
            PlayerStats.Instance.SetTotalTears(userDataSaver.totalTears);
            UpdateScoreText(); 
        }
        else
        {
            Debug.LogError("UserDataSaver не найден!");
        }
    }
    private void Start()
    {
        npcManager = FindObjectOfType<NPCManager>();
        playerControler = FindObjectOfType<PlayerControler>();
        GameObject scoreTextObject = GameObject.FindGameObjectWithTag("ScoreText");
        if (scoreTextObject != null)
        {
            scoreText = scoreTextObject.GetComponent<Text>(); 
        }
        else
        {
            Debug.LogError("Текстовое поле для очков не найдено по тегу 'ScoreText'!");
        }
        if (animator == null)
        {
            animator = AnimatorManager.Instance.npcAnimator;
        }
        if (animatorDialog == null)
        {
            animatorDialog = AnimatorManager.Instance.dialogAnimator; 
        }
        LoadData();
    }

    public void InteractWithNPC(int npcId)
    {
        DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();

        if (!interactedNPCs.Contains(npcId))
        {
            interactedNPCs.Add(npcId);
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
                connection.Close();
            }
            Orders newOrder = new Orders { npcID = npcId, HasTaken = true, ingredients = new List<Ingredient>() };
            NPCManager.Instance.orders.Add(newOrder);
            if (npcManager != null)
            {
                npcManager.AddInteractedNPC(npcId);
            }
            else
            {
                Debug.LogError("NPCManager не найден!");
            }
        }
        else if (isOrderComplete && !isWaitingForOrder) 
        {
            dialogue = new Dialogue
            {
                sentences = new string[] { "Спасибо за выполненный заказ!" }
            };

            PlayerStats.Instance.AddPoints(2);
            UpdateScoreText();
            hasCompletedDialogue = true; 
            playerControler.SetHasDish(false);   
            dialogueUI.StartDialogue(dialogue);
            StartCoroutine(WaitForDialogueEnd(dialogueUI));
        }
        else if (isWaitingForOrder) 
        {
            dialogue = new Dialogue
            {
                sentences = new string[] { "Я жду свой заказ." }
            };
        }
        else
        {
            Debug.Log($"NPC с ID {npcId} уже существует в списке взаимодействий.");
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            DialogueUI dialogueUI = FindObjectOfType<DialogueUI>();
            if (dialogueUI == null)
            {
                Debug.LogError("DialogueUI не найден!");
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
                InteractWithNPC(npcID);
                dialogueUI.StartDialogue(dialogue);
                isDialogueActive = true;
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
    private IEnumerator WaitForDialogueEnd(DialogueUI dialogueUI)
    {
        yield return new WaitUntil(() => !dialogueUI.IsDialogueActive()); 
        npcManager.MoveAndDestroyNPC(gameObject); 
    }
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "" + PlayerStats.Instance.GetPoints();
        }
        else
        {
            Debug.LogError("Текстовое поле для очков не установлено!");
        }
    }
}
