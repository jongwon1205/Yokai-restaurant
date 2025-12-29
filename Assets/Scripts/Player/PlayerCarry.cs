using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    [Header("Held Food")]
    public FoodDataSO heldFood;

    [Header("Carry Icon (머리 위 UI)")]
    [SerializeField] private Vector3 iconOffset = new Vector3(0f, 0.6f, 0f);
    [SerializeField] private int iconSortingOrder = 100;

    private SpriteRenderer carryIconRenderer;

    private void Awake()
    {
        GameObject iconObj = new GameObject("CarryFoodIcon");
        iconObj.transform.SetParent(transform);
        iconObj.transform.localPosition = iconOffset;

        carryIconRenderer = iconObj.AddComponent<SpriteRenderer>();
        carryIconRenderer.sortingOrder = iconSortingOrder;
        carryIconRenderer.enabled = false;
    }

    private void LateUpdate()
    {
        if (carryIconRenderer != null)
            carryIconRenderer.transform.localPosition = iconOffset;
    }


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

        UpdateCarryIcon();

        Debug.Log("음식 픽업 성공 / 음식=" + heldFood.foodName);
        return true;
    }

    public FoodDataSO Drop()
    {
        FoodDataSO f = heldFood;
        heldFood = null;

        UpdateCarryIcon();

        Debug.Log("음식 내려놓기 / 음식=" + (f != null ? f.foodName : "null"));
        return f;
    }

    private void UpdateCarryIcon()
    {
        Debug.Log("아이콘 갱신 / heldFood=" + (heldFood != null ? heldFood.foodName : "null") + " / iconNull=" + (heldFood == null || heldFood.icon == null));

        if (carryIconRenderer == null)
            return;

        if (heldFood == null || heldFood.icon == null)
        {
            carryIconRenderer.sprite = null;
            carryIconRenderer.enabled = false;
            return;
        }

        carryIconRenderer.sprite = heldFood.icon;
        carryIconRenderer.enabled = true;
    }
}
