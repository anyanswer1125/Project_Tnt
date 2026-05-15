using UnityEngine;
using UnityEngine.SceneManagement;
public class ClearUIManager : MonoBehaviour
{
    [Header("UI 설정")]
    public GameObject clearPanel;     // 클리어 UI 전체 부모
    public Animator scrollAnimator;  // 종이가 펼쳐지는 애니메이션

    private bool isCleared = false;   // 중복 실행 방지

    private void Start()
    {
        // 시작할 때 클리어 창은 꺼둡니다.
        if (clearPanel != null) clearPanel.SetActive(false);
    }

    // 플레이어와 보물상자가 부딪혔을 때 실행
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isCleared) return; // 이미 클리어했다면 무시

        if (collision.CompareTag("Player"))
        {
            Debug.Log("보물 획득! 클리어!");
            isCleared = true;

            // 1. 클리어 창 켜기
            if (clearPanel != null) clearPanel.SetActive(true);

            // 2. 종이 펼치기 애니메이션 재생
            if (scrollAnimator != null)
            {
                scrollAnimator.Play("ClearScroll_Open"); // 만든 애니메이션 이름과 일치해야 함
            }
        }
    }

    private void Update()
    {
        // 클리어 창이 떴을 때만 키 입력을 받습니다.
        if (isCleared)
        {
            // Z키: 다음 스테이지 (현재 씬 번호 + 1)
            if (Input.GetKeyDown(KeyCode.Z))
            {
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
                SceneManager.LoadScene(nextSceneIndex);
            }
            // X키: 홈(타이틀) 화면으로 (보통 0번 씬)
            else if (Input.GetKeyDown(KeyCode.X))
            {
                SceneManager.LoadScene("TitleScene"); // 본인의 타이틀 씬 이름으로 변경
            }
        }
    }
}
