using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenManager : MonoBehaviour
{
    public static KitchenManager Instance;

    private List<OrderTicket> pendingOrders = new List<OrderTicket>();

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

        Debug.Log("주문 등록 / 음식=" + ticket.food.foodName + " / 손님=" + ticket.customer.name + " / deviceType=" + ticket.food.deviceType);
    }

    public bool TryDequeueCookableTicket(CookingDeviceType deviceType, out OrderTicket ticket)
    {        
        for (int i = 0; i < pendingOrders.Count; i++)
        {
            if (pendingOrders[i] == null) continue;
            if (pendingOrders[i].food == null) continue;
            if (pendingOrders[i].food.deviceType != deviceType) continue;

            ticket = pendingOrders[i];
            pendingOrders.RemoveAt(i);

            Debug.Log("조리 시작용 티켓 꺼냄 / 음식=" + ticket.food.foodName + " / 손님=" + (ticket.customer != null ? ticket.customer.name : "null"));
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
