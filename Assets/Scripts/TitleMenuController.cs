using UnityEngine;
using UnityEngine.UI;

public class TitleMenuController : MonoBehaviour
{
    [SerializeField] private Button startbtn;
    [SerializeField] private Button optionsbtn;
    [SerializeField] private Button exitbtn;
    [SerializeField] private Transform arrowboths;
    [SerializeField] private bool selectMenu = false; // 메뉴를 선택했는 지 확인 ( 선택했다면 키 입력을 막을 용도)

    [SerializeField] private MapMeunController mapMeunController;
    [SerializeField] private OptionsMenuController OptionsMenuController;

    // 키보드 제어를 위한 변수들
    private Button[] menuButtons;       // 버튼들을 순서대로 담을 배열
    private int currentSelection = 0;   // 현재 선택된 버튼 인덱스
    private bool isMoved = false;       // 중복 입력 방지 플래그

    string titleMenupath = "1/";

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        startbtn = transform.Find(titleMenupath + "GameStartButton").GetComponent<Button>();
        optionsbtn = transform.Find(titleMenupath + "OptionsButton").GetComponent<Button>();
        exitbtn = transform.Find(titleMenupath + "GameEndButton").GetComponent<Button>();
        arrowboths = transform.Find(titleMenupath + "ArrowBoths");

        // 1. 버튼들을 배열로 묶어줍니다 (순서대로 위->아래 배치)
        menuButtons = new Button[] { startbtn, optionsbtn, exitbtn };

        mapMeunController = FindAnyObjectByType<MapMeunController>();
        mapMeunController.gameObject.SetActive(false);

        OptionsMenuController = FindAnyObjectByType<OptionsMenuController>();
        OptionsMenuController.gameObject.SetActive(false);

        // 클릭 이벤트 연결
        startbtn.onClick.AddListener(() => MapUIOn());       // 맵 기능
        optionsbtn.onClick.AddListener(() => OptionsUIOn()); // 옵션 기능
        exitbtn.onClick.AddListener(() => GameExit());       // 종료 기능

        // 2. 게임 시작 시 첫 번째 버튼(스타트)에 포커스를 줍니다.
        SelectButton(currentSelection);
    }

    // 키보드 위/아래 이동 및 스페이스바 선택 처리
    private void HandleKeyboardNavigation()
    {
        if (selectMenu) return; //메뉴를 선택했다면 종료

        float yInput = Input.GetAxisRaw("Vertical"); // W/S, 위/아래 방향키

        // 위/아래 이동 처리
        if (yInput != 0)
        {
            if (!isMoved)
            {
                if (yInput > 0) // 위로 이동
                {
                    currentSelection--;
                    if (currentSelection < 0) currentSelection = menuButtons.Length - 1;
                }
                else if (yInput < 0) // 아래로 이동
                {
                    currentSelection++;
                    if (currentSelection >= menuButtons.Length) currentSelection = 0;
                }

                SelectButton(currentSelection);
                isMoved = true;
            }
        }
        else
        {
            isMoved = false; // 키에서 손을 떼면 플래그 리셋
        }

        // 일반 엔터로 선택 처리
        if (Input.GetKeyDown(KeyCode.Return))
        {

            if (menuButtons.Length > 0)
            {
                selectMenu = true; // 메뉴를 선택했다고 표시하여 이후 키 입력을 막음
                // 현재 선택된 버튼의 onClick 이벤트를 코드로 실행!
                menuButtons[currentSelection].onClick.Invoke();
            }
        }
    }

    // UI 시각적 포커스 이동 함수
    private void SelectButton(int index)
    {
        if (menuButtons == null || menuButtons.Length == 0) return;

        // 유니티 UI 시스템이 해당 버튼을 '선택 상태(Selected)'로 만듭니다.
        menuButtons[index].Select();
        arrowboths.transform.localPosition = menuButtons[index].transform.localPosition;
    }

    // 맵UI 활성화
    private void MapUIOn()
    {
        mapMeunController.gameObject.SetActive(true);
        mapMeunController.Initialize();
        Debug.Log("맵 창 열기 로직");
    }

    // 옵션창 열기
    private void OptionsUIOn()
    {
        OptionsMenuController.gameObject.SetActive(true);
        Debug.Log("옵션 창 열기 로직");
    }

    // 게임 종료
    private void GameExit()
    {
        Debug.Log("게임 종료 버튼 클릭됨!");

#if UNITY_EDITOR
        // 유니티 에디터에서 테스트 중일 때 재생(플레이) 모드를 끕니다.
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // 실제 빌드된 게임 파일에서 게임을 종료합니다.
    Application.Quit();
#endif
    }

    public void ResetMenuSelection()
    {
        selectMenu = false; // 메뉴 선택 상태 초기화
        currentSelection = 0; // 첫 번째 버튼으로 선택 초기화
        SelectButton(currentSelection); // UI 포커스 이동
    }

    void Update()
    {
        HandleKeyboardNavigation();
    }
}
