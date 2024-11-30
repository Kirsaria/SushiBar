using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public static AnimatorManager Instance { get; private set; }
    public Animator npcAnimator;
    public Animator dialogAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetNPCAnimator(Animator animator)
    {
        npcAnimator = animator;
    }

    public void SetDialogAnimator(Animator animator)
    {
        dialogAnimator = animator;
    }
}
