using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FeverTimeUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;

    private void Start()
    {
        SetVisible(false);
    }

    private void Update()
    {
        FeverTimeManager fever = FeverTimeManager.Instance;
        if (fever == null)
        {
            SetVisible(false);
            return;
        }

        if (!fever.IsFeverTime)
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);

        float remaining = fever.FeverRemainingTime;
        if (timeText != null)
            timeText.text = $"FEVER {remaining:0.0}";
    }

    private void SetVisible(bool isOn)
    {
        if (timeText == null) return;
        
        if (timeText.gameObject.activeSelf != isOn)
            timeText.gameObject.SetActive(isOn);

        if (!isOn)
            timeText.text = string.Empty;
    }
}
