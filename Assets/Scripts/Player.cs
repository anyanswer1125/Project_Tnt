using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.2f; // 이동에 걸리는 시간 (예: 0.2초)
    [SerializeField] private float jumpHeight = 0.1f; // 점프 높이
    [SerializeField] private Transform tr;
    [SerializeField] private LayerMask targetLay;
    // 키를 한번만 입력받기 위한 변수
    private bool isMoveing;

    Animator animator;

    Vector3 Pos => transform.position;

    private void Start()
    {
        animator = GetComponent<Animator>();
        tr = transform.Find("PlayerSprite");
    }


    IEnumerator Movement(Vector3 dir)
    {
        // 이미지 전환
        if (dir.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (dir.x > 0)
            transform.localScale = new Vector3(1, 1, 1);

        // 이동 되는 동안 키를 입력받지 않게 true
        isMoveing = true;
        //animator.SetBool("Run", isMoveing);
        // 내가 움직일 거리 (내 위치 + 움직일 방향)
        Vector3 targetPos = Pos + dir;

        float elapsedTime = 0f;// 경과 시간 초기화
        // 경과 시간이 설정한 이동 시간(duration)보다 작을 동안 반복
        while (elapsedTime < moveDuration)
        {
            float progress = elapsedTime / moveDuration; // 0에서 1까지 진행률
            // Lerp(시작, 끝, 비율): 비율(0~1)에 따라 위치를 결정함
            // 경과 시간을 전체 이동 시간으로 나누어 비율을 계산 (예: 0.1초 / 0.2초 = 0.5)
            // 평면 이동 (X, Z축)
            Vector3 currentPos = Vector3.Lerp(Pos, targetPos, progress);
            // 높이 이동 (Y축): 사인 곡선을 이용해 0 -> 1 -> 0으로 변함
            float yOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            currentPos.y += yOffset;

            transform.position = currentPos;
            // 매 프레임 시간을 더해줌
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 오차범위에 도달하면 위치를 보정함
        transform.position = targetPos;
        // 다음 키를 입력받기 위해서 false
        isMoveing = false;
        //animator.SetBool("Run", isMoveing);
    }

    bool CanMove(Vector2 dir)
    {
        // 1. 레이 시작 지점: 내 몸(캐릭터)에 닿지 않게 이동 방향으로 0.6f만큼 밀기
        Vector2 rayStart = (Vector2)transform.position;

        // 2. 레이 쏘기 (거리 1f)
        RaycastHit2D hit = Physics2D.Raycast(rayStart, dir, 1f);

        // 3. 디버그 로그 추가
        if (hit.collider != null)
        {
            // 무언가에 부딪혔을 때: 부딪힌 대상의 이름과 레이어 출력
            Debug.Log($"<color=red>[막힘]</color> {hit.collider.name} (레이어: {LayerMask.LayerToName(hit.collider.gameObject.layer)}) : {hit.collider.gameObject.layer}");

        }
        else
        {
            // 아무것도 없을 때
            Debug.Log("<color=green>[통과]</color> 앞이 비어있습니다. 이동 가능!");
        }

        // 4. 씬(Scene) 뷰 시각화 (빨간색=막힘, 녹색=통과)
        Debug.DrawRay(rayStart, dir * 1f, hit.collider != null ? Color.red : Color.green, 0.5f);

        // 결과 반환: 부딪힌 게 없어야 true(이동 가능)
        return hit.collider == null;
    }


    void Update()
    {
        if (!isMoveing)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");


            // 대각선을 방지하기 위한 조건문
            if (x != 0 && CanMove(new Vector3(x, 0, 0)))
                StartCoroutine(Movement(new Vector3(x, 0, 0)));
            else if (y != 0 && CanMove(new Vector3(0, y, 0)))
                StartCoroutine(Movement(new Vector3(0, y, 0)));

        }
    }
}
