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
        player.IsMoving(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player == null) return;

        player.SetState(State.Win);
    }
}
