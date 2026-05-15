using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MapController : MonoBehaviour
{
    private int currentIndex = 1;
    private string MapName;
    private static bool isMapSelection = false;//선택하고 있는지
    [Header("Maps")]
    [SerializeField] public TextMeshProUGUI textMesh;
    public GameObject[] mapPanels;
    public GameObject mainMenuPanel;
    public GameObject mapsParent;
    // 메인 메뉴 컨트롤러에서 호출할 핵심 함수

    // [수정] 이름을 찾지 말고, 인스펙터에서 직접 드래그해서 넣어주세요.
    public GameObject map1_1;
        
    public void StartGameWithMap(string mapName)
    {
        mainMenuPanel.SetActive(false);

        if (map1_1 != null)
        {
            map1_1.SetActive(true);
            Debug.Log("Map1-1 활성화 성공");
        }
        else
        {
            Debug.LogError("Map1-1 오브젝트를 Maps 부모 밑에서 찾을 수 없습니다!");
        }
    }
    //현재 맵 이름 기억
    IEnumerator MapSelectionReady()
    {
        yield return null;
        Debug.Log("준비");
    }

    IEnumerator MapSelectionTransition(string mapName)
    {
        // 2. 메인 메뉴 비활성화
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        // 3. 모든 맵 자식들 일단 끄기 (초기화)
        foreach (Transform child in mapsParent.transform)
        {
            child.gameObject.SetActive(false);
        }
        // 4. 요청된 맵(map1-1)만 활성화
        Transform targetMap = mapsParent.transform.Find(mapName);
        if (targetMap != null)
        {
            targetMap.gameObject.SetActive(true);
        }
        // 5. 중복 입력 방지를 위한 한 프레임 대기
        yield return null;
        if(isMapSelection == true)
            Debug.Log(mapName + " 활성화 완료");
    }

    private void OnEnable()
    {
        GetName();
    }
    // [핵심] 어디서든 이 함수를 호출하면 패널이 교체됨
    public void ChangePanel(string targetMapName)
    {
        // 1. 메인 메뉴를 비활성화 (이후에도 MapManager는 살아있음)
        mainMenuPanel.SetActive(false);

        // 2. 모든 맵 패널을 일단 끔 (자식들을 순회하며 비활성화)
        foreach (Transform child in mapsParent.transform)
        {
            child.gameObject.SetActive(false);
        }

        // 3. 원하는 맵 패널만 활성화
        Transform target = mapsParent.transform.Find(targetMapName);
        if (target != null)
        {
            target.gameObject.SetActive(true);
        }
    }
    public void ReturnToMain()
    {
        ChangePanel("mainmenu");
    }
    void GetName()
    {
        // 1. 전체 텍스트를 가져옴
        string fullText = textMesh.text;

        // 2. 줄바꿈(\n)을 기준으로 나눔
        string[] lines = fullText.Split('\n');

        // 3. 첫 번째 줄(index 0)을 선택하고, 이전 답변처럼 하이픈과 공백 제거
        MapName = lines[0].Replace("-", "").Trim();

        Debug.Log("추출된 결과: " + MapName); // 출력: 1-1
    }
    public void LoadThisScene()
    {
        GetName();
        if (!string.IsNullOrEmpty(MapName)) SceneManager.LoadScene("Map" + MapName);
        else Debug.LogError("맵 이름 없음");
    }
    void UpdatePanels()
    {
        for (int i = 0; i < mapPanels.Length; i++)
        {
            // 현재 인덱스만 켜고 나머지는 모두 끔
            mapPanels[i].SetActive(i == currentIndex);
        }
    }
    public void ChangePage(int direction)
    {
        currentIndex = Mathf.Clamp(currentIndex + direction, 0, mapPanels.Length - 1);
        UpdatePanels();
    }
    private void Update()
    {
        //맵 선택 모드일 때만 키보드 입력 체크
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
                LoadThisScene();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                mapPanels[currentIndex].SetActive(false);
                mainMenuPanel.SetActive(true);
                isMapSelection = false;
            }
        }
    }
}
