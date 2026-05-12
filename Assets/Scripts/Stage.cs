using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Stage : MonoBehaviour
{
    int SceneCounter = 0;

    [Header("UI 연결")]
    public TextMeshProUGUI prevText;
    public TextMeshProUGUI nextText;
    public GameObject firstStageButton;

    [Header("설정")]
    public int currentStageIndex = 1;
    public int maxStageCount = 3;

    private Animator anim;

    void OnEnable()
    {
        if (firstStageButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstStageButton);
        }
    }
    IEnumerator Start()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(firstStageButton.gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelExit();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))// 어디로 이동하는지 삽입필요
        {
            currentStageIndex = SceneCounter;
            LoadSelectedStage();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow)) //나중에 애니메이션 넣으면 pressbutton 클래스로 이동
            SceneCounter++;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SceneCounter--;
        PressButton();
    }

    private void PressButton()// 버튼눌림
    {
        //anim = GetComponent<Animator>();
        //if (Input.GetKeyDown(KeyCode.LeftArrow))  // 눌렀을때 anim trigger 설정
        //    anim.SetTrigger("Prev");

        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //    anim.SetTrigger("Next");
    }

    private void LoadSelectedStage()
    {        
        SceneController controller = Object.FindFirstObjectByType<SceneController>();

        if (controller != null)
        {
            //index를 조정하세요.
            controller.LoadNextScene(currentStageIndex);
        }
    }

    public void CancelExit()
    {
        // 메인 메뉴의 하이픈 지시자를 다시 켜고 스테이지 패널 닫기
        if (MainMenu.instance != null)
        {
            MainMenu.instance.BackFromOption();
        }
        gameObject.SetActive(false);
    }
}