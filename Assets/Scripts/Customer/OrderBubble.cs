using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderBubble : MonoBehaviour
{
    public Image iconImage;

    public void Show(Sprite icon)
    {
        if (iconImage == null) return;
        iconImage.sprite = icon;
        iconImage.enabled = true;
    }

    public void Hide()
    {
        if (iconImage == null) return;
        iconImage.enabled = false;
    }
}
