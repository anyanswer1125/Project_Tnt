using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] Player player;

    private void Start()
    {
        Initialize();
    }
    private void Initialize()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "GoalBox")
        {
            player.IsMoving(false);
            Destroy(gameObject);
        }

    }
}
