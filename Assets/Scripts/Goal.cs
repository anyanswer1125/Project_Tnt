using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Goal : MonoBehaviour
{
    public LevelData nextStageData;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player == null) return;

        player.SetState(State.Win);

        // 1. 현재 씬의 이름을 가져옵니다.
        string currentSceneName = SceneManager.GetActiveScene().name;
        string saveKey = currentSceneName + "_Stars";

        // 2. 별점 데이터를 저장합니다 (MapPreviewUI에서 사용하는 이름 규칙과 맞춰야 함)
        // 일단 클리어 시 별 3개를 준다고 가정합니다.
        PlayerPrefs.SetInt(saveKey,3);
        PlayerPrefs.Save(); // 데이터 즉시 물리 저장

        Debug.Log($"[저장체크] 키: {saveKey} | 3점 저장함");

        if (nextStageData !=null)
        {
            nextStageData.isUnlocked = true;
            Debug.Log(nextStageData.levelName + "이 해금되었습니다!");
        }
        // 스테이지 화면으로 돌아가기 1.5초뒤
        Invoke("GoBackToSelect",1.5f);
    }
    void GoBackToSelect()
    {
        
        SceneManager.LoadScene("StageSelect");
    }
   
}

    
