using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.0f; // 페이드 속도 조절

    private void Awake()
    {
        // 이 오브젝트가 포함된 캔버스가 씬이 바뀌어도 사라지지 않게 합니다.
        DontDestroyOnLoad(transform.root.gameObject);
    }

    // 외부에서 씬을 바꿀 때 이 메서드를 호출합니다
    public void LoadNextScene(int sceneIndex)
    {
        StartCoroutine(FadeAndLoad(sceneIndex));
    }

    private IEnumerator FadeAndLoad(int sceneIndex)
    {
        Debug.Log($"{sceneIndex}번");//오류 호출 씬 번호를 넘는지 확인
        //  페이드 아웃

        yield return StartCoroutine(Fade(1));

        // 실제 씬 로드
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneIndex);
        if (op == null)
        {
            Debug.LogError($"씬 로드 실패! Build Settings에 인덱스 {sceneIndex}인 씬이 있는지 확인하세요.");
            yield break; // 코루틴 중단
        }
        while (!op.isDone) yield return null;//한 프레임 로딩 대기

        //  페이드 인
        yield return StartCoroutine(Fade(0));
    }

    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}