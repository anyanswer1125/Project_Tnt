using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{    
    public static MainMenuController instance;
    private int currentIndex = 0;
    private static bool isMapSelection = false;//선택하고 있는지

    [Header("화살표")]
    public GameObject selectPoint;

    [Header("연결할 패널 및 버튼")]
    public GameObject optionPanel;
    public GameObject soundMusicButton;
    public GameObject gameStartButton;    
    public GameObject mainMenuPanel;

    public MapController mapController;
    public void OnclickGameStart()
    {
        if (mapController != null)
        {
            mapController.OpenMapPanel();
        }
        else
        {
            Debug.LogError("인스펙터에서 MapController를 연결해주세요!");
        }
    }
    public void OnclickOption()
    {
        //패널 활성화
        if (optionPanel != null) optionPanel.SetActive(true);

        if (selectPoint != null) selectPoint.SetActive(false);

        //옵션의 첫 번째 버튼으로 포커스 이동
        if (soundMusicButton != null)
        {
            EventSystem.current.SetSelectedGameObject(soundMusicButton);
        }
    }
    public void OnclickNews()
    {
    }
    public void OnclickQuit()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 빌드된 게임에서 실행 중일 때
        Application.Quit();
#endif
    }
    
    public void BackFromOption()
    {
        // 1. 하이픈 지시자 다시 켜기
        if (selectPoint != null) selectPoint.SetActive(true);

        // 2. 커서가 다시 움직이도록 메인 메뉴 버튼 선택
        if (gameStartButton != null)
        {
            EventSystem.current.SetSelectedGameObject(gameStartButton);
        }
    }    
    void Start()
    {
        if (isMapSelection)
        {
            mainMenuPanel.SetActive(false); // 메인 타이틀 메뉴 숨기기
            currentIndex = 0; // 혹은 마지막으로 플레이한 인덱스
        }
        
    }
    void Awake() { instance = this; }
}
