using UnityEngine;
using System.Collections;

public class GoalBox : MonoBehaviour
{
    [SerializeField] private GameObject goalObj;
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Key")
        {
            Animator animator = GetComponent<ObjectMovement>().ObjAnimator;
            animator.SetTrigger("Goal");
        }
    }

    // 애니메이션 이벤트에서 호출
    private void Goal()
    {
        Vector2 goalPos = transform.position;
        goalPos.y -= 0.5f;
        Instantiate(goalObj, goalPos, Quaternion.identity);

    }

    // 애니메이션 이벤트에서 호출
    private void DestroyGoalBox()
    {
        Destroy(gameObject);
    }
}
