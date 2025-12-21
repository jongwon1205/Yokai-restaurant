using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodSelectButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;

    private FoodDataSO food;
    private Action<FoodDataSO> onClick;

    public void Set(FoodDataSO food, Action<FoodDataSO> onClick)
    {
        this.food = food;
        this.onClick = onClick;

        if (iconImage != null) iconImage.sprite = food.icon;
        if (nameText != null) nameText.text = food.foodName;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                Debug.Log("버튼 클릭 들어옴: " + (this.food != null ? this.food.foodName : "null"));
                this.onClick?.Invoke(this.food);
            });
        }
    }
}
