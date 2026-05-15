using UnityEngine;

public class ClearTrigger : MonoBehaviour
{
    [Header("설정")]
    public string playerTag = "Player"; // 플레이어의 태그
    public Animator scrollAnimator;
    [SerializeField]public GameObject clearPanel;      // 띄울 UI 패널 (ClearPanel)
    private bool isTrigger = false;

    // 이 스크립트가 붙은 오브젝트에 무언가 닿으면 실행됩니다.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        clearPanel.SetActive(false);
        // 닿은 물체의 태그가 Player인지 확인
        if (collision.CompareTag(playerTag))
        {
            Debug.Log("스테이지 클리어! UI를 활성화합니다.");

            if (clearPanel != null)
            {
                clearPanel.SetActive(true);
            }
            if(scrollAnimator!=null)
            {
                scrollAnimator.Play("ClearScroll_Open");
            }

        }
    }
}
