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
