using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookCompleteUI : MonoBehaviour
{
    [SerializeField] private Image foodIcon;

    private void Awake()
    {
        Hide();
    }

    public void Show(FoodDataSO food)
    {
        if (food == null) return;

        gameObject.SetActive(true);

        if (foodIcon != null)
            foodIcon.sprite = food.icon;
    }

    public void Hide()
    {
        if (foodIcon != null)
            foodIcon.sprite = null;

        gameObject.SetActive(false);
    }
}
