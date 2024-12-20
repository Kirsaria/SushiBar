using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAnimator : MonoBehaviour
{
    public Animator startAnim;
    public DialogueManager dm;

    public void OnTriggerExit2D(Collider2D collision)
    {
        startAnim.SetBool("startOpen", false);
        dm.EndDialogue();
    }
}
