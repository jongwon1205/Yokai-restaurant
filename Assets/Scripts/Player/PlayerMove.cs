using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Rigidbody2D rigid;
    private Vector2 inputVector;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        inputVector.x = Input.GetAxisRaw("Horizontal");
        inputVector.y = Input.GetAxisRaw("Vertical");

        inputVector = inputVector.normalized;
    }

    private void FixedUpdate()
    {
        rigid.MovePosition(rigid.position + inputVector * moveSpeed * Time.fixedDeltaTime);
    }
}
