using UnityEngine;

public class FloorButtonScript : MonoBehaviour
{
    private Animator floorAnim; //버튼 애니메이터
    
    [SerializeField] private bool activeFloor;

    private TrapGroup trap; // 현재 트랩 그룹

    public bool ActiveFloor => activeFloor; // activeFloor 외부 get 프로퍼티
    //활성화 할 스파이크 오브젝트 할당

    // 초기화 메서드
    public void Initialize()
    {
        floorAnim = GetComponent<Animator>();
        floorAnim.SetBool("ActiveFloorButton", activeFloor);
    }

    // 현재 그룹을 받아오는 함수
    public void SetTrapGroup(TrapGroup group)
    {
        trap = group;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        activeFloor = false;    // 버튼이 눌림 (False)
        floorAnim.SetBool("ActiveFloorButton", activeFloor);    // 눌림 애니메이션
        trap.Trap(); // 현재 그룹의 버튼들이 눌렸는지 확인하는 메서드
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        activeFloor = true; // 버튼이 안눌림 (True)
        floorAnim.SetBool("ActiveFloorButton", activeFloor);    // 올라오는 애니메이션
        trap.Trap(); // 현재 그룹의 버튼들이 눌렸는지 확인하는 메서드
    }
}
