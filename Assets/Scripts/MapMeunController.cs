using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapMeunController : MonoBehaviour
{
    [SerializeField] private List<Button> fullMapList;      //전체 맵 리스트
    [SerializeField] private List<Button> currentMapList;   //현재 가지고 있는 맵 리스트 (실질적으로 이 리스트를 씀)
    [SerializeField] private int mapCount = 1;            //내가 가져야 할 맵의 개수
    [SerializeField] private int currentSelection = 0;  //내가 선택한 맵의 인덱스
    [SerializeField] private TitleMenuController titleMenuController;
    [SerializeField] private Image arrowBothLeft;   // 왼쪽 화살표 이미지
    [SerializeField] private Image arrowBothRight;  //  오른쪽 화살표 이미지
    private bool isMoved = false; // 중복 입력 방지 플래그

    Color arrBothGoColor = new Color(1f, 1f, 1f, 1f); // 완전한 흰색 (넘길 수 있는 상태)
    Color arrBothStopColor = new Color(1f, 1f, 1f, 0.5f); // 반투명한 흰색(못넘기는 상태)

    // 초기화 함수
    public void Initialize()
    {
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

    // 맵 리스트에서 mapCount 개수만큼 맵을 가져와 currentMapList에 추가하는 함수
    private void GetMap()
    {
        currentMapList.Clear(); // 리스트 비움

        if (mapCount > fullMapList.Count)
        {
            Debug.LogWarning("mapCount가 fullMapList의 개수를 초과했습니다. mapCount를 fullMapList의 개수로 조정합니다.");
            mapCount = fullMapList.Count; // mapCount를 fullMapList의 개수로 조정
        }

        mapCount = Mathf.Clamp(mapCount, 1, fullMapList.Count); // mapCount가 fullMapList의 범위를 넘지 않도록 제한

        for (int i = 0; i < mapCount; i++)
        {
            currentMapList.Add(fullMapList[i]);

            int sceneIndex = i;
            sceneIndex += 1 ; // 스테이지 씬 번호는 1부터 시작한다고 가정 (0은 타이틀 씬)

            // 복사한 변수를 넘겨줍니다.
            currentMapList[i].onClick.AddListener(() => LoadScene(sceneIndex));
        }

        currentMapList[currentSelection].gameObject.SetActive(true); // 첫 번째 맵 활성화
        UpdateArrowStates();
    }

    // 키보드 왼쪽/오른쪽 이동 및 엔터 선택 처리
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
                        currentSelection = currentMapList.Count - 1; // 인덱스  범위 제한
                    }
                }
                else if (xInput < 0) // 왼쪽로 이동
                {
                    currentSelection--;
                    if (currentSelection < 0)
                    {
                        currentSelection = 0; // 인덱스 범위 제한
                    }

                }
                isMoved = true;

                MapSetActive(currentSelection);
                UpdateArrowStates();
            }
        }
        else
        {
            isMoved = false; // 키에서 손을 떼면 플래그 리셋
        }

        // 일반 엔터로 선택 처리
        if (Input.GetKeyDown(KeyCode.Return))
        {

            if (currentMapList.Count > 0)
            {
                // 현재 선택된 버튼의 onClick 이벤트를 코드로 실행!
                currentMapList[currentSelection].onClick.Invoke();
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            titleMenuController.ResetMenuSelection(); // 타이틀 메뉴로 돌아갈 때 다시 메뉴 선택 가능하도록 설정
            this.gameObject.SetActive(false);
        }
    }

    // 현재 선택된 맵 버튼 활성화, 나머지는 비활성화
    private void MapSetActive(int currentSelection)
    {
        for (int i = 0; i < currentMapList.Count; i++)
        {
            currentMapList[i].gameObject.SetActive(false);
        }

        currentMapList[currentSelection].gameObject.SetActive(true);
    }

    // 화살표 가용 상태에 따른 알파값 제어
    private void UpdateArrowStates()
    {
        // 예외 처리: 만약 리스트에 아무것도 없거나 1개 이하일 경우 양쪽 다 멈춤 처리
        if (currentMapList == null || currentMapList.Count <= 1)
        {
            if (arrowBothLeft != null) arrowBothLeft.color = arrBothStopColor;
            if (arrowBothRight != null) arrowBothRight.color = arrBothStopColor;
            return;
        }

        // 1. 왼쪽 화살표: 현재 가장 왼쪽(0번)이면 반투명, 아니면 진하게
        if (arrowBothLeft != null)
        {
            arrowBothLeft.color = (currentSelection == 0) ? arrBothStopColor : arrBothGoColor;
        }

        // 2. 오른쪽 화살표: 현재 가장 오른쪽(마지막 방 번호)이면 반투명, 아니면 진하게
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
