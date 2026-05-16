using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 이동을 위해 필수
using TMPro;
public class StageButton : MonoBehaviour
{
    public LevelData data; //  데이터를 넣을 칸
    public TextMeshProUGUI levelText; // 스테이지 번호를 표시할 텍스트
    public GameObject LockIcon; // 잠금 오브젝트 넣을 칸
    public MapPreviewUI previewUI;

    //void OnEnable()
    //{
    //    RefreshUI();
    //}
    void Start()
    {
        if (data != null)
        {
            // levelText가 연결되었을 때만 글자를 바꾸도록 'if' 문을 추가합니다.
            if (levelText != null)
            {
                levelText.text = data.levelName;
            }

            if (LockIcon != null)
            {
                // 데이터와 isUnlock가 true면 자물쇠를 끄고
                // false면 자물쇠를 켭니다
                LockIcon.SetActive(!data.isUnlocked);
            }
        }

        //// 현재 씬의 이름이 StageSelect 라면
        //if (SceneManager.GetActiveScene().name == "StageSelect")
        //{
        //    // 게임용 뒤로가기 버튼 오브젝트 찾아서 비활성화
        //    GameObject gameBtn = GameObject.Find("MainMenuBackButton");
        //    if (gameBtn != null) gameBtn.SetActive(false);
        //}

    }

    //public void RefreshUI()
    //{
    //    if(data != null)
    //    {
    //      if(levelText !=null);
    //        {
    //            levelText.text = data.levelName;
    //        }

    //        if (LockIcon != null)
    //        {
    //            LockIcon.SetActive(!data.isUnlocked);
    //        }

    //        Button btn = GetComponent<Button>();
    //        if(btn !=null)
    //        {
    //            btn.interactable = data.isUnlocked;
    //        }
    //        if (data.levelName == "1") btn.interactable = true;
    //    }
    //}

    // 버튼을 눌렀을 때 실행될 함수
    public void OnClickButton()
    {
        if (data != null && data.isUnlocked && previewUI != null)
        {
            previewUI.Open(data); // 종이 띄우는 명령어

            //    // 페이드 매니저를 찾습니다.
            //    FadeManager fade = Object.FindAnyObjectByType<FadeManager>();

            //    if (fade != null)
            //    {
            //        // 페이드 아웃 후 데이터에 적힌 씬으로 이동!
            //        StartCoroutine(fade.FadeOut(data.sceneName));
            //    }
            //    else
            //    {
            //        UnityEngine.SceneManagement.SceneManager.LoadScene(data.sceneName);
            //    }
            //}
        }
    }
    public void GoToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoadScene"); // 메인 메뉴 씬 이름
    }
}
