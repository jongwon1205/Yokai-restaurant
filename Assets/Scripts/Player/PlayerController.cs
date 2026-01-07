using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float interactRadius = 0.6f;

    private Rigidbody2D rigid;
    private Vector2 inputVector;
    private PlayerCarry carry;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        carry = GetComponent<PlayerCarry>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");
        inputVector = inputVector.normalized;

        if (animator != null)
        {
            animator.SetBool("isWalking", inputVector != Vector2.zero);
        }

        if (spriteRenderer != null)
        {
            if (inputVector.x < -0.01f) spriteRenderer.flipX = false;   // ¿ÞÂÊ
            else if (inputVector.x > 0.01f) spriteRenderer.flipX = true; // ¿À¸¥ÂÊ
        }

        if (Input.GetKeyDown(KeyCode.E))
            TryInteract();
    }

    private void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + inputVector * moveSpeed * Time.fixedDeltaTime);
    }

    private void TryInteract()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);

        IInteractable best = null;
        float bestDistSqr = float.MaxValue;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null) continue;

            IInteractable interactable = hits[i].GetComponentInParent<IInteractable>();
            if (interactable != null)
            {
                float distSqr = ((Vector2)hits[i].transform.position - (Vector2)transform.position).sqrMagnitude;
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;
                    best = interactable;
                }
                continue;
            }

            CustomerController customer = hits[i].GetComponentInParent<CustomerController>();
            if (customer != null && carry != null && carry.HasFood())
            {
                if (customer.TryServe(carry.heldFood))
                {
                    carry.Drop();
                    return;
                }
            }
        }

        if (best != null)
        {
            best.Interact(carry);
        }
    }
}
