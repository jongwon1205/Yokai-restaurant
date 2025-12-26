using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTestStarter : MonoBehaviour
{
    [SerializeField] private DialogueManager manager;
    [SerializeField] private DialogueDataSO data;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            manager.StartDialogue(data);
        }
    }
}
