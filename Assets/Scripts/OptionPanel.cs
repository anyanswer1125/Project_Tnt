using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class OptionPanel : MonoBehaviour
{
    public static OptionPanel instance;

    [Header("UI 연결")]
    public TextMeshProUGUI SoundMusicText;
    public TextMeshProUGUI BackGroundMusicText;
    public TextMeshProUGUI ModeText;
    public GameObject soundMusicButton; // 인스펙터에서 SoundMusic 버튼을 드래그하세요.

    private int sfx = 60, bgm = 60;
    private bool isFull = true;

    void Awake()
    {
        instance = this;
    }

    // 옵션창이 활성화될 때마다 자동으로 실행되는 유니티 내장 함수
    void OnEnable()
    {
        // 1. 켜지자마자 SoundMusic 버튼을 선택 상태로 만듦
        if (soundMusicButton != null)
        {
            EventSystem.current.SetSelectedGameObject(soundMusicButton);
            UpdateValueDisplay("SoundMusic"); // 초기 값 표시
        }
    }

    void Update()
    {
        GameObject selected = EventSystem.current.currentSelectedGameObject;
        if (selected == null) return;

        // 좌우 방향키 입력
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Adjust(selected.name, -10);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Adjust(selected.name, 10);
    }

    void Adjust(string name, int amount)
    {
        switch (name)
        {
            case "SoundMusic":
                sfx = Mathf.Clamp(sfx + amount, 0, 100);
                SoundMusicText.text = sfx.ToString();
                break;
            case "BackGroundMusic":
                bgm = Mathf.Clamp(bgm + amount, 0, 100);
                BackGroundMusicText.text = bgm.ToString();
                break;
            case "Mode":
                // 전체화면/창모드는 한 번만 눌러도 바뀌게 (amount 무시)
                isFull = !isFull;
                ModeText.text = isFull ? "전체화면" : "창모드";
                Screen.fullScreen = isFull;
                break;
            default:
                break;
        }
    }

    // 버튼을 옮겨 다닐 때마다 값을 미리 보여주기 위한 함수
    public void UpdateValueDisplay(string name)
    {
        if (name == "SoundMusic") SoundMusicText.text = sfx.ToString();
        else if (name == "BackGroundMusic") BackGroundMusicText.text = bgm.ToString();
        else if (name == "Mode") ModeText.text = isFull ? "전체화면" : "창모드";
    }

    public void SaveAndExit()
    {
        PlayerPrefs.SetInt("SFX", sfx);
        PlayerPrefs.SetInt("BGM", bgm);
        PlayerPrefs.Save();

        // MainMenu.instance를 통해 메인 메뉴 오브젝트를 다시 켭니다.
        if (MainMenu.instance != null)
        {
            // 메인 메뉴 게임 오브젝트 자체를 활성화
            MainMenu.instance.gameObject.SetActive(true);
            // 하이픈과 포커스 조절 함수 호출
            MainMenu.instance.BackFromOption();
        }
        this.gameObject.SetActive(false); // 옵션 패널 끄기
    }

    public void CancelExit()
    {
        // 저장 없이 그냥 돌아가기
        MainMenu.instance.BackFromOption();
        gameObject.SetActive(false);
    }
}