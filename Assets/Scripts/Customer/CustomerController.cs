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
    public SpriteRenderer spriteRenderer;

    public float arriveStopDistance = 0.05f;
    public OrderBubble orderBubble;

    [Header("결과 아이콘(만족/불만족) 표시 후 다음 상태로 넘어가는 딜레이")]
    [SerializeField] private float resultUiDelay = 1.2f;

    [Header("Sound")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip happyClip;
    [SerializeField] private AudioClip angryClip;

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

    private bool hasFood;
    private bool isHappy;
    public bool HasFood => hasFood;

    private FoodDataSO orderedFood;
    private bool hasOrdered;

    private CustomerState lastLoggedState;

    private Animator animator;

    private bool hasShownResultUi;
    private Coroutine resultRoutine;

    private Queue<Transform> moveQueue;
    private Transform currentMoveTarget;

    private bool hasPreparedExitPath;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

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
        this.exitPoint = exitPoint;

        seatPoint = seat != null ? seat.seatPoint : null;

        if (this.seat != null)
            this.seat.isOccupied = true;

        moveSpeed = data != null ? data.moveSpeed : 2f;

        this.patienceTime = Mathf.Max(0.1f, patienceTime);
        currentPatience = this.patienceTime;

        this.eatTime = Mathf.Max(0.1f, eatTime);
        currentEatTime = this.eatTime;

        hasFood = false;
        isHappy = false;

        orderedFood = null;
        hasOrdered = false;

        hasShownResultUi = false;
        if (resultRoutine != null)
        {
            StopCoroutine(resultRoutine);
            resultRoutine = null;
        }

        moveQueue = null;
        currentMoveTarget = null;
        hasPreparedExitPath = false;

        if (spriteRenderer != null && data != null && data.sprite != null)
            spriteRenderer.sprite = data.sprite;

        if (orderBubble != null)
            orderBubble.Hide();

        state = CustomerState.Arrive;

        lastLoggedState = (CustomerState)(-1);
        LogStateIfChanged();
        SetMoving(false);
    }

    private void SetMoving(bool isMoving)
    {
        if (animator == null) return;
        animator.SetBool("isMoving", isMoving);
    }

    private void Update()
    {
        switch (state)
        {
            case CustomerState.Arrive:
                UpdateArrive();
                break;
            case CustomerState.MoveToSeat:
                UpdateMoveToSeat();
                break;
            case CustomerState.Order:
                UpdateOrder();
                break;
            case CustomerState.Wait:
                UpdateWait();
                break;
            case CustomerState.Eat:
                UpdateEat();
                break;
            case CustomerState.Exit:
                UpdateExit();
                break;
        }

        LogStateIfChanged();
    }

    private void UpdateArrive()
    {
        if (seat == null || seatPoint == null)
        {
            PrepareExitPath();
            state = CustomerState.Exit;
            SetMoving(true);
            return;
        }

        PrepareEnterPath();

        state = CustomerState.MoveToSeat;
        SetMoving(true);
    }

    private void PrepareEnterPath()
    {
        moveQueue = new Queue<Transform>();

        if (seat != null)
        {
            Transform[] points = seat.GetEnterWaypoints();
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i] == null) continue;
                    moveQueue.Enqueue(points[i]);
                }
            }
        }

        if (seatPoint != null)
            moveQueue.Enqueue(seatPoint);

        currentMoveTarget = moveQueue.Count > 0 ? moveQueue.Dequeue() : null;
    }

    private void PrepareExitPath()
    {
        moveQueue = new Queue<Transform>();

        if (seat != null)
        {
            Transform[] points = seat.GetExitWaypoints();
            if (points != null)
            {
                for (int i = 0; i < points.Length; i++)
                {
                    if (points[i] == null) continue;
                    moveQueue.Enqueue(points[i]);
                }
            }
        }

        if (exitPoint != null)
            moveQueue.Enqueue(exitPoint);

        currentMoveTarget = moveQueue.Count > 0 ? moveQueue.Dequeue() : null;
        hasPreparedExitPath = true;
    }

    private void UpdateMoveToSeat()
    {
        if (currentMoveTarget == null)
        {
            PrepareExitPath();
            state = CustomerState.Exit;
            return;
        }

        MoveTowards(currentMoveTarget.position);

        if (Vector2.Distance(transform.position, currentMoveTarget.position) <= arriveStopDistance)
        {
            transform.position = currentMoveTarget.position;

            if (moveQueue != null && moveQueue.Count > 0)
            {
                currentMoveTarget = moveQueue.Dequeue();
                return;
            }

            SetMoving(false);
            seat.isOccupied = true;
            state = CustomerState.Order;
        }
    }

    private void UpdateOrder()
    {
        if (hasOrdered) return;

        orderedFood = manager != null ? manager.GetRandomFoodForCustomer(data) : null;

        if (orderedFood == null)
        {
            PrepareExitPath();
            state = CustomerState.Exit;
            return;
        }

        hasOrdered = true;

        if (orderBubble != null)
            orderBubble.Show(orderedFood.icon);

        if (KitchenManager.Instance != null)
            KitchenManager.Instance.AddPendingOrder(new OrderTicket(this, orderedFood));

        state = CustomerState.Wait;
    }

    private void UpdateWait()
    {
        if (hasShownResultUi) return;

        currentPatience -= Time.deltaTime;

        if (currentPatience <= 0f)
        {
            isHappy = false;
            hasShownResultUi = true;

            if (resultRoutine != null) StopCoroutine(resultRoutine);
            resultRoutine = StartCoroutine(ShowAngryThenExit());
        }
    }

    private IEnumerator ShowAngryThenExit()
    {
        if (uiAudioSource != null && angryClip != null)
            uiAudioSource.PlayOneShot(angryClip);

        if (orderBubble != null)
            orderBubble.ShowAngry();

        yield return new WaitForSeconds(resultUiDelay);

        if (orderBubble != null)
            orderBubble.Hide();

        PrepareExitPath();
        state = CustomerState.Exit;
        resultRoutine = null;
    }

    private void UpdateEat()
    {
        currentEatTime -= Time.deltaTime;

        if (currentEatTime <= 0f)
        {
            PrepareExitPath();
            state = CustomerState.Exit;
        }
    }

    private void UpdateExit()
    {
        SetMoving(true);

        if (!hasPreparedExitPath)
            PrepareExitPath();

        if (currentMoveTarget == null)
        {
            LeaveAndCleanup();
            return;
        }

        MoveTowards(currentMoveTarget.position);

        if (Vector2.Distance(transform.position, currentMoveTarget.position) <= arriveStopDistance)
        {
            transform.position = currentMoveTarget.position;

            if (moveQueue != null && moveQueue.Count > 0)
            {
                currentMoveTarget = moveQueue.Dequeue();
                return;
            }

            LeaveAndCleanup();
        }
    }

    private void MoveTowards(Vector3 targetPos)
    {
        Vector2 next = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        transform.position = next;
    }

    private void LeaveAndCleanup()
    {
        if (seat != null)
            seat.isOccupied = false;

        if (manager != null)
            manager.OnCustomerLeft(this, isHappy);

        Destroy(gameObject);
    }

    public bool TryServe(FoodDataSO food)
    {
        if (food == null) return false;
        if (orderedFood == null) return false;
        if (food != orderedFood) return false;
        if (state != CustomerState.Wait) return false;

        hasFood = true;
        isHappy = true;

        FeverTimeManager.Instance?.AddCorrectDelivery(1);

        if (resultRoutine != null) StopCoroutine(resultRoutine);
        resultRoutine = StartCoroutine(ShowHappyThenEat());

        return true;
    }

    private IEnumerator ShowHappyThenEat()
    {
        hasShownResultUi = true;

        if (uiAudioSource != null && happyClip != null)
            uiAudioSource.PlayOneShot(happyClip);

        if (orderBubble != null)
            orderBubble.ShowHappy();

        yield return new WaitForSeconds(resultUiDelay);

        if (orderBubble != null)
            orderBubble.Hide();

        currentEatTime = eatTime;
        state = CustomerState.Eat;

        resultRoutine = null;
    }

    public void OnFoodCooked(FoodDataSO food)
    {
        TryServe(food);
    }

    public FoodDataSO GetOrderedFood()
    {
        return orderedFood;
    }

    public bool HasOrdered()
    {
        return hasOrdered;
    }

    public bool IsWaiting()
    {
        return state == CustomerState.Wait;
    }

    private void LogStateIfChanged()
    {
        if (state == lastLoggedState) return;
        lastLoggedState = state;

        string foodName = orderedFood != null ? orderedFood.foodName : "None";
    }

    private string GetStateText(CustomerState s)
    {
        if (s == CustomerState.Arrive) return "등장";
        if (s == CustomerState.MoveToSeat) return "자리로 이동중";
        if (s == CustomerState.Order) return "주문중";
        if (s == CustomerState.Wait) return "대기중";
        if (s == CustomerState.Eat) return "음식 먹는중";
        if (s == CustomerState.Exit) return "퇴장중";
        return s.ToString();
    }
}
