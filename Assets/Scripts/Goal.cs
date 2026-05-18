using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{

    [SerializeField] Player player;
    public GameObject clearPanel;
    public Animator scrollAnimator; // 종이 펼쳐지는 애니메이터

    private bool isCleared = false; // 클리어 상태인지 확인

    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        player = FindAnyObjectByType<Player>();
        // key와 goalbox가 서로 충돌되고 사라지고 나서 플레이어가 움직을 수 있도록 함
        player.IsMoving(false);
        Debug.Log("현재 플레이어 : " + player);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 이름표(Tag)가 Player인지 먼저 확인 (전사/도적/마법사 모두 해당)
        if (collision.CompareTag("Player") && TurnManager.instance.TurnCount > 0)
        {
            Debug.Log(collision.name + "가 골인했습니다!");

            isCleared = true;

                // 2. 판넬이 연결되어 있다면 켜주기
                if (clearPanel != null)
                {
                    clearPanel.SetActive(true);
                }

                // 3. 기존에 있던 Player 승리 로직 (에러 방지용)
                Player playerScript = collision.GetComponent<Player>();
                if (playerScript != null)
                {
                    playerScript.SetState(State.Win);
                }
            }
        }
    private void Update()
    {
        // 클리어 상태일 때만 키 입력을 받음
        if (isCleared)
        {
            // Z키를 누르면 다음 레벨로 (현재 씬 번호 + 1)
            if (Input.GetKeyDown(KeyCode.Z))
            {
                int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

                // 다음 씬이 빌드 설정에 있는지 확인 후 로드
                if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
                {
                    SceneManager.LoadScene(nextSceneIndex);
                }
                else
                {
                    Debug.Log("마지막 레벨입니다! 타이틀로 돌아갑니다.");
                    SceneManager.LoadScene(0); // 타이틀 씬 이름으로 변경하세요
                }
            }

            // X키를 누르면 타이틀 화면으로
            if (Input.GetKeyDown(KeyCode.X))
            {
                SceneManager.LoadScene(0); // 본인의 타이틀 씬 이름을 따옴표 안에 적으세요
            }
        }
    }
}




