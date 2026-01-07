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

    [Header("Cookable Foods (기구가 만들 수 있는 음식 목록 - 패널 표시/안전장치용)")]
    [SerializeField] private List<FoodDataSO> cookableFoods = new List<FoodDataSO>();

    [Header("Ready Food UI (완성 아이콘)")]
    [SerializeField] private GameObject readyUiRoot;
    [SerializeField] private UnityEngine.UI.Image readyIcon;

    [SerializeField] private CookCompleteUI completeUI;

    [Header("Fever Auto Cook")]
    [SerializeField] private float feverPollInterval = 0.2f;

    private bool isCooking;
    private FoodDataSO readyFood;

    private float nextFeverPollTime;

    public Transform PromptAnchor => promptAnchor != null ? promptAnchor : transform;

    private void Start()
    {
        HideReadyUI();
    }

    private void Update()
    {
        TryAutoCookByFever();
    }

    private void TryAutoCookByFever()
    {
        if (FeverTimeManager.Instance == null) return;
        if (!FeverTimeManager.Instance.IsFeverTime) return;

        if (Time.time < nextFeverPollTime) return;
        nextFeverPollTime = Time.time + feverPollInterval;

        if (isCooking) return;
        if (readyFood != null) return;

        if (KitchenManager.Instance == null) return;

        OrderTicket ticket;
        if (KitchenManager.Instance.TryDequeueCookableTicket(deviceType, out ticket))
        {
            if (ticket == null || ticket.food == null) return;

            if (!IsFoodAllowed(ticket.food))
                return;

            StartCoroutine(CookRoutine(ticket.food));
        }
    }

    public void SetHighlighted(bool isOn)
    {
        if (highlightObject != null)
            highlightObject.SetActive(isOn);
    }

    public bool Interact(PlayerCarry carry)
    {
        // 1) 완성된 음식이 있으면 줍기
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

        // 2) 조리 중이면 패널 못 열게
        if (isCooking)
            return false;

        FoodSelectPanel panel = FoodSelectPanel.Instance;
        if (panel == null)
            return false;

        // ✅ 전체 메뉴를 보여준다 (이 기구가 만들 수 있는 목록)
        List<FoodDataSO> foodsToShow = BuildUniqueCookableList();

        panel.Toggle(this, foodsToShow, (selectedFood) =>
        {
            if (selectedFood == null) return;
            if (isCooking) return;
            if (readyFood != null) return;

            // ✅ 안전장치: 이 기구에서 만들 수 있는 음식만
            if (!IsFoodAllowed(selectedFood))
                return;

            // ✅ 주문 티켓이 있으면 "주문 티켓"을 먼저 소비(있으면 좋고 없어도 상관 없음)
            if (KitchenManager.Instance != null)
            {
                OrderTicket ticket;
                bool hasTicket = KitchenManager.Instance.TryTakeOrderByFood(deviceType, selectedFood, out ticket);

                // 티켓이 있으면 ticket.food로 조리(보통 selectedFood와 같음)
                if (hasTicket && ticket != null && ticket.food != null)
                {
                    StartCoroutine(CookRoutine(ticket.food));
                    return;
                }
            }

            // ✅ 핵심: 주문이 없어도 무조건 조리 시작
            StartCoroutine(CookRoutine(selectedFood));
        });

        return true;
    }

    public bool TryInteract(PlayerCarry carry)
    {
        return Interact(carry);
    }

    private List<FoodDataSO> BuildUniqueCookableList()
    {
        List<FoodDataSO> result = new List<FoodDataSO>();

        if (cookableFoods == null) return result;

        for (int i = 0; i < cookableFoods.Count; i++)
        {
            FoodDataSO food = cookableFoods[i];
            if (food == null) continue;

            // 기구 타입 안전장치(원하면 제거 가능)
            if (food.deviceType != deviceType) continue;

            if (!result.Contains(food))
                result.Add(food);
        }

        return result;
    }

    private bool IsFoodAllowed(FoodDataSO food)
    {
        if (food == null) return false;

        // 기구 타입 체크 (원하면 제거 가능)
        if (food.deviceType != deviceType)
            return false;

        // cookableFoods가 비어있으면 타입만으로 허용
        if (cookableFoods == null || cookableFoods.Count == 0)
            return true;

        return cookableFoods.Contains(food);
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
