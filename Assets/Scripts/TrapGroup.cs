using UnityEngine;
using System.Collections.Generic;

// TrapGroup 클래스는 트랩그룹으로 버튼이 여러개이거나 함정이 여러개 일 경우 전체적으로 관리를 해야하므로 TrapGroup에서 버튼리스트와 함정리스트를 관리함
public class TrapGroup : MonoBehaviour
{
    [SerializeField] private List<FloorButtonScript> floorbuttons = new List<FloorButtonScript>();   //버튼리스트
    [SerializeField] private List<SpikeScript> spikes = new List<SpikeScript>();  //함정리스트
    private void Start()
    {
        Initialize();
    }
    // 초기화 메서드
    private void Initialize()
    {
        // 버튼 리스트 할당하기전 클리어하여 비움
        floorbuttons.Clear();
        // 함정 리스트 할당하기전 클리어하여 비움
        spikes.Clear();
        // 자식 오브젝트들로부터 컴포넌트를 검색하여 리스트에 추가
        floorbuttons.AddRange(GetComponentsInChildren<FloorButtonScript>());
        // 자식 오브젝트들로부터 컴포넌트를 검색하여 리스트에 추가
        spikes.AddRange(GetComponentsInChildren<SpikeScript>());

        // 함정 초기화 메서드 호출 (호출 순서 꼬이는 것을 방지)
        foreach (var v in spikes)
        {
            v.Initialize();
        }
        // 버튼 초기화 메서드 호출 (호출 순서 꼬이는 것을 방지)
        foreach (var v in floorbuttons)
        {
            v.Initialize();
            v.SetTrapGroup(this);
        }
    }

    // 그룹으로 지정된 버튼들이 모두 눌렸는지 확인하는 메서드 (외부에서 호출하도록 설계)
    public void Trap()
    {
        // 리스트가 비어있을 경우를 대비한 방어 코드
        if (floorbuttons == null || spikes == null) return;

        // 하나라도 안 눌린 버튼(True)이 있는지 체크
        bool isAnyButtonNotPressed = false;

        foreach (var btn in floorbuttons)
        {
            if (btn.ActiveFloor == true) // 버튼이 안 눌린 상태라면
            {
                isAnyButtonNotPressed = true;
                break; // 하나라도 True(안 눌림)이면 함정은 True(활성)
            }
        }

        //판별된 값을 모든 스파이크에 전달합니다.
        foreach (var spike in spikes)
        {
            // 모든 버튼이 False(눌림)가 되어야만 isAnyButtonNotPressed가 False(비활성화)
            spike.FuncSpike(isAnyButtonNotPressed);
        }
    }
}
