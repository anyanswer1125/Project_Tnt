using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
// using Unity.Android.Gradle.Manifest;
public class MapPreviewUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject allClearStamp; //클리어 도장
    public Image[] starImage;        // 별 이미지 

    [Header("Sprites")]
    public Sprite fullStar;         // 채워진 별 스프라이트
    public Sprite emptyStar;        // 빈 별 스프라이트

    public GameObject panel;    // MapPreviewPanel 오브젝트
    public Text stageTitle;     // 스테이지 제목 텍스트
    public Image stageScreenshot; // 스테이지 미리보기 이미지
    private string targetSceneName;

    // 스테이지 버튼을 클릭 했을 때 호출
    public void Open(LevelData data)
    {
        targetSceneName = data.sceneName; // 이동할 씬 이름 저장
        stageTitle.text = data.levelName;

        // 덷이터에 이미지ㅣ가 등록되어 있다면 그 이미지를 보여줌
        if (data != null && stageScreenshot != null && data.mapPreviewSprite != null)
        {
            stageScreenshot.sprite = data.mapPreviewSprite;
        }
        stageTitle.text = data.levelName;

        // 저장된 별점이 있는지 확인해서 UI 그려줍니다
        UpdateStageUI(data.sceneName);
      
        

        panel.SetActive(true); // 팝업창 켜기
    }

    private void UpdateStageUI(string sceneName)
    {
        // 저장된 별점 로드 (씬 이름으로 키값으로 사용)
        string saveKey = sceneName + "_Stars";
        int savedStars = PlayerPrefs.GetInt(saveKey, 0);

        Debug.Log($"[불러오기 체크] 키: {saveKey} | 불러온 별점: {savedStars}");

        // 별 이미지 교체
        for (int i = 0; i < starImage.Length; i++)
        {
            if (starImage[i] == null) continue;// 비어있음 방지

            if (i < savedStars)
                // 1. 별을 획득했을 때: 밝게 보이기
                starImage[i].color = Color.white;
            else
                // 2. 별을 못 얻었을 때: 어둡고 투명하게 (빈 공간 느낌)
                starImage[i].color = new Color(0, 0, 0, 0.3f); // 점수가 없으면 어둡게
        }
            // 별이 1개라도 있다면 클리어 도장 활성화
            if (allClearStamp != null)
        { 
            allClearStamp.SetActive(savedStars > 0);
        }
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

