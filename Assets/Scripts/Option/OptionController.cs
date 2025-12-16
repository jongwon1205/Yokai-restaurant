using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionController : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;

    public void Open()
    {
        optionsPanel.SetActive(true);
    }

    public void Close()
    {
        optionsPanel.SetActive(false);
    }
}
