using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public float speed;
    public Animator animator;
    private Vector2 direction;
    private Rigidbody2D rb;
    public bool hasDish = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Speed", direction.sqrMagnitude);
        if(hasDish && speed > 0.1)
        {
            animator.SetBool("HasDish", true);
        }
        else
        {
            animator.SetBool("HasDish", false);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }
    public void SetHasDish(bool value)
    {
        hasDish = value;
    }
}
