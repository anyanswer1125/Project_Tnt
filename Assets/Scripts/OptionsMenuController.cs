using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 옵션 메뉴 항목 타입
public enum OptionType
{
    SFX,         // 효과음 볼륨
    BGM,         // 배경음 볼륨
    DisplayMode, // 화면 모드 (전체화면, 창모드 등)
    Save,        // 저장 후 닫기 버튼
    Cancel       // 취소 버튼
}

[System.Serializable]
public class OptionItem
{
    public OptionType type; // 옵션 항목 타입
    public TMP_Text valueText;  // 보여줄 Text
    public int currentIndex = 0; // 옵션의 현재 인덱스
    public List<string> valueOptions = new List<string>(); // 들어갈 내용의 리스트
}

public class OptionsMenuController : MonoBehaviour
{
    [Header("Option Items")]
    // 옵션 리스트
    [SerializeField] private List<OptionItem> optionItems = new List<OptionItem>();
    // 현재 선택한 항목 인덱스 (0부터 시작)
    [SerializeField] private int currentSelection = 0;

    [Header("Controllers & Arrows")]
    [SerializeField] private TitleMenuController titleMenuController;
    [SerializeField] private Image arrowLeft;  // 왼쪽 화살표 이미지.
    [SerializeField] private Image arrowRight; // 오른쪽 화살표 이미지
    [SerializeField] private Transform arrowParent; // 화살표 부모 오브젝트

    private bool isVerticalMoved = false;   // 위/아래 이동 중복 방지
    private bool isHorizontalMoved = false; // 좌/우 값 변경 중복 방지

    Color arrGoColor = new Color(1f, 1f, 1f, 1f);     // 갈 수 있는 상태 (활성화 상태)
    Color arrStopColor = new Color(1f, 1f, 1f, 0.3f); // 갈 수 없는 상태 (비활성화 상태)

    [Header("Saved Value Shortcuts (외부 대입용 변수)")]
    public int sfxSavedIndex = 0;          // 저장된 SFX value 값
    public int bgmSavedIndex = 0;          // 저장된 BGM value 값
    public int displayModeSavedIndex = 0;  // 저장된 화면모드 값

    // 초기화 함수
    public void Initialize()
    {
        titleMenuController = FindAnyObjectByType<TitleMenuController>();

        if (transform.Find("Arrow")) arrowParent = transform.Find("Arrow");

        // 부모를 찾았다면 자식 오브젝트도 찾음
        if (arrowParent != null)
        {
            if (arrowParent.Find("ArrowLeft")) arrowLeft = arrowParent.Find("ArrowLeft").GetComponent<Image>();
            if (arrowParent.Find("ArrowRight")) arrowRight = arrowParent.Find("ArrowRight").GetComponent<Image>();
        }

        InitOptionValues(); // 1. 각 옵션 항목에 들어갈 선택지 글자들(0~100, 전체화면 등)을 셋업합니다.
        LoadSettings();     // 저장된 값 불러오기
        UpdateOptionTexts();
        UpdateArrowStates();

        // 화살표 위치 초기화
        if (optionItems.Count > 0 && arrowParent != null)
        {
            arrowParent.localPosition = optionItems[currentSelection].valueText.transform.localPosition;
        }
    }

    // 각 옵션들 ValueOptions 값 초기화
    private void InitOptionValues()
    {
        foreach (var item in optionItems)
        {
            item.valueOptions.Clear();

            switch (item.type)
            {
                case OptionType.SFX:
                    // 0부터 100까지 10단위로 늘어남
                    for (int i = 0; i <= 100; i += 10)
                        item.valueOptions.Add(i.ToString());

                    item.currentIndex = item.valueOptions.Count - 1; // 초기값으로 100 설정
                    break;

                case OptionType.BGM:
                    // 0부터 100까지 10단위로 늘어남
                    for (int i = 0; i <= 100; i += 10)
                        item.valueOptions.Add(i.ToString());
                    item.currentIndex = item.valueOptions.Count - 1; // 초기값으로 100 설정
                    break;

                case OptionType.DisplayMode:
                    // 화면 선택지
                    item.valueOptions.Add("전체화면");
                    item.valueOptions.Add("전체창모드");
                    item.valueOptions.Add("창 모드");
                    item.currentIndex = 0; // 초기값을 전체화면으로 설정
                    break;

                case OptionType.Save:
                    item.valueOptions.Add("저장 후 닫기"); // 버튼처럼 쓸 것이므로 하나만 넣음
                    break;

                case OptionType.Cancel:
                    item.valueOptions.Add("취소"); // 버튼처럼 쓸 것이므로 하나만 넣음
                    break;
            }
        }
    }

