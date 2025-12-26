using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DialogueUI ui;

    [Header("Typing (optional)")]
    [SerializeField] private bool useTypewriter = true;
    [SerializeField] private float typeInterval = 0.03f;

    private DialogueDataSO currentData;
    private int lineIndex;

    private bool isPlaying;
    private bool isTyping;
    private string currentFullText;
    private Coroutine typingRoutine;

    // [SerializeField] private PlayerMove playerMove; // 프로젝트 이동 스크립트 이름에 맞게 변경

    private void Update()
    {
        if (!isPlaying) return;

        if (Input.GetMouseButtonDown(0))
        {
            OnClickNext();
        }
    }

    public void StartDialogue(DialogueDataSO data)
    {
        if (data == null || data.lines == null || data.lines.Length == 0) return;

        currentData = data;
        lineIndex = 0;
        isPlaying = true;

        // 시스템 멈춤
        Time.timeScale = 0f;

        // if (playerMove != null) playerMove.SetInputLocked(true);

        ui.Show();
        ShowCurrentLine();
    }

    private void OnClickNext()
    {
        // 타이핑 중이면 -> 즉시 전체 표시
        if (isTyping)
        {
            FinishTypingInstant();
            return;
        }

        // 다음 줄로
        lineIndex++;

        if (currentData == null || lineIndex >= currentData.lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        DialogueLine line = currentData.lines[lineIndex];

        currentFullText = line.text;

        ui.SetLine(line.speakerName, "", line.portrait);

        if (!useTypewriter)
        {
            ui.SetBodyText(currentFullText);
            isTyping = false;
            return;
        }

        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = StartCoroutine(TypeRoutine(currentFullText));
    }

    private IEnumerator TypeRoutine(string fullText)
    {
        isTyping = true;

        for (int i = 0; i <= fullText.Length; i++)
        {
            ui.SetBodyText(fullText.Substring(0, i));
            yield return new WaitForSecondsRealtime(typeInterval);
        }

        isTyping = false;
        typingRoutine = null;
    }

    private void FinishTypingInstant()
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = null;

        ui.SetBodyText(currentFullText);
        isTyping = false;
    }

    public void EndDialogue()
    {
        isPlaying = false;
        currentData = null;
        lineIndex = 0;

        if (typingRoutine != null) StopCoroutine(typingRoutine);
        typingRoutine = null;
        isTyping = false;

        ui.Hide();

        // 시스템 재개
        Time.timeScale = 1f;

        // if (playerMove != null) playerMove.SetInputLocked(false);
    }
}
