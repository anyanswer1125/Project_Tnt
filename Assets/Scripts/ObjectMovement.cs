using UnityEngine;
using System.Collections;
using System;
using Unity.VisualScripting;

public class ObjectMovement : MonoBehaviour
{
    private float moveDuration = 0.2f; // 이동에 걸리는 시간 (예: 0.2초)
    private float jumpHeight = 0.1f; // 점프 높이
    [SerializeField] private LayerMask floorButtonLay;  //함정 레이어
    [SerializeField] private Animator objAnimator;

    //[SerializeField] private GameObject vfx_PushEffect;

    private Vector3 Pos => transform.position;

    private void Start()
    {
        Initialize();
    }

    // 초기화 함수
    private void Initialize()
    {
        objAnimator = GetComponent<Animator>();
    }


    // 이동 로직
    IEnumerator Movement(Player player, Vector3 dir)
    {
        Animator animator = player.GetComponent<Animator>();
        animator.SetTrigger("Push");

        Vector3 centerPos = (transform.position + player.transform.position) / 2f;
        player.PlayVfxPush(centerPos);

        //Vector3 direction = player.transform.position - transform.position;
        //Instantiate(vfx_PushEffect,centerPos, transform.rotation);
        //PlayVfx(centerPos);

        // 애니메이션 타이밍
        yield return new WaitForSeconds(0.12f);
        objAnimator.SetTrigger("Moving");

        // 이동 되는 동안 키를 입력받지 않게 true
        player.IsMoving(true);
        // 내가 움직일 거리 (내 위치 + 움직일 방향)
        Vector3 targetPos = Pos + dir;

        float elapsedTime = 0f; // 경과 시간 초기화
        // 경과 시간이 설정한 이동 시간(duration)보다 작을 동안 반복
        while (elapsedTime < moveDuration)
        {
            float progress = elapsedTime / moveDuration; // 0에서 1까지 진행률
            // Lerp(시작, 끝, 비율): 비율(0~1)에 따라 위치를 결정함
            // 경과 시간을 전체 이동 시간으로 나누어 비율을 계산 (예: 0.1초 / 0.2초 = 0.5)
            // 평면 이동 (X, Z축)
            Vector3 currentPos = Vector3.Lerp(Pos, targetPos, progress);
            // 높이 이동 (Y축): 사인 곡선을 이용해 0 -> 1 -> 0으로 변함
            //float yOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            //currentPos.y += yOffset;

            transform.position = currentPos;
            // 매 프레임 시간을 더해줌
            elapsedTime += Time.deltaTime;

            // 다음 프레임까지 기다림 (루프를 프레임 단위로 나눔)
            yield return null;
        }

        // 오차범위에 도달하면 위치를 보정함
        transform.position = targetPos;
        // 다음 키를 입력받기 위해서 false
        player.IsMoving(false);
    }

    // 이동 할 수 있는지 확인하는 함수
    bool CanMove(Player player, Vector2 dir)
    {
        float sideOffset = 0.6f;  // 물체 중심에서 옆으로 밀어낼 거리
        float upOffset = 0.5f;    // 물체 중심에서 위로 올릴 거리 (높이)
        float rayDistance = 0.5f; // 감지 거리

        // 2. 레이 시작 지점 계산
        // 위로 upOffset만큼 올리고, 현재 이동하려는 방향(dir)으로 sideOffset만큼 미리 밀어줌
        Vector2 rayStart = (Vector2)transform.position + Vector2.up * upOffset + (dir * sideOffset);

        // 3. 레이 쏘기 (실제 거리 rayDistance 사용)
        RaycastHit2D hit = Physics2D.Raycast(rayStart, dir, rayDistance);

        // 4. 시각화 (실제 레이와 디버그 레이의 길이를 0.5f로 일치시킴)
        Debug.DrawRay(rayStart, dir * rayDistance, hit.collider != null ? Color.red : Color.green, 0.5f);

        if (hit.collider != null)
        {
            // floorButtonLay는 발판이기에 통과함
            if ((1 << hit.collider.gameObject.layer & floorButtonLay) != 0) return true;

            player.IsMoving(false);
            Debug.Log($"<color=red>[막힘]</color> {hit.collider.name}");
            return false;
        }

        Debug.Log("<color=green>[통과]</color> 이동 가능");
        return true;
    }


    public void ObjMovement(Player player, Vector3 dir)
    { 
        if (CanMove(player, dir))
        {
            StartCoroutine(Movement(player, dir));
        }
    }
}
