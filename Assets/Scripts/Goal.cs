using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    public GameObject clearPannel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Key"))
        {
            Debug.Log("스테이지 클리어");

            // 다음 스테이지 잠금 해제
            LevelUnlocker unlocker = FindAnyObjectByType <LevelUnlocker>();
            if(unlocker != null)
            {
                unlocker.UnLockNextLevel();
            }

            if(clearPannel !=null)
            {
                clearPannel.SetActive(true);
            }
            // 열쇠를 상자 안으로 사라지게함
            collision.gameObject.SetActive(false);

            FadeManager fade = Object.FindAnyObjectByType<FadeManager>();
            if(fade !=null)
            {
                StartCoroutine(fade.FadeOut("StageSelect"));
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("StageSelect");
            }
        }
    }
  
}
