using UnityEngine;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour // 보조 스크립트
{
    public static Score instance;
    private bool isWork = false;
    [SerializeField] private GameObject scorePanel;
    [SerializeField] public GameObject[] stars;

    // 애니메이션 이벤트나 외부에서 호출할 메서드
    public void ShowScorePanel()
    {
        gameObject.SetActive(true); // 패널 활성화
        isWork = true;
        Time.timeScale = 0f; // 게임 일시정지
    }
    public void CurrentStar()
    {
        int activeStar = 0;
        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null && stars[i].activeSelf) activeStar++;
        }
        string currentSceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt(currentSceneName + "_Stars", activeStar);
   
        string cleanName = currentSceneName.Replace("Map", ""); //공백제거
        string[] splitName = cleanName.Split('-');

        if (splitName.Length == 2)
        {
            int worldIndex = int.Parse(splitName[0]);   // 1
            int stageIndex = int.Parse(splitName[1]);   // 1

            // 데이터 저장 (메인메뉴에서 활용)
            PlayerPrefs.SetInt("LastClearedWorld", worldIndex);
            PlayerPrefs.SetInt("LastClearedStage", stageIndex);

            if (stageIndex < 2)
            {
                // 다음 스테이지 정보 계산 (예: 1-2)
                int nextStage = stageIndex + 1;
                PlayerPrefs.SetInt("NextStageToPlay", nextStage);
            }
            else
            {
                int nextStage = worldIndex + 1;
                PlayerPrefs.SetInt("NextStageToPlay", nextStage);
            }
        }
        PlayerPrefs.Save();
    }
    public void SaveCurrentStageData()//클리어시 저장 anim 끝난뒤 적용 코루틴 필요
    {
        string currentSceneName = SceneManager.GetActiveScene().name; // 예: "Map1-1"
        int starCount = 0;
        PlayerPrefs.SetInt(currentSceneName + "_Stars", starCount);
        PlayerPrefs.Save();
    }
    //public void GameClear()
    //{
    //    string currentSceneName = SceneManager.GetActiveScene().name;

    //    if (MainMenu.instance != null)
    //    {
    //        MainMenu.instance.ActivateClearPanel(currentSceneName);
    //    }
    //}
    public void NextScene()// 추 후 호출
    {
        
        if (PlayerPrefs.GetInt("LastClearedStage", 1)<2)
        {
            int world = PlayerPrefs.GetInt("LastClearedWorld", 1);
            int nextStage = PlayerPrefs.GetInt("LastClearedStage", 1) + 1;
            string nextSceneName = "Map" + world + "-" + nextStage;
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            int world = PlayerPrefs.GetInt("LastClearedWorld", 1)+1;
            int nextStage = PlayerPrefs.GetInt("LastClearedStage", 1);
            string nextSceneName = "Map" + world + "-" + nextStage;
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void Update()
    {
        if (!isWork) return;

        // 패널이 떴을 때만 작동하는 키 입력
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))//NextScenename바꿔야댐
        {
            ResumeGame();
            SceneManager.LoadScene("NextSceneName"); // 스테이지 이름 가져오는 메서드 필요
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResumeGame();
            SceneManager.LoadScene("Title");// 메인 메뉴로
        }
    }

    void ResumeGame()
    {
        Time.timeScale = 1f; // 시간 다시 흐르게 설정
        isWork = false;
    }
    void Awake() { instance = this; }
}