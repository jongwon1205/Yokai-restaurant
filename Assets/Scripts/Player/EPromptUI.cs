using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EPromptUI : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 40f);

    private Camera mainCam;
    private Transform target;

    private void Awake()
    {
        mainCam = Camera.main;
        Hide();
    }

    private void LateUpdate()
    {
        if (target == null || mainCam == null || root == null) return;

        Vector3 screenPos = mainCam.WorldToScreenPoint(target.position);
        root.position = (Vector2)screenPos + screenOffset;
    }

    public void Show(Transform worldTarget)
    {
        target = worldTarget;
        if (root != null) root.gameObject.SetActive(true);
    }

    public void Hide()
    {
        target = null;
        if (root != null) root.gameObject.SetActive(false);
    }
}
