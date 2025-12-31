using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    [Header("Held Food")]
    public FoodDataSO heldFood;

    [Header("Carry Icon (¸Ó¸® À§ UI)")]
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
            return false;
        }

        heldFood = food;

        UpdateCarryIcon();

        return true;
    }

    public FoodDataSO Drop()
    {
        FoodDataSO f = heldFood;
        heldFood = null;

        UpdateCarryIcon();

        return f;
    }

    public bool TryDiscard()
    {
        if (heldFood == null)
        {
            return false;
        }


        heldFood = null;
        UpdateCarryIcon();
        return true;
    }

    private void UpdateCarryIcon()
    {
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