    private void Update()
    {
        HandleKeyboardNavigation();
    }

    // 키보드 조작(위/아래 이동, 좌/우 값 변경, 엔터 선택)을 실시간으로 감지하고 처리하는 핵심 함수
    private void HandleKeyboardNavigation()
    {
        float yInput = Input.GetAxisRaw("Vertical");   // 키보드 W/S 버튼 또는 위/아래 방향키 값 (-1, 0, 1)
        float xInput = Input.GetAxisRaw("Horizontal"); // 키보드 A/D 버튼 또는 왼쪽/오른쪽 방향키 값 (-1, 0, 1)


        // 1. 위/아래 입력 처리 (메뉴 항목 변경)
        if (yInput != 0)
        {
            if (!isVerticalMoved)
            {
                if (yInput > 0) // 위쪽 방향키를 눌렀을 때
                {
                    currentSelection--;
                    if (currentSelection < 0) currentSelection = 0; // 인덱스 범위 조정
                }
                else if (yInput < 0) // 아래쪽 방향키를 눌렀을 때
                {
                    currentSelection++;
                    if (currentSelection > optionItems.Count - 1) currentSelection = optionItems.Count - 1; // 인덱스 범위 조정
                }

                // 현재 선택한 옵션 항목
                OptionItem currentItem = optionItems[currentSelection];

                // 화살표 위치 이동
                if (arrowParent != null) arrowParent.localPosition = currentItem.valueText.transform.localPosition;

                isVerticalMoved = true; // "이미 이동 처리했음" 플래그를 켜서 꾹 누르고 있어도 마구 스크롤되지 않게 막음
                UpdateArrowStates();   // 메뉴가 바뀌었으므로 좌우 화살표들의 투명도 상태도 다시 계산
            }
        }
        else
        {
            isVerticalMoved = false;
        }


        // 2. 좌/우 입력 처리
        if (xInput != 0) // 키보드 왼쪽이나 오른쪽을 누르고 있다면
        {
            if (!isHorizontalMoved && optionItems.Count > 0) // 마찬가지로 꾹 누르기 중복 방지
            {
                OptionItem currentItem = optionItems[currentSelection]; // 현재 고른 항목

                if (xInput > 0) // 오른쪽 방향키 (수치 증가)
                {
                    currentItem.currentIndex++;
                    if (currentItem.currentIndex > currentItem.valueOptions.Count - 1)
                    {
                        currentItem.currentIndex = currentItem.valueOptions.Count - 1; // 최대 인덱스
                    }
                }
                else if (xInput < 0) // 왼쪽 방향키 (수치 감소)
                {
                    currentItem.currentIndex--;
                    if (currentItem.currentIndex < 0) currentItem.currentIndex = 0; // 최소 인덱스
                }

                isHorizontalMoved = true; // 좌우 중복 입력 방지
                UpdateOptionTexts();   // 유저가 방 번호를 바꿨으니 화면에 나오는 글자도 바로 교체합니다.
                ApplyOptionSetting(currentItem); // [옵션 프리뷰] 화면 모드 같은 설정을 인게임에 즉시 미리보기로 반영합니다.
                UpdateArrowStates();   // 현재 값이 최소/최대치에 도달했는지 확인해 화살표 투명도를 조절합니다.
            }
        }
        else
        {
            isHorizontalMoved = false;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            EnterEvent(optionItems[currentSelection]);
        }
    }

    // 옵션의 문자 업데이트 함수: 각 옵션 항목의 valueText에 현재 선택된 인덱스에 해당하는 문자열을 대입하여 화면에 보이는 글자를 바꿈
    private void UpdateOptionTexts()
    {
        foreach (var item in optionItems)
        {
            if (item.valueText != null && item.valueOptions.Count > 0)
            {
                // 글자 컴포넌트에 현재 인덱스 방에 들어있는 문자열을 대입합니다 (예: "전체화면")
                item.valueText.text = item.valueOptions[item.currentIndex];
            }
        }
    }

