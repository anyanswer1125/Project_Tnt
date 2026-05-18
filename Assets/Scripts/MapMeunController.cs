using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapMeunController : MonoBehaviour
{
    [SerializeField] private List<Button> fullMapList;      // 전체 맵 리스트
    [SerializeField] private List<Button> currentMapList;   // 현재 선택할 수 있는 맵 리스트 (공개/해금된 맵 리스트의 의미)
    [SerializeField] private int mapCount = 1;            // 현재 해금된 맵의 총 개수
    [SerializeField] private int currentSelection = 0;  // 현재 선택된 맵의 인덱스
    [SerializeField] private TitleMenuController titleMenuController;
    [SerializeField] private Image arrowBothLeft;   // 왼쪽 화살표 이미지
    [SerializeField] private Image arrowBothRight;  // 오른쪽 화살표 이미지
    private bool isMoved = false; // 중복 입력 방지 플래그

    Color arrBothGoColor = new Color(1f, 1f, 1f, 1f); // 활성화 색상 (이동 할 수 있는 상태)
    Color arrBothStopColor = new Color(1f, 1f, 1f, 0.5f); // 비활성화 색상 (더이상 이동 불가)

    // 초기화 함수
    public void Initialize()
    {
        SoundManager.Instance.PlaySFX(104);
        fullMapList.AddRange(transform.Find("Map").GetComponentsInChildren<Button>(true));
        arrowBothLeft = transform.Find("ArrowBothLeft").GetComponent<Image>();
        arrowBothRight = transform.Find("ArrowBothRight").GetComponent<Image>();
        GetMap();
        titleMenuController = FindAnyObjectByType<TitleMenuController>();
    }

    private void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    // 맵 리스트에서 mapCount 개수만큼 현재 오픈된 맵을 currentMapList에 추가하는 함수
    private void GetMap()
    {
        currentMapList.Clear(); // 리스트 비우기

        if (mapCount > fullMapList.Count)
        {
            Debug.LogWarning("mapCount가 fullMapList의 개수를 초과했습니다. mapCount를 fullMapList의 개수로 맞춥니다.");
            mapCount = fullMapList.Count; // mapCount를 fullMapList의 개수로 제한
        }

        mapCount = Mathf.Clamp(mapCount, 1, fullMapList.Count); // mapCount가 fullMapList의 범위를 벗어나지 않도록 제한

        for (int i = 0; i < mapCount; i++)
        {
            currentMapList.Add(fullMapList[i]);

            int sceneIndex = i;
            sceneIndex += 1; // 빌드 인덱스 상 씬 번호가 1번부터 시작한다고 가정 (0번은 타이틀 씬)

            // 버튼에 씬 로드 이벤트를 바인딩합니다.
            currentMapList[i].onClick.AddListener(() => LoadScene(sceneIndex));
        }

        currentMapList[currentSelection].gameObject.SetActive(true); // 첫 번째 맵 활성화
        UpdateArrowStates();
    }

    // 키보드 좌/우 이동 및 선택, 취소 처리
    private void HandleKeyboardNavigation()
    {
        float xInput = Input.GetAxisRaw("Horizontal"); // A/D, 왼쪽/오른쪽 방향키

        // 왼쪽/오른쪽 이동 처리
        if (xInput != 0)
        {
            if (!isMoved)
            {
                if (xInput > 0) // 오른쪽 이동
                {
                    currentSelection++;
                    if (currentSelection > currentMapList.Count - 1)
                    {
                        currentSelection = currentMapList.Count - 1; // 인덱스 최대치 고정
                    }
                }
                else if (xInput < 0) // 왼쪽으로 이동
                {
                    currentSelection--;
                    if (currentSelection < 0)
                    {
                        currentSelection = 0; // 인덱스 최소치 고정
                    }
                }
                isMoved = true;

                MapSetActive(currentSelection);
                UpdateArrowStates();

                SoundManager.Instance.PlaySFX(102);
            }
        }
        else
        {
            isMoved = false; // 키보드에서 손을 떼면 플래그 해제
        }

        // 일반 엔터키로 선택 처리
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentMapList.Count > 0)
            {
                // 현재 선택된 버튼의 onClick 이벤트를 강제로 실행!
                currentMapList[currentSelection].onClick.Invoke();
                SoundManager.Instance.PlaySFX(103);
            }
        }

        // ESC 키로 뒤로가기 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            titleMenuController.ResetMenuSelection(); // 타이틀 메뉴로 돌아가서 버튼 셀렉트 상태를 다시 초기화하도록 설정
            this.gameObject.SetActive(false);
            SoundManager.Instance.PlaySFX(103);
        }
    }

    // 현재 선택된 맵 버튼 활성화, 이전/다음 맵 비활성화
    private void MapSetActive(int currentSelection)
    {
        for (int i = 0; i < currentMapList.Count; i++)
        {
            currentMapList[i].gameObject.SetActive(false);
        }

        currentMapList[currentSelection].gameObject.SetActive(true);
    }

    // 화살표 활성 상태에 따라 투명도 제어
    private void UpdateArrowStates()
    {
        // 예외 처리: 현재 리스트에 아무것도 없거나 1개만 있을 경우 화살표 둘 다 비활성화 처리
        if (currentMapList == null || currentMapList.Count <= 1)
        {
            if (arrowBothLeft != null) arrowBothLeft.color = arrBothStopColor;
            if (arrowBothRight != null) arrowBothRight.color = arrBothStopColor;
            return;
        }

        // 1. 왼쪽 화살표: 현재 가장 왼쪽(0번)이면 비활성화, 아니면 활성화
        if (arrowBothLeft != null)
        {
            arrowBothLeft.color = (currentSelection == 0) ? arrBothStopColor : arrBothGoColor;
        }

        // 2. 오른쪽 화살표: 현재 가장 오른쪽(끝 인덱스)이면 비활성화, 아니면 활성화
        if (arrowBothRight != null)
        {
            arrowBothRight.color = (currentSelection == currentMapList.Count - 1) ? arrBothStopColor : arrBothGoColor;
        }
    }

    private void Update()
    {
        HandleKeyboardNavigation();
    }
}
