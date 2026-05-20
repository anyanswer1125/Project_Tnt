using System;
using UnityEngine;

public enum SpikeType
{
    Up, //올라올 SpikeType
    Down    //내려갈 SpikeType
}
public class SpikeScript : MonoBehaviour
{
    private Animator spikeAnim; //함정 애니메이터
    private BoxCollider2D boxCollider2D;

    [SerializeField] private bool spikeActive;
    [SerializeField] private SpikeType spikeType;   //현재 SpikeScript의 타입

    public SpikeType SpikeType => spikeType;

    public bool SpikeActive => spikeActive;

    // 초기화 메서드
    public void Initialize(bool spikeActive)
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        spikeAnim = GetComponent<Animator>();      
        boxCollider2D.enabled = spikeActive;
        boxCollider2D.isTrigger = spikeActive;
        this.spikeActive = spikeActive;
        if (spikeActive)
        {
            spikeAnim.SetBool("ActiveSpike_On", spikeActive);
        }
    }

    // 함정 (활성,비활성) 애니메이터 상태 변환 메서드
    public void FuncSpike(bool active)
    {
        // audiosource.PlayOneShot(sfx_SpikeOnOff);
        boxCollider2D.enabled = active; 
        boxCollider2D.isTrigger = active;
        spikeActive = active;
        spikeAnim.SetBool("ActiveSpike_Off", !spikeActive);
        spikeAnim.SetBool("ActiveSpike_On", spikeActive);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        // 예외처리
        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>();

        // 플레이어의 캐릭터 타입이 도적이라면 충돌 무시
        if (player.PlayerCharacterType == Character.Thief && spikeActive)
        {
            if (!player.IsOnSpike)
                player.OnSpike(true);
            return;
        }

        // 함정이 솟아있는 상태에서 플레이어와 접촉 시 패배 처리
        if (spikeActive && !player.StagePlayEnd)
            player.SetState(State.Lose);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 예외처리
        if (!collision.CompareTag("Player")) return;

        Player player = collision.GetComponent<Player>();

        if (player.PlayerCharacterType == Character.Thief)
        {
            Animator a = player.GetComponent<Animator>();
            a.SetBool("OnSpikeIdle", false);
        }
    }
    public void PlaySfxSpikeOnOff()
    {
        SoundManager.Instance.PlaySFX(113);
    }
}
