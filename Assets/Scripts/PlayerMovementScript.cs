using UnityEngine;
using System.Collections.Generic;
using System;

public class PlayerMovementScript : MonoBehaviour
{
    private GameObject[] obstacles;
    private GameObject[] objToPush;
    private bool readyToMove = true;

    void Start()
    {
        // 씬 내의 모든 장애물과 상자를 초기화 시 검색하여 저장합니다.
        obstacles = GameObject.FindGameObjectsWithTag("Obstacles");
        objToPush = GameObject.FindGameObjectsWithTag("ObjToPush");
    }
    void Update()// 실행문
    {
        Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (moveInput.sqrMagnitude > 0.5f)
        {
            if (readyToMove)// 스위치가 켜져 있을 때만 이동을 실행합니다.
            {
                readyToMove = false;//움직이기 시작
                MovePlayer(moveInput);//실제로 움직임
            }
        }
        else
        {
            readyToMove = true;
        }
    }
    private void MovePlayer(Vector2 direction)
    {
        // 상하좌우 한 방향으로만 고정 (대각선 방지)
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y)) direction.y = 0;
        else direction.x = 0;
        direction.Normalize();

        Vector2 targetPos = (Vector2)transform.position + direction;

        // 에외 처리 [Wall]
        //if (targetPos.)



        // 1. 벽/함정에 막혔는지 확인
        if (IsPosBlockedByObstacle(targetPos)) return;

        // 2. 상자에 막혔는지 확인 (상자가 있다면 밀기 시도)
        GameObject box = GetPushableObject(targetPos);
        if (box != null)
        {
            // 상자가 밀릴 위치 계산
            Vector2 boxNextPos = targetPos + direction;

            // 상자의 다음 위치가 벽이나 다른 상자로 막혀있는지 확인
            if (IsPosBlockedByObstacle(boxNextPos) || GetPushableObject(boxNextPos) != null)
            {
                return; // 상자를 밀 수 없으므로 플레이어도 못 움직임
            }

            // 상자 이동
            box.transform.Translate(direction);
            //Debug.Log("박스 이동");
        }

        // 3. 플레이어 이동
        transform.Translate(direction);
    }

    // 특정 위치에 "Obstacles"가 있는지 확인하는 함수
    private bool IsPosBlockedByObstacle(Vector2 pos)
    {
        foreach (var obj in obstacles)
        {
            if ((Vector2)obj.transform.position == pos) return obj;// 벽의 위치와 목표 지점이 같으면 "막혔다(true)"
        }
        return false;//길 안막힘
    }
    private GameObject GetPushableObject(Vector2 pos)// 해당 위치에 상자가 있는지 체크하고, 있다면 그 상자 정보를 넘겨주는 함수입니다
    {
        foreach (var obj in objToPush)
        {
            //Debug.Log("박스 있음");
            if ((Vector2)obj.transform.position == pos)
            {
                Debug.Log("박스 있음");
                return obj;// 상자의 위치와 목표 지점이 같으면 그 상자 오브젝트를 반환합니다.
            } 
                
        }
        
        Debug.Log("박스 없음");
        return null;//상자없으면
    }
}
// 맵 위에 구현 완료
// heavySheet 구별하기