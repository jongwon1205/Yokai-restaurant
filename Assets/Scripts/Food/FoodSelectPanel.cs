using System;
using System.Collections.Generic;
using UnityEngine;

public class FoodSelectPanel : MonoBehaviour
{
    public static FoodSelectPanel Instance { get; private set; }

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private FoodSelectButton buttonPrefab;

    private Action<FoodDataSO> onSelect;

    private void Awake()
    {
        Instance = this;
        Close();
    }

    public void Open(CookingDevice device, List<FoodDataSO> foods, Action<FoodDataSO> onSelect)
    {
        if (panelRoot != null) panelRoot.SetActive(true);

        this.onSelect = onSelect;

        // 기존 버튼 삭제
        if (contentRoot != null)
        {
            for (int i = contentRoot.childCount - 1; i >= 0; i--)
                Destroy(contentRoot.GetChild(i).gameObject);
        }

        if (foods == null || foods.Count == 0 || buttonPrefab == null || contentRoot == null)
            return;

        for (int i = 0; i < foods.Count; i++)
        {
            FoodDataSO food = foods[i];
            if (food == null) continue;

            FoodSelectButton btn = Instantiate(buttonPrefab, contentRoot);
            btn.Set(food, HandleSelect);
        }
    }

    private void HandleSelect(FoodDataSO food)
    {
        Debug.Log("HandleSelect 호출됨: " + (food != null ? food.foodName : "null"));
        onSelect?.Invoke(food);
        Close();
    }

    public void Close()
    {
        Debug.Log("Close 호출됨 / panelRoot=" + (panelRoot != null ? panelRoot.name : "null") +
                  " / activeBefore=" + (panelRoot != null ? panelRoot.activeSelf.ToString() : "null"));
        onSelect = null;
        if (panelRoot != null) panelRoot.SetActive(false);
    }
}
