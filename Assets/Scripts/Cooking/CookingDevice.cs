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

    public bool Interact(PlayerCarry carry)
    {
        if (readyFood != null)
        {
            if (carry != null && carry.TryPickUp(readyFood))
            {
                readyFood = null;
                HideReadyUI();

                if (completeUI != null)
                    completeUI.Hide();

                return true;
            }

            return false;
        }

        if (isCooking)
            return false;

        FoodSelectPanel panel = FoodSelectPanel.Instance;
        if (panel == null)
            return false;

        if (cookableFoods == null)
            cookableFoods = new List<FoodDataSO>();

        // ⭐ 핵심: 원본 리스트 보호 (패널이 리스트를 수정해도 cookableFoods가 안 바뀜)
        List<FoodDataSO> foodsForUi = new List<FoodDataSO>(cookableFoods);

        panel.Toggle(this, foodsForUi, (selectedFood) =>
        {
            if (selectedFood == null) return;
            if (isCooking) return;
            if (readyFood != null) return;

            StartCoroutine(CookRoutine(selectedFood));
        });

        return true;
    }

    public bool TryInteract(PlayerCarry carry)
    {
        return Interact(carry);
    }

    private IEnumerator CookRoutine(FoodDataSO food)
    {
        HideReadyUI();
        isCooking = true;

        if (completeUI != null)
            completeUI.Hide();

        yield return new WaitForSeconds(food.cookTime);

        readyFood = food;
        isCooking = false;

        ShowReadyUI(food);

        if (completeUI != null)
            completeUI.Show(food);
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
