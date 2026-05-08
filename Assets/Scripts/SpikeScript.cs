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

    private void OnTriggerStay2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        // 예외처리
        if (player == null) return;

        // 함정이 솟아있는 상태에서 플레이어와 접촉 시 패배 처리
        if (spikeActive && !player.StagePlayEnd)
            player.SetState(State.Lose);
    }
}
