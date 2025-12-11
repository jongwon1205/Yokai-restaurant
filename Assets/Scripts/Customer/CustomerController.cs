using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomerState
{
    Arrive,
    MoveToSeat,
    Order,
    Wait,
    Eat,
    Exit
}

public class CustomerController : MonoBehaviour
{
    [Header("스프라이트")]
    public SpriteRenderer spriteRenderer;

    private CustomerManager manager;
    private Seat seat;
    private Transform seatPoint;
    private Transform exitPoint;

    private CustomerDataSO data;

    private CustomerState state;

    private float moveSpeed;
    private float patienceTime;
    private float currentPatience;
    private float eatTime;
    private float currentEatTime;

    private bool hasFood = false;
    private bool isHappy = false;

    public void Init(
        CustomerDataSO data,
        CustomerManager manager,
        Seat seat,
        float patienceTime,
        float eatTime,
        Transform exitPoint
    )
    {
        this.data = data;
        this.manager = manager;
        this.seat = seat;
        this.seatPoint = seat.seatPoint;
        this.exitPoint = exitPoint;

        this.moveSpeed = data.moveSpeed;
        this.patienceTime = patienceTime;
        this.eatTime = eatTime;

        currentPatience = this.patienceTime;
        currentEatTime = this.eatTime;

        state = CustomerState.Arrive;

        if (spriteRenderer != null && data.sprite != null)
        {
            spriteRenderer.sprite = data.sprite;
        }

        Debug.Log($"손님 생성: {data.displayName} (Patience:{this.patienceTime}, Eat:{this.eatTime})");
    }

    private void Update()
    {
        switch (state)
        {
            case CustomerState.Arrive:
                HandleArrive();
                break;
            case CustomerState.MoveToSeat:
                HandleMoveToSeat();
                break;
            case CustomerState.Order:
                HandleOrder();
                break;
            case CustomerState.Wait:
                HandleWait();
                break;
            case CustomerState.Eat:
                HandleEat();
                break;
            case CustomerState.Exit:
                HandleExit();
                break;
        }
    }

    private void HandleArrive()
    {
        ChangeState(CustomerState.MoveToSeat);
    }

    private void HandleMoveToSeat()
    {
        if (seatPoint == null) return;

        transform.position = Vector3.MoveTowards(transform.position, seatPoint.position, moveSpeed * Time.deltaTime);

        Vector3 dir = seatPoint.position - transform.position;
        if (spriteRenderer != null)
        {
            if (dir.x > 0.01f) spriteRenderer.flipX = false;
            else if (dir.x < -0.01f) spriteRenderer.flipX = true;
        }

        float distance = Vector3.Distance(transform.position, seatPoint.position);
        if (distance < 0.05f)
        {
            transform.position = seatPoint.position;
            ChangeState(CustomerState.Order);
        }
    }

    private void HandleOrder()
    {
        // TODO: 주문 UI/말풍선 연출
        Debug.Log($"{data.displayName} 주문!");

        // TODO: KitchenQueue에 OrderTicket 전달 (나중에 연결)
        ChangeState(CustomerState.Wait);
    }

    private void HandleWait()
    {
        if (hasFood)
        {
            ChangeState(CustomerState.Eat);
            return;
        }

        currentPatience -= Time.deltaTime;

        // TODO: 인내심 게이지 UI 업데이트 (currentPatience / patienceTime)

        if (currentPatience <= 0f)
        {
            isHappy = false;
            ChangeState(CustomerState.Exit);
        }
    }

    private void HandleEat()
    {
        currentEatTime -= Time.deltaTime;

        // TODO: 먹는 애니메이션/이펙트

        if (currentEatTime <= 0f)
        {
            isHappy = true;
            ChangeState(CustomerState.Exit);
        }
    }

    private void HandleExit()
    {
        if (exitPoint == null)
        {
            NotifyExitAndDestroy();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, exitPoint.position, moveSpeed * Time.deltaTime);

        Vector3 dir = exitPoint.position - transform.position;
        if (spriteRenderer != null)
        {
            if (dir.x > 0.01f) spriteRenderer.flipX = false;
            else if (dir.x < -0.01f) spriteRenderer.flipX = true;
        }

        float distance = Vector3.Distance(transform.position, exitPoint.position);
        if (distance < 0.05f)
        {
            transform.position = exitPoint.position;
            NotifyExitAndDestroy();
        }
    }

    private void NotifyExitAndDestroy()
    {
        if (manager != null)
        {
            manager.OnCustomerExit(this, seat, isHappy, data);
        }

        Destroy(gameObject);
    }

    private void ChangeState(CustomerState newState)
    {
        state = newState;
    }

    public void OnFoodServed()
    {
        hasFood = true;
        Debug.Log($"{data.displayName} 음식 서빙 완료!");
    }
}
