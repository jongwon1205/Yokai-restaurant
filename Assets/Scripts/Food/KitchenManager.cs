using System.Collections.Generic;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    public static KitchenManager Instance;

    private readonly List<OrderTicket> pendingOrders = new List<OrderTicket>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddPendingOrder(OrderTicket ticket)
    {
        if (ticket == null) return;
        if (ticket.food == null) return;
        if (ticket.customer == null) return;

        pendingOrders.Add(ticket);
    }

    // ✅ 특정 기구 타입에 해당하는 "주문 음식 후보"만 뽑아준다 (중복 음식 제거)
    public List<FoodDataSO> GetCookableOrderFoods(CookingDeviceType deviceType)
    {
        List<FoodDataSO> result = new List<FoodDataSO>();

        for (int i = 0; i < pendingOrders.Count; i++)
        {
            OrderTicket t = pendingOrders[i];
            if (t == null || t.food == null) continue;
            if (t.food.deviceType != deviceType) continue;

            // 중복 제거
            if (!result.Contains(t.food))
                result.Add(t.food);
        }

        return result;
    }

    // ✅ 플레이어가 패널에서 특정 음식을 "선택"했을 때,
    // 그 음식에 해당하는 주문 티켓 1개를 꺼내준다.
    public bool TryTakeOrderByFood(CookingDeviceType deviceType, FoodDataSO food, out OrderTicket ticket)
    {
        for (int i = 0; i < pendingOrders.Count; i++)
        {
            OrderTicket t = pendingOrders[i];
            if (t == null || t.food == null) continue;

            if (t.food.deviceType != deviceType) continue;
            if (t.food != food) continue;

            ticket = t;
            pendingOrders.RemoveAt(i);
            return true;
        }

        ticket = null;
        return false;
    }

    public int GetPendingCount()
    {
        return pendingOrders.Count;
    }
}
