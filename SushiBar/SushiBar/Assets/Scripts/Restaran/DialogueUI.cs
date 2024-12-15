using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    public TMP_Text dialogueText;
    public GameObject dialoguePanel;
    private Coroutine typingCoroutine;
    private bool isDialogueActive = false; 

    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true; 
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
            yield return new WaitForSeconds(0.05f); 
        }
    }

    public void EndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        dialoguePanel.SetActive(false);
        isDialogueActive = false; 
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}