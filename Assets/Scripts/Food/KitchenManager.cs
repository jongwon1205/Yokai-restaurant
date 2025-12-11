using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrderTicket
{
    public CustomerController customer; // 주문한 손님
    public FoodDataSO food;            // 어떤 음식을 주문했는지
    public float remainingCookTime;    // 남은 조리 시간

    public OrderTicket(CustomerController customer, FoodDataSO food)
    {
        this.customer = customer;
        this.food = food;
        this.remainingCookTime = food != null ? food.cookTime : 0f;
    }
}

public class KitchenManager : MonoBehaviour
{
    [Header("메뉴 리스트")]
    public FoodDataSO[] menuFoods;

    [Header("자동 조리 사용")]
    public bool autoCook = true;

    private List<OrderTicket> tickets = new List<OrderTicket>();

    private void Update()
    {
        if (!autoCook) return;

        // 아주 단순한 "시간 지나면 자동 조리 완료" 방식
        for (int i = tickets.Count - 1; i >= 0; i--)
        {
            OrderTicket ticket = tickets[i];
            if (ticket == null) continue;

            ticket.remainingCookTime -= Time.deltaTime;

            if (ticket.remainingCookTime <= 0f)
            {
                CompleteCook(ticket);
                tickets.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 손님이 주문할 때 호출하는 함수
    /// </summary>
    public void AddOrder(CustomerController customer, FoodDataSO food)
    {
        if (customer == null)
        {
            Debug.LogWarning("주문을 추가하려 했지만 Customer가 null입니다.");
            return;
        }

        if (food == null)
        {
            Debug.LogWarning("주문을 추가하려 했지만 FoodDataSO가 null입니다.");
            return;
        }

        OrderTicket ticket = new OrderTicket(customer, food);
        tickets.Add(ticket);

        Debug.Log($"[주문 접수] 손님: {customer.name} / 메뉴: {food.foodName} / 조리시간: {food.cookTime}");
        // TODO: 주방 UI에 주문 티켓 추가
    }

    /// <summary>
    /// 조리가 완료된 주문 처리
    /// </summary>
    private void CompleteCook(OrderTicket ticket)
    {
        if (ticket == null) return;

        // 손님이 이미 나갔다면 처리 X
        if (ticket.customer == null)
        {
            Debug.LogWarning($"[조리 완료] 손님이 이미 떠났습니다. 메뉴: {ticket.food.foodName} 폐기");
            return;
        }

        // 실제 서빙 처리
        ServeFood(ticket.customer, ticket.food);
    }

    /// <summary>
    /// 손님에게 음식을 서빙하는 실제 동작
    /// </summary>
    private void ServeFood(CustomerController customer, FoodDataSO food)
    {
        // TODO: 여기서 테이블 위에 음식 프리팹 Instantiate 할 수도 있음

        customer.OnFoodServed();
        Debug.Log($"[서빙 완료] {food.foodName} → {customer.name}");
    }

    /// <summary>
    /// 메뉴 리스트에서 랜덤으로 하나 뽑는 유틸 함수 (손님 주문 시 사용)
    /// </summary>
    public FoodDataSO GetRandomFood()
    {
        if (menuFoods == null || menuFoods.Length == 0)
        {
            Debug.LogWarning("메뉴 리스트가 비어 있습니다.");
            return null;
        }

        int index = Random.Range(0, menuFoods.Length);
        return menuFoods[index];
    }
}
