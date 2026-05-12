using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Unity.Android.Gradle.Manifest;
public class MapPreviewUI : MonoBehaviour
{
    public GameObject panel;    // MapPreviewPanel 오브젝트
    public Text stageTitle;     // 스테이지 제목 텍스트
    public Image stageScreenshot; // 스테이지 미리보기 이미지
    private string targetSceneName;

    // 스테이지 버튼을 클릭 했을 때 호출
    public void Open(LevelData data)
    {
        targetSceneName = data.sceneName; // 이동할 씬 이름 저장

        // 덷이터에 이미지ㅣ가 등록되어 있다면 그 이미지를 보여줌
        if (data != null && stageScreenshot != null && data.mapPreviewSprite != null)
        {
            stageScreenshot.sprite = data.mapPreviewSprite;
        }
        stageTitle.text = data.levelName;
        

        panel.SetActive(true); // 팝업창 켜기
    }

    public void OnStartButtonClick()
    {
        FadeManager fade =Object.FindAnyObjectByType<FadeManager>();
        if(fade !=null)
        {
            StartCoroutine(fade.FadeOut(targetSceneName));
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);
        }
    }
    public void Close()
    {
        panel.SetActive(false);
    }
   
}

