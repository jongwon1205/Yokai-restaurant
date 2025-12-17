using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Stage Goals")]
    public int clearPassCount = 10;  // 이 수만큼 '정상 퇴장(지나감)' 하면 클리어
    public int failMissLimit = 3;    // 이 수만큼 '놓침' 발생하면 실패

    [Header("Result UI")]
    public GameObject clearPanel;
    public GameObject failPanel;

    public int passCount { get; private set; }
    public int missCount { get; private set; }

    private bool isEnded;

    private void Start()
    {
        if (clearPanel != null) clearPanel.SetActive(false);
        if (failPanel != null) failPanel.SetActive(false);
    }

    public void AddPass()
    {
        if (isEnded) return;

        passCount++;
        CheckResult();
    }

    public void AddMiss()
    {
        if (isEnded) return;

        missCount++;
        CheckResult();
    }

    private void CheckResult()
    {
        if (passCount >= clearPassCount)
        {
            EndStage(true);
            return;
        }

        if (missCount >= failMissLimit)
        {
            EndStage(false);
            return;
        }
    }

    private void EndStage(bool isClear)
    {
        isEnded = true;

        if (isClear)
        {
            if (clearPanel != null) clearPanel.SetActive(true);
        }
        else
        {
            if (failPanel != null) failPanel.SetActive(true);
        }

        //게임 멈춤
        Time.timeScale = 0f;
    }
}
