using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GoalBox : MonoBehaviour
{
    [SerializeField] private GameObject treasureObj;
    [SerializeField] private GameObject clearPanel;//클리어시 활성화할 패널
    [SerializeField] private AudioSource audioSource;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Key")
        {
            Animator animator = GetComponent<ObjectMovement>().ObjAnimator;
            animator.SetTrigger("Goal");//클리어
        }
    }

    // 애니메이션 이벤트에서 호출
    private void Goal()
    {
        Vector2 goalPos = transform.position;
        goalPos.y -= 0.5f;
        // Instantiate(goalObj, goalPos, Quaternion.identity);
        treasureObj.transform.position = goalPos; 
        treasureObj.SetActive(true);
        

    }
    IEnumerator ClearAndWait()
    {
        // 클리어 애니메이션 넣기
        // Time.timeScale = 0.5f;

        //대기
        yield return new WaitForSeconds(2.0f);
        clearPanel.SetActive(true);
    }
    private void GameClear()//버튼 선택 메서드 맵패널 활성화된 상태로 갈지 다음씬으로 갈지
    {
        StartCoroutine(ClearAndWait());
        //대기상태 넣기
        if (Input.GetKeyDown(KeyCode.P))
            LoadNextScene();
        if (Input.GetKeyDown(KeyCode.Q))
            return;//맵패널 활성화된 상태
    }
    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        int nextSceneIndex = currentSceneIndex + 1;
        //다음씬 있는지 확인
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }

    }

    //간격 두고 클리어 이미지 표시 (활성화/비활성화) panel 사용
    //멈춤상태 적용 엔터 입력시 현재 선택된 씬 확인 +1씬 출력

    // 애니메이션 이벤트에서 호출
    private void DestroyGoalBox()
    {
        Destroy(gameObject);
    }
    public void PlaySfx_ChestOpen()
    {
        audioSource.Play();
    }
}
