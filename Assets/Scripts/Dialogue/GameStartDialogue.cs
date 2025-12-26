using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartDialogue : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private DialogueDataSO startDialogue;

    private void Start()
    {
        if (dialogueManager == null || startDialogue == null) return;
        dialogueManager.StartDialogue(startDialogue);
    }
}
