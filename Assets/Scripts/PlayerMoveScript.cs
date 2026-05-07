using UnityEngine;
using System.Collections;

public class GridMovement : MonoBehaviour
{
    [SerializeField] private float moveDistance = 1.0f; // 이동 거리 (1칸)
    [SerializeField] private float moveTime;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    bool isMoving = false;

    void Update()
    {
        // Raycast Draw 
        

        if (!isMoving)
        {
            if (Input.GetKeyDown(KeyCode.W)) StartCoroutine(SmoothMove(Vector3.up));
            else if (Input.GetKeyDown(KeyCode.S)) StartCoroutine(SmoothMove(Vector3.down));
            else if (Input.GetKeyDown(KeyCode.A)) StartCoroutine(SmoothMove(Vector3.left));
            else if (Input.GetKeyDown(KeyCode.D)) StartCoroutine(SmoothMove(Vector3.right));
        }
    }

    IEnumerator SmoothMove(Vector3 direction)
    {
        animator.SetTrigger("MoveTrigger");
        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (direction * moveDistance);
        float elapsedTime = 0;

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isMoving = false;
    }
}