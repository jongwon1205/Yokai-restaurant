using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderTicket
{
    public CustomerController customer;
    public FoodDataSO food;
    public float remainingCookTime;

    public OrderTicket(CustomerController customer, FoodDataSO food)
    {
        this.customer = customer;
        this.food = food;
        remainingCookTime = food != null ? food.cookTime : 0f;
    }
}
