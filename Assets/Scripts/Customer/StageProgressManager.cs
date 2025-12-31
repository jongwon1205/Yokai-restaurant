using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageProgressManager : MonoBehaviour
{
    [Header("Goal")]
    [SerializeField] private int clearCustomerCount = 10;

    [Header("UI")]
    [SerializeField] private GameObject clearPanel;
    [SerializeField] private Button nextStageButton;

    [Header("Scene")]
    [SerializeField] private string nextSceneName;

    private int clearedCustomers;
    private bool hasCleared;

    private void Awake()
    {
        if (clearPanel != null)
            clearPanel.SetActive(false);

        if (nextStageButton != null)
            nextStageButton.onClick.AddListener(GoNextStage);
    }

    public void AddClearedCustomer(int amount = 1)
    {
        if (hasCleared) return;

        clearedCustomers += amount;

        if (clearedCustomers >= clearCustomerCount)
        {
            hasCleared = true;
            ShowClearPanel();
        }
    }

    private void ShowClearPanel()
    {
        if (clearPanel != null)
            clearPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void GoNextStage()
    {
        Time.timeScale = 1f;

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        SceneManager.LoadScene(nextIndex);
    }
}
