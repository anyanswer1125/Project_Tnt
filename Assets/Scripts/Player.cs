using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.PlayMode;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum State
{
    None,   //기존 상태
    Win,
    Lose
}

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.2f; //캐릭터의 이동 속도 (값이 클수록 빠름)
    // [SerializeField] private float moveDuration = 0.2f; // 이동에 걸리는 시간 (예: 0.2초)
    [SerializeField] private float jumpHeight = 0.1f; // 점프 높이
    [SerializeField] private LayerMask objLayer;    // 움직을 수 있는 게임오브젝트 레이어
    [SerializeField] private LayerMask floorLayer;    // 갈 수 있는 레이어
    [SerializeField] private LayerMask enemyLayer;  // 적 레이어
    [SerializeField] private LayerMask obstacleLayer;  // 레이어
    [SerializeField] private LayerMask heavyBox; // 무거운 박스

    [SerializeField] private GameObject vfx_JumpEffectObj; //점프 이펙트 프리팹
    [SerializeField] private GameObject vfx_PushEffect; // 푸쉬 이펙트 프리팹
    [SerializeField] private Transform JumpEffectPos;

    [SerializeField] private State currentState; // 에디터에서 확인하는 용도
    [SerializeField] private Animator animator; // Player의 animator

    [SerializeField] private bool isMoving; // 키를 한번만 입력받기 위한 변수
    [SerializeField] private bool isOnSpike = false; // 도적타입일 때 함정위에 있는지 체크(현재 도적타입에 대한 변수이므로 초기값은 false이여야 함)
    [SerializeField] private bool isWizardSkill;    // 마법사타입일 때 스킬을 사용할 수 있는 지 여부
    [SerializeField] new BoxCollider2D collider2D;  // Player 콜라이더

    [SerializeField] private Character playerCharacterType; //이 플레이어의 캐릭터 타입

    [SerializeField] private GameObject goalTimelineObj;
    [SerializeField] private GameObject TeleportCursor; // 마법사의 스킬 커서
    [SerializeField] private GameObject TeleportRangeCursor;    // 마법사의 스킬 범위
    [SerializeField] private SpriteRenderer teleportSprite; // 마법사의 스킬 사용가능 한지 표시하는 역할
    //SFX
    [SerializeField] private AudioClip sfx_MoveSound;
    [SerializeField] private AudioClip sfx_PushBox;
    [SerializeField] private AudioClip sfx_Attack;
    [SerializeField] private AudioClip sfx_BumpSound;

    [SerializeField] private GameObject winTimelineObj;
    [SerializeField] private GameObject cameraShakeObj;

    [SerializeField] private List<TTam> TTam;   // TTam오브젝트 리스트

    private AudioSource audiosource;

    private bool canWarriorMove = true;

    private bool stagePlayEnd; // 스테이지 플레이 종료

    public bool StagePlayEnd => stagePlayEnd; // 스테이지 플레이 종료 get 프로퍼티

    public Transform Transform => transform;

    public Character PlayerCharacterType => playerCharacterType;    //플레이어의 캐릭터타입 get 프로퍼티

    public bool PlayerisMoving => isMoving;

    public bool IsOnSpike => isOnSpike;

    private void Start()
    {
        //Initialize();
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
        StopAllCoroutines();    // 모든 코루틴 종료
        ResetAllAnimatorParameters();   //모든 애니메이션의 동작을 멈춤
        animator.SetBool("Lose", true);
        StartCoroutine(AnimationLose());
        stagePlayEnd = true;
        collider2D.isTrigger = false;
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

        // 다시 시작
        RestartCurrentScene();
    }

    // 게임을 다시 시작 합니다.
    private void RestartCurrentScene()
    {
        // 현재 활성화된 씬을 다시 로드 (초기화)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // 외부에서 Transform를 받을 메서드
    public void SetPlayerTransform(Transform transform)
    {
        this.transform.position = transform.position;
        this.transform.localScale = transform.localScale;
    }

    // Win 상태 메서드
    private void PlayerWin()
    {
        ResetAllAnimatorParameters();   //모든 애니메이터의 동작을 멈춤

        winTimelineObj.SetActive(true);
        animator.SetBool("Win", true);
        stagePlayEnd = true;
        // goalTimelineObj.SetActive(true);

        // 임시 테스트할려고 만듬 (추후에 지워야함)
        Test();
    }

    // 임시 테스트할려고 만듬 (추후에 지워야함)
    public void Test()
    {
        int nextScene = SceneManager.GetActiveScene().buildIndex;
        nextScene++;

        SceneManager.LoadScene(nextScene);
    }

    public void PlayerTurn()
    {
        if (TurnManager.instance.TurnCount > 1)
        {
            TurnManager.instance.SetTurnCount();
        }
        else
        {
            TurnManager.instance.SetTurnCount();
            SetState(State.Lose);
        }
    }

    // 애니메이터의 Parameters의 모든 값을 0이나 false
    private void ResetAllAnimatorParameters()
    {
        if (animator == null) return;

        // 애니메이터에 등록된 모든 파라미터를 하나씩 훑습니다.
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Trigger:
                    animator.ResetTrigger(param.name);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(param.name, false);
                    break;
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(param.name, 0f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(param.name, 0);
                    break;
            }
        }
    }


    // 초기화 메서드
    public void Initialize()
    {
        vfx_JumpEffectObj = Instantiate(vfx_JumpEffectObj);
        vfx_JumpEffectObj.SetActive(false);

        vfx_PushEffect = Instantiate(vfx_PushEffect);
        vfx_PushEffect.SetActive(false);

        audiosource = GetComponent<AudioSource>();

        SetState(State.None);
        animator = GetComponent<Animator>();
        stagePlayEnd = false;
        isMoving = false;

        collider2D = GetComponent<BoxCollider2D>();

        collider2D.isTrigger = true;

        isOnSpike = false; //현재 도적타입에 대한 변수이므로 초기값은 false이여야 함

        // 현재 캐릭터 타입이 Wizard이라면
        if (playerCharacterType == Character.Wizard)
        {
            TeleportCursor = transform.Find(nameof(TeleportCursor)).gameObject;
            TeleportRangeCursor = transform.Find(nameof(TeleportRangeCursor)).gameObject;
            teleportSprite = TeleportCursor.transform.Find("Square").GetComponent<SpriteRenderer>();
            WizardSkillSetActive(false);
            TTam.AddRange(GetComponentsInChildren<TTam>());
            // 초기 위치에서 마법사 스킬을 쓸 수 있는 지 체크
            isWizardSkill = CanWizardSkill();
        }
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
        // -- 그냥 여기에다가 SFX 추가할게요
        //audiosource.PlayOneShot(sfx_PushBox);
        SoundManager.Instance.PlaySFX(109);

        vfx_PushEffect.SetActive(true);
        vfx_PushEffect.transform.position = pos;
        vfx_PushEffect.transform.rotation = transform.rotation;

    }

    public void PlayVfxAttack(Vector3 pos)
    {

        vfx_PushEffect.SetActive(true);
        vfx_PushEffect.transform.position = pos;
        vfx_PushEffect.transform.rotation = transform.rotation;
    }

    // isMoving를 외부에 전달하는 용도
    public void IsMoving(bool isMoveing)
    {
        this.isMoving = isMoveing;
    }

    // isOnSpike를 외부에 전달하는 용도
    public void OnSpike(bool isOnSpike)
    {
        this.isOnSpike = isOnSpike;
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
        isMoving = true;

        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + dir;
        ResetAllAnimatorParameters();
        if (playerCharacterType == Character.Wizard)
            animator.SetBool("De", false);

        if (isOnSpike)
        {
            //animator.SetBool("OnSpikeIdle", false);
            animator.SetBool("OnSpikeMove", true);
        }
        else
            animator.SetBool("Move", true);


        //audiosource.PlayOneShot(sfx_MoveSound);
        SoundManager.Instance.PlaySFX(105);

        // 두 지점 사이의 거리 (보통 1이겠지만, 혹시 모르니 계산)
        //float distance = Vector3.Distance(startPos, targetPos);
        //float counter = 0;

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
        if (isOnSpike)
        {
            animator.SetBool("OnSpikeMove", false);
            animator.SetBool("OnSpikeIdle", true);

        }
        animator.SetBool("Move", false);
        if (playerCharacterType == Character.Wizard)
            isWizardSkill = CanWizardSkill();
        PlayerTurn();
    }

    // 이동 할 수 있는 지 확인하는 함수
    private bool CanMove(Vector2 dir)
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
            // 레이어의 비트연산을 int로 변환
            int hitLayer = 1 << hit.collider.gameObject.layer;

            // 플레이어가 워리어타입이고 레이어가 enemylayer 일때
            if ((hitLayer & enemyLayer) != 0 && playerCharacterType == Character.Warrior)
            {
                Monster enemy = hit.collider.GetComponent<Monster>();
                ImageStats(dir);
                isMoving = true;
                animator.SetTrigger("Attack");
                vfx_PushEffect.SetActive(true);

                Vector3 centerPos = ((enemy.transform.position + Vector3.down * 0.5f) + transform.position) / 2f;

                vfx_PushEffect.transform.position = centerPos;
                vfx_PushEffect.transform.rotation = transform.rotation;
                canWarriorMove = false;
                // audiosource.PlayOneShot(sfx_Attack);
                cameraShakeObj.GetComponent<CameraShakeScript>().CameraShake();

                enemy.MonsterDie(this);
                return false;
            }

            if((hitLayer& heavyBox) != 0 && playerCharacterType == Character.Warrior)
            {
                isMoving = true;
                ImageStats(dir);
                ObjectMovement obj = hit.collider.GetComponent<ObjectMovement>();
                // obj가 null 경우 리턴 true를 하여 통과하게 함
                if (obj == null) return true;
                obj.ObjMovement(this, dir);
                return false;
            }

            // 이동 방향에 Floor 레이어가 감지되면 통과 허용
            if ((hitLayer & floorLayer) != 0) return true;

            // 이동 할 수 있는 오브젝트 레이어일때
            if ((hitLayer & objLayer) != 0)
            {
                isMoving = true;
                ImageStats(dir);
                ObjectMovement obj = hit.collider.GetComponent<ObjectMovement>();
                // obj가 null 경우 리턴 true를 하여 통과하게 함
                if (obj == null) return true;
                obj.ObjMovement(this, dir);
                return false;
            }

            // 무언가에 부딪혔을 때: 부딪힌 대상의 이름과 레이어 출력
            Debug.Log($"<color=red>[막힘]</color> {hit.collider.name} (레이어 이름: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, 레이어 인덱스: {hit.collider.gameObject.layer})");
            if (!audiosource.isPlaying)
            {
                audiosource.PlayOneShot(sfx_BumpSound);
                //SoundManager.Instance.PlaySFX()
            }
        }
        else
        {
            // 아무것도 없을 때
            Debug.Log("<color=green>[통과]</color> 앞이 비어있습니다. 이동 가능!");
        }

        // isOnSpike가 활성화가 된 상태라면
        if (isOnSpike)
        {
            isOnSpike = false;
            animator.SetBool("OnSpikeIdle", false);
        }
        // 4. 씬(Scene) 뷰 시각화 (빨간색=막힘, 녹색=통과) (실제 레이와 디버그 레이의 길이를 0.5f로 일치시킴)
        Debug.DrawRay(rayStart, dir * rayDistance, hit.collider != null ? Color.red : Color.green, 0.5f);

        // 결과 반환: 부딪힌 게 없어야 true(이동 가능)
        return hit.collider == null;
    }

    // 마법사의 스킬을 사용할때 텔포를 할 수 있는지 체크하는 함수( 텔포를 쓸 수 있다면 컬러는 블루, 없다면 레드로 표시)
    bool CheckTeleportValidity(Vector2 dir)
    {
        Vector2 checkPos = (Vector2)transform.position + dir + Vector2.up * 0.5f;

        // 1. 해당 지점에 '장애물'이 있는지 확인
        Collider2D hit = Physics2D.OverlapPoint(checkPos, obstacleLayer);
        // 2. 조건 뒤집기: 부딪힌 게 없어야(null이어야) 갈 수 있는 곳입니다.
        if (hit == null)
        {
            //Debug.Log(checkPos);
            SkillCheckColor(Color.blue);
            return true;
        }
        else
        {
            FloorButtonScript buttonScript = hit.GetComponent<FloorButtonScript>();
            if (buttonScript != null)
            {
                //Debug.Log(checkPos);
                SkillCheckColor(Color.blue);
                return true;
            }
            Debug.Log(checkPos);
            SkillCheckColor(Color.red);
            // 장애물이 감지됨
            Debug.Log($"<color=red>[막힘]</color> {hit.name} 장애물이 앞을 막고 있습니다.");
            return false;
        }
    }

    // 마법사 근처에 enemy가 있는지 체크하는 용도의 함수
    private bool CanWizardSkill()
    {
        // 1. 검사할 중심점 (현재 캐릭터의 위치)
        Vector2 center = transform.position + Vector3.up * 0.5f;

        // 2. 사각형의 크기 (x 범위가 -2~2라면 가로 길이는 4, y도 마찬가지라면 4)
        Vector2 size = new Vector2(5f, 5f);

        // 3. 해당 범위 내에 적 레이어가 있는지 체크
        Collider2D hit = Physics2D.OverlapBox(center, size, 0f, enemyLayer);

        if (hit != null)
        {
            // 사각형 범위 안에 적이 하나라도 걸린 경우
            //Debug.Log($"<color=red>[위험]</color> 범위 내에 {hit.name} 적이 있습니다!");
            animator.SetBool("De", true);
            foreach (var v in TTam)
            {
                v.gameObject.SetActive(true);
            }
            return false;
        }
        animator.SetBool("De", false);
        foreach (var v in TTam)
        {
            v.gameObject.SetActive(false);
        }
        return true;
    }
    // 외부에서 호출 하는 용도의 메서드
    public void isCanWizardSkill()
    {
        CanWizardSkill();
        CheckTeleportValidity(Vector3.zero);
        isWizardSkill = CanWizardSkill();
    }

    // 스킬 체크 컬러를 나타내는 용도의 메서드
    private void SkillCheckColor(Color color)
    {
        color.a = 0.5f;           // 변수의 알파값을 수정한 뒤
        teleportSprite.color = color; // 다시 스프라이트에 대입
    }

    // 마법사 스킬
    private IEnumerator WizardSkill()
    {
        isMoving = true;
        bool isSelected = false;
        bool isSelectedTrun = false;

        animator.SetBool("Skill", true);
        WizardSkillSetActive(true);
        // 스킬 커서 위치 초기화
        TeleportCursor.transform.position = transform.position + Vector3.up * 0.5f;
        // 시작 위치
        Vector3 startPos = TeleportCursor.transform.position;
        // 시작점에서 한번 체크
        CheckTeleportValidity(new Vector3(0, 0, 0));

        // 현재 커서가 얼마나 이동했는지 기록할 변수
        float currentX = 0f;
        float currentY = 0f;

        while (!isSelected)
        {
            if (Input.anyKeyDown)
            {
                float x = Input.GetAxisRaw("Horizontal");
                float y = Input.GetAxisRaw("Vertical");

                if (x != 0 || y != 0)
                {
                    // 1. 이동량 누적 (Time.deltaTime과 속도를 곱하면 더 부드럽게 이동 가능합니다)
                    // 여기서는 단순 이동으로 구현하되 범위를 제한합니다.
                    currentX += x;
                    currentY += y;

                    // 2. Mathf.Clamp(값, 최소, 최대)를 사용하여 범위 제한
                    currentX = Mathf.Clamp(currentX, -2f, 2f);
                    currentY = Mathf.Clamp(currentY, -2f, 2f);



                    if (CheckTeleportValidity(new Vector3(currentX, currentY, 0)))
                    {
                        isSelectedTrun = true;
                        //Debug.Log("이동 가능");
                    }
                    else
                    {
                        isSelectedTrun = false;
                        //Debug.Log("이동 불가");
                    }
                    Vector3 dir = new Vector3(currentX, currentY, 0);
                    TeleportCursor.transform.position = startPos + dir;
                }

                if (Input.GetKeyDown(KeyCode.Z))
                {
                    WizardSkillSetActive(false);
                    isSelected = true;
                }
                else if (Input.GetKeyDown(KeyCode.Space) && isSelectedTrun)
                {
                    transform.position = TeleportCursor.transform.position - Vector3.up * 0.5f; //커서위치와 player 위치의 높이가 0.5 차이 나기 때문에 - 를 함
                    WizardSkillSetActive(false);
                    isSelected = true;
                    SoundManager.Instance.PlaySFX(108);
                    PlayerTurn();
                }

            }
            yield return null;
        }

        isSelectedTrun = false;
        animator.SetBool("Skill", false);
        isMoving = false;
        isWizardSkill = CanWizardSkill();   //마법사로 이동한 후에도 체크
    }

    private void WizardSkillSetActive(bool active)
    {
        TeleportCursor.SetActive(active);
        TeleportRangeCursor.SetActive(active);
    }


    void Update()
    {
        if (canWarriorMove && !isMoving && !stagePlayEnd)
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            if (playerCharacterType == Character.Wizard && isWizardSkill == true && Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(WizardSkill());
                return;
            }

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

            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartCurrentScene();
            }
        }
    }

    public void SfxWarriorAttack()
    {
        //audiosource.PlayOneShot(sfx_Attack);
        SoundManager.Instance.PlaySFX(107);
    }
    public void WarrirMoveFalse()
    {
        canWarriorMove = false;
        Debug.Log("워리어 이동 불가");
    }
    public void WarrirMoveTrue()
    {
        canWarriorMove = true;
        Debug.Log("워리어 이동 가능");
    }
}