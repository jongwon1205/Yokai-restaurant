using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CookingDeviceType
{
    Pot,
    Skewer,
    Pan,
    cutting_board
}

public class CookingDevice : MonoBehaviour, IInteractable
{
    public CookingDeviceType deviceType;

    [Header("Prompt / Highlight")]
    [SerializeField] private Transform promptAnchor;
    [SerializeField] private GameObject highlightObject;

    [Header("Cookable Foods (E 눌렀을 때 목록 UI)")]
    [SerializeField] private List<FoodDataSO> cookableFoods = new List<FoodDataSO>();

    [Header("Ready Food UI (완성 아이콘)")]
    [SerializeField] private GameObject readyUiRoot;  
    [SerializeField] private UnityEngine.UI.Image readyIcon;

    [SerializeField] private CookCompleteUI completeUI;

    private bool isCooking;
    private FoodDataSO readyFood;
    private CustomerController readyCustomer;

    public Transform PromptAnchor => promptAnchor != null ? promptAnchor : transform;

    private void Start()
    {
        HideReadyUI();
    }

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
                readyFood = null;
                readyCustomer = null;

                HideReadyUI();

                if (completeUI != null)
                    completeUI.Hide();

                return true;
            }

            return false;
        }

        if (isCooking)
        {
            return false;
        }

        if (cookableFoods != null && cookableFoods.Count > 0)
        {
            FoodSelectPanel panel = FoodSelectPanel.Instance;
            if (panel == null)
            {
                return false;
            }

            panel.Toggle(this, cookableFoods, (selectedFood) =>
            {
                if (selectedFood == null) return;
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
                readyFood = null;
                readyCustomer = null;
                HideReadyUI();

                if (completeUI != null)
                    completeUI.Hide();

                return true;
            }
            return false;
        }

        if (isCooking)
        {
            return false;
        }

        if (KitchenManager.Instance == null) return false;

        OrderTicket ticket;
        if (!KitchenManager.Instance.TryDequeueCookableTicket(deviceType, out ticket))
            return false;

        StartCoroutine(CookRoutine(ticket.food, ticket.customer));
        return true;
    }

    private IEnumerator CookRoutine(FoodDataSO food, CustomerController customer)
    {
        HideReadyUI();
        isCooking = true;

        if (completeUI != null)
            completeUI.Hide();

        Debug.Log("조리중... / 음식=" + food.foodName);

        yield return new WaitForSeconds(food.cookTime);

        readyFood = food;
        readyCustomer = customer;
        isCooking = false;

        ShowReadyUI(readyFood);

        if (completeUI != null)
            completeUI.Show(readyFood);

        Debug.Log("조리 완료 / 음식=" + readyFood.foodName);
    }

    private void ShowReadyUI(FoodDataSO food)
    {
        if (readyUiRoot != null)
            readyUiRoot.SetActive(true);

        if (readyIcon != null)
            readyIcon.sprite = food != null ? food.icon : null;
    }

    private void HideReadyUI()
    {
        if (readyUiRoot != null)
            readyUiRoot.SetActive(false);

        if (readyIcon != null)
            readyIcon.sprite = null;
    }
}
