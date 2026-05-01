using UnityEngine;

public class PlayerMoveScript : MonoBehaviour
{
    private Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position = new Vector3(transform.position.x + 1, transform.position.y,transform.position.z);
        }
    }
}
