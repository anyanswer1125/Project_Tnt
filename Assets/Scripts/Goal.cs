using Unity.VisualScripting;
using UnityEngine;

public class Goal : MonoBehaviour
{

    [SerializeField] Player player;

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
        Player player = collision.GetComponent<Player>();
        if (player == null) return;


        // goalTimelineObj.SetActive(true);
        player.SetState(State.Win);
    }
}
