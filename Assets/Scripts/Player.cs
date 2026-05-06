using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveDuration = 0.2f; // 이동에 걸리는 시간 (예: 0.2초)
    [SerializeField] private float jumpHeight = 0.1f; // 점프 높이
    [SerializeField] private LayerMask objLayer;    // 움직을 수 있는 게임오브젝트 레이어
    [SerializeField] private LayerMask floorButtonLayer;    // 함정 레이어

    [SerializeField] private GameObject vfx_JumpEffectObj;
    [SerializeField] private Transform JumpEffectPos;

    private bool isMoving; // 키를 한번만 입력받기 위한 변수

    Vector3 Pos => transform.position;

    private void Start()
    {

    }

    // IsMoveing로 외부에 전달하는 용도
    public void IsMoving(bool isMoveing)
    {
        this.isMoving = isMoveing;
    }

    // 이미지 전환 로직
    private void ImageStats(Vector3 dir)
    {
        if (dir.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (dir.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
    }

    IEnumerator Movement(Vector3 dir)
    {
        // 이미지 전환
        ImageStats(dir);

        // 이동 되는 동안 키를 입력받지 않게 true
        isMoving = true;
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
            float yOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            currentPos.y += yOffset;

            transform.position = currentPos;
            // 매 프레임 시간을 더해줌
            elapsedTime += Time.deltaTime;

            // 다음 프레임까지 기다림 (루프를 프레임 단위로 나눔)
            yield return null;
        }

        // 오차범위에 도달하면 위치를 보정함
        transform.position = targetPos;
        // 다음 키를 입력받기 위해서 false
        isMoving = false;
    }

    // 이동 할 수 있는 지 확인하는 함수
    bool CanMove(Vector2 dir)
    {
        float sideOffset = 0.6f;  // 캐릭터 중심에서 옆으로 밀어낼 거리 (0.5f는 자기 콜라이더에 걸리므로 0.1f 여유치 추가)
        float upOffset = 0.5f;    // 캐릭터 중심에서 위로 올릴 거리 (높이)
        float rayDistance = 0.5f; // 감지 거리

        // 2. 레이 시작 지점 계산
        // 위로 upOffset만큼 올리고, 현재 이동하려는 방향(dir)으로 sideOffset만큼 미리 밀어줌
        Vector2 rayStart = (Vector2)transform.position + Vector2.up * upOffset + (dir * sideOffset);

        // 3. 레이 쏘기 (실제 거리 rayDistance 사용)
        RaycastHit2D hit = Physics2D.Raycast(rayStart, dir, rayDistance);

        // 3. 디버그 로그 추가
        if (hit.collider != null)
        {
            // 이동 방향에 FloorButton 레이어가 감지되면 통과 허용
            if (((1 << hit.collider.gameObject.layer) & floorButtonLayer) != 0) return true;

            if (((1 << hit.collider.gameObject.layer) & objLayer) != 0)
            {
                isMoving = true;
                ImageStats(dir);
                ObjectMovement obj = hit.collider.GetComponent<ObjectMovement>();
                obj.ObjMovement(this, dir);
                return false;
            }
            // 무언가에 부딪혔을 때: 부딪힌 대상의 이름과 레이어 출력
            Debug.Log($"<color=red>[막힘]</color> {hit.collider.name} (레이어 이름: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, 레이어 인덱스: {hit.collider.gameObject.layer})");

        }
        else
        {
            // 아무것도 없을 때
            Debug.Log("<color=green>[통과]</color> 앞이 비어있습니다. 이동 가능!");
        }

        // 4. 씬(Scene) 뷰 시각화 (빨간색=막힘, 녹색=통과) (실제 레이와 디버그 레이의 길이를 0.5f로 일치시킴)
        Debug.DrawRay(rayStart, dir * rayDistance, hit.collider != null ? Color.red : Color.green, 0.5f);

        // 결과 반환: 부딪힌 게 없어야 true(이동 가능)
        return hit.collider == null;
    }


    void Update()
    {
        if (!isMoving)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");


            // 대각선을 방지하기 위한 조건문
            if (x != 0 && CanMove(new Vector3(x, 0, 0)))
            {
                Instantiate(vfx_JumpEffectObj,JumpEffectPos.position,transform.rotation);
                StartCoroutine(Movement(new Vector3(x, 0, 0)));
            }
            else if (y != 0 && CanMove(new Vector3(0, y, 0)))
            {
                Instantiate(vfx_JumpEffectObj,JumpEffectPos.position,transform.rotation);
                StartCoroutine(Movement(new Vector3(0, y, 0)));
            }

        }
    }
}
