using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionController : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private AudioOptions audioOptions;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Open()
    {
        optionsPanel.SetActive(true);
    }

    public void Close()
    {
        optionsPanel.SetActive(false);
    }

    public void OnClickOk()
    {
        if (audioOptions != null)
        {
            audioOptions.Save();
        }

        Close();
    }

    public void OnClickCancel()
    {
        if (audioOptions != null)
        {
            audioOptions.Cancel();
        }

        Close();
    }
}
