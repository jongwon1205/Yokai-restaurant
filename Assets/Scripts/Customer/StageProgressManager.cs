using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageProgressManager : MonoBehaviour
{
    public static StageProgressManager Instance;

    [Header("Goal")]
    [SerializeField] private int clearCustomerCount = 10;

    [Header("Fail Condition (불만족/실패 손님 제한)")]
    [SerializeField] private int maxFailCustomerCount = 3;

    [Header("UI")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private Button nextStageButton;

    [SerializeField] private GameObject failPanel;
    [SerializeField] private Button retryButton;

    [Header("Scene")]
    [SerializeField] private string nextSceneName;

    private int clearedCustomers;
    private int failedCustomers;

    private bool hasCleared;
    private bool hasFailed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (clearPanel != null)
            clearPanel.SetActive(false);

        if (failPanel != null)
            failPanel.SetActive(false);

        if (nextStageButton != null)
        {
            nextStageButton.onClick.RemoveListener(GoNextStage);
            nextStageButton.onClick.AddListener(GoNextStage);
        }

        if (retryButton != null)
        {
            retryButton.onClick.RemoveListener(RetryStage);
            retryButton.onClick.AddListener(RetryStage);
        }
    }

    public void AddClearedCustomer()
    {
        AddClearedCustomer(1);
    }

    public void AddFailedCustomer()
    {
        AddFailedCustomer(1);
    }

    // =========================
    // Core
    // =========================

    public void AddClearedCustomer(int amount)
    {

        if (hasCleared || hasFailed) return;
        if (amount <= 0) amount = 1; 

        clearedCustomers += amount;

        if (clearedCustomers >= clearCustomerCount)
        {
            hasCleared = true;
            ShowClearPanel();
        }
    }

    public void AddFailedCustomer(int amount)
    {

        if (hasCleared || hasFailed) return;
        if (amount <= 0) amount = 1; 

        failedCustomers += amount;

        if (failedCustomers >= maxFailCustomerCount)
        {
            hasFailed = true;
            ShowFailPanel();
        }
    }

    private void ShowClearPanel()
    {

        if (clearPanel != null)
            clearPanel.SetActive(true);

        if (failPanel != null)
            failPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    private void ShowFailPanel()
    {

        if (failPanel != null)
            failPanel.SetActive(true);

        if (clearPanel != null)
            clearPanel.SetActive(false);

        Time.timeScale = 0f;
    }

    public void GoNextStage()
    {

        if (hasFailed) return;

        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextIndex);
    }

    public void RetryStage()
    {

        Time.timeScale = 1f;

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public int ClearedCustomers => clearedCustomers;
    public int FailedCustomers => failedCustomers;
}
