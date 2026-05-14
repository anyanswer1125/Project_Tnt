using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    private int currentIndex = 1;
    private static bool isMapSelection = false;//선택하고 있는지

    [Header("화살표")]
    public GameObject selectPoint;

    [Header("연결할 패널 및 버튼")]
    public GameObject optionPanel;
    public GameObject soundMusicButton;
    public GameObject gameStartButton;

    [Header("Maps")]
    public GameObject mainMenuPanel;
    public GameObject[] mapPanels;

    public void OnclickGameStart()
    {
        mainMenuPanel.SetActive(false); // 메인 메뉴 숨기기
        StartCoroutine(MapSelectionDelayed());//대기
    }
    IEnumerator MapSelectionDelayed()//중요 없을 시 중복입력으로 처음 맵 등장하자마자 씬 실행
    {
        yield return null;
        isMapSelection = true;// 맵 선택 모드 활성화
        currentIndex = 0;
        UpdatePanels();
    }
    public void OnclickOption()
    {
        Debug.Log("OnclickOption");
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
    void UpdatePanels()
    {
        for (int i = 0; i < mapPanels.Length; i++)
        {
            // 현재 인덱스만 켜고 나머지는 모두 끔
            mapPanels[i].SetActive(i == currentIndex);
        }
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
    void StartMap()
    {
        GameObject currentPanel = mapPanels[currentIndex];

        MapController map = currentPanel.GetComponent<MapController>();
        if (map != null)
        {
            map.enabled = true;
        }
        // 패널 = 씬 이름 동일하게
        SceneManager.LoadScene(currentPanel.name);
    }
    // 좌우 버튼에 연결하거나 키 입력으로 실행
    public void ChangePage(int direction)
    {
        currentIndex = Mathf.Clamp(currentIndex + direction, 0, mapPanels.Length - 1);
        UpdatePanels();
    }
    void Start()
    {
        // 다른 씬에서 넘어올 때 isMapSelection이 true로 설정되어 있다면
        if (isMapSelection)
        {
            mainMenuPanel.SetActive(false); // 메인 타이틀 메뉴 숨기기
            currentIndex = 0; // 혹은 마지막으로 플레이한 인덱스
            UpdatePanels(); // 맵 패널들 활성화
        }
    }
    void Update()
    {
        // 맵 선택 모드일 때만 키보드 입력 체크
        if (isMapSelection)
        {
            //좌우 방향키로 패널 교체
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ChangePage(1);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ChangePage(-1);
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                StartMap();
            }
        }

    }
    void Awake() { instance = this; }
}
