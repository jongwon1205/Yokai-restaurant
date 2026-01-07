using System;
using System.Collections;
using UnityEngine;

public class FeverTimeManager : MonoBehaviour
{
    public static FeverTimeManager Instance { get; private set; }

    [Header("Fever Á¶°Ç")]
    [SerializeField] private int triggerCorrectCount = 3;
    [SerializeField] private float feverDuration = 8f;

    public bool IsFeverTime { get; private set; }
    public int CurrentCorrectCount { get; private set; }

    public float FeverDuration => feverDuration;

    public float FeverRemainingTime
    {
        get
        {
            if (!IsFeverTime) return 0f;
            return Mathf.Max(0f, feverEndTime - Time.time);
        }
    }

    public float FeverNormalized
    {
        get
        {
            if (!IsFeverTime) return 0f;
            if (feverDuration <= 0f) return 0f;
            return Mathf.Clamp01(FeverRemainingTime / feverDuration);
        }
    }

    public event Action<bool> OnFeverChanged;

    private Coroutine feverRoutine;

    private float feverEndTime;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCorrectDelivery(int amount = 1)
    {
        if (amount <= 0) return;
        if (IsFeverTime) return;

        CurrentCorrectCount += amount;

        if (CurrentCorrectCount >= triggerCorrectCount)
        {
            StartFever();
        }
    }

    private void StartFever()
    {
        if (feverRoutine != null)
            StopCoroutine(feverRoutine);

        feverRoutine = StartCoroutine(FeverRoutine());
    }

    private IEnumerator FeverRoutine()
    {
        IsFeverTime = true;
        CurrentCorrectCount = 0;

        feverEndTime = Time.time + feverDuration;

        OnFeverChanged?.Invoke(true);

        yield return new WaitForSeconds(feverDuration);

        IsFeverTime = false;
        OnFeverChanged?.Invoke(false);
        feverRoutine = null;
    }
}
