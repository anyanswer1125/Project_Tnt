using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel; 
    public GameObject settingsPanel; // 설정창
    private bool isPaused = false;
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC 눌림 감지됨!");
            if (isPaused) Resume();
            else Pause();
        }
    }
    public void Pause()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // 게임시간정지
        isPaused = true;
    }
    public void Resume()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // 게임시간 다시 재생
        isPaused = false;
    }
    public void Restart()
    {
        Time.timeScale = 1f; // 시간을 다시 돌려놓고 씬 로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitToMap()
    {
        Time.timeScale = 1f; // 시간을 다시 돌려놓고 이동
        SceneManager.LoadScene("LoadScene");
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true); //설정창 켜기

    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false); //설정창 닫기
    }
}
