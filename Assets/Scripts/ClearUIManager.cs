using UnityEngine;
using UnityEngine.SceneManagement;
public class ClearUIManager : MonoBehaviour
{
   void Update()
    {
        if(gameObject.activeSelf)
        {
            if(Input.GetKeyDown(KeyCode.Z)) // Z를 누르면 다음 레벨
            {
                GoToNextLevel();
            }
            else if(Input.GetKeyDown(KeyCode.X)) // X를 누르면 타이틀 화면으로
            {
                GoToTitle();
            }
        }
    }
  public void GoToNextLevel()
    {
        // 현재 스테이지 인덱스 + 1번 씬을 로드함
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // 다음 씬이 빌드 설정에 있는지 확인 후 로드
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("마지막 스테이지 입니다");
            GoToTitle(); // 마지막이면 타이틀로 보냅니다
        }
    }
    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
