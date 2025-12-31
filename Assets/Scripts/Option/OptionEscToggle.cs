using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionEscToggle : MonoBehaviour
{
    private static OptionEscToggle instance;

    [SerializeField] private OptionController optionController;

    [Header("Toggle Cooldown")]
    [SerializeField] private float toggleCooldown = 0.2f;

    private float lastToggleUnscaledTime = -999f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TryToggle();
        }
    }

    private void TryToggle()
    {
        if (optionController == null)
        {
            Debug.LogError("[OptionEscToggle] OptionController가 연결되지 않았습니다.");
            return;
        }

        if (Time.unscaledTime - lastToggleUnscaledTime < toggleCooldown)
            return;

        lastToggleUnscaledTime = Time.unscaledTime;

        // 옵션 패널의 active 상태를 보고 토글
        GameObject panel = GetOptionsPanel(optionController);
        if (panel == null)
        {
            Debug.LogError("[OptionEscToggle] OptionController의 optionsPanel을 찾지 못했습니다.");
            return;
        }

        if (panel.activeSelf) optionController.Close();
        else optionController.Open();
    }

    
    private GameObject GetOptionsPanel(OptionController controller)
    {
        return null;
    }
}
