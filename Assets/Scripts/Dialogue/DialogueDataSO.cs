using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Dialogue/Dialogue Data", fileName = "DialogueData")]
public class DialogueDataSO : ScriptableObject
{
    public DialogueLine[] lines;
}

[Serializable]
public class DialogueLine
{
    public string speakerName;

    [TextArea(2, 4)]
    public string text;

    [Header("√ ªÛ»≠")]
    public Sprite portrait;
}
