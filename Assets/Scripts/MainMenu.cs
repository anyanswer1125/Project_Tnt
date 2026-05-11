using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    [Header("하이픈 지시자")]
    public GameObject selectPoint;

    [Header("연결할 패널 및 버튼")]
    // 이 부분들이 선언되어 있어야 컨텍스트 오류가 나지 않습니다.
    public GameObject optionPanel;       // 옵션 패널 오브젝트
    public GameObject soundMusicButton;  // 옵션 내의 'SoundMusic' 버튼
    public GameObject gameStartButton;   // 메인 메뉴의 'GameStart' 버튼

    public void OnclickGameStart()
    {
        SceneManager.LoadSceneAsync(1);//map씬으로 이동
    }
    public void OnclickOption()
    {
        // 1. 패널 활성화
        if (optionPanel != null) optionPanel.SetActive(true);

        // 2. 하이픈 지시자 끄기
        if (selectPoint != null) selectPoint.SetActive(false);

        // 3. 옵션의 첫 번째 버튼으로 포커스 이동
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
    void Awake() { instance = this; }
}
