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

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        carry = GetComponent<PlayerCarry>();
    }

    private void Update()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");
        inputVector = inputVector.normalized;

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

        for (int i = 0; i < hits.Length; i++)
        {
            CookingDevice device = hits[i].GetComponent<CookingDevice>();
            if (device != null)
            {
                if (device.TryInteract(carry))
                    return;
            }

            CustomerController customer = hits[i].GetComponent<CustomerController>();
            if (customer != null && carry != null && carry.HasFood())
            {
                if (customer.TryServe(carry.heldFood))
                {
                    carry.Drop();
                    return;
                }
            }
        }
    }
}
