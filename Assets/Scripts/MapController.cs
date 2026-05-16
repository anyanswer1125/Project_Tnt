using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    [Header("--- Panels & Background ---")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject[] mapPanels; // 인스펙터의 Map1-1 ~ Map4-1 배열
    [SerializeField] private Image backgroundImage;

    [Header("--- Map Selection Settings ---")]
    [SerializeField] private Transform mapListParent; // 부모 (Maps)

    private List<MapData> loadedMapList = new List<MapData>();
    private Dictionary<string, Star_TurnTable> localStageDict = new Dictionary<string, Star_TurnTable>();
    private int _currentMapIdx = 0;

    // 에디터에 디자인해둔 UI 원래 위치를 저장할 변수
    private Vector3 originPos;

    private void Start()
    {
        // 1. 게임이 시작되자마자 에디터에 세팅해둔 완벽한 고정 위치를 백업합니다.
        if (mapListParent != null)
        {
            originPos = mapListParent.localPosition;
        }

        // JsonManager가 깨어나서 싱글톤 인스턴스가 등록될 때까지 안전하게 대기
        StartCoroutine(WaitAndInitSystem());
    }

    private IEnumerator WaitAndInitSystem()
    {
        while (JsonManager.Instance == null)
        {
            yield return null;
        }

        InitMapSystem();
        ReturnToMain();
    }

    private void InitMapSystem()
    {
        loadedMapList.Clear();

        // JsonManager의 비공개(private) 메서드인 LoadDataDictionary를 "stage" 파일명과 "MapName을 키로 쓰겠다"는 규칙으로 강제 호출
        var method = typeof(JsonManager).GetMethod("LoadDataDictionary", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (method != null)
        {
            var genericMethod = method.MakeGenericMethod(typeof(Star_TurnTable));
            System.Func<Star_TurnTable, int> dummyKey = data => 0; // 명시적 타입 일치를 위한 임시 규칙

            localStageDict = new Dictionary<string, Star_TurnTable>();

            for (int i = 0; i < mapPanels.Length; i++)
            {
                if (mapPanels[i] == null) continue;

                MapData newMap = new MapData();
                string cleanName = mapPanels[i].name.Trim();

                newMap.MapName = cleanName;   // "Map1-1"
                newMap.SceneName = cleanName; // "Map1-1"
                newMap.isCleared = false;            // 초기값

                loadedMapList.Add(newMap);
            }
        }
        Debug.Log("Json연결 및 씬 경로 패치 완료");
        RefreshMapUI();
    }
    // [새로운 스크립트(StageClearManager)가 직접 호출해 주는 함수]
    public void RegisterStageClear(string stageName)
    {
        foreach (GameObject panel in mapPanels)
        {
            if (panel != null && panel.name.Trim() == stageName.Trim())
            {
                // 해당 오브젝트에 부착된 StageDataController를 찾아 클리어 상태로 만듭니다.
                StageClearManager stageData = panel.GetComponent<StageClearManager>();
                if (stageData != null)
                {
                    stageData.SetClearStatus(true); // 스스로 별 오브젝트 활성화 지시
                }
                break;
            }
        }
    }

    private void Update()
    {
        // 메인 메뉴가 꺼져 있고, 로드된 맵이 있을 때만 화살표/엔터 입력 처리
        if (!mainMenuPanel.activeSelf && loadedMapList.Count > 0)
        {            
            HandleMapSelectionInput();
        }
    }
    // 메인 메뉴 컨트롤러 등 외부에서 버튼을 클릭했을 때 호출하는 함수  
    private void HandleMapSelectionInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _currentMapIdx = Mathf.Clamp(_currentMapIdx - 1, 0, loadedMapList.Count - 1);
            RefreshMapUI(); // 인덱스 변경 시 즉시 끄고 켜기
            Debug.Log($"⬅ 현재 선택된 맵 오브젝트: {loadedMapList[_currentMapIdx].MapName}");
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            _currentMapIdx = Mathf.Clamp(_currentMapIdx + 1, 0, loadedMapList.Count - 1);
            RefreshMapUI(); // 인덱스 변경 시 즉시 끄고 켜기
            Debug.Log($"➡ 현재 선택된 맵 오브젝트: {loadedMapList[_currentMapIdx].MapName}");
        }
        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            string targetScene = loadedMapList[_currentMapIdx].SceneName;
            Debug.Log($"엔터누르면 {targetScene} 씬 로드 시도!");            
            SceneManager.LoadScene(targetScene);
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMain();
        }
    }

    // 실제로 Map 오브젝트를 활성화/비활성화하고 하위 Score를 제어하는 핵심 메서드
    private void RefreshMapUI()
    {
        for (int i = 0; i < mapPanels.Length; i++)
        {
            if (mapPanels[i] == null) continue;

            // 인덱스(i == _currentMapIdx)의 게임오브젝트만 활성화
            bool isActiveStage = (i == _currentMapIdx);
            mapPanels[i].SetActive(isActiveStage);

            // 활성화된 오브젝트 하위에서 "Score" 자식을 찾아 클리어 여부에 따라 On/Off
            if (isActiveStage && i < loadedMapList.Count)
            {
                Transform scoreTr = mapPanels[i].transform.Find("Score");
                if (scoreTr != null)
                {
                    scoreTr.gameObject.SetActive(loadedMapList[i].isCleared);
                }
            }
        }
    }

    // --- 패널 제어 기능 ---
    private void AllPanelsOff()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        foreach (GameObject panel in mapPanels) if (panel != null) panel.SetActive(false);
    }

    // 외부 호출용 함수 1개 구조 유지 + 내부에 안전 지연 코루틴 탑재
    public void OpenMapPanel()
    {
        // 즉시 코루틴을 태워 이전 프레임의 마우스 클릭/엔터 입력을 흘려보냅니다.
        StartCoroutine(OpenMapPanelRoutine());
    }

    private IEnumerator OpenMapPanelRoutine()
    {
        yield return null;

        AllPanelsOff();
        _currentMapIdx = 0; // 진입 시 무조건 첫 번째 맵(Map1-1) 조준
        RefreshMapUI();     // 첫 번째 맵만 켜고 나머지 싹 끄기

        if (backgroundImage != null) backgroundImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        Debug.Log(" 맵 선택 패널 진입 완료 (유령 엔터키 방지 처리)");
    }

    public void ReturnToMain()
    {
        AllPanelsOff();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (backgroundImage != null) backgroundImage.color = Color.white;
        Debug.Log(" 메인 메뉴 화면");
    }
}