using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CookingDeviceType
{
    Pot,
    Skewer,
    Pan
}

public class CookingDevice : MonoBehaviour
{
    public CookingDeviceType deviceType;

    private bool isCooking;
    private FoodDataSO readyFood;
    private CustomerController readyCustomer;

    public bool TryInteract(PlayerCarry carry)
    {
        if (readyFood != null)
        {
            if (carry != null && carry.TryPickUp(readyFood))
            {
                Debug.Log("음식 픽업 / 음식=" + readyFood.foodName + " / 기구=" + deviceType);

                readyFood = null;
                readyCustomer = null;
                return true;
            }
            return false;
        }

        if (isCooking)
        {
            Debug.Log("이미 조리중 / 기구=" + deviceType);
            return false;
        }

        if (KitchenManager.Instance == null) return false;

        OrderTicket ticket;
        if (!KitchenManager.Instance.TryDequeueCookableTicket(deviceType, out ticket))
            return false;

        Debug.Log("조리 시작 / 음식=" + ticket.food.foodName +
                  " / 손님=" + ticket.customer.name +
                  " / 기구=" + deviceType);

        StartCoroutine(CookRoutine(ticket));
        return true;
    }

    private IEnumerator CookRoutine(OrderTicket ticket)
    {
        isCooking = true;

        Debug.Log("조리중... / 음식=" + ticket.food.foodName +
                  " / cookTime=" + ticket.food.cookTime +
                  " / 기구=" + deviceType);

        yield return new WaitForSeconds(ticket.food.cookTime);

        readyFood = ticket.food;
        readyCustomer = ticket.customer;
        isCooking = false;

        Debug.Log("조리 완료 / 음식=" + readyFood.foodName +
                  " / 손님=" + readyCustomer.name +
                  " / 기구=" + deviceType);
    }
}
