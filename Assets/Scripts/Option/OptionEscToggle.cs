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
        // ✅ 중복 생성 방지 (여러 개 있으면 하나만 남김)
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

        // ✅ 중복 호출/연속 입력 방지 (TimeScale 0에서도 동작)
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

    // OptionController를 수정하지 않기 위해, reflection 없이 "직접 참조" 방식으로 바꾸는 게 제일 깔끔하지만
    // 지금은 OptionController 내부가 private라 접근이 안 되므로, 아래처럼 "옵션 패널을 직접 연결"하는 방식을 추천함.
    private GameObject GetOptionsPanel(OptionController controller)
    {
        // ❗ OptionController를 그대로 두겠다면,
        // 이 함수는 사용하지 말고 아래 '옵션 패널 직접 연결' 방식으로 바꾸는게 안전함.
        return null;
    }
}
