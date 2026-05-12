using UnityEngine;
using System.Collections;

public enum State
{
    None,   //기존 상태
    Win,
    Lose
}

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.2f; //캐릭터의 이동 속도 (값이 클수록 빠름)
    [SerializeField] private float moveDuration = 0.2f; // 이동에 걸리는 시간 (예: 0.2초)
    [SerializeField] private float jumpHeight = 0.1f; // 점프 높이
    [SerializeField] private LayerMask objLayer;    // 움직을 수 있는 게임오브젝트 레이어
    [SerializeField] private LayerMask floorButtonLayer;    // 함정 레이어

    [SerializeField] private GameObject vfx_JumpEffectObj; //점프 이펙트 프리팹
    [SerializeField] private GameObject vfx_PushEffect; // 푸쉬 이펙트 프리팹
    [SerializeField] private Transform JumpEffectPos;

    [SerializeField] private State currentState; // 에디터에서 확인하는 용도
    [SerializeField] private Animator animator; // Player의 animator
    [SerializeField] private TurnManager turnManager; // TurnManager 인스턴스

    [SerializeField] private bool isMoving; // 키를 한번만 입력받기 위한 변수

    [SerializeField] private GameObject goalTimelineObj;

    private bool stagePlayEnd; // 스테이지 플레이 종료

    public State CurrentState => currentState; // 현재 상태 get 프로퍼티

    public bool StagePlayEnd => stagePlayEnd; // 스테이지 플레이 종료 get 프로퍼티

    Vector3 Pos => transform.position;

    private void Start()
    {
        Initialize();
    }

    // 상태 변환 메서드 (외부에서 쓸 수 있도록 공개)
    public void SetState(State state)
    {
        switch (state)
        {
            case State.None:
                break;
            case State.Win:
                PlayerWin();
                break;
            case State.Lose:
                PlayerLose();
                break;
        }
        // 현재 상태 갱신
        currentState = state;
    }

    // Lose 상태 메서드
    private void PlayerLose()
    {
        animator.SetBool("Lose", true);
        StartCoroutine(AnimationLose());
        stagePlayEnd = true;
    }
    private IEnumerator AnimationLose()
    {
        float jumpHeight = 6.0f;     // 튀어오르는 높이
        float loseDuration = 1.2f;  // 전체 애니메이션 시간
        float fallDepth = -8.0f;    // 화면 아래로 떨어질 깊이
        float sideDistance = 1.0f;  // 옆으로 밀려날 거리
        float elapsedTime = 0f;

        Vector3 startPos = transform.position; // 시작 위치 고정

        // 1. 캐릭터가 바라보는 방향에 따라 좌우 날아가는 방향 결정
        float direction = transform.localScale.x > 0 ? 1f : -1f;

        // 2. 최종 목적지 설정 (약간 옆으로 이동하며 화면 아래로 추락)
        Vector3 targetPos = startPos + new Vector3(direction * sideDistance, fallDepth, 0f);

        while (elapsedTime < loseDuration)
        {
            elapsedTime += Time.deltaTime;

            // 0에서 1까지의 진행률
            float p = Mathf.Clamp01(elapsedTime / loseDuration);

            // 선형 보간으로 전체적인 궤적(X, Y, Z)을 계산
            // 시작점(startPos)에서 목적지(targetPos)까지 대각선 아래로 이동합니다.
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, p);

            // Y축에만 사인 곡선을 더해 '점프' 효과 추가
            // p가 0일 때 Sin(0) = 0
            // p가 0.5일 때 Sin(90도) = 1 (최고점)
            // p가 1일 때 Sin(180도) = 0 (다시 원래 궤적으로 복귀)
            float yOffset = Mathf.Sin(p * Mathf.PI) * jumpHeight;

            // 최종 위치 적용
            transform.position = new Vector3(currentPos.x, currentPos.y + yOffset, currentPos.z);

            yield return null;
        }
    }


    // Win 상태 메서드
    private void PlayerWin()
    {
        animator.SetBool("Win", true);
        stagePlayEnd = true;
        // goalTimelineObj.SetActive(true);

    }

    // 초기화 메서드
    private void Initialize()
    {
        vfx_JumpEffectObj = Instantiate(vfx_JumpEffectObj);
        vfx_JumpEffectObj.SetActive(false);

        vfx_PushEffect = Instantiate(vfx_PushEffect);
        vfx_PushEffect.SetActive(false);

        SetState(State.None);
        animator = GetComponent<Animator>();
        stagePlayEnd = false;
        isMoving = false;

        turnManager = FindAnyObjectByType<TurnManager>();
    }

    // 오브젝트 풀링을 통한 VFX 재사용: 생성 비용 최적화 및 위치 재설정
    private void PlayVfxJump()
    {
        vfx_JumpEffectObj.SetActive(true);
        vfx_JumpEffectObj.transform.position = JumpEffectPos.position;
        vfx_JumpEffectObj.transform.rotation = JumpEffectPos.rotation;
    }

    // 오브젝트 풀링을 통한 VFX 재사용: 생성 비용 최적화 및 위치 재설정 ( Push는 ObjectMovement에서 호출)
    public void PlayVfxPush(Vector3 pos)
    {
        vfx_PushEffect.SetActive(true);
        vfx_PushEffect.transform.position = pos;
        vfx_PushEffect.transform.rotation = transform.rotation;
    }

    // isMoving로 외부에 전달하는 용도
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
        ImageStats(dir);
        turnManager?.SetTurnCount();
        isMoving = true;
    
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + dir;
    
        animator.SetBool("Move", true);
    
        // 두 지점 사이의 거리 (보통 1이겠지만, 혹시 모르니 계산)
        float distance = Vector3.Distance(startPos, targetPos);
        float counter = 0;
    
        // 거리를 속도로 나누면 이동에 필요한 '시간'이 나옵니다.
        // 하지만 속도 기반으로 매 프레임 위치를 옮기는 게 더 직관적입니다.
        float progress = 0f;
    
        while (progress < 1f)
        {
            // 매 프레임 이동 진행률을 속도에 맞춰 증가시킴
            // 거리(1)를 이동하는 데 걸리는 비율을 계산
            progress += Time.deltaTime * moveSpeed;
    
            // 이동 위치 계산
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, progress);
    
            // 점프 효과 (선택 사항)
            float yOffset = Mathf.Sin(progress * Mathf.PI) * jumpHeight;
            currentPos.y += yOffset;
    
            transform.position = currentPos;
            
            yield return null;
        }
    
        transform.position = targetPos;
        isMoving = false;
        animator.SetBool("Move", false);
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
                // obj가 null 경우 리턴 true를 하여 통과하게 함
                if (obj == null) return true;
                obj.ObjMovement(this, dir);
                return false;
            }
            SpikeScript spike = hit.collider.GetComponent<SpikeScript>();//스파이크 받고
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
        if (!isMoving && !stagePlayEnd)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");


            // 대각선을 방지하기 위한 조건문
            if (x != 0 && CanMove(new Vector3(x, 0, 0)))
            {
                //Instantiate(vfx_JumpEffectObj,JumpEffectPos.position,transform.rotation);
                PlayVfxJump();
                StartCoroutine(Movement(new Vector3(x, 0, 0)));
            }
            else if (y != 0 && CanMove(new Vector3(0, y, 0)))
            {
                //Instantiate(vfx_JumpEffectObj,JumpEffectPos.position,transform.rotation);
                PlayVfxJump();
                StartCoroutine(Movement(new Vector3(0, y, 0)));
            }

        }
    }
}
// 장애물 인식 raycast gameover로 일단 만들기
// 움직임 로직 수정 앞에 트랩있을때 인식불가