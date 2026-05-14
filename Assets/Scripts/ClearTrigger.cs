using UnityEngine;

public class ClearTrigger : MonoBehaviour
{
    [Header("설정")]
    public string playerTag = "Player"; // 플레이어의 태그
    public GameObject clearPanel;      // 띄울 UI 패널 (ClearPanel)

    // 이 스크립트가 붙은 오브젝트에 무언가 닿으면 실행됩니다.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 닿은 물체의 태그가 Player인지 확인
        if (collision.CompareTag(playerTag))
        {
            Debug.Log("스테이지 클리어! UI를 활성화합니다.");

            if (clearPanel != null)
            {
                clearPanel.SetActive(true);
            }

            // 필요하다면 게임 일시정지 (선택 사항)
            // Time.timeScale = 0f;
        }
    }
}
