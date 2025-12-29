using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OrderBubble : MonoBehaviour
{
    [Header("공통 이미지")]
    public Image iconImage;

    [Header("만족 / 불만족 아이콘")]
    public Sprite happyIcon;
    public Sprite angryIcon;

    [Header("표시 시간")]
    public float resultShowTime = 1.2f;

    private Coroutine showRoutine;

    public void Show(Sprite icon)
    {
        if (iconImage == null) return;

        if (showRoutine != null)
            StopCoroutine(showRoutine);

        iconImage.sprite = icon;
        iconImage.enabled = true;
    }

    public void ShowHappy()
    {
        ShowResult(happyIcon);
    }

    public void ShowAngry()
    {
        ShowResult(angryIcon);
    }

    private void ShowResult(Sprite icon)
    {
        if (iconImage == null || icon == null) return;

        if (showRoutine != null)
            StopCoroutine(showRoutine);

        showRoutine = StartCoroutine(ResultRoutine(icon));
    }

    private IEnumerator ResultRoutine(Sprite icon)
    {
        iconImage.sprite = icon;
        iconImage.enabled = true;

        yield return new WaitForSeconds(resultShowTime);

        Hide();
        showRoutine = null;
    }

    public void Hide()
    {
        if (iconImage == null) return;
        iconImage.enabled = false;
    }
}
