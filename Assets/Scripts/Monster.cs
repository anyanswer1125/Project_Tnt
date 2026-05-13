using System;
using Unity.VisualScripting;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Animator monsterAnimator;
    BoxCollider2D boxCollider2D;
    Player targetPlayer; // 변수명 구분 (클래스명과 중복 방지)
    bool isDead = false; // 중복 사망 방지 플래그
    [SerializeField] private ParticleSystem slimeParticle;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        monsterAnimator = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return; // 이미 죽었다면 로직 무시

        SpikeScript s = collision?.GetComponent<SpikeScript>();
        if(s.SpikeActive == true && s != null)
        {
            MonsterDie();
        }

        Player player = collision.GetComponent<Player>();
        if (player == null) return;

        player.SetState(State.Lose);
    }

    public void MonsterDie(Player p = null)
    {
        if (isDead) return;
        isDead = true;

        // 콜라이더를 아예 꺼버려서 충돌 및 물리 연산 제외
        if (boxCollider2D != null) boxCollider2D.enabled = false;

        monsterAnimator.SetTrigger("Die");
        targetPlayer = p;
        
    }

    public void PlayEffect()
    {
        if (slimeParticle != null)
        {
            slimeParticle.Play();
            boxCollider2D.enabled = false;
        }
        if (targetPlayer != null)
        {
            targetPlayer.IsMoving(false);
        }
    }
    // 애니메이션 이벤트에서 호출 (가장 끝 프레임)
    private void MonsterDestroy()
    {
        // 일회성 오브젝트이므로 확실히 제거
        Destroy(gameObject);
    }
}
