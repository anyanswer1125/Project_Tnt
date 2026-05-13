using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    private Animator spikeAnim; //함정 애니메이터

    [SerializeField] private bool spikeActive;

    // 초기화 메서드
    public void Initialize()
    {
        spikeAnim = GetComponent<Animator>();
        spikeActive = true;
        spikeAnim.SetBool("ActiveSpike", true);
    }

    // 함정 (활성,비활성) 애니메이터 상태 변환 메서드
    public void FuncSpike(bool active)
    {
        spikeActive = active;
        spikeAnim.SetBool("ActiveSpike", spikeActive);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
}