    // 화살표 상태 업데이트 함수
    private void UpdateArrowStates()
    {
        if (optionItems == null || optionItems.Count == 0) return;

        OptionItem currentItem = optionItems[currentSelection];

        // 만약 선택할 수 있는 데이터 목록이 1개 이하일 경우 (예: '저장 후 닫기'나 '취소' 버튼 위에 커서가 있을 때)
        if (currentItem.valueOptions.Count <= 1)
        {
            // 좌우로 바꿀 값이 없으므로 왼쪽, 오른쪽 화살표 이미지를 모두 투명(비활성화 색상)하게 바꾼 뒤 함수를 바로 종료합니다.
            if (arrowLeft != null) arrowLeft.color = arrStopColor;
            if (arrowRight != null) arrowRight.color = arrStopColor;
            return;
        }

        // 왼쪽 화살표 처리: 현재 가장 첫 번째 방(0번 방)에 도달했다면 더 내려갈 곳이 없으므로 투명하게, 아니면 선명하게 만듭니다.
        if (arrowLeft != null)
        {
            arrowLeft.color = (currentItem.currentIndex == 0) ? arrStopColor : arrGoColor;
        }

        // 오른쪽 화살표 처리: 현재 가장 마지막 방에 도달했다면 더 올라갈 곳이 없으므로 투명하게, 아니면 선명하게 만듭니다.
        if (arrowRight != null)
        {
            arrowRight.color = (currentItem.currentIndex == currentItem.valueOptions.Count - 1) ? arrStopColor : arrGoColor;
        }
    }

    // 옵션 세팅을 실제 게임에 적용하는 함수
    private void ApplyOptionSetting(OptionItem item)
    {
        switch (item.type)
        {
            case OptionType.SFX:
                // TODO: 오디오 믹서나 오디오 매니저 스크립트를 이곳에 연동하여 효과음 볼륨을 조절하세요.
                break;

            case OptionType.BGM:
                // TODO: 오디오 믹서나 오디오 매니저 스크립트를 이곳에 연동하여 배경음 볼륨을 조절하세요.
                break;

            case OptionType.DisplayMode:
                switch (item.currentIndex)
                {
                    case 0: Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; break; // 전체화면 모드
                    case 1: Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;    // 테두리 없는 전체 창모드
                    case 2: Screen.fullScreenMode = FullScreenMode.Windowed; break;            // 일반 창모드
                }
                break;
        }
    }

    // 변경된 옵션 값을 저장하는 함수
    private void SaveSettings()
    {
        foreach (var item in optionItems)
        {
            switch (item.type)
            {
                case OptionType.SFX:
                    sfxSavedIndex = item.currentIndex;
                    if (int.TryParse(item.valueOptions[item.currentIndex], out int sfxVolume))
                        SoundManager.Instance.SetSfxVolume(sfxVolume);
                    break;
                case OptionType.BGM:
                    bgmSavedIndex = item.currentIndex;
                    if (int.TryParse(item.valueOptions[item.currentIndex], out int bgmVolume))
                        SoundManager.Instance.SetBgmVolume(bgmVolume);
                    break;
                case OptionType.DisplayMode:
                    displayModeSavedIndex = item.currentIndex;
                    break;
            }
        }

    }

    // 저정된 값을 불러오는 함수
    private void LoadSettings()
    {
        foreach (var item in optionItems)
        {
            switch (item.type)
            {
                case OptionType.SFX:
                    item.currentIndex = sfxSavedIndex;
                    break;
                case OptionType.BGM:
                    item.currentIndex = bgmSavedIndex;
                    break;
                case OptionType.DisplayMode:
                    item.currentIndex = displayModeSavedIndex;
                    break;
            }
            ApplyOptionSetting(item); // 저장된 값 전달
        }
        Debug.Log("OptionsMenu: 내부 int 변수값들을 기반으로 UI가 새로 로드");
    }

    // 엔터 이벤트 함수
    private void EnterEvent(OptionItem item)
    {
        switch (item.type)
        {
            case OptionType.Save:
                SaveSettings(); // 저장
                gameObject.SetActive(false);
                titleMenuController.ResetMenuSelection();
                break;

            case OptionType.Cancel:
                LoadSettings(); // 취소
                UpdateOptionTexts();
                gameObject.SetActive(false);
                titleMenuController.ResetMenuSelection();
                break;
        }
    }
}
