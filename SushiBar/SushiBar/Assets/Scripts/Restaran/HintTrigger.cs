using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HintTrigger : MonoBehaviour
{
    public Animator animator;
    public string nextSceneName;
    private bool playerInTrigger = false;
    private NPCManager npcManager;
    public GameObject cookingCanvas;
    public GameObject cameraMain;
    public GameObject cameraCooking;
    public PlayerControler playerControler;

    private void Start()
    {
        npcManager = FindObjectOfType<NPCManager>();
        playerControler = FindObjectOfType<PlayerControler>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (animator != null)
            {
                animator.SetTrigger("IsTriggered");
            }
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (animator != null)
            {
                animator.SetTrigger("IsTriggered");
            }
            playerInTrigger = false;
        }
    }

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E) && playerControler.hasDish == false)
        {
            // Сохранение списка взаимодействовавших NPC
            if (npcManager != null)
            {
                npcManager.SaveInteractedNPCIDs();
            }
            else
            {
                Debug.LogError("NPCManager не найден!");
            }
            cameraMain.SetActive(false);
            cameraCooking.SetActive(true);
            // Переход на другую сцену
            cookingCanvas.SetActive(true);
            var orderSceneManager = FindObjectOfType<OrderSceneManager>();
            if (orderSceneManager != null)
            {
                orderSceneManager.DisplayOrders(); // Убедитесь, что метод доступен
            }
            else
            {
                Debug.LogError("OrderSceneManager не найден!");
            }
        }
    }
}
