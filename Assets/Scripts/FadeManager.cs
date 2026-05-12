using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
public class FadeManager : MonoBehaviour
{
    public Image fadeImage;
    public float fadeSpeed = 1f;

    void Start()
    {
        // 씬이 시작될 때 투명해지면서 화면이 나타남
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        float alpha = 1f;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        fadeImage.gameObject.SetActive(false);
    }
    // 다음 씬으로 가기 전 검게 변함 (Fade Out)
    public IEnumerator FadeOut(string sceneName)
    {
        float alpha = 0f;
        while(alpha<1f)
        {
            alpha += Time.deltaTime * fadeSpeed;
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
}
