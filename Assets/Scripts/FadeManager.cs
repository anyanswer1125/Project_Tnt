using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    public Animator fadeAnimator; // CircleFadeImage에 있는 애니메이터 연결
    public string fadeOutTrigger = "Out"; // 애니메이터에서 설정한 트리거 이름

    void Start()
    {
        // 시작할 때 화면이 열리는 애니메이션은 자동으로 실행되거나 
        // 여기서 별도의 트리거를 줄 수 있습니다.
    }

    // 다음 씬으로 가기 전 애니메이션 실행
    public IEnumerator FadeOut(string sceneName)
    {
        if (fadeAnimator != null)
        {
            // 1. 애니메이션 실행 (애니메이터에 설정된 'Out' 트리거 등 사용)
            fadeAnimator.SetTrigger(fadeOutTrigger);

            // 2. 애니메이션이 끝날 때까지 대기 (약 1초, 실제 애니메이션 길이에 맞춰 조절)
            yield return new WaitForSeconds(1.0f);
        }

        // 3. 씬 로드
        SceneManager.LoadScene(sceneName);
    }

    // 숫자로 로드하는 버전
    public IEnumerator FadeOut(int sceneIndex)
    {
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger(fadeOutTrigger);
            yield return new WaitForSeconds(1.0f);
        }
        SceneManager.LoadScene(sceneIndex);
    }
}