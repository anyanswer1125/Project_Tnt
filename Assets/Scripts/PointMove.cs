using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class PointMove : MonoBehaviour
{
    [Header("시각 요소")]
    public GameObject hyphenVisual;
    public GameObject optionVisual;
    public TextMeshProUGUI valueText; // 숫자나 "전체화면/창모드"가 표시될 곳

    [Header("설정")]
    public float yOffset = 0f;
    private bool isOptionMode = false;

    // 데이터 저장 변수
    private int sfxVolume = 60;
    private int bgmVolume = 60;
    private bool isFullScreen = true;

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        // 1. 위치 추적 및 흔들림 효과
        Vector3 targetPos = selected.transform.position;
        targetPos.y += yOffset + Mathf.Sin(Time.time * 10f) * 2f;
        transform.position = targetPos;

        // 2. 옵션 모드일 때 키 입력 처리
        if (isOptionMode)
        {
            HandleOptionAdjustment(selected.name);
        }
    }

    void HandleOptionAdjustment(string buttonName)
    {
        bool left = Input.GetKeyDown(KeyCode.LeftArrow);
        bool right = Input.GetKeyDown(KeyCode.RightArrow);

        if (!left && !right) return;

        switch (buttonName)
        {
            case "Sound Music": // 효과음 (이미지 기준 이름)
                sfxVolume = Mathf.Clamp(sfxVolume + (left ? -10 : 10), 0, 100);
                valueText.text = sfxVolume.ToString();
                break;

            case "Background Music": // 배경음
                bgmVolume = Mathf.Clamp(bgmVolume + (left ? -10 : 10), 0, 100);
                valueText.text = bgmVolume.ToString();
                break;

            case "Mode": // 전체화면/창모드
                isFullScreen = !isFullScreen; // 좌우 상관없이 토글
                valueText.text = isFullScreen ? "전체화면" : "창모드";
                // 실제 화면 모드 적용 (빌드 후 작동)
                Screen.fullScreen = isFullScreen;
                break;
            case "Save":
                //SaveAndClose();
                break;
        }
    }

    // 옵션 모드 진입/퇴장 (OnClick 연결용)
    public void SetOptionMode(bool active)
    {
        isOptionMode = active;
        hyphenVisual.SetActive(!active);
        optionVisual.SetActive(active);

        if (active)
        {
            // 진입 시 현재 선택된 버튼의 값을 즉시 표시
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null) UpdateInitialValue(selected.name);
        }
    }

    // 세이브 버튼 전용 함수 (OnClick 연결용)
    public void SaveAndClose(GameObject optionPanel)
    {
        // 1. 데이터 저장 (PlayerPrefs 사용)
        PlayerPrefs.SetInt("SFX", sfxVolume);
        PlayerPrefs.SetInt("BGM", bgmVolume);
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
        PlayerPrefs.Save();

        Debug.Log("설정 저장 완료!");

        // 2. 옵션 모드 해제 및 창 닫기
        SetOptionMode(false);
        optionPanel.SetActive(false);

        // 3. 메인 메뉴의 첫 번째 버튼으로 포커스 복구
        // EventSystem.current.SetSelectedGameObject(firstButton); // 필요시 추가 처음 버튼 표시
    }

    void UpdateInitialValue(string buttonName)
    {
        if (buttonName.Contains("Sound")) valueText.text = sfxVolume.ToString();
        else if (buttonName.Contains("Background")) valueText.text = bgmVolume.ToString();
        else if (buttonName.Contains("Mode")) valueText.text = isFullScreen ? "전체화면" : "창모드";
        else valueText.text = ""; // Save 버튼 등에서는 텍스트 비움
    }
}