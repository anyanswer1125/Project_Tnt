using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [Header("UI Root")]
    public GameObject mainMenuPanel;    // 메인 메뉴 패널 (처음 화면)
    public GameObject OptionMenuPanel;    // 메인 메뉴 패널 (처음 화면)
    public GameObject mapsParent;       // 'maps' 오브젝트 (자식으로 map1-1, map1-2 등이 있음)

    private GameObject currentMapSet;   // 현재 활성화된 맵 세트 (map1-1 등)를 기억
    private string selectedSceneName;   // Enter를 눌렀을 때 이동할 씬 이름
    private bool isReadyToStart = false; // 씬 진입 대기 상태 여부

    void Start()
    {
        // 1. 저장된 클리어 데이터를 불러옴 (함수 하단 정의)
        LoadClearStatus();
        // 2. 초기 상태로 map1-1를 보여줌 

    }

    void Update()
    {
        // 3. ESC 키를 누르면 현재 맵 세트를 끄고 메인 메뉴로 복귀함
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPanel("mainmenu");
        }

        // 4. 맵 세트가 켜진 상태에서 Enter를 누르면 저장된 씬으로 이동함
        if (isReadyToStart && (Input.GetKeyDown(KeyCode.Return) || (Input.GetKeyDown(KeyCode.KeypadEnter))))
        {
            SceneManager.LoadScene(selectedSceneName);
        }
    }

    // 모든 패널 전환을 담당하는 핵심 함수
    public void ShowPanel(string targetName)
    {
        // 먼저 메인 메뉴 패널을 비활성화함
        mainMenuPanel.SetActive(false);
        OptionMenuPanel.SetActive(false);

        // 현재 켜져 있는 맵 세트가 있다면 비활성화함 (7개 중 무엇이든 상관없음)
        if (currentMapSet != null)
        {
            currentMapSet.SetActive(false);
        }



        if (isReadyToStart == false)
        {
            currentMapSet = null; // 활성화된 맵 세트 정보 초기화

            // maps(부모)의 자식들 중에서 targetName과 이름이 똑같은 오브젝트를 찾음
            Transform targetTransform = mapsParent.transform.Find(targetName);

            if (targetTransform != null)
            {
                // 찾은 맵 세트(예: map1-1)를 활성화함 (하위 패널, 버튼, TMP 등 모두 같이 켜짐)
                currentMapSet = targetTransform.gameObject;
                currentMapSet.SetActive(true);

                // 씬 이동을 위해 이름을 저장하고 대기 상태로 전환함
                selectedSceneName = targetName;
                isReadyToStart = true;
            }
            else
            {
                // 이름을 잘못 입력했거나 오브젝트가 없을 경우 경고 출력
                Debug.LogError($"'{targetName}'을(를) maps 자식 중에서 찾을 수 없습니다!");
            }
        }
    }
    // 클리어 여부를 저장된 항목에서 불러오기
    private void LoadClearStatus()
    {
        // PlayerPrefs 등을 이용해 각 맵의 클리어 상태를 불러올 수 있음
        // 예: PlayerPrefs.GetInt("map1-1_Cleared", 0);
        Debug.Log("모든 맵의 클리어 데이터를 불러왔습니다.");
    }

    // 게임 종료 함수
    public void ReturntoMenu()
    {
        // 어플리케이션을 종료함
        mainMenuPanel.SetActive(true);
    }
}