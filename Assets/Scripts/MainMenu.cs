using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("하이픈 지시자 부모 오브젝트")]
    public GameObject selectPoint;
    public void OnclickGameStart()
    {
        SceneManager.LoadSceneAsync(1);//map씬으로 이동
    }
    public void OnclickOption()
    {

    }
    public void OnclickNews()
    {
    }
    public void OnclickQuit()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 빌드된 게임에서 실행 중일 때
        Application.Quit();
#endif
    }
}
