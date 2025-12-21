using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CookingDeviceType
{
    Pot,
    Skewer,
    Pan
}

public class CookingDevice : MonoBehaviour, IInteractable
{
    public CookingDeviceType deviceType;

    [Header("Prompt / Highlight")]
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private GameObject highlightObject;

    [Header("Cookable Foods (E 눌렀을 때 목록 UI)")]
    [SerializeField] private List<FoodDataSO> cookableFoods = new List<FoodDataSO>();

    private bool isCooking;
    private FoodDataSO readyFood;
    private CustomerController readyCustomer;

    public Transform PromptAnchor => promptAnchor != null ? promptAnchor : transform;

    public void SetHighlighted(bool isOn)
    {
        if (highlightObject != null)
            highlightObject.SetActive(isOn);
    }

    // PlayerInteractDetector가 E 눌렀을 때 호출
    public bool Interact(PlayerCarry carry)
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

        if (cookableFoods != null && cookableFoods.Count > 0)
        {
            FoodSelectPanel panel = FoodSelectPanel.Instance;
            if (panel == null)
            {
                Debug.LogWarning("FoodSelectPanel.Instance가 씬에 없음");
                return false;
            }

            panel.Open(this, cookableFoods, (selectedFood) =>
            {
                if (selectedFood == null) return;

                Debug.Log("조리 시작(선택) / 음식=" + selectedFood.foodName + " / 기구=" + deviceType);
                StartCoroutine(CookRoutine(selectedFood, null));
            });

            return true;
        }

        return TryInteract(carry);
    }

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

        StartCoroutine(CookRoutine(ticket.food, ticket.customer));
        return true;
    }

    private IEnumerator CookRoutine(FoodDataSO food, CustomerController customer)
    {
        isCooking = true;

        Debug.Log("조리중... / 음식=" + food.foodName +
                  " / cookTime=" + food.cookTime +
                  " / 기구=" + deviceType);

        yield return new WaitForSeconds(food.cookTime);

        readyFood = food;
        readyCustomer = customer;
        isCooking = false;

        Debug.Log("조리 완료 / 음식=" + readyFood.foodName +
                  " / 손님=" + (readyCustomer != null ? readyCustomer.name : "없음") +
                  " / 기구=" + deviceType);
    }
}
