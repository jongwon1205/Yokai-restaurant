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

    public bool IsOpen
    {
        get
        {
            return panelRoot != null && panelRoot.activeSelf;
        }
    }

    private void Awake()
    {
        Instance = this;
        Close();
    }

    public void Toggle(CookingDevice device, List<FoodDataSO> foods, Action<FoodDataSO> onSelect)
    {
        if (IsOpen)
        {
            Close();
            return;
        }

        Open(device, foods, onSelect);
    }

    public void Open(CookingDevice device, List<FoodDataSO> foods, Action<FoodDataSO> onSelect)
    {
        if (panelRoot != null) panelRoot.SetActive(true);

        this.onSelect = onSelect;

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
        onSelect?.Invoke(food);
        Close();
    }

    public void Close()
    {
        onSelect = null;
        if (panelRoot != null) panelRoot.SetActive(false);
    }
}
