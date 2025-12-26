using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private GameObject root;

    [Header("UI")]
    [SerializeField] private TMP_Text speakerText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Image portraitImage;

    public bool IsOpen => root != null && root.activeSelf;

    private void Awake()
    {
        Hide();
    }

    public void Show()
    {
        if (root != null) root.SetActive(true);
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
    }

    public void SetLine(string speaker, string body, Sprite portrait)
    {
        if (speakerText != null) speakerText.text = speaker;
        if (bodyText != null) bodyText.text = body;

        if (portraitImage != null)
        {
            portraitImage.sprite = portrait;
            portraitImage.enabled = portrait != null;
        }
    }

    public void SetBodyText(string body)
    {
        if (bodyText != null) bodyText.text = body;
    }
}
