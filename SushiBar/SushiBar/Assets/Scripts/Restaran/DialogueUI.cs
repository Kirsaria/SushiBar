using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;
    private Coroutine typingCoroutine;
    private bool isDialogueActive = false; // Переменная для отслеживания состояния диалога

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true; // Устанавливаем состояние диалога в активное
        dialoguePanel.SetActive(true);
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeSentence(string.Join(" ", dialogue.sentences)));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.05f); // Задержка между буквами
        }
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        dialoguePanel.SetActive(false);
        isDialogueActive = false; // Устанавливаем состояние диалога в неактивное
    }

    // Метод для проверки состояния диалога
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}