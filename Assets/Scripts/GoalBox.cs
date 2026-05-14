using UnityEngine;
using System.Collections;

public class GoalBox : MonoBehaviour
{
    [SerializeField] private GameObject treasureObj;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "Key")
        {
            Animator animator = GetComponent<ObjectMovement>().ObjAnimator;
            audioSource.Play();
            animator.SetTrigger("Goal");
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

    // 애니메이션 이벤트에서 호출
    private void DestroyGoalBox()
    {
        Destroy(gameObject);
    }
}
