using System.Collections.Generic;
using UnityEngine;
using System;
public class Character : MonoBehaviour
{
    //능력 사용 e 취소 f키
    // [1] 직업의 종류를 열거형으로 정의합니다.
    public enum JobType { Thief, Wizard, Warrior }

    // [2] 직업별 상세 정보를 담을 클래스입니다.
    [Serializable]
    public class CharacterData
    {
        public JobType type;        // 직업 종류 (비교용)
        public string specialSkill; // 고유 스킬명

        // 생성자: 데이터를 편하게 넣기 위해 만듭니다.
        public CharacterData(JobType type, string skill)
        {
            this.type = type;
            this.specialSkill = skill;
        }
    }
    // [3] 모든 직업의 데이터를 보관할 리스트입니다.
    public List<CharacterData> allJobs = new List<CharacterData>();
    // [4] 현재 이 캐릭터의 직업
    public JobType currentMyJob;
    void Start()
    {
        PlayerMovementScript ObjToPush = GetComponent<PlayerMovementScript>();
        // 리스트에 직업 데이터들을 미리 채워넣습니다. (백과사전 만들기)
        allJobs.Add(new CharacterData(JobType.Thief, "은신 + 지형지물 뛰어넘기"));
        allJobs.Add(new CharacterData(JobType.Wizard, "텔레포트"));
        allJobs.Add(new CharacterData(JobType.Warrior, "몬스터 처치 + 무거운 물체밀기"));
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))//능력사용
        {
            ExecuteJobAction(currentMyJob);
        }
        else if(Input.GetKeyDown(KeyCode.F))//능력취소
        {
            return;//커서 캐릭터로 이동코드
        }
    }

    // [5] 직업을 구별해서 스킬을 실행하는 핵심 함수입니다.
    void ExecuteJobAction(JobType currentJob)
    {
        // 리스트에서 현재 직업(currentJob)과 일치하는 데이터를 찾습니다.
        CharacterData myData = allJobs.Find(x => x.type == currentJob);

        if (myData == null) return; // 데이터를 못 찾으면 종료

        Debug.Log("--- 직업 특성 확인 ---");
        Debug.Log("현재 직업: " + myData.type);//혹시나 오류날 시, 어디까지 출력되는지 확인용 확인 후 지우기

        // [6]직업별 고유 행동을 정의합니다.
        switch (myData.type)
        {
            case JobType.Thief:
                //은신 + 함정 무시코드 짜기
                break;

            case JobType.Wizard://텔레포트 사용코드 짜기 3*3범위내 몬스터 있을 시 사용불가
                Vector2 currentpos = transform.position;//현재위치
                Collider2D enemy = Physics2D.OverlapCircle(transform.position, 1.0f, LayerMask.GetMask("Enemy"));//적 화인
                if (enemy != null)// 반경 1m 안에 적이 한 명이라도 있는지 확인
                {
                    return;//캐릭터로 커서 돌리기 텔레포트 사용불가
                }
                else
                {

                }
                break;

            case JobType.Warrior:
                //무거운 박스 밀 수 있음 + 몬스터 처치가능
                break;

            default:
                Debug.Log("직업을 찾을 수 없습니다.");
                break;
        }
    }
}