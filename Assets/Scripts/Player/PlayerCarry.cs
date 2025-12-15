using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    public FoodDataSO heldFood;

    public bool HasFood()
    {
        return heldFood != null;
    }

    public bool TryPickUp(FoodDataSO food)
    {
        if (food == null) return false;
        if (heldFood != null)
        {
            Debug.Log("픽업 실패(이미 들고 있음) / 현재=" + heldFood.foodName);
            return false;
        }

        heldFood = food;
        Debug.Log("음식 픽업 성공 / 음식=" + heldFood.foodName);
        return true;
    }

    public FoodDataSO Drop()
    {
        FoodDataSO f = heldFood;
        heldFood = null;

        Debug.Log("음식 내려놓기 / 음식=" + (f != null ? f.foodName : "null"));
        return f;
    }
}
